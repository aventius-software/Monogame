using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using PlatformerWithTiledMapDemo.Shared;
using System;

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

        sprite.Effect = player.Facing == FacingState.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        switch (player.State)
        {
            case CharacterState.Jumping:
                if (sprite.CurrentAnimation != nameof(PlayerAnimationState.Jumping))
                    sprite.SetAnimation(nameof(PlayerAnimationState.Jumping));

                //sprite.Effect = player.Facing == FacingState.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                break;

            case CharacterState.Running:
                if (sprite.CurrentAnimation != nameof(PlayerAnimationState.Running))
                    sprite.SetAnimation(nameof(PlayerAnimationState.Running));

                //sprite.Effect = player.Facing == FacingState.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                
                break;
            
            case CharacterState.Idle:
                if (sprite.CurrentAnimation != nameof(PlayerAnimationState.Idle))
                    sprite.SetAnimation(nameof(PlayerAnimationState.Idle));
                break;

            //case EntityState.Kicking:
            //    if (sprite.CurrentAnimation != "kick")
            //        sprite.SetAnimation("kick").OnAnimationEvent += (s, e) =>
            //        {
            //            if (e == AnimationEventTrigger.AnimationCompleted)
            //            {
            //                player.State = State.Idle;
            //            }
            //        };
            //    break;
            //case EntityState.Punching:
            //    if (sprite.CurrentAnimation != "punch")
            //        sprite.SetAnimation("punch").OnAnimationEvent += (s, e) =>
            //        {
            //            if (e == AnimationEventTrigger.AnimationCompleted)
            //            {
            //                player.State = State.Idle;
            //            }
            //        };
            //    break;
            //case EntityState.Cool:
            //    if (sprite.CurrentAnimation != "cool")
            //        sprite.SetAnimation("cool");
            //    break;
            default:
                sprite.SetAnimation(nameof(PlayerAnimationState.Idle));
                break;
        }
    }
}
