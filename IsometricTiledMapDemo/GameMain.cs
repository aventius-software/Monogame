using IsometricTiledMapDemo.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IsometricTiledMapDemo;

public class GameMain : Game
{
    private Camera _camera;
    private SpriteFont _font;
    private GraphicsDeviceManager _graphics;
    private IsometricTiledMapService _isometricMapService;
    private Point _mousePosition;
    private Vector2 _position;
    private Vector3 _selectedTile;
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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load font
        _font = Content.Load<SpriteFont>("font");

        // Set the origin for the camera
        var origin = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

        // Setup isometric map service        
        _isometricMapService = new IsometricTiledMapService(Content, _spriteBatch)
        {
            Origin = origin
        };

        // Create a camera
        _camera = new Camera();
        _camera.SetWorldDimensions(new Vector2(_isometricMapService.WorldWidth, _isometricMapService.WorldHeight));

        // Place the character at some 'world' position coordinates
        _position = new Vector2(1, 1);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Move the player
        var keyboard = Keyboard.GetState();
        var direction = Vector2.Zero;
        var speed = 1;

        if (keyboard.IsKeyDown(Keys.Up)) direction.Y = -speed;
        if (keyboard.IsKeyDown(Keys.Down)) direction.Y = speed;
        if (keyboard.IsKeyDown(Keys.Left)) direction.X = -speed;
        if (keyboard.IsKeyDown(Keys.Right)) direction.X = speed;

        _position += direction;

        // Restrict movement to the world
        if (_position.X < 0) _position.X = speed;
        if (_position.X > _isometricMapService.WorldWidth) _position.X = _isometricMapService.WorldWidth - speed;
        if (_position.Y < 0) _position.Y = speed;
        if (_position.Y > _isometricMapService.WorldHeight) _position.Y = _isometricMapService.WorldHeight - speed;

        // Set camera to the player position, set offset so we account for the character sprite origin
        // being the top left corner of the sprite, this makes the camera constrain to the end of the
        // map 'minus' the width/height of the character. Otherwise we'd get a gap at the end of the map
        _camera.LookAt(_position, new Vector2(0, 0));

        // Highlight tile under the mouse
        _mousePosition = Mouse.GetState().Position;
        var worldMouse = _mousePosition + _camera.Position.ToPoint();

        _selectedTile = _isometricMapService.HighlightTile(worldMouse.ToVector2());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.TransformMatrix);

        _isometricMapService.Draw();

        _spriteBatch.End();

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, $"Mouse: {_mousePosition.X}, {_mousePosition.Y}", new Vector2(0, 0), Color.White);
        _spriteBatch.DrawString(_font, $"Camera: {_camera.Position.X}, {_camera.Position.Y}", new Vector2(0, 16), Color.White);
        _spriteBatch.DrawString(_font, $"Selected: {_selectedTile.X}, {_selectedTile.Y}, {_selectedTile.Z}", new Vector2(0, 32), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
