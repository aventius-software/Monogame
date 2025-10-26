using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using PlatformerWithTiledMapDemo.Shared.Characters;
using PlatformerWithTiledMapDemo.Shared.Physics;
using System;

namespace PlatformerWithTiledMapDemo.Player;

internal class PlayerSpawnSystem : EntitySystem
{
    private const int _frameWidth = 50, _frameHeight = 37;
    private const int _hitboxWidth = 20, _hitboxHeight = 23;
    private const int _hitboxOffsetX = 14, _hitboxOffsetY = 14;

    private readonly ContentManager _contentManager;

    public PlayerSpawnSystem(ContentManager contentManager) : base(Aspect.All(typeof(PlayerComponent)))
    {
        _contentManager = contentManager;
    }

    /// <summary>
    /// Initialise the player entity with an animated sprite. For more information on animated sprites 
    /// in MonoGame.Extended, see https://www.monogameextended.net/docs/features/2d-animations/spritesheet/
    /// and https://www.monogameextended.net/docs/features/2d-animations/animatedsprite/
    /// </summary>
    /// <param name="mapperService"></param>
    public override void Initialize(IComponentMapperService mapperService)
    {
        // Load the sprite sheet
        var texture = _contentManager.Load<Texture2D>("Player/adventurer-Sheet");
        var textureAtlas = Texture2DAtlas.Create("PlayerAtlas", texture,
            regionWidth: _frameWidth,
            regionHeight: _frameHeight,
            margin: 0,
            spacing: 0);

        var spriteSheet = new SpriteSheet("PlayerSpriteSheet", textureAtlas);

        // Define the animations
        TimeSpan duration = TimeSpan.FromSeconds(0.1);

        spriteSheet.DefineAnimation(nameof(CharacterAnimationState.Idle), builder =>
        {
            builder.IsLooping(true)
                .AddFrame(0, duration)
                .AddFrame(1, duration)
                .AddFrame(2, duration)
                .AddFrame(3, duration);
        });

        spriteSheet.DefineAnimation(nameof(CharacterAnimationState.Running), builder =>
        {
            builder.IsLooping(true)
                .AddFrame(8, duration)
                .AddFrame(9, duration)
                .AddFrame(10, duration)
                .AddFrame(11, duration)
                .AddFrame(12, duration)
                .AddFrame(13, duration);
        });

        spriteSheet.DefineAnimation(nameof(CharacterAnimationState.Jumping), builder =>
        {
            builder.IsLooping(true)
                .AddFrame(14, duration)
                .AddFrame(15, duration)
                .AddFrame(16, duration)
                .AddFrame(17, duration)
                .AddFrame(18, duration)
                .AddFrame(19, duration)
                .AddFrame(20, duration)
                .AddFrame(21, duration)
                .AddFrame(22, duration)
                .AddFrame(23, duration);
        });
        
        // Create the player entity
        var entity = CreateEntity();
        entity.Attach(new AnimatedSprite(spriteSheet, nameof(CharacterAnimationState.Idle)));
        entity.Attach(new CharacterComponent());
        entity.Attach(new Transform2(new Vector2(0, 0)));
        entity.Attach(new PhysicsComponent { CollisionBoxOffsetBounds = new RectangleF(_hitboxOffsetX, _hitboxOffsetY, _hitboxWidth, _hitboxHeight) });
        entity.Attach(new PlayerComponent());
    }
}
