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
    private readonly GraphicsDevice _graphicsDevice;
    private readonly MapInitialisationSystem _mapInitialisationSystem;
    private readonly MapRenderingSystem _mapRenderingSystem;
    private readonly PlatformPhysicsSystem _platformPhysicsSystem;
    private readonly PlayerAnimationSystem _playerAnimationSystem;
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly PlayerSpawnSystem _playerSpawnSystem;
    private readonly SpriteRenderingSystem _spriteRenderingSystem;
    private readonly WorldBuilder _worldBuilder;

    private World _world;

    public GamePlayScreen(
        CameraSystem cameraSystem,
        Game game,
        GraphicsDevice graphicsDevice,
        MapInitialisationSystem mapInitialisationSystem,
        MapRenderingSystem mapRenderingSystem,
        PlatformPhysicsSystem platformPhysicsSystem,
        PlayerAnimationSystem playerAnimationSystem,
        PlayerControlSystem playerControlSystem,
        PlayerSpawnSystem playerSpawnSystem,
        SpriteRenderingSystem spriteRenderingSystem,
        WorldBuilder ecsWorldBuilder) : base(game)
    {
        _cameraSystem = cameraSystem;
        _graphicsDevice = graphicsDevice;
        _mapInitialisationSystem = mapInitialisationSystem;
        _mapRenderingSystem = mapRenderingSystem;
        _platformPhysicsSystem = platformPhysicsSystem;
        _playerAnimationSystem = playerAnimationSystem;
        _playerControlSystem = playerControlSystem;
        _playerSpawnSystem = playerSpawnSystem;
        _spriteRenderingSystem = spriteRenderingSystem;
        _worldBuilder = ecsWorldBuilder;
    }

    public override void Draw(GameTime gameTime)
    {
        _graphicsDevice.Clear(Color.CornflowerBlue);
        _world.Draw(gameTime);
    }

    public override void LoadContent()
    {
        // Add systems to the world
        _world = _worldBuilder

            // Add initialisation systems first
            .AddSystem(_mapInitialisationSystem)
            .AddSystem(_playerSpawnSystem)

            // Update systems
            .AddSystem(_platformPhysicsSystem)
            .AddSystem(_playerControlSystem)
            .AddSystem(_playerAnimationSystem)
            .AddSystem(_cameraSystem)

            // Drawing systems (in order)
            .AddSystem(_mapRenderingSystem)
            .AddSystem(_spriteRenderingSystem)

            // Build the ECS world ;-)
            .Build();

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        _world.Update(gameTime);
    }
}
