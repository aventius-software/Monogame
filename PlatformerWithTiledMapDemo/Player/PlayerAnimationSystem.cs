using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace PlatformerWithTiledMapDemo.Player;

internal class PlayerAnimationSystem : EntityProcessingSystem
{
    private ComponentMapper<PlayerComponent> _playerMapper;
    private ComponentMapper<AnimatedSprite> _spriteMapper;

    public PlayerAnimationSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(AnimatedSprite)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _playerMapper = mapperService.GetMapper<PlayerComponent>();
        _spriteMapper = mapperService.GetMapper<AnimatedSprite>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        var player = _playerMapper.Get(entityId);
        var sprite = _spriteMapper.Get(entityId);
    }
}
