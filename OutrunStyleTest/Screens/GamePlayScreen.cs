using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using OutrunStyleTest.Camera;
using OutrunStyleTest.Player;
using OutrunStyleTest.Track;

namespace OutrunStyleTest.Screens;

/// <summary>
/// This is the main gameplay screen where all the action happens and the ECS world is created and updated.
/// </summary>
internal class GamePlayScreen : GameScreen
{
    private readonly CameraInitialisationSystem _cameraInitialisationSystem;
    private readonly CameraUpdateSystem _cameraUpdateSystem;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly PlayerInitialisationSystem _playerInitialisationSystem;
    private readonly PlayerMovementSystem _playerMovementSystem;
    private readonly TrackInitialisationSystem _trackInitialisationSystem;
    private readonly TrackRenderSystem _trackRenderSystem;
    private readonly TrackUpdateSystem _trackUpdateSystem;
    private readonly WorldBuilder _worldBuilder;

    private World _world;

    public GamePlayScreen(
        CameraInitialisationSystem cameraInitialisationSystem,
        CameraUpdateSystem cameraUpdateSystem,
        Game game,
        GraphicsDevice graphicsDevice,
        PlayerControlSystem playerControlSystem,
        PlayerInitialisationSystem playerInitialisationSystem,
        PlayerMovementSystem playerMovementSystem,
        TrackInitialisationSystem trackInitialisationSystem,
        TrackRenderSystem trackRenderSystem,
        TrackUpdateSystem trackUpdateSystem,
        WorldBuilder worldBuilder) : base(game)
    {
        _cameraInitialisationSystem = cameraInitialisationSystem;
        _cameraUpdateSystem = cameraUpdateSystem;
        _graphicsDevice = graphicsDevice;
        _playerControlSystem = playerControlSystem;
        _playerInitialisationSystem = playerInitialisationSystem;
        _playerMovementSystem = playerMovementSystem;
        _trackInitialisationSystem = trackInitialisationSystem;
        _trackRenderSystem = trackRenderSystem;
        _trackUpdateSystem = trackUpdateSystem;
        _worldBuilder = worldBuilder;
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the screen
        _graphicsDevice.Clear(Color.CornflowerBlue);

        // Draw the world
        _world.Draw(gameTime);
    }

    public override void LoadContent()
    {
        // Add systems to the ESC world
        _world = _worldBuilder

            // Add initialisation systems first
            .AddSystem(_cameraInitialisationSystem)
            .AddSystem(_trackInitialisationSystem)
            .AddSystem(_playerInitialisationSystem)

            // Then add all our update systems - note that the order matters!
            .AddSystem(_playerControlSystem)
            .AddSystem(_playerMovementSystem)
            .AddSystem(_cameraUpdateSystem)
            .AddSystem(_trackUpdateSystem)

            // After that, we add any drawing/rendering systems
            .AddSystem(_trackRenderSystem)

            // Then finally, build the ECS world ;-)
            .Build();

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        _world.Update(gameTime);
    }
}
