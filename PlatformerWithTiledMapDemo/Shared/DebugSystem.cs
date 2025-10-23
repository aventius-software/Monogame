using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System.Linq;

namespace PlatformerWithTiledMapDemo.Shared;

internal class DebugSystem : EntityDrawSystem
{
    private readonly ContentManager _contentManager;
    private readonly SpriteBatch _spriteBatch;

    private SpriteFont _font;
    private ComponentMapper<PhysicsComponent> _physicsMapper;

    public DebugSystem(ContentManager contentManager, SpriteBatch spriteBatch) : base(Aspect.All(typeof(PhysicsComponent)))
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;
    }

    public override void Draw(GameTime gameTime)
    {
        var entityId = ActiveEntities.First();
        var physicsComponent = _physicsMapper.Get(entityId);

        _spriteBatch.Begin();
        
        _spriteBatch.DrawString(
            spriteFont: _font,
            text: $"Is on the ground: {physicsComponent.IsOnGround}",
            position: new Vector2(10, 10),
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 2f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.DrawString(
            spriteFont: _font,
            text: $"Y velocity: {physicsComponent.Velocity.Y}",
            position: new Vector2(10, 40),
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 2f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _physicsMapper = mapperService.GetMapper<PhysicsComponent>();
        _font = _contentManager.Load<SpriteFont>("Fonts/Default");
    }
}
