using OutrunStyleTest.Components;
using Scellecs.Morpeh;
using System.Numerics;

namespace OutrunStyleTest.Systems;

/// <summary>
/// This system reads input to allow control of the player. Basically the input device sets
/// stuff in the transform component which is used to handle movement/position ;-)
/// </summary>
internal class PlayerControlSystem : ISystem
{
    public World World { get; set; }

    private Filter _playerFilter;
    private Filter _trackFilter;

    public PlayerControlSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Setup filters
        _playerFilter = World.Filter.With<PlayerComponent>().Build();
        _trackFilter = World.Filter.With<TrackComponent>().Build();

        // Initialise the player
        var player = _playerFilter.First();

        ref var playerComponent = ref player.GetComponent<PlayerComponent>();
        playerComponent.Position = Vector3.Zero;
        playerComponent.MaxSpeed = 100f / (1f / 60f);
        playerComponent.Speed = 0;
    }

    public void OnUpdate(float deltaTime)
    {
        // Get keyboard state
        //var keyboardState = Keyboard.GetState();

        var player = _playerFilter.First();
        ref var playerComponent = ref player.GetComponent<PlayerComponent>();
        playerComponent.Position.Z = playerComponent.Speed * deltaTime;

        var track = _trackFilter.First();
        ref var trackComponent = ref track.GetComponent<TrackComponent>();        

        if (playerComponent.Position.Z >= trackComponent.Length) playerComponent.Position.Z -= trackComponent.Length;

        // Get the movement component
        //ref var transformComponent = ref entity.GetComponent<TransformComponent>();
        //ref var driftComponent = ref entity.GetComponent<DriftComponent>();

        //// Remember to reset the input direction on each update!
        //transformComponent.Direction = Vector2.Zero;

        //// Acceleration and braking
        //if (keyboardState.IsKeyDown(Keys.Up)) transformComponent.Direction.Y = 1;
        //else if (keyboardState.IsKeyDown(Keys.Down)) transformComponent.Direction.Y = -1;

        //// Turning
        //if (keyboardState.IsKeyDown(Keys.Left)) transformComponent.Direction.X = -1;
        //else if (keyboardState.IsKeyDown(Keys.Right)) transformComponent.Direction.X = 1;

        //// Press space to skid/drift ;-)
        //if (keyboardState.IsKeyDown(Keys.Space)) driftComponent.IsSkidding = true;
        //else driftComponent.IsSkidding = false;        
    }
}
