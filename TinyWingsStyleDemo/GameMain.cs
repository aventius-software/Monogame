using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using TinyWingsStyleDemo.Services;

namespace TinyWingsStyleDemo;

public class GameMain : Game
{
    private Camera _camera;
    private GraphicsDeviceManager _graphics;
    private HillGeneratorService _hillGeneratorService;
    private HillSegment[] _hillSegments;
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
        // TODO: Add your initialization logic here
        _shapeDrawingService = new ShapeDrawingService(GraphicsDevice);
        _hillGeneratorService = new HillGeneratorService();

        // Generate the initial hills
        var numberOfHills = 25;
        var numberOfHillSegments = 16;
        var startPosition = new Vector2(0, 240);

        _hillSegments = _hillGeneratorService.GenerateHills(numberOfHills, numberOfHillSegments, startPosition);
        
        // Create a camera
        _camera = new Camera();

        // Tell the camera the dimensions of the world
        var w = _hillSegments.Last().End.X - _hillSegments[0].Start.X;
        var h = Math.Abs(_hillSegments.Max(x => x.End.Y) - _hillSegments.Min(x => x.Start.Y));
        _camera.SetWorldDimensions(new Vector2(w, h));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Set the camera origin to the middle of the viewport, also note the offset for the size of the character sprite
        _camera.SetOrigin(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

        // Place the character at some 'world' position coordinates
        _position = new Vector2(0, 0);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Move the player
        var keyboard = Keyboard.GetState();
        var direction = Vector2.Zero;
        var speed = 8;

        if (keyboard.IsKeyDown(Keys.Up)) direction.Y = -speed;
        if (keyboard.IsKeyDown(Keys.Down)) direction.Y = speed;
        if (keyboard.IsKeyDown(Keys.Left)) direction.X = -speed;
        if (keyboard.IsKeyDown(Keys.Right)) direction.X = speed;

        _position += direction;

        // Set camera to the player/characters position, set offset so we account for the character sprite origin
        // being the top left corner of the sprite, this makes the camera constrain to the end of the
        // map 'minus' the width/height of the character. Otherwise we'd get a gap at the end of the map
        _camera.LookAt(_position, Vector2.Zero);//, new Vector2(_characterTexture.Width, _characterTexture.Height));

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Start drawing, note the 'transformMatrix' which is from our camera
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.TransformMatrix);

        // First draw the map (so it will be under the character)
        foreach (var segment in _hillSegments)
        {

            var start = Vector2.Transform(segment.Start, _camera.TransformMatrix);
            var end = Vector2.Transform(segment.End, _camera.TransformMatrix);
            _shapeDrawingService.DrawLine(start, end, Color.White);
        }

        // Now draw character after, this way it will be on top of the map
        //_spriteBatch.Draw(
        //    texture: _characterTexture,
        //    position: _position,
        //    color: Color.White);

        // We're done...
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
