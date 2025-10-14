using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Screens;
using OutrunStyleTest.Screens;
using OutrunStyleTest.Track;
using Shared.Extensions;
using Shared.Services;
using System.Reflection;

namespace OutrunStyleTest;

/// <summary>
/// A simple demo of an outrun style pseudo 3D road system...
/// 
/// A lot of the code based on this example here https://github.com/ssusnic/Pseudo-3d-Racer and a few other examples 
/// I found knocking around on the web and github (see list below). So most of the credit goes to people creating 
/// those projects, this is just a bit of refactoring and some small changes/tweaks for doing it using Monogame.
/// 
/// Use the arrow keys up/down to accelerate and brake, and left/right arrow keys to move the camera left or right. Its 
/// a little rough around the edges and some improvements could do with being made (to my code, not the original).
/// 
/// As well as this being a simple demo to show how to do a pseudo-3D road system, it also shows how to use an ECS 
/// architecture. For this we're using the MonoGame Extended ECS library. The code is also structured using vertical 
/// slices... so all the code for the track is in the Track folder, and all the code for the player is in the Player 
/// folder etc. This keeps all the code for a particular feature together, making it easier to find and work on 
/// later on.
/// 
/// Some useful links I found and used for pseudo 3D road drawing:-
/// 
/// https://www.youtube.com/watch?v=N60lBZDEwJ8&list=PLB_ibvUSN7mzUffhiay5g5GUHyJRO4DYr&index=8
/// http://www.extentofthejam.com/pseudo/
/// https://codeincomplete.com/articles/javascript-racer-v1-straight/
/// https://jakesgordon.com/writing/javascript-racer/
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

        // Set the resolution to 1080p
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        // Add the monogame extended screen manager
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
    }

    /// <summary>
    /// We're using the Microsoft.Extensions.DependencyInjection library for our DI container, so
    /// here we configure all our services and then build the service provider which will be used
    /// </summary>
    /// <returns></returns>
    private ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // The Monogame Extended screen manager needs the 'Game' object... I do find this a
        // little 'ugly', but it is what it is... so we'll register it here so we can inject
        // it later on
        services.AddSingleton<Game>(this);

        // Add services we want to inject, in this example we're going to inject the SpriteBatch service
        // into some other classes eventually via constructor. So lets register SpriteBatch and also
        // the GraphicsDevice (which SpriteBatch needs) so we can do that later on whenever... Note that
        // we're registering Singletons as we only ever should have ONE SpriteBatch in our game!        
        services.AddSingleton(Content);
        services.AddSingleton(GraphicsDevice);

        // Core game services                              
        services.AddSingleton<ShapeDrawingService>();
        services.AddSingleton<SpriteBatch>();
        services.AddSingleton<TrackBuilderService>();
        services.AddSingleton<TrackDrawingService>();

        // Add our ECS world        
        services.AddSingleton<WorldBuilder>();

        // Register ALL our ECS systems in this assembly using our extension method
        services.AddAllImplementationsAsSelf<ISystem>(ServiceLifetime.Singleton, Assembly.GetExecutingAssembly());

        // Add screens
        services.AddSingleton<GamePlayScreen>();

        return services.BuildServiceProvider();
    }

    protected override void Initialize()
    {
        // Create service collection... note that we're not using the Monogame 'container' as it cannot do constructor
        // injection, so instead we're using the standard Microsoft container, although others could be used if preferred
        var serviceProvider = ConfigureServices();

        // Initialize the screen manager with the service provider so it can resolve screens
        base.Initialize();

        // Now, we can use the screen manager to load the first game screen ;-)
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
