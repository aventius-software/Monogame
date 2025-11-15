using IsometricDynamicMapDemo.Map;
using IsometricDynamicMapDemo.Screens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using Shared.Extensions;
using Shared.Services;
using System;
using System.Reflection;

namespace IsometricDynamicMapDemo;

/// <summary>
/// This is a bit more of a dynamic example of an isometric map which works
/// around some of the limitations present in Monogame Extendeds Tiled map
/// isometric functionality. In other words, we've got to build our own
/// map service to draw etc...
/// 
/// This will use DotTiled to read in an initial Tiled map, but will use
/// a custom renderer instead of Monogame Extendeds Tiled map functionality. We'll
/// use Monogame Extendeds ECS system to build simple systems and map service to
/// handle map drawing and updating. We'll follow the 'vertical slices' style
/// architecture to keep everything related together. I've used this in many
/// web projects, but not really seen any game dev stuff use it, but it seems
/// to suit it nicely, so... ;-)
/// 
/// Uses art from: 
/// https://opengameart.org/content/isometric-64x64-outside-tileset
/// 
/// A good reference link for a deeper dive is:
/// https://clintbellanger.net/articles/isometric_math/
/// 
/// Tile types as used in OpenTTD:
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// 
/// What we're aiming for something like the map/terrain style and functionality
/// a little bit similar to games such as OpenTTD, rollercoaster tycoon 1 & 2 and 
/// C&C Tiberian Sun etc...
/// 
/// - multilayer rendering
/// - sprite/object rendering on/between appropriate layers
/// - easy mouse selection of tiles
/// - dynamic terrain deformation and changes
/// - dynamic lighting
/// - simple map rotation
/// - sloped tiles
/// </summary>
public class GameMain : Game
{
    // Since we're using 16 bit pixel style graphics we'll use kind
    // 16 bit style 'virtual' resolution which we'll scale later to
    // whatever screen size
    private const int _virtualResolutionWidth = 1920 / 2, _virtualResolutionHeight = 1080 / 2;

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

        // Try these 2 different frame timing configurations. You may need
        // to alter your graphics card settings to get best 'smoothness' if
        // your settings are overriding game settings (e.g. forcing vsync on)
        //UseFixedFramerate(59);
        UseVariableFramerate();

        // Apply changes
        _graphics.ApplyChanges();

        // Setup the Monogame Extended screen manager
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
    }

    /// <summary>
    /// We'll use Microsoft.Extensions.DependencyInjection to register services so
    /// we can inject services into other classes via constructor injection later on
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
                _virtualResolutionWidth,
                _virtualResolutionHeight);

            return new OrthographicCamera(viewportAdapter);
        });

        // Add our map service to handle loading/rendering of Tiled maps
        //services.AddSingleton<IsometricMapService>();
        services.AddSingleton<DiamondTileMapRenderer>();

        // We'll add our custom render target service so we can use our virtual resolution
        // but scale correctly to all different screen sizes easily        
        services.AddSingleton<CustomRenderTarget>(options =>
        {
            var service = new CustomRenderTarget(GraphicsDevice, options.GetService<SpriteBatch>());
            service.InitialiseRenderDestination(_virtualResolutionWidth, _virtualResolutionHeight, Color.Black);

            return service;
        });

        // Add our ECS world        
        services.AddSingleton<WorldBuilder>();

        // Register all our ECS systems (in this assembly). This little extension method
        // will register all implementations of ISystem found in the assembly specified. Otherwise
        // we'd need to manually register ALL our ECS systems individually/manually...
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

    /// <summary>
    /// Sets the graphics configuration to work with a fixed specified framerate
    /// </summary>
    /// <param name="targetFps"></param>
    private void UseFixedFramerate(int targetFps)
    {
        // For a fixed framerate we need to enable the fixed timestep
        IsFixedTimeStep = true;
        InactiveSleepTime = TimeSpan.Zero; // Helps in some configurations

        // No vsync
        _graphics.SynchronizeWithVerticalRetrace = true;

        // If we want a different target fps from the default (which in Monogame is 60), then
        // we need to set the target 'time elapsed' we want for the specified target fps
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / targetFps));
    }

    /// <summary>
    /// Disables fixed framerate and uses VRR, which if you monitor supports it will
    /// give you lovely smooth motion, but if it doesn't then you'll get screen tearing!
    /// </summary>
    private void UseVariableFramerate()
    {
        // Disable the fixed timestep
        IsFixedTimeStep = false;
        InactiveSleepTime = TimeSpan.Zero; // Helps in some configurations

        // No vsync
        _graphics.SynchronizeWithVerticalRetrace = false;
    }
}
