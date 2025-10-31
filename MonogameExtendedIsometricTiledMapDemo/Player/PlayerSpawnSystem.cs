using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonogameExtendedIsometricTiledMapDemo.Shared.Physics;

namespace MonogameExtendedIsometricTiledMapDemo.Player;

internal class PlayerSpawnSystem : EntitySystem
{    
    public PlayerSpawnSystem() : base(Aspect.All(typeof(PlayerComponent)))
    {        
    }
    
    public override void Initialize(IComponentMapperService mapperService)
    {        
        // Create the player entity
        var entity = CreateEntity();
        entity.Attach(new Transform2(new Vector2(0, 0)));
        entity.Attach(new PhysicsComponent());
        entity.Attach(new PlayerComponent());
    }
}
