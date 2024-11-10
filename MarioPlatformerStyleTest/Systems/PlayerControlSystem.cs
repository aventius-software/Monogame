﻿using MarioPlatformerStyleTest.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Scellecs.Morpeh;
using System.Linq;

namespace MarioPlatformerStyleTest.Systems;

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

        // Set players initial position
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();
        playerComponent.Position = new Vector2(150, 0);
    }

    const int GRAVITY = 500;
    const int JUMP = 400;
    public void OnUpdate(float deltaTime)
    {
        // Get the component
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();

        // Do player stuff like checking controls etc...
        var keyboard = Keyboard.GetState();

        //var direction = Vector2.Zero;
        var speed = 150;

        //if (keyboard.IsKeyDown(Keys.Up)) direction.Y = -speed;
        //if (keyboard.IsKeyDown(Keys.Down)) direction.Y = speed;
        
        if (keyboard.IsKeyDown(Keys.Left)) playerComponent.Velocity.X = -speed;
        else if (keyboard.IsKeyDown(Keys.Right)) playerComponent.Velocity.X = speed;
        else playerComponent.Velocity.X = 0;

        // Update the players position
        //playerComponent.Velocity = direction;
        //playerComponent.Position += playerComponent.Velocity;

        playerComponent.Velocity.Y += GRAVITY * deltaTime;

        if (keyboard.IsKeyDown(Keys.Space) && playerComponent.IsOnTheGround)
        {
            playerComponent.Velocity.Y = -JUMP;
        }

        // Restrict movement to the world
        //if (playerComponent.Position.X < 0) playerComponent.Position.X = speed;
        //if (playerComponent.Position.X > _gameWorld.WorldWidth - _character.Width) _position.X = _gameWorld.WorldWidth - speed - _character.Width;
        //if (playerComponent.Position.Y < 0) _position.Y = speed;
        //if (playerComponent.Position.Y > _gameWorld.WorldHeight - _character.Height) _position.Y = _gameWorld.WorldHeight - speed - _character.Height;
    }
}
