using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using PlatformerWithTiledMapDemo.Map;
using PlatformerWithTiledMapDemo.Screens;
using Shared.Extensions;
using Shared.Services;
using System.Reflection;

namespace PlatformerWithTiledMapDemo;

/// <summary>
/// Demo game showing how to use Tiled maps in a platformer game. Uses MonoGame and MonoGame.Extended
/// and game art from Kenney.nl (www.kenney.nl). See https://opengameart.org/content/platformer-art-deluxe
/// </summary>
public class GameMain : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly ScreenManager _screenManager;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Set the preferred back buffer size (window size)
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        // Setup the Monogame Extended screen manager component
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
    }

    /// <summary>
    /// We'll use Microsoft.Extensions.DependencyInjection to register services so
    /// we can inject them via constructor injection later on
    /// </summary>
    /// <returns></returns>
    private ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // The Monogame Extended screen manager needs the 'Game' object, so
        // we'll register it here so we can inject it later on
        services.AddSingleton<Game>(this);

        // Add services we want to inject, in this example we're going to inject the SpriteBatch service
        // into some other classes eventually via constructor. So lets register SpriteBatch and also
        // the GraphicsDevice (which SpriteBatch needs) so we can do that later on whenever... Note that
        // we're registering Singletons as we only ever should have ONE SpriteBatch in our game!        
        services.AddSingleton(Content);
        services.AddSingleton(GraphicsDevice);

        // Core game services                        
        services.AddSingleton<SpriteBatch>();

        // Setup our camera and viewport, see link to documentation below
        // https://www.monogameextended.net/docs/features/camera/orthographic-camera/
        services.AddSingleton<OrthographicCamera>(options =>
        {
        // Setup a viewport adapter to handle different screen sizes/aspect ratios
        var viewportAdapter = new BoxingViewportAdapter(
            Window,
            GraphicsDevice,
            320, 240);// GraphicsDevice.Viewport.Width,
                //GraphicsDevice.Viewport.Height);

            return new OrthographicCamera(viewportAdapter);
        });

        // Register the Tiled map renderer so it can be injected later on
        services.AddSingleton<TiledMapRenderer>(options =>
        {
            return new TiledMapRenderer(GraphicsDevice);
        });

        // Add our map service to handle loading/rendering of Tiled maps, which
        // uses the TiledMapRenderer internally to do its job
        services.AddSingleton<MapService>();

        // We'll add our custom render target service so we can use a virtual resolution
        // and scale to different screen sizes more easily
        services.AddSingleton<CustomRenderTarget>();

        // Add our ECS world        
        services.AddSingleton<WorldBuilder>();

        // Register all our ECS systems (in this assembly). This extension method
        // will register all implementations of ISystem found in the assembly. The
        // code for this is in the Shared project in the Extensions folder.
        services.AddAllImplementationsAsSelf<ISystem>(ServiceLifetime.Singleton, Assembly.GetExecutingAssembly());

        // Add screens
        services.AddSingleton<GamePlayScreen>();

        return services.BuildServiceProvider();
    }

    protected override void Initialize()
    {
        // Create service collection (not using the Monogame 'container' as it cannot do constructor
        // injection), so instead we're using the standard Microsoft container ;-)
        var serviceProvider = ConfigureServices();

        // Initialize the screen manager with the service provider so it can resolve screens
        base.Initialize();

        // Now we can use the screen manager to load the first game screen
        _screenManager.LoadScreen(serviceProvider.GetService<GamePlayScreen>());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // All update logic is now handled by the screen management service        
        base.Update(gameTime);
    }
}
