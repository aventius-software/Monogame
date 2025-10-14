using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System.Linq;

namespace OutrunStyleTest.Player;

/// <summary>
/// This system reads input to allow control of the player, there's nothing very fancy going on here.
/// </summary>
internal class PlayerControlSystem : EntityProcessingSystem
{
    private int _playerEntityId;

    public PlayerControlSystem() : base(Aspect.All(typeof(PlayerComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Save the entity id's we'll need
        _playerEntityId = ActiveEntities.Single(id => GetEntity(id).Has<PlayerComponent>());
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        // Get keyboard state
        var keyboardState = Keyboard.GetState();

        // We'll need to reference the player component
        var playerComponent = GetEntity(_playerEntityId).Get<PlayerComponent>();

        // Acceleration and braking
        if (keyboardState.IsKeyDown(Keys.Up) && playerComponent.Speed < playerComponent.MaxSpeed - playerComponent.AccelerationRate)
        {
            // Increase the players speed
            playerComponent.Speed += playerComponent.AccelerationRate;
        }
        else if (keyboardState.IsKeyDown(Keys.Down) && playerComponent.Speed > playerComponent.AccelerationRate)
        {
            // Slow the players speed
            playerComponent.Speed -= playerComponent.AccelerationRate;
        }

        // Steering
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            // Move the player left
            playerComponent.Position.X -= playerComponent.SteeringStrength;
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            // Move the player right
            playerComponent.Position.X += playerComponent.SteeringStrength;
        }
    }
}
