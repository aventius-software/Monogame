using MarioPlatformerStyleTest.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// This initialiser just sets up the player entity ready for gameplay ;-)
/// </summary>
internal class PlayerInitialiser : IInitializer
{
    public World World { get; set; }

    private readonly ContentManager _contentManager;

    public PlayerInitialiser(World world, ContentManager contentManager)
    {
        World = world;
        _contentManager = contentManager;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Find the player entity
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        var playerEntity = playerFilter.First();

        // Configure players gravity multipliers
        ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
        playerComponent.LowJumpGravityMultiplier = 1.25f;
        playerComponent.FallingGravityMultiplier = 1f;

        // Load player textures and set jump strength
        ref var characterComponent = ref playerEntity.GetComponent<CharacterComponent>();
        characterComponent.Texture ??= _contentManager.Load<Texture2D>("character");
        characterComponent.JumpStrength = 450;

        // Set initial position and speed
        ref var transformComponent = ref playerEntity.GetComponent<TransformComponent>();
        transformComponent.Width = characterComponent.Texture.Width;
        transformComponent.Height = characterComponent.Texture.Height;
        transformComponent.Speed = 250;
        transformComponent.Position = new Vector2(150, 0);
    }
}
