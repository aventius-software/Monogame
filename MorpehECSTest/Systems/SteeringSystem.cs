using Microsoft.Xna.Framework;
using MorpehECSTest.Components;
using Scellecs.Morpeh;
using System;

namespace MorpehECSTest.Systems;

/// <summary>
/// We need a system to handle steering, note that this is to 'simulate' steering the vehicle
/// its not an input control system!
/// </summary>
internal class SteeringSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;

    public SteeringSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        _filter = World.Filter.With<SteeringComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            // Get references for our components            
            ref var angularDragComponent = ref entity.GetComponent<AngularDragComponent>();
            ref var rigidBodyComponent = ref entity.GetComponent<RigidBodyComponent>();
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var engineComponent = ref entity.GetComponent<EngineComponent>();
            ref var steeringComponent = ref entity.GetComponent<SteeringComponent>();

            // Steers left or right depending on the X direction
            if (transformComponent.Direction.X == 0)
            {
                // We're not turning left or right, so slowly reduce the angular damping so that the vehicle doesn't keep turning forever
                rigidBodyComponent.Body.AngularDamping = MathHelper.Lerp(rigidBodyComponent.Body.AngularDamping, angularDragComponent.Drag, angularDragComponent.RateOfChange * deltaTime);
            }
            else
            {
                // We're turning one way or the other, so apply some angular drag
                rigidBodyComponent.Body.AngularDamping = angularDragComponent.Drag;
            }

            // Apply steering to (increase) angular velocity, note we take into account the forward/linear velocity
            // as we want to turn less if we are moving forward/reverse slower. A very slow moving car doesn't have
            // a high turning speed, so this is how we (roughly) simulate that feeling
            rigidBodyComponent.Body.AngularVelocity += transformComponent.Direction.X * MathHelper.ToRadians(steeringComponent.TurnSpeed) * (Math.Abs(rigidBodyComponent.Body.LinearVelocity.Length()) / engineComponent.MaxForwardSpeed);
        }
    }
}
