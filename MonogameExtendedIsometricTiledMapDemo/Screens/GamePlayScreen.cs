using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using MonogameExtendedIsometricTiledMapDemo.Camera;
using MonogameExtendedIsometricTiledMapDemo.Map;
using MonogameExtendedIsometricTiledMapDemo.Player;
using MonogameExtendedIsometricTiledMapDemo.Shared.Debugging;
using Shared.Services;

namespace MonogameExtendedIsometricTiledMapDemo.Screens;

internal class GamePlayScreen : GameScreen
{
    private readonly CameraSystem _cameraSystem;
    private readonly CustomRenderTarget _customRenderTarget;
    private readonly DebugSystem _debugSystem;
    private readonly MapDrawingSystem _mapDrawingSystem;
    private readonly MapInitialisationSystem _mapInitialisationSystem;
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly PlayerSpawnSystem _playerSpawnSystem;
    private readonly WorldBuilder _worldBuilder;

    private World _world;

    public GamePlayScreen(
        CameraSystem cameraSystem,
        CustomRenderTarget customRenderTarget,
        DebugSystem debugSystem,
        Game game,
        MapDrawingSystem mapDrawingSystem,
        MapInitialisationSystem mapInitialisationSystem,
        PlayerControlSystem playerControlSystem,
        PlayerSpawnSystem playerSpawnSystem,
        WorldBuilder worldBuilder) : base(game)
    {
        _cameraSystem = cameraSystem;
        _customRenderTarget = customRenderTarget;
        _debugSystem = debugSystem;
        _mapDrawingSystem = mapDrawingSystem;
        _mapInitialisationSystem = mapInitialisationSystem;
        _playerControlSystem = playerControlSystem;
        _playerSpawnSystem = playerSpawnSystem;
        _worldBuilder = worldBuilder;
    }

    public override void Draw(GameTime gameTime)
    {
        // Tell the system we want to render to the custom render target
        _customRenderTarget.Begin();

        // Now draw everything as normal
        _world.Draw(gameTime);

        // Finally, draw the render target to the screen
        _customRenderTarget.Draw();
    }

    public override void LoadContent()
    {
        // Add systems to the ECS world
        _world = _worldBuilder

            // Add initialisation systems first
            .AddSystem(_mapInitialisationSystem)
            .AddSystem(_playerSpawnSystem)

            // Update systems            
            .AddSystem(_playerControlSystem)
            .AddSystem(_cameraSystem)

            // Drawing systems (in order)
            .AddSystem(_mapDrawingSystem)

            // Other systems...
            .AddSystem(_debugSystem)

            // Build the ECS world ;-)
            .Build();

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        _world.Update(gameTime);
    }
}
