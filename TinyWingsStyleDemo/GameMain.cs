using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Linq;
using TinyWingsStyleDemo.Services;

namespace TinyWingsStyleDemo;

/// <summary>
/// A simple tiny wings style demo using Aether Physics 2D... Just press space to 'slide' or 'dive' ;-)
/// </summary>
public class GameMain : Game
{
    private const float _gravity = 9.8f;
    private const int _hillSegmentWidth = 24;
    private const int _maximumHillSegmentOffsetY = 40;
    private const int _maximumHillSteepness = 35;
    private const float _minimumCharacterVelocity = 2.5f;
    private const int _numberOfHills = 30;
    private const int _numberOfSegmentsPerHill = 32;
    private const float _slidePower = 4.5f;

    private Camera _camera;
    private Body _characterRigidBody;
    private Texture2D _characterTexture;
    private GraphicsDeviceManager _graphics;
    private HillGeneratorService _hillGeneratorService;
    private HillSegment[] _hillSegments;
    private bool _isSliding;
    private PhysicsWorld _physicsWorld;
    private readonly int _pixelsPerMetre = 32;
    private Vector2 _position;
    private ShapeDrawingService _shapeDrawingService;
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
        var hillStartPosition = new Vector2(0, GraphicsDevice.Viewport.Height);

        _hillSegments = _hillGeneratorService.GenerateHills(
            startPosition: hillStartPosition,
            numberOfHills: _numberOfHills,
            segmentsPerHill: _numberOfSegmentsPerHill,
            segmentWidth: _hillSegmentWidth,
            maxOffsetY: _maximumHillSegmentOffsetY,
            maxHillSteepness: _maximumHillSteepness);

        // Create a camera
        _camera = new Camera();

        // Tell the camera the dimensions of the world
        var worldWidth = _hillSegments.Last().End.X - _hillSegments[0].Start.X;
        var worldHeight = Math.Abs(_hillSegments.Max(segment => segment.End.Y) - _hillSegments.Min(segment => segment.Start.Y));

        // Set the camera world dimensions, make it 3 times the height so
        // that the camera can move up and down a bit to follow the player
        _camera.SetWorldDimensions(new Vector2(worldWidth, worldHeight * 3));

        // Create the physics 'world'
        _physicsWorld = new PhysicsWorld();

        // We'll use a pixels per metre value for the simulation (as the physics engine works in metres/kilograms/etc...)
        // that kind of feels right for motion on screen/window of 800 x 480 pixels. About 32 pixels per metre seems ok
        _physicsWorld.SetDisplayUnitToSimUnitRatio(_pixelsPerMetre);

        // We want 'normal' gravity to make things fall
        _physicsWorld.Gravity = new Vector2(0, _gravity);

        // Create physics 'edges' for the hills so we can 'slide' along them
        for (int segment = 0; segment < _hillSegments.Length; segment++)
        {
            // Create a physics body for our ground segment       
            var groundSegmentBody = _physicsWorld.CreateBody();

            // Set the simulation coordinates for the start and end of this ground segment
            var startCoordinates = _physicsWorld.ToSimUnits(_hillSegments[segment].Start);
            var endCoordinates = _physicsWorld.ToSimUnits(_hillSegments[segment].End);

            // Attach an 'edge' (just a 'line' effectively) fixture and set friction to some very low value (to make it slippery)
            var edgeFixture = groundSegmentBody.CreateFixture(new EdgeShape(startCoordinates, endCoordinates));
            edgeFixture.Friction = 0.01f;
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load a texture for our character
        _characterTexture = Content.Load<Texture2D>("Textures/ball");

        // Place the character at some starting 'world' position coordinates
        _position = new Vector2(_characterTexture.Width, (GraphicsDevice.Viewport.Height / 2) - (_characterTexture.Height + _maximumHillSegmentOffsetY));

        // Create a rigid body for our character so it can interact with the physics simulation               
        _characterRigidBody = _physicsWorld.CreateCircle(
            radius: _physicsWorld.ToSimUnits(_characterTexture.Width / 2),
            density: 1,
            position: _physicsWorld.ToSimUnits(_position),
            bodyType: BodyType.Dynamic);

        // Fix the rotation for the physics simulation as we'll calculate it ourselves
        _characterRigidBody.FixedRotation = true;

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
            // If the player is 'sliding' we amplify gravity
            _physicsWorld.Gravity = new Vector2(0, _gravity * _slidePower);
        }
        else
        {
            // Otherwise gravity is set to normal
            _physicsWorld.Gravity = new Vector2(0, _gravity);
        }

        // Check for minimum velocity as we want the character to keep slowly moving forward
        // even if they are heading up hill, otherwise they'd fall back down the hill...
        var velocity = _characterRigidBody.LinearVelocity;

        if (velocity.X < _minimumCharacterVelocity)
        {
            // Set 'X' axis velocity to our minimum value
            _characterRigidBody.LinearVelocity = new Vector2(_minimumCharacterVelocity, velocity.Y);
        }

        // Set the rotation angle depending on velocity, so that the character tends downwards
        // when 'diving' and upwards when 'launching' ;-)
        _characterRigidBody.Rotation = (float)Math.Atan2(velocity.Y, velocity.X);

        // Set camera to follow the player/characters position
        _position = _physicsWorld.ToDisplayUnits(_characterRigidBody.Position);
        _camera.LookAt(_position, Vector2.Zero);

        // 'Smoothly' zoom out a little when the player moves faster, then
        // back in again when they move slower
        var zoom = MathHelper.SmoothStep(1f, 2f / _characterRigidBody.LinearVelocity.Length(), 0.45f);
        _camera.Zoom(zoom);

        // Simulate (or 'step' through) the physics simulation for this frame
        _physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // First draw the map (so it will be under the character)
        foreach (var segment in _hillSegments)
        {
            // Transform the coordinates according to the camera
            var start = Vector2.Transform(segment.Start, _camera.TransformMatrix);
            var end = Vector2.Transform(segment.End, _camera.TransformMatrix);

            // Draw ground line
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
