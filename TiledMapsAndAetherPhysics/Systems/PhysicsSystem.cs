using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledMapsAndAetherPhysics.Services;
using Scellecs.Morpeh;
using TiledMapsAndAetherPhysics.Components;
using Physics = nkast.Aether.Physics2D.Dynamics;

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
        // on the screen correspond to how many simulation units. Since the physics engine works
        // in metric units. So you need to give it a value to say 'X' number of pixels in the
        // game world equals 1 metre in the physics simulation. So if our character is 64 pixels
        // wide, and if we say the character in real life (or in your game world) would be say about
        // 2 metres then 64/2 = 32. That would mean that 32 pixels on screen will be equal to 1 metre
        // in the physics simulation. The same thinking must also apply to other unit conversions! So
        // here we set a ratio of pixels (in the game world) to metres (in the physics simulation). Don't
        // set the ratio too small, like say 1, as this would mean 1 pixel = 1 metre, and all movement and
        // forces and length limits would seem slow or small and the physics engine would hit its limits!
        _physicsWorld.SetDisplayUnitToSimUnitRatio(32);

        // Create 'edges' for the physics engine so we don't go off screen
        var topLeft = new Vector2(0, 0);
        var topRight = new Vector2(_mapService.WorldWidth, 0);
        var bottomLeft = new Vector2(0, _mapService.WorldHeight);
        var bottomRight = new Vector2(_mapService.WorldWidth, _mapService.WorldHeight);

        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(topRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topRight), _physicsWorld.ToSimUnits(bottomRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(bottomLeft), _physicsWorld.ToSimUnits(bottomRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(bottomLeft));

        // We've added an object layer in the Tiled map called 'Physics objects'. In it we've 'drawn'
        // rectangles around all the rectangular shapes in the map that we want to add to the physics
        // simulation as rectangles. Alternatively you could also add polygons in Tiled and in the
        // physics engine for more complex map shapes. Or, you could create small rectangles for
        // specific types of tile that you want to be part of the physics simulation
        var physicsObjectLayer = _mapService.GetObjectLayer("Physics objects");

        foreach (var physicsObject in physicsObjectLayer.Objects)
        {
            var width = (int)physicsObject.Width;
            var height = (int)physicsObject.Height;
            var x = physicsObject.X + (width / 2);
            var y = physicsObject.Y + (height / 2);

            _physicsWorld.CreateRectangle(
                width: _physicsWorld.ToSimUnits(width),
                height: _physicsWorld.ToSimUnits(height),
                density: 1,
                position: _physicsWorld.ToSimUnits(new Vector2(x, y)),
                rotation: 0,
                bodyType: Physics.BodyType.Static);
        }

        // Build our entity filter, basically we want any entity with a physics body component ;-)
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

            // Set/save game world position (not physics simulation position) and rotation for
            // other systems to use. Like when the sprite rendering happens (so that those other
            // systems don't need to think about unit conversion or worry about physics)
            transformComponent.Position = _physicsWorld.ToDisplayUnits(rigidBodyComponent.Body.Position);
            transformComponent.Rotation = rigidBodyComponent.Body.Rotation;
        }
    }
}
