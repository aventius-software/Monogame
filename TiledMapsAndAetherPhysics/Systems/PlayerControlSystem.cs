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
        //ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();
        //ref var transformComponent = ref _playerEntity.GetComponent<TransformComponent>();
        ref var rigidBodyComponent = ref _playerEntity.GetComponent<RigidBodyComponent>();

        // Do player stuff like checking controls etc...
        var keyboard = Keyboard.GetState();

        // Set velocity
        //transformComponent.Velocity = Vector2.Zero;

        //if (keyboard.IsKeyDown(Keys.Left)) transformComponent.Velocity.X = -transformComponent.Acceleration;
        //else if (keyboard.IsKeyDown(Keys.Right)) transformComponent.Velocity.X = transformComponent.Acceleration;

        //if (keyboard.IsKeyDown(Keys.Up)) transformComponent.Velocity.Y = -transformComponent.Acceleration;
        //else if (keyboard.IsKeyDown(Keys.Down)) transformComponent.Velocity.Y = transformComponent.Acceleration;

        //transformComponent.Position += transformComponent.Velocity * deltaTime;

        if (keyboard.IsKeyDown(Keys.Left)) rigidBodyComponent.Body.ApplyForce(new Vector2(-10, 0));
        else if (keyboard.IsKeyDown(Keys.Right)) rigidBodyComponent.Body.ApplyForce(new Vector2(10, 0));

        if (keyboard.IsKeyDown(Keys.Up)) rigidBodyComponent.Body.ApplyForce(new Vector2(0, -10));
        else if (keyboard.IsKeyDown(Keys.Down)) rigidBodyComponent.Body.ApplyForce(new Vector2(0, 10));
    }
}
