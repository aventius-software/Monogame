using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using PlatformerWithTiledMapDemo.Camera;
using PlatformerWithTiledMapDemo.Map;
using PlatformerWithTiledMapDemo.Player;
using PlatformerWithTiledMapDemo.Shared;

namespace PlatformerWithTiledMapDemo.Screens;

internal class GamePlayScreen : GameScreen
{
    private readonly CameraSystem _cameraSystem;
    private readonly DebugSystem _debugSystem;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly MapBackgroundRenderingSystem _mapBackgroundRenderingSystem;
    private readonly MapForegroundRenderingSystem _mapForegroundRenderingSystem;
    private readonly MapInitialisationSystem _mapInitialisationSystem;
    private readonly MapPlatformRenderingSystem _mapPlatformRenderingSystem;
    private readonly PlatformPhysicsSystem _platformPhysicsSystem;
    private readonly PlayerAnimationSystem _playerAnimationSystem;
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly PlayerSpawnSystem _playerSpawnSystem;
    private readonly SpriteRenderingSystem _spriteRenderingSystem;
    private readonly WorldBuilder _worldBuilder;

    private World _world;

    public GamePlayScreen(
        CameraSystem cameraSystem,
        DebugSystem debugSystem,
        Game game,
        GraphicsDevice graphicsDevice,
        MapBackgroundRenderingSystem mapBackgroundRenderingSystem,
        MapForegroundRenderingSystem mapForegroundRenderingSystem,
        MapInitialisationSystem mapInitialisationSystem,
        MapPlatformRenderingSystem mapPlatformRenderingSystem,
        PlatformPhysicsSystem platformPhysicsSystem,
        PlayerAnimationSystem playerAnimationSystem,
        PlayerControlSystem playerControlSystem,
        PlayerSpawnSystem playerSpawnSystem,
        SpriteRenderingSystem spriteRenderingSystem,
        WorldBuilder worldBuilder) : base(game)
    {
        _cameraSystem = cameraSystem;
        _debugSystem = debugSystem;
        _graphicsDevice = graphicsDevice;
        _mapBackgroundRenderingSystem = mapBackgroundRenderingSystem;
        _mapForegroundRenderingSystem = mapForegroundRenderingSystem;
        _mapInitialisationSystem = mapInitialisationSystem;
        _mapPlatformRenderingSystem = mapPlatformRenderingSystem;
        _platformPhysicsSystem = platformPhysicsSystem;
        _playerAnimationSystem = playerAnimationSystem;
        _playerControlSystem = playerControlSystem;
        _playerSpawnSystem = playerSpawnSystem;
        _spriteRenderingSystem = spriteRenderingSystem;
        _worldBuilder = worldBuilder;
    }

    public override void Draw(GameTime gameTime)
    {
        _graphicsDevice.Clear(Color.Black);
        _world.Draw(gameTime);
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
            .AddSystem(_playerAnimationSystem)
            .AddSystem(_platformPhysicsSystem)
            .AddSystem(_cameraSystem)

            // Drawing systems (in order)
            .AddSystem(_mapBackgroundRenderingSystem)
            .AddSystem(_mapPlatformRenderingSystem)
            .AddSystem(_mapForegroundRenderingSystem)

            // Sprites are drawn on top of the map
            .AddSystem(_spriteRenderingSystem)

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
