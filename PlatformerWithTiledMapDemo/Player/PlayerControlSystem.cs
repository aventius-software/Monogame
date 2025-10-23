using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using PlatformerWithTiledMapDemo.Shared;

namespace PlatformerWithTiledMapDemo.Player;

internal class PlayerControlSystem : EntityProcessingSystem
{
    private ComponentMapper<PhysicsComponent> _physicsMapper;
    private ComponentMapper<PlayerComponent> _playerMapper;

    public PlayerControlSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(PhysicsComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _physicsMapper = mapperService.GetMapper<PhysicsComponent>();
        _playerMapper = mapperService.GetMapper<PlayerComponent>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {        
        var physicsComponent = _physicsMapper.Get(entityId);
        var playerComponent = _playerMapper.Get(entityId);

        var keyboardState = Keyboard.GetState();

        if (physicsComponent.IsOnGround)
        {
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                physicsComponent.Velocity.Y -= physicsComponent.JumpStrength;
                playerComponent.State = CharacterState.Jumping;
            }
        }

        if (keyboardState.IsKeyDown(Keys.Right))
        {
            physicsComponent.Velocity.X += physicsComponent.MoveSpeed;
            playerComponent.State = CharacterState.Walking;
            playerComponent.Facing = FacingState.Right;
        }

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            physicsComponent.Velocity.X -= physicsComponent.MoveSpeed;
            playerComponent.State = CharacterState.Walking;
            playerComponent.Facing = FacingState.Left;
        }

        // Apply some friction to the player's horizontal movement
        physicsComponent.Velocity.X *= physicsComponent.GroundFriction;
    }
}
