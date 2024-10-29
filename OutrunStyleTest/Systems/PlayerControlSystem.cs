using Microsoft.Xna.Framework.Input;
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

    private Entity _player;
    private Filter _playerFilter;
    private Entity _track;
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
        // Get player entity
        _playerFilter = World.Filter.With<PlayerComponent>().Build();
        _player = _playerFilter.First();

        // Get track entity
        _trackFilter = World.Filter.With<TrackComponent>().Build();
        _track = _trackFilter.First();

        // Initialise the player
        ref var playerComponent = ref _player.GetComponent<PlayerComponent>();
        playerComponent.Position = Vector3.Zero;
        playerComponent.MaxSpeed = 100f / (1f / 60f);
        playerComponent.Speed = 0;
    }

    public void OnUpdate(float deltaTime)
    {
        ref var playerComponent = ref _player.GetComponent<PlayerComponent>();
        playerComponent.Position.Z += playerComponent.Speed * deltaTime;

        // Get keyboard state
        var keyboardState = Keyboard.GetState();

        // Acceleration and braking
        if (keyboardState.IsKeyDown(Keys.Up) && playerComponent.Speed < playerComponent.MaxSpeed - 10) playerComponent.Speed += 10;
        else if (keyboardState.IsKeyDown(Keys.Down) && playerComponent.Speed > 10) playerComponent.Speed -= 10;
        
        // Update the players position
        ref var trackComponent = ref _track.GetComponent<TrackComponent>();

        if (playerComponent.Position.Z >= trackComponent.Length) playerComponent.Position.Z -= trackComponent.Length;
    }
}
