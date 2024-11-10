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
            transformComponent.Velocity.Y += _gravity * deltaTime;
        }
    }
}
