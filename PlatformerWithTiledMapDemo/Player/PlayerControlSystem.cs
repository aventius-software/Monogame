using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using PlatformerWithTiledMapDemo.Shared.Characters;
using PlatformerWithTiledMapDemo.Shared.Physics;

namespace PlatformerWithTiledMapDemo.Player;

internal class PlayerControlSystem : EntityProcessingSystem
{
    private ComponentMapper<CharacterComponent> _characterMapper;
    private ComponentMapper<PhysicsComponent> _physicsMapper;

    public PlayerControlSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(CharacterComponent), typeof(PhysicsComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _characterMapper = mapperService.GetMapper<CharacterComponent>();
        _physicsMapper = mapperService.GetMapper<PhysicsComponent>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        // Get the components we need to work with
        var characterComponent = _characterMapper.Get(entityId);
        var physicsComponent = _physicsMapper.Get(entityId);

        // Get the current keyboard state
        var keyboardState = Keyboard.GetState();

        // First, we always reset to idle if we're on the ground. Later, if the
        // player is moving left or right (and they're on the ground), then we'll
        // update their state accordingly
        if (physicsComponent.IsOnGround)
            characterComponent.State = CharacterState.Idle;

        // Now, we handle left/right movement input
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            // Turn left and accelerate up to our 'running' velocity
            characterComponent.Facing = FacingState.Left;
            physicsComponent.Velocity.X -= physicsComponent.RunAcceleration;

            // If the player is on the ground and they're moving left
            // or right then they must be running
            if (physicsComponent.IsOnGround)
                characterComponent.State = CharacterState.Running;
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            // Turn right and accelerate up to our 'running' velocity
            characterComponent.Facing = FacingState.Right;
            physicsComponent.Velocity.X += physicsComponent.RunAcceleration;

            // If the player is on the ground and they're moving left
            // or right then they must be running
            if (physicsComponent.IsOnGround)
                characterComponent.State = CharacterState.Running;
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
                characterComponent.State = CharacterState.Jumping;
            }
        }

        // Here will make some small adjustments to jumping control so players can jump lower or
        // higher. First, check if the player is jumping, but NOT holding down the jump button...
        if (characterComponent.State == CharacterState.Jumping && !keyboardState.IsKeyDown(Keys.Up))
        {
            // In this case, we reduce the jump a bit quicker so they just do a small/low jump
            physicsComponent.GravityMultiplier = 2f;
        }
        else if (physicsComponent.IsMovingDownwards)
        {
            // When falling, we could tweak the fall speed a little to make
            // the player drop faster than normal gravity would make them...
            physicsComponent.GravityMultiplier = 1f;
        }
        else
        {
            // If we're here then either the player is holding the jump button
            // down (in which case, we do a normal jump) or we're not jumping. In
            // this case, set the multiplier to '1' which will make NO change to
            // the gravity value...
            physicsComponent.GravityMultiplier = 1f;
        }

        // Clamp horizontal velocity to max horizontal speed
        physicsComponent.Velocity.X = MathHelper.Clamp(physicsComponent.Velocity.X, -physicsComponent.MaximumHorizontalSpeed, physicsComponent.MaximumHorizontalSpeed);

        // Apply some friction to the player's horizontal movement if they're on
        // the ground or a bit of air drag if they're in the air
        physicsComponent.Velocity.X *= physicsComponent.IsOnGround ? physicsComponent.GroundFriction : physicsComponent.AirDrag;
    }
}
