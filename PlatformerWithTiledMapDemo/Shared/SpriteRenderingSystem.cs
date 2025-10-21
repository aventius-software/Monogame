using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace PlatformerWithTiledMapDemo.Shared;

internal class SpriteRenderingSystem : EntityDrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly SpriteBatch _spriteBatch;    

    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
    private ComponentMapper<Sprite> _spriteMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public SpriteRenderingSystem(SpriteBatch spriteBatch, OrthographicCamera camera)
        : base(Aspect.All(typeof(Transform2)).One(typeof(AnimatedSprite), typeof(Sprite)))
    {
        _spriteBatch = spriteBatch;
        _camera = camera;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
        _spriteMapper = mapperService.GetMapper<Sprite>();
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

        foreach (var entity in ActiveEntities)
        {
            var sprite = _animatedSpriteMapper.Has(entity)
                ? _animatedSpriteMapper.Get(entity)
                : _spriteMapper.Get(entity);

            var transform = _transformMapper.Get(entity);

            if (sprite is AnimatedSprite animatedSprite)
                animatedSprite.Update(gameTime);

            _spriteBatch.Draw(sprite, transform);
        }

        _spriteBatch.End();
    }
}
