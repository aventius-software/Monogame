using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using TiledMapsAndAetherPhysics.Components;
using TiledMapsAndAetherPhysics.Services;
using Physics = nkast.Aether.Physics2D.Dynamics;

namespace TiledMapsAndAetherPhysics.Systems;

/// <summary>
/// This initialiser just sets up the player entity ready for gameplay ;-)
/// </summary>
internal class PlayerInitialiser : IInitializer
{
    public World World { get; set; }

    private readonly ContentManager _contentManager;
    private readonly PhysicsWorld _physicsWorld;

    public PlayerInitialiser(World world, ContentManager contentManager, PhysicsWorld physicsWorld)
    {
        World = world;
        _contentManager = contentManager;
        _physicsWorld = physicsWorld;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Find the player entity
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        var playerEntity = playerFilter.First();

        // Remove components just incase (if we're starting again)                        
        playerEntity.RemoveComponent<RigidBodyComponent>();
        playerEntity.RemoveComponent<SpriteComponent>();
        playerEntity.RemoveComponent<TransformComponent>();

        // Configure the player component
        ref var playerComponent = ref playerEntity.AddComponent<PlayerComponent>();

        // Load textures
        ref var spriteComponent = ref playerEntity.AddComponent<SpriteComponent>();
        spriteComponent.Texture ??= _contentManager.Load<Texture2D>("circle");
        spriteComponent.Origin = new Vector2(spriteComponent.Texture.Width / 2, spriteComponent.Texture.Height / 2);

        // Set physics engine stuff        
        ref var rigidBodyComponent = ref playerEntity.AddComponent<RigidBodyComponent>();
        rigidBodyComponent.Body = _physicsWorld.CreateRectangle(          
            width: _physicsWorld.ToSimUnits(spriteComponent.Texture.Width),
            height: _physicsWorld.ToSimUnits(spriteComponent.Texture.Height),
            density: 0f,
            position: _physicsWorld.ToSimUnits(new Vector2(300, 300)),
            rotation: 0f,
            bodyType: Physics.BodyType.Dynamic);

        // Add a transform component to store our position
        ref var transformComponent = ref playerEntity.AddComponent<TransformComponent>();
        transformComponent.Position = _physicsWorld.ToDisplayUnits(rigidBodyComponent.Body.Position);        
    }
}
