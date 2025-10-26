using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace PlatformerWithTiledMapDemo.Shared.Characters;

internal class SpriteAnimationSystem : EntityUpdateSystem
{
    private ComponentMapper<CharacterComponent> _characterMapper;
    private ComponentMapper<AnimatedSprite> _spriteMapper;

    public SpriteAnimationSystem() : base(Aspect.All(typeof(CharacterComponent), typeof(AnimatedSprite)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _characterMapper = mapperService.GetMapper<CharacterComponent>();
        _spriteMapper = mapperService.GetMapper<AnimatedSprite>();
    }

    public override void Update(GameTime gameTime)
    {        
        foreach (var entityId in ActiveEntities)
        {
            var character = _characterMapper.Get(entityId);
            var sprite = _spriteMapper.Get(entityId);

            // Set the sprite facing in the correct direction
            sprite.Effect = character.Facing == FacingState.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Change animation depending on the characters state
            switch (character.State)
            {
                case CharacterState.Jumping:
                    if (sprite.CurrentAnimation != nameof(CharacterAnimationState.Jumping))
                        sprite.SetAnimation(nameof(CharacterAnimationState.Jumping));
                    break;

                case CharacterState.Running:
                    if (sprite.CurrentAnimation != nameof(CharacterAnimationState.Running))
                        sprite.SetAnimation(nameof(CharacterAnimationState.Running));
                    break;

                case CharacterState.Idle:
                    if (sprite.CurrentAnimation != nameof(CharacterAnimationState.Idle))
                        sprite.SetAnimation(nameof(CharacterAnimationState.Idle));
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
                    sprite.SetAnimation(nameof(CharacterAnimationState.Idle));
                    break;
            }

            sprite.Update(gameTime);
        }
    }
}
