using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace OutrunStyleTest.Player;

/// <summary>
/// This creates and initialises the player entity and sets up the player component.
/// </summary>
internal class PlayerInitialisationSystem : EntitySystem
{
    public PlayerInitialisationSystem() : base(Aspect.All(typeof(PlayerComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Create the player entity
        var playerEntity = CreateEntity();
        playerEntity.Attach(new PlayerComponent());

        // Get the player component
        var playerComponent = playerEntity.Get<PlayerComponent>();

        // Initialise the player
        playerComponent.AccelerationRate = 20;
        playerComponent.Position = Vector3.Zero;
        playerComponent.MaxSpeed = 5000f;
        playerComponent.Speed = 0;
        playerComponent.SteeringStrength = 30f;
    }
}
