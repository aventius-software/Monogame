using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using OpenTTDStyleIsometricMap.Services;

namespace OpenTTDStyleIsometricMap;

public class GameMain : Game
{
    private const float MinZoom = 0.5f;
    private const float MaxZoom = 5f;

    private OrthographicCamera _camera;
    private GraphicsDeviceManager _graphics;
    private IsometricMapService _isometricMapService;
    private SpriteBatch _spriteBatch;
    private MouseState _previousMouseState;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1920,
            PreferredBackBufferHeight = 1080
        };

        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Setup a viewport adapter to handle different screen sizes/aspect ratios
        var viewportAdapter = new BoxingViewportAdapter(
            Window,
            GraphicsDevice,
            _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight);

        _camera = new OrthographicCamera(viewportAdapter)
        {
            // Default zoom level - you can adjust this as needed
            Zoom = 2f
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load the isometric map service and its resources
        _isometricMapService = new IsometricMapService(Content, _spriteBatch, _camera);
        _isometricMapService.LoadTileTextureAtlas("tiles_grass");

        // Define some tiles with different slope types to populate the map with - in a real
        // game you would likely have many more of these and load them from some kind of level
        // data file, but for this demo we'll just hardcode a few to show the concept
        var none = new Tile
        {
            SlopeType = SlopeType.NONE
        };

        var flat = new Tile
        {
            SlopeType = SlopeType.SLOPE_FLAT
        };

        var slopeE = new Tile
        {
            SlopeType = SlopeType.SLOPE_E
        };

        var slopeW = new Tile
        {
            SlopeType = SlopeType.SLOPE_W
        };

        var slopeNW = new Tile
        {
            SlopeType = SlopeType.SLOPE_NW
        };

        var slopeN = new Tile
        {
            SlopeType = SlopeType.SLOPE_N
        };

        var slopeSteepN = new Tile
        {
            SlopeType = SlopeType.SLOPE_STEEP_N
        };

        // Set the map data in the isometric map service - this is a 3D array
        // of tiles where the first dimension represents elevation, the second
        // dimension represents the Y coordinate, and the third dimension
        // represents the X coordinate... this is a bit of a poor little
        // map but it allows us to demonstrate different slope types and
        // elevations in a small area
        _isometricMapService.SetMap(new Tile[,,]
        {
            // Elevation 0
            {
                // Top left corner
                // Y=0          Y=1             Y=2             Y=3         Y=4     Y=5     // Top right corner
                { none,         none,           none,           slopeNW,    flat,   flat }, // X=0
                { none,         none,           none,           slopeN,     flat,   flat }, // X=1
                { none,         none,           slopeN,         flat,       flat,   flat }, // X=2
                { flat,         flat,           flat,           flat,       flat,   flat }, // X=3
                { flat,         flat,           flat,           flat,       flat,   flat }, // X=4
                { flat,         flat,           flat,           flat,       flat,   flat }  // X=5                
            },

            // Elevation 1
            {
                { none,         none,           slopeNW,        none,       none,   none },
                { none,         none,           slopeSteepN,    none,       none,   none },
                { none,         slopeSteepN,    none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none }
            },

            // Elevation 2
            {
                { none,         slopeNW,        none,           none,       none,   none },
                { none,         slopeSteepN,    none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none }
            },

            // Elevation 3
            {
                { slopeNW,      none,           none,           none,       none,   none },
                { slopeSteepN,  none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none },
                { none,         none,           none,           none,       none,   none }
            },
        });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Move around the map
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
            _camera.Move(new Vector2(-5, 0));
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
            _camera.Move(new Vector2(5, 0));
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
            _camera.Move(new Vector2(0, -5));
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
            _camera.Move(new Vector2(0, 5));

        // Get mouse position in screen coordinates
        var mouseState = Mouse.GetState();
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        // Use the mouse wheel to zoom in and out
        if (mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
            _camera.Zoom *= 1.1f; // Zoom in
        else if (mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
            _camera.Zoom /= 1.1f; // Zoom out

        _camera.Zoom = MathHelper.Clamp(_camera.Zoom, MinZoom, MaxZoom);

        _previousMouseState = mouseState;

        // Update the isometric map service with the current mouse position
        _isometricMapService.SetActiveScreenCoordinates(mousePosition);

        // Update the isometric map service (e.g., for hover effects)
        _isometricMapService.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.WhiteSmoke);

        // Start drawing with the camera's view matrix and point sampling for pixel art
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

        // Draw the isometric map
        _isometricMapService.Draw();

        // Done...
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
