using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledMapsAndAetherPhysics.Services;
using Scellecs.Morpeh;
using TiledMapsAndAetherPhysics.Components;

namespace TiledMapsAndAetherPhysics.Systems;

/// <summary>
/// We need a system to 'update' the physics engine
/// </summary>
internal class PhysicsSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly MapService _mapService;
    private readonly PhysicsWorld _physicsWorld;

    public PhysicsSystem(PhysicsWorld physicsWorld, GraphicsDevice graphicsDevice, MapService mapService)
    {
        _physicsWorld = physicsWorld;
        _graphicsDevice = graphicsDevice;
        _mapService = mapService;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // For this 'simulation' we don't use gravity so...
        _physicsWorld.Gravity = Vector2.Zero;

        // For the physics simulation to work correctly we need to indicate how many pixels
        // on the screen correspond to how many simulation units. So 'X' number of pixels
        // for 1 metre in the physics simulation. Our car is 64 pixels long, so if we say
        // the car is about 4 metres then 64/4 = 16. So 16 pixels will be 1 metre in the
        // physics simulation. The same thinking must also apply to other unit conversions!
        _physicsWorld.SetDisplayUnitToSimUnitRatio(16);

        // Create 'edges' for the physics engine so we don't go off screen
        var topLeft = new Vector2(0, 0);
        var topRight = new Vector2(_mapService.WorldWidth, 0);
        var bottomLeft = new Vector2(0, _mapService.WorldHeight);
        var bottomRight = new Vector2(_mapService.WorldWidth, _mapService.WorldHeight);

        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(topRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topRight), _physicsWorld.ToSimUnits(bottomRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(bottomLeft), _physicsWorld.ToSimUnits(bottomRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(bottomLeft));

        // Build our entity filter
        _filter = World.Filter.With<RigidBodyComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        // Simulate our physics stuff ;-)
        _physicsWorld.Step(deltaTime);

        // For each entity...
        foreach (var entity in _filter)
        {
            // Get the components we want
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var rigidBodyComponent = ref entity.GetComponent<RigidBodyComponent>();

            // Set game world position (not physics simulation position)
            transformComponent.Position = _physicsWorld.ToDisplayUnits(rigidBodyComponent.Body.Position);
            transformComponent.Rotation = rigidBodyComponent.Body.Rotation;
        }
    }
}
