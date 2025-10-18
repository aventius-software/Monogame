using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;

namespace MonogameExtendedTiledMapDemo;

/// <summary>
/// This is just the code example taken from the MonoGame.Extended Tiled map rendering example. See the link
/// https://www.monogameextended.net/docs/features/tiled/ for this. Don't forget about the setup for the content 
/// pipeline extensions which this needs, see link below...
/// here https://www.monogameextended.net/docs/getting-started/installation-monogame/#optional-set-up-mgcb-editor
/// </summary>
public class GameMain : Game
{
    private OrthographicCamera _camera;
    private Vector2 _cameraPosition;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private TiledMap _tiledMap;
    private TiledMapRenderer _tiledMapRenderer;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        BoxingViewportAdapter viewportAdapter = new(Window, GraphicsDevice, 800, 600);
        _camera = new OrthographicCamera(viewportAdapter);

        // Set initial camera position to show map from top-left
        _cameraPosition = _camera.Origin;
        _camera.LookAt(_cameraPosition);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _tiledMap = Content.Load<TiledMap>("tiled/samplemap");
        _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _tiledMapRenderer.Update(gameTime);

        MoveCamera(gameTime);
        _camera.LookAt(_cameraPosition);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // Save current blend state and set to AlphaBlend for proper transparency
        BlendState previousBlendState = GraphicsDevice.BlendState;
        GraphicsDevice.BlendState = BlendState.AlphaBlend;

        _tiledMapRenderer.Draw(_camera.GetViewMatrix());

        // Restore previous blend state
        GraphicsDevice.BlendState = previousBlendState;

        base.Draw(gameTime);
    }

    private Vector2 GetMovementDirection()
    {
        Vector2 movementDirection = Vector2.Zero;
        KeyboardState state = Keyboard.GetState();

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

        // Normalize to prevent faster diagonal movement
        if (movementDirection != Vector2.Zero)
        {
            movementDirection.Normalize();
        }

        return movementDirection;
    }

    private void MoveCamera(GameTime gameTime)
    {
        float speed = 200f;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 movementDirection = GetMovementDirection();

        _cameraPosition += speed * movementDirection * deltaTime;
    }
}
