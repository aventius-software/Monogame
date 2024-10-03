using Microsoft.Xna.Framework;
using MorpehECSTest.Components;
using MorpehECSTest.Physics;
using Scellecs.Morpeh;
using System;

namespace MorpehECSTest.Systems;

/// <summary>
/// This system handles movement depending on velocity, direction etc...
/// </summary>
internal class MovementSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;
    private readonly PhysicsWorld _physicsWorld;

    public MovementSystem(World world, PhysicsWorld physicsWorld)
    {
        World = world;
        _physicsWorld = physicsWorld;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        _filter = World.Filter.With<TransformComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            // Get references for our components            
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var rigidBodyComponent = ref entity.GetComponent<RigidBodyComponent>();

            // Set movement vectors and velocities
            transformComponent.ForwardVector = new((float)Math.Cos(rigidBodyComponent.Body.Rotation), (float)Math.Sin(rigidBodyComponent.Body.Rotation));
            transformComponent.ForwardVelocity = transformComponent.ForwardVector * Vector2.Dot(rigidBodyComponent.Body.LinearVelocity, transformComponent.ForwardVector);
            transformComponent.RightVector = new((float)Math.Cos(rigidBodyComponent.Body.Rotation + Math.PI / 2), (float)Math.Sin(rigidBodyComponent.Body.Rotation + Math.PI / 2));
            transformComponent.RightVelocity = transformComponent.RightVector * Vector2.Dot(rigidBodyComponent.Body.LinearVelocity, transformComponent.RightVector);

            // Now we can calculate a forward speed, position and rotation
            transformComponent.ForwardSpeed = Vector2.Dot(rigidBodyComponent.Body.LinearVelocity, transformComponent.ForwardVector);
            transformComponent.Position = _physicsWorld.ToDisplayUnits(rigidBodyComponent.Body.Position);
            transformComponent.Rotation = rigidBodyComponent.Body.Rotation;
        }
    }
}
