using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using PlatformerWithTiledMapDemo.Shared;
using System;

namespace PlatformerWithTiledMapDemo.Player;

internal class PlayerSpawnSystem : EntitySystem
{
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
        var texture = _contentManager.Load<Texture2D>("Player/p1_spritesheet");
        var textureAtlas = Texture2DAtlas.Create("PlayerAtlas", texture, 72, 96);
        var spriteSheet = new SpriteSheet("PlayerSpriteSheet", textureAtlas);

        // Define the animations
        TimeSpan duration = TimeSpan.FromSeconds(0.1);

        spriteSheet.DefineAnimation(nameof(PlayerAnimationState.Idle), builder =>
        {
            builder.IsLooping(true)
                .AddFrame(0, duration)
                .AddFrame(1, duration);
        });

        // Create the player entity
        var entity = CreateEntity();
        entity.Attach(new AnimatedSprite(spriteSheet, nameof(PlayerAnimationState.Idle)));
        entity.Attach(new Transform2(new Vector2(0, 0)));
        entity.Attach(new PhysicsComponent());
        entity.Attach(new PlayerComponent());
    }
}
