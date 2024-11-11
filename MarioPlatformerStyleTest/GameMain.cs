using MarioPlatformerStyleTest.Screens;
using MarioPlatformerStyleTest.Services;
using MarioPlatformerStyleTest.Systems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest;

public class GameMain : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private ScreenManagementService _screenManagementService;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Create service collection (not using the Monogame 'container' as it cannot do constructor
        // injection), so instead we're using the standard Microsoft container ;-)
        var services = new ServiceCollection();

        // Add services we want to inject, in this example we're going to inject the SpriteBatch service
        // into some other classes eventually via constructor. So lets register SpriteBatch and also
        // the GraphicsDevice (which SpriteBatch needs) so we can do that later on whenever... Note that
        // we're registering Singletons as we only ever should have ONE SpriteBatch in our game!
        services.AddSingleton(GraphicsDevice);
        services.AddSingleton<SpriteBatch>();
        services.AddSingleton(Content);

        // Now our normal services        
        services.AddSingleton<GamePlayScreen>();
        services.AddSingleton<ScreenCollection, GameScreenCollection>();
        services.AddSingleton<ScreenManagementService>();
        services.AddSingleton<Camera>();
        services.AddSingleton<MapService>();

        // Add our ECS world
        services.AddSingleton<World>(options =>
        {
            return World.Create();
        });

        // Our ECS systems
        services.AddSingleton<CameraSystem>();
        services.AddSingleton<MapRenderSystem>();
        services.AddSingleton<PlatformCollisionSystem>();
        services.AddSingleton<PlayerControlSystem>();
        services.AddSingleton<PlayerRenderSystem>();
        services.AddSingleton<PhysicsSystem>();

        // Add our ECS initialisers
        services.AddSingleton<PlayerInitialiser>();

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // We don't need to get the SpriteBatch like we did before, or anything else.. we only need to get the
        // screen management service that this class requires. The SpriteBatch will be created by the container
        // since the screen management service needs it to be injected via its constructor...
        // ah, the joys of DI ;-)
        _screenManagementService = serviceProvider.GetService<ScreenManagementService>();
        _screenManagementService.ChangeScreen<GamePlayScreen>();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // All update logic is now handled by the screen management service
        _screenManagementService.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // All draw functionality is now handled by the screen management service
        _screenManagementService.Draw(gameTime);

        base.Draw(gameTime);
    }
}
