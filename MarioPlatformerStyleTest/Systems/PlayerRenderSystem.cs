using MarioPlatformerStyleTest.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using System.Linq;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// Handle player controls by checking the input device (e.g. keyboard) to see
/// what we need to make the player do depending on what the user has pressed
/// </summary>
internal class PlayerRenderSystem : ISystem
{
    public World World { get; set; }

    private readonly ContentManager _contentManager;
    private Entity _playerEntity;
    private Texture2D _playerTexture;
    private readonly SpriteBatch _spriteBatch;

    public PlayerRenderSystem(World world, SpriteBatch spriteBatch, ContentManager contentManager)
    {
        World = world;
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Find the player entity
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        _playerEntity = playerFilter.First();

        _playerTexture = _contentManager.Load<Texture2D>("circle");
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the component
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();

        // Draw the player        
        _spriteBatch.Draw(texture: _playerTexture, position: playerComponent.Position, color: Color.White);
    }
}
