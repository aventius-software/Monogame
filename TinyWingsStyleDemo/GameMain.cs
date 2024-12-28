using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Linq;
using TinyWingsStyleDemo.Services;

namespace TinyWingsStyleDemo;

public class GameMain : Game
{
    private Camera _camera;
    private Body _characterRigidBody;
    private float _characterMinimumRotationAngle = MathHelper.ToRadians(-25);
    private float _characterMaximumRotationAngle = MathHelper.ToRadians(25);
    private Texture2D _characterTexture;
    private GraphicsDeviceManager _graphics;
    private readonly float _gravity = 9.8f;
    private HillGeneratorService _hillGeneratorService;
    private HillSegment[] _hillSegments;
    private bool _isSliding;
    private readonly float _minimumCharacterVelocity = 1f;
    private PhysicsWorld _physicsWorld;
    private int _pixelsPerMetre = 32;
    private Vector2 _position;
    private ShapeDrawingService _shapeDrawingService;
    private readonly float _slidePower = 3f;
    private SpriteBatch _spriteBatch;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Initialise our services
        _shapeDrawingService = new ShapeDrawingService(GraphicsDevice);
        _hillGeneratorService = new HillGeneratorService();

        // Generate the hills       
        var startPosition = new Vector2(0, GraphicsDevice.Viewport.Height / 2);

        _hillSegments = _hillGeneratorService.GenerateHills(
            startPosition: startPosition,
            numberOfHills: 25,
            segmentsPerHill: 32,
            segmentWidth: 16,
            maxOffsetY: 25,
            maxHillSteepness: 25);

        // Create a camera
        _camera = new Camera();

        // Tell the camera the dimensions of the world
        var w = _hillSegments.Last().End.X - _hillSegments[0].Start.X;
        var h = Math.Abs(_hillSegments.Max(x => x.End.Y) - _hillSegments.Min(x => x.Start.Y));
        _camera.SetWorldDimensions(new Vector2(w, h));

        // Create the physics 'world'
        _physicsWorld = new PhysicsWorld();

        // We'll use a pixels per metre value for the simulation (as the physics engine works in metres/kilograms/etc...)
        // that kind of feels right for motion on screen/window of 800 x 480 pixels. About 32 pixels per metre seems ok
        _physicsWorld.SetDisplayUnitToSimUnitRatio(_pixelsPerMetre);

        // We want normal gravity to make things fall
        _physicsWorld.Gravity = new Vector2(0, _gravity);

        // Create physics edges for the hills
        for (int i = 0; i < _hillSegments.Length; i++)
        {
            // Create a body for our 'edge' (just a 'line' effectively)           
            var edge = _physicsWorld.CreateBody();

            // Attach a fixture and set friction to a low value (to make it slippery)
            var fixture = edge.CreateFixture(new EdgeShape(_physicsWorld.ToSimUnits(_hillSegments[i].Start), _physicsWorld.ToSimUnits(_hillSegments[i].End)));
            fixture.Friction = 0.01f;
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load a texture for our character
        _characterTexture = Content.Load<Texture2D>("Textures/ball");

        // Place the character at some 'world' position coordinates
        _position = new Vector2(64, 240 - (64 + 25));

        // Create a rigid body for our character
        _characterRigidBody = _physicsWorld.CreateRoundedRectangle(
            width: _physicsWorld.ToSimUnits(_characterTexture.Width),
            height: _physicsWorld.ToSimUnits(_characterTexture.Height),
            xRadius: _physicsWorld.ToSimUnits(_characterTexture.Width / 8),
            yRadius: _physicsWorld.ToSimUnits(_characterTexture.Height / 8),
            segments: 1,
            density: 1,
            position: _physicsWorld.ToSimUnits(_position),
            rotation: 0,
            bodyType: BodyType.Dynamic);

        //_characterRigidBody = _physicsWorld.CreateCircle(
        //    radius: _physicsWorld.ToSimUnits(_characterTexture.Width / 2),
        //    density: 1,
        //    position: _physicsWorld.ToSimUnits(_position),
        //    bodyType: BodyType.Dynamic);

        //_characterRigidBody = _physicsWorld.CreateCapsule(
        //    height: _physicsWorld.ToSimUnits(_characterTexture.Height),
        //    topRadius: _physicsWorld.ToSimUnits(8),
        //    topEdges: 2,
        //    bottomRadius: _physicsWorld.ToSimUnits(8),
        //    bottomEdges: 2,
        //    density: 1,
        //    position: _physicsWorld.ToSimUnits(_position),
        //    rotation: 0,
        //    bodyType: BodyType.Dynamic);

        // Set the camera origin to the middle of the viewport, also note the offset for the size of the character sprite
        _camera.SetOrigin(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Handle 'sliding'
        _isSliding = Keyboard.GetState().IsKeyDown(Keys.Space);

        if (_isSliding)
        {
            _physicsWorld.Gravity = new Vector2(0, _gravity * _slidePower);
        }
        else _physicsWorld.Gravity = new Vector2(0, _gravity);

        // Check for minimum velocity        
        var velocity = _characterRigidBody.LinearVelocity;
        
        if (velocity.X < _minimumCharacterVelocity)
        {
            // Set 'X' axis velocity to our minimum value
            _characterRigidBody.LinearVelocity = new Vector2(_minimumCharacterVelocity, velocity.Y);
        }

        // Constrain our characters rotation between certain limits        
        var rotation = _characterRigidBody.Rotation;

        // Clamp the rotation within the limits
        if (rotation < _characterMinimumRotationAngle)
        {
            _characterRigidBody.Rotation = _characterMinimumRotationAngle;
            _characterRigidBody.AngularVelocity = 0;
        }
        else if (rotation > _characterMaximumRotationAngle)
        {
            _characterRigidBody.Rotation = _characterMaximumRotationAngle;
            _characterRigidBody.AngularVelocity = 0;
        }

        // Set camera to the player/characters position, set offset so we account for the character sprite origin
        // being the top left corner of the sprite, this makes the camera constrain to the end of the
        // map 'minus' the width/height of the character. Otherwise we'd get a gap at the end of the map
        _position = _physicsWorld.ToDisplayUnits(_characterRigidBody.Position);
        _camera.LookAt(_position, Vector2.Zero);

        // Next 'step' for the physics simulation
        _physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // First draw the map (so it will be under the character)
        foreach (var segment in _hillSegments)
        {
            var start = Vector2.Transform(segment.Start, _camera.TransformMatrix);
            var end = Vector2.Transform(segment.End, _camera.TransformMatrix);
            _shapeDrawingService.DrawLine(start, end, Color.White);
        }

        // Start drawing, note the 'transformMatrix' which is from our camera
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.TransformMatrix);

        // Now draw character after, this way it will be on top of the map
        _spriteBatch.Draw(
            texture: _characterTexture,
            position: _physicsWorld.ToDisplayUnits(_characterRigidBody.Position),
            sourceRectangle: new Rectangle(0, 0, _characterTexture.Width, _characterTexture.Height),
            color: _isSliding ? Color.Red : Color.White,
            rotation: _characterRigidBody.Rotation,
            origin: new Vector2(_characterTexture.Width / 2, _characterTexture.Height / 2),
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        // We're done...
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
