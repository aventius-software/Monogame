using MarioPlatformerStyleTest.Components;
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
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the components
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();
        ref var transformComponent = ref _playerEntity.GetComponent<TransformComponent>();
        ref var characterComponent = ref _playerEntity.GetComponent<CharacterComponent>();

        // Do player stuff like checking controls etc...
        var keyboard = Keyboard.GetState();

        // Set velocity in the X axis depending on input
        if (keyboard.IsKeyDown(Keys.Left)) transformComponent.Velocity.X = -transformComponent.Speed * deltaTime;
        else if (keyboard.IsKeyDown(Keys.Right)) transformComponent.Velocity.X = transformComponent.Speed * deltaTime;
        else transformComponent.Velocity.X = 0;

        // Has the player pressed jump?
        if (keyboard.IsKeyDown(Keys.Space))
        {
            // Set flag for other systems to know the player has pressed (or still is pressing) the jump button
            playerComponent.IsJumpPressed = true;

            // Only perform a jump though if the player is on the ground
            if (characterComponent.IsOnTheGround) transformComponent.Velocity.Y = -characterComponent.JumpStrength;
        }
        else playerComponent.IsJumpPressed = false;
    }
}
