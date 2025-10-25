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
        // Get the components we need to work with
        var physicsComponent = _physicsMapper.Get(entityId);
        var playerComponent = _playerMapper.Get(entityId);

        // Get the current keyboard state
        var keyboardState = Keyboard.GetState();

        if (physicsComponent.IsOnGround)
        {
            // If we're on the ground we always first reset to idle state
            playerComponent.State = CharacterState.Idle;

            // Is the player trying to jump?
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                physicsComponent.Velocity.Y -= physicsComponent.JumpStrength;
                playerComponent.State = CharacterState.Jumping;
            }
        }

        // Handle left/right movement input
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            physicsComponent.Velocity.X -= physicsComponent.RunAcceleration;
            playerComponent.State = CharacterState.Walking;
            playerComponent.Facing = FacingState.Left;
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            physicsComponent.Velocity.X += physicsComponent.RunAcceleration;
            playerComponent.State = CharacterState.Walking;
            playerComponent.Facing = FacingState.Right;
        }

        // Clamp horizontal velocity to max running speed
        physicsComponent.Velocity.X = MathHelper.Clamp(physicsComponent.Velocity.X, -physicsComponent.MaximumRunningSpeed, physicsComponent.MaximumRunningSpeed);

        // Apply some friction to the player's horizontal movement
        physicsComponent.Velocity.X *= physicsComponent.GroundFriction;
    }
}
