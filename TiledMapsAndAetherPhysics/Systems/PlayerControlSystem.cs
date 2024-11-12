using TiledMapsAndAetherPhysics.Components;
using Microsoft.Xna.Framework.Input;
using Scellecs.Morpeh;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TiledMapsAndAetherPhysics.Systems;

/// <summary>
/// Handle player controls by checking the input device (e.g. keyboard) to see
/// what we need to make the player do depending on what the user has pressed
/// </summary>
internal class PlayerControlSystem : ISystem
{
    public World World { get; set; }

    private Entity _playerEntity;

    public PlayerControlSystem(World world)
    {
        World = world;
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
        ref var rigidBodyComponent = ref _playerEntity.GetComponent<RigidBodyComponent>();

        // Do player stuff like checking controls etc...
        var keyboard = Keyboard.GetState();

        // Apply forces to the player in the X axis
        if (keyboard.IsKeyDown(Keys.Left)) rigidBodyComponent.Body.ApplyForce(new Vector2(-10, 0));
        else if (keyboard.IsKeyDown(Keys.Right)) rigidBodyComponent.Body.ApplyForce(new Vector2(10, 0));

        // Apply forces to the player in the Y axis
        if (keyboard.IsKeyDown(Keys.Up)) rigidBodyComponent.Body.ApplyForce(new Vector2(0, -10));
        else if (keyboard.IsKeyDown(Keys.Down)) rigidBodyComponent.Body.ApplyForce(new Vector2(0, 10));
    }
}
