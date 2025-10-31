using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonogameExtendedIsometricTiledMapDemo.Shared.Physics;

namespace MonogameExtendedIsometricTiledMapDemo.Player;

internal class PlayerControlSystem : EntityProcessingSystem
{    
    private ComponentMapper<PhysicsComponent> _physicsMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public PlayerControlSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(PhysicsComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {        
        _physicsMapper = mapperService.GetMapper<PhysicsComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        // Get the components we need to work with        
        var physicsComponent = _physicsMapper.Get(entityId);
        var transformComponent = _transformMapper.Get(entityId);

        // Get the current keyboard state
        var keyboardState = Keyboard.GetState();

        // First, we always reset to idle if we're on the ground. Later, if the
        // player is moving left or right (and they're on the ground), then we'll
        // update their state accordingly
        physicsComponent.Velocity = Vector2.Zero;

        // Now, we handle left/right movement input
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            // Turn left and accelerate up to our 'running' velocity            
            physicsComponent.Velocity.Y = -physicsComponent.RunAcceleration;
        }
        else if (keyboardState.IsKeyDown(Keys.Down))
        {
            // Turn right and accelerate up to our 'running' velocity            
            physicsComponent.Velocity.Y = physicsComponent.RunAcceleration;
        }

        // Now, we handle left/right movement input
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            // Turn left and accelerate up to our 'running' velocity            
            physicsComponent.Velocity.X = -physicsComponent.RunAcceleration;
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            // Turn right and accelerate up to our 'running' velocity            
            physicsComponent.Velocity.X = physicsComponent.RunAcceleration;            
        }

        transformComponent.Position += physicsComponent.Velocity;
    }
}
