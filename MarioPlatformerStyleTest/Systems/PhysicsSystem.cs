using MarioPlatformerStyleTest.Components;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// Handle simple physics of the game
/// </summary>
internal class PhysicsSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;
    private float _gravity;

    public PhysicsSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Build our filter
        _filter = World.Filter.With<TransformComponent>().Build();

        // Set initial gravity
        _gravity = 500f;
    }
    
    public void OnUpdate(float deltaTime)
    {
        // Apply physics (like gravity) to all entities
        foreach (var entity in _filter)
        {            
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            var modifiedGravity = _gravity;

            // Special case for the player character
            if (entity.Has<PlayerComponent>())
            {
                // Get the player component
                ref var playerComponent = ref entity.GetComponent<PlayerComponent>();

                // If the player is jumping
                if (transformComponent.IsMovingUpwards && !playerComponent.IsJumpPressed)
                {
                    // If the player is jumping and not holding the jump button, reduce
                    // the jump a bit quicker so they just do a small/low jump. Then
                    // if they are holding the jump button they'll do a normal 'full' jump
                    modifiedGravity = _gravity * playerComponent.LowJumpGravityMultiplier;
                }
                else if (transformComponent.IsMovingDownwards)
                {
                    // When falling, we can tweak the fall speed a little to make
                    // the player drop faster than normal gravity would make them...
                    modifiedGravity = _gravity * playerComponent.FallingGravityMultiplier;
                }
            }

            transformComponent.Velocity.Y += modifiedGravity * deltaTime;
        }
    }
}
