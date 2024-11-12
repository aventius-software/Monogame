using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using TiledMapsAndAetherPhysics.Components;

namespace TiledMapsAndAetherPhysics.Systems;

/// <summary>
/// Depending on your 'stance' you may or may not agree with this... but we're 
/// going to use a system to render our sprites
/// </summary>
internal class SpriteRenderSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;
    private readonly SpriteBatch _spriteBatch;

    public SpriteRenderSystem(World world, SpriteBatch spriteBatch)
    {
        World = world;
        _spriteBatch = spriteBatch;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<TransformComponent>()
            .With<SpriteComponent>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var spriteComponent = ref entity.GetComponent<SpriteComponent>();

            // Draw this vehicle
            _spriteBatch.Draw(
                texture: spriteComponent.Texture,
                position: transformComponent.Position,
                sourceRectangle: null,
                color: Color.White,
                rotation: transformComponent.Rotation,
                origin: spriteComponent.Origin,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
