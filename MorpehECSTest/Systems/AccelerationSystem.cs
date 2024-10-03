using Microsoft.Xna.Framework;
using MorpehECSTest.Components;
using Scellecs.Morpeh;

namespace MorpehECSTest.Systems;

/// <summary>
/// This system handles acceleration (and braking) for vehicles
/// </summary>
internal class AccelerationSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;

    public AccelerationSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        _filter = World.Filter.With<EngineComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            // Get references for our components
            ref var engineComponent = ref entity.GetComponent<EngineComponent>();
            ref var linearDragComponent = ref entity.GetComponent<LinearDragComponent>();
            ref var rigidBodyComponent = ref entity.GetComponent<RigidBodyComponent>();
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();

            // If we've reached max forward speed, do nothing more - just return...
            if (transformComponent.ForwardSpeed > engineComponent.MaxForwardSpeed && transformComponent.Direction.Y > 0) break;
            if (transformComponent.ForwardSpeed < -engineComponent.MaxReversingSpeed && transformComponent.Direction.Y < 0) break;
            if (rigidBodyComponent.Body.LinearVelocity.LengthSquared() > engineComponent.MaxForwardSpeed * engineComponent.MaxForwardSpeed && transformComponent.Direction.Y > 0) break;

            // If the car is accelerating or braking we don't apply any drag/friction (yes, not totally accurate), but if
            // the car isn't accelerating or braking we do apply linear 'drag' (friction) so it will bring the car to a
            // halt if no input (accelerate/brake) is pressed
            if (transformComponent.Direction.Y == 0)
            {
                // We're NOT accelerating or braking, so apply a 'gradual' drag to slow us down and bring the vehicle to a halt
                rigidBodyComponent.Body.LinearDamping = MathHelper.Lerp(rigidBodyComponent.Body.LinearDamping, linearDragComponent.Drag, linearDragComponent.RateOfChange * deltaTime);
            }
            else
            {
                // We ARE accelerating or braking, so don't apply any drag
                rigidBodyComponent.Body.LinearDamping = 0;
            }

            // Finally apply the engine force in the 'forward' direction
            rigidBodyComponent.Body.ApplyForce(transformComponent.ForwardVector * engineComponent.EnginePower * transformComponent.Direction.Y);
        }
    }
}
