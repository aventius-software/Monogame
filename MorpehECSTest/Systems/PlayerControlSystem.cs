using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MorpehECSTest.Components;
using Scellecs.Morpeh;

namespace MorpehECSTest.Systems;

/// <summary>
/// This system reads input to allow control of the player. Basically the input device sets
/// stuff in the transform component which is used to handle movement/position ;-)
/// </summary>
internal class PlayerControlSystem : ISystem
{
    public World World { get; set; }
    private Filter filter;

    public PlayerControlSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        filter = World.Filter.With<PlayerComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        // Get keyboard state
        var keyboardState = Keyboard.GetState();

        foreach (var entity in filter)
        {
            // Get the movement component
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var driftComponent = ref entity.GetComponent<DriftComponent>();

            // Remember to reset the input direction on each update!
            transformComponent.Direction = Vector2.Zero;

            // Acceleration and braking
            if (keyboardState.IsKeyDown(Keys.Up)) transformComponent.Direction.Y = 1;
            else if (keyboardState.IsKeyDown(Keys.Down)) transformComponent.Direction.Y = -1;

            // Turning
            if (keyboardState.IsKeyDown(Keys.Left)) transformComponent.Direction.X = -1;
            else if (keyboardState.IsKeyDown(Keys.Right)) transformComponent.Direction.X = 1;

            // Press space to skid/drift ;-)
            if (keyboardState.IsKeyDown(Keys.Space)) driftComponent.IsSkidding = true;
            else driftComponent.IsSkidding = false;
        }
    }
}
