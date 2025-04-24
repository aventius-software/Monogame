using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace DotTiledTest;

/// <summary>
/// Simple test of the DotTiled 'Tiled' map loader. See https://dcronqvist.github.io/DotTiled/docs/quickstart.html
/// Also uses the camera from Monogame.Extended to show movement around the map
/// </summary>
public class GameMain : Game
{
    private OrthographicCamera _camera;
    private GraphicsDeviceManager _graphics;
    private MapService _mapService;
    private SpriteBatch _spriteBatch;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Initialise/load the map
        _mapService = new MapService(_spriteBatch, Content);
        _mapService.LoadTiledMap("test map.tmx", "test tile atlas");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        const float movementSpeed = 200;
        _camera.Move(GetMovementDirection() * movementSpeed * gameTime.GetElapsedSeconds());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

        // Begin drawing for sprites
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.GetViewMatrix());

        // Draw map
        _mapService.Draw();

        // Done...
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Vector2 GetMovementDirection()
    {
        var movementDirection = Vector2.Zero;
        var state = Keyboard.GetState();

        if (state.IsKeyDown(Keys.Down))
        {
            movementDirection += Vector2.UnitY;
        }
        if (state.IsKeyDown(Keys.Up))
        {
            movementDirection -= Vector2.UnitY;
        }
        if (state.IsKeyDown(Keys.Left))
        {
            movementDirection -= Vector2.UnitX;
        }
        if (state.IsKeyDown(Keys.Right))
        {
            movementDirection += Vector2.UnitX;
        }

        return movementDirection;
    }
}
