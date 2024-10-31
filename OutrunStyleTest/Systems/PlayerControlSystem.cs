﻿using Microsoft.Xna.Framework.Input;
using OutrunStyleTest.Components;
using Scellecs.Morpeh;
using System.Numerics;

namespace OutrunStyleTest.Systems;

/// <summary>
/// This system reads input to allow control of the player, there's nothing fancy
/// going here. Some improvements would/could be steering resistance or friction
/// on different surfaces etc...
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
        playerComponent.AccelerationRate = 10;
        playerComponent.Position = Vector3.Zero;
        playerComponent.MaxSpeed = 10000f;
        playerComponent.Speed = 0;
        playerComponent.SteeringRate = 30f;
    }

    public void OnUpdate(float deltaTime)
    {
        // Move the player forward (Z position) according to the players speed
        ref var playerComponent = ref _player.GetComponent<PlayerComponent>();
        playerComponent.Position.Z += playerComponent.Speed * deltaTime;

        // Get keyboard state
        var keyboardState = Keyboard.GetState();

        // Acceleration and braking
        if (keyboardState.IsKeyDown(Keys.Up) && playerComponent.Speed < playerComponent.MaxSpeed - playerComponent.AccelerationRate)
        {
            playerComponent.Speed += playerComponent.AccelerationRate;
        }
        else if (keyboardState.IsKeyDown(Keys.Down) && playerComponent.Speed > playerComponent.AccelerationRate)
        {
            playerComponent.Speed -= playerComponent.AccelerationRate;
        }

        // Steering
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            playerComponent.Position.X -= playerComponent.SteeringRate;
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            playerComponent.Position.X += playerComponent.SteeringRate;
        }

        // Update the players position
        ref var trackComponent = ref _track.GetComponent<TrackComponent>();

        // Back to start if finished
        if (playerComponent.Position.Z >= trackComponent.Length) playerComponent.Position.Z -= trackComponent.Length;
    }
}