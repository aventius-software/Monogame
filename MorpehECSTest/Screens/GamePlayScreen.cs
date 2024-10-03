using Microsoft.Xna.Framework;
using MorpehECSTest.Entities;
using MorpehECSTest.Systems;
using Scellecs.Morpeh;

namespace MorpehECSTest.Screens;

internal class GamePlayScreen : IScreen
{
    private readonly AccelerationSystem _accelerationSystem;
    private readonly DriftSystem _driftSystem;
    private readonly World _ecsWorld;
    private readonly MovementSystem _movementSystem;
    private readonly PhysicsSystem _physicsSystem;
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly SpriteDrawingSystem _spriteDrawingSystem;
    private readonly SteeringSystem _steeringSystem;
    private readonly VehicleFactory _vehicleFactory;

    private SystemsGroup _renderSystemsGroup;
    private SystemsGroup _updateSystemsGroup;

    public GamePlayScreen(
        World ecsWorld,
        VehicleFactory vehicleFactory,
        AccelerationSystem accelerationSystem,
        DriftSystem driftSystem,
        MovementSystem movementSystem,
        PhysicsSystem physicsSystem,
        PlayerControlSystem playerControlSystem,
        SpriteDrawingSystem spriteDrawingSystem,
        SteeringSystem steeringSystem)
    {
        _ecsWorld = ecsWorld;
        _vehicleFactory = vehicleFactory;

        _accelerationSystem = accelerationSystem;
        _driftSystem = driftSystem;
        _movementSystem = movementSystem;
        _physicsSystem = physicsSystem;
        _playerControlSystem = playerControlSystem;
        _spriteDrawingSystem = spriteDrawingSystem;
        _steeringSystem = steeringSystem;
    }

    public void Draw(GameTime gameTime)
    {
        _renderSystemsGroup.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public void Initialise()
    {
        // Add render systems
        _renderSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _renderSystemsGroup.AddSystem(_spriteDrawingSystem);
        _renderSystemsGroup.Initialize();

        // Add all our update systems - order matters!
        _updateSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _updateSystemsGroup.AddSystem(_accelerationSystem);

        // Must add movement before drift system!
        _updateSystemsGroup.AddSystem(_movementSystem);
        _updateSystemsGroup.AddSystem(_driftSystem);        

        // Rest of them order doesn't matter
        _updateSystemsGroup.AddSystem(_physicsSystem);
        _updateSystemsGroup.AddSystem(_playerControlSystem);
        _updateSystemsGroup.AddSystem(_steeringSystem);
        _updateSystemsGroup.Initialize();

        // Create player entity
        _vehicleFactory.Create(new Vector2(50, 50), 1.5f, true);

        // Create some enemies
        _vehicleFactory.Create(new Vector2(150, 150), 1.5f);
        _vehicleFactory.Create(new Vector2(400, 200), 1.5f);
        _vehicleFactory.Create(new Vector2(50, 300), 1.5f);
        _vehicleFactory.Create(new Vector2(600, 400), 1.5f);
    }

    public void LoadContent()
    {
    }

    public void UnloadContent()
    {
    }

    public void Update(GameTime gameTime)
    {
        _updateSystemsGroup.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
}
