using MarioPlatformerStyleTest.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using System.Linq;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// Handle player character drawing
/// </summary>
internal class PlayerRenderSystem : ISystem
{
    public World World { get; set; }

    private Entity _playerEntity;
    private readonly SpriteBatch _spriteBatch;

    public PlayerRenderSystem(World world, SpriteBatch spriteBatch)
    {
        World = world;
        _spriteBatch = spriteBatch;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Find the player entity
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        _playerEntity = playerFilter.First();
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the components
        ref var characterComponent = ref _playerEntity.GetComponent<CharacterComponent>();
        ref var transformComponent = ref _playerEntity.GetComponent<TransformComponent>();

        // Draw the player        
        _spriteBatch.Draw(texture: characterComponent.Texture, position: transformComponent.Position, color: Color.White);
    }
}
