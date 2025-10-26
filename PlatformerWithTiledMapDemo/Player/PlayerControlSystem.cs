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

        // First, we always reset to idle if we're on the ground. Later, if the
        // player is moving left or right (and they're on the ground), then we'll
        // update their state accordingly
        if (physicsComponent.IsOnGround)
            playerComponent.State = CharacterState.Idle;

        // Now, we handle left/right movement input
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            // Turn left and accelerate up to our 'running' velocity
            playerComponent.Facing = FacingState.Left;
            physicsComponent.Velocity.X -= physicsComponent.RunAcceleration;

            // If the player is on the ground and they're moving left
            // or right then they must be running
            if (physicsComponent.IsOnGround)
                playerComponent.State = CharacterState.Running;
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            // Turn right and accelerate up to our 'running' velocity
            playerComponent.Facing = FacingState.Right;
            physicsComponent.Velocity.X += physicsComponent.RunAcceleration;

            // If the player is on the ground and they're moving left
            // or right then they must be running
            if (physicsComponent.IsOnGround)
                playerComponent.State = CharacterState.Running;
        }

        // Finally, after checking for left/right movement, we can test for
        // jumping. But the player can ONLY jump if they're on the ground. We
        // do this last because otherwise the left/right movement checks and
        // player state changes would take precedence over the jump state if
        // the player moved left or right. So, by testing for jumping last
        // then the jump state (and thus animation) will take precedence
        if (physicsComponent.IsOnGround)
        {
            // We're on the ground, so the player is able to jump, so
            // check to see if they've pushed the jump buttom
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                physicsComponent.Velocity.Y -= physicsComponent.JumpStrength;
                playerComponent.State = CharacterState.Jumping;
            }
        }

        // Clamp horizontal velocity to max horizontal speed
        physicsComponent.Velocity.X = MathHelper.Clamp(physicsComponent.Velocity.X, -physicsComponent.MaximumHorizontalSpeed, physicsComponent.MaximumHorizontalSpeed);

        // Apply some friction to the player's horizontal movement if they're on
        // the ground or a bit of air drag if they're in the air
        physicsComponent.Velocity.X *= physicsComponent.IsOnGround ? physicsComponent.GroundFriction : physicsComponent.AirDrag;
    }
}
