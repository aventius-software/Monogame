using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledMapWithCamera.Services;

namespace TiledMapWithCamera;

public class GameMain : Game
{
    private Camera _camera;
    private Texture2D _character;
    private GameWorld _gameWorld;
    private GraphicsDeviceManager _graphics;
    private Vector2 _position;
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

        // Load some character texture to move around the map
        _character = Content.Load<Texture2D>("circle");

        // Create map service and load map
        _gameWorld = new GameWorld(_spriteBatch, Content);
        _gameWorld.LoadTiledMap($"{Content.RootDirectory}/test map.tmx", "test tile atlas");

        // Create a camera
        _camera = new Camera();

        // Tell the camera the dimensions of the world
        _camera.SetWorldDimensions(new Vector2(
            _gameWorld.WorldWidth,
            _gameWorld.WorldHeight));

        // Set the camera origin to the middle of the viewport, also note the offset for the size of the character sprite
        _camera.SetOrigin(new Vector2(
            GraphicsDevice.Viewport.Width / 2 - _character.Width / 2,
            GraphicsDevice.Viewport.Height / 2 - _character.Height / 2));

        // Place the character at some 'world' position coordinates
        _position = new Vector2(64, 64);
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

        // Restrict movement to the world
        if (_position.X < 0) _position.X = speed;
        if (_position.X > _gameWorld.WorldWidth - _character.Width) _position.X = _gameWorld.WorldWidth - speed - _character.Width;
        if (_position.Y < 0) _position.Y = speed;
        if (_position.Y > _gameWorld.WorldHeight - _character.Height) _position.Y = _gameWorld.WorldHeight - speed - _character.Height;

        // Set camera to the player position, set offset so we account for the character sprite origin
        // being the top left corner of the sprite, this makes the camera constrain to the end of the
        // map 'minus' the width/height of the character. Otherwise we'd get a gap at the end of the map
        _camera.LookAt(_position, new Vector2(_character.Width, _character.Height));

        // (Optionally) tell map where the player is, this helps the world map
        // drawing restrict tile drawing to only the tiles visible at the
        // specified position within the viewport area also specified. If we
        // comment this line out, the map service will draw all map tiles
        _gameWorld.SetViewport(
            _camera.Position,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Start drawing, notice the 'transformMatrix' which is from our camera
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.TransformMatrix);

        // First draw the map (so it will be under the character)
        _gameWorld.Draw(gameTime);

        // Now draw character after, this way it will be on top of the map
        _spriteBatch.Draw(
            texture: _character,
            position: _position,
            color: Color.White);

        // We're done...
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
