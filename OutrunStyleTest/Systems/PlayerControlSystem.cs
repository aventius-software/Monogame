using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OutrunStyleTest.Components;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Systems;

/// <summary>
/// This system reads input to allow control of the player, there's nothing fancy
/// going here. Some improvements would/could be steering resistance or friction
/// on different surfaces etc...
/// </summary>
internal class PlayerControlSystem : ISystem
{
    public World World { get; set; }

    private Entity _playerEntity;
    private Entity _trackEntity;

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
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        _playerEntity = playerFilter.First();

        // Get track entity
        var trackFilter = World.Filter.With<TrackComponent>().Build();
        _trackEntity = trackFilter.First();

        // Initialise the player
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();
        playerComponent.AccelerationRate = 25;
        playerComponent.Position = Vector3.Zero;
        playerComponent.MaxSpeed = 10000f;
        playerComponent.Speed = 0;
        playerComponent.SteeringRate = 30f;
    }

    public void OnUpdate(float deltaTime)
    {
        // We'll need to reference the player component
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();

        // Get keyboard state
        var keyboardState = Keyboard.GetState();

        // Acceleration and braking
        if (keyboardState.IsKeyDown(Keys.Up) && playerComponent.Speed < playerComponent.MaxSpeed - playerComponent.AccelerationRate)
        {
            // Increase the players speed
            playerComponent.Speed += playerComponent.AccelerationRate;
        }
        else if (keyboardState.IsKeyDown(Keys.Down) && playerComponent.Speed > playerComponent.AccelerationRate)
        {
            // Slow the players speed
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

        // Update the players position in the Z direction according to the current speed       
        playerComponent.Position.Z += playerComponent.Speed * deltaTime;

        // Back to start of the track if we've reached the end
        ref var trackComponent = ref _trackEntity.GetComponent<TrackComponent>();
        if (playerComponent.Position.Z >= trackComponent.Track.TotalLength) playerComponent.Position.Z -= trackComponent.Track.TotalLength;

        // This is a little fiddly... but basically to put the player 'on' the track instead of just floating in the
        // air we need to put the player at the same Y position of the current segment (the camera hovers a little above
        // the player so that's no problem). The problem here is that each segment has a different Y coordinate to (unless
        // its a flat section of track) the following segments Y coordinate. This means the player (or camera) will appear
        // to 'snap' between the different Y coordinatea as the player travels in the Z direction through a segment until
        // they hit the next segment. What we need is to smoothly move from the Y coordinate of the current segment to the
        // next segments Y coordinate...
        var currentSegment = trackComponent.Track.GetSegmentAtPosition(playerComponent.Position.Z);
        var nextSegment = trackComponent.Track.GetSegmentAtPosition(playerComponent.Position.Z + trackComponent.Track.SegmentHeight);

        // Figure out how far the player is (in the Z axis) through the current segment before they reach the next segment
        var percent = (playerComponent.Position.Z % trackComponent.Track.SegmentHeight) / trackComponent.Track.SegmentHeight;

        // Now we can interpolate between the current segments Y coordinate to the next segments Y coordinate
        playerComponent.Position.Y = MathHelper.Lerp(
            currentSegment.ZMap.WorldCoordinates.Y,
            nextSegment.ZMap.WorldCoordinates.Y,
            percent);
    }
}
