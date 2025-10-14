using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using OutrunStyleTest.Track;
using System.Linq;

namespace OutrunStyleTest.Player;

/// <summary>
/// Moves the player along the track according to their speed.
/// </summary>
internal class PlayerMovementSystem : EntityUpdateSystem
{
    private int _playerEntityId;
    private int _trackEntityId;

    public PlayerMovementSystem() : base(Aspect.One(typeof(PlayerComponent), typeof(TrackComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Save the entity id's we'll need
        _playerEntityId = ActiveEntities.Single(id => GetEntity(id).Has<PlayerComponent>());
        _trackEntityId = ActiveEntities.Single(id => GetEntity(id).Has<TrackComponent>());
    }

    public override void Update(GameTime gameTime)
    {
        // We'll need to reference the player component
        var playerComponent = GetEntity(_playerEntityId).Get<PlayerComponent>();
        var trackComponent = GetEntity(_trackEntityId).Get<TrackComponent>();

        // Update the players position in the Z direction (i.e. into the screen) according
        // to the players current speed       
        playerComponent.Position.Z += playerComponent.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Put the player back at start of the track if we've reached the end. This way
        // we can keep going round and round the track forever and ever... Although, note that
        // the if the track is not a perfect loop (i.e. the start and end don't meet at the same
        // height) there will be a visible 'jump' in the Y position when this happens!
        if (playerComponent.Position.Z >= trackComponent.Track.TotalLength)
            playerComponent.Position.Z -= trackComponent.Track.TotalLength;

        // This is a little fiddly to explain, but...
        //
        // ...basically to have the player 'on' the track instead of just floating in the air we
        // need to put the player at the same Y position of the current track segment (the camera
        // actually will hover a little above the player). The problem here is that each track segment
        // can have a different Y coordinate to the next segments Y coordinate (unless its just a flat
        // section of track of course). This means the player (or camera) will appear to 'snap' between
        // the different Y coordinates of each segment as the player travels in the Z direction (i.e.
        // into the screen) through a segment until they get to the next segment. What we really want
        // is to smoothly move from the Y coordinate of the current segment to Y coordinate of the next
        // segment. So, to do this we need to know which segment the player is currently in and which
        // segment is next, then we can work out how far the player is through the current segment...
        var currentSegment = trackComponent.Track.GetSegmentAtPosition(playerComponent.Position.Z);
        var nextSegment = trackComponent.Track.GetSegmentAtPosition(playerComponent.Position.Z + trackComponent.Track.SegmentHeight);

        // Figure out how far the player has (in respect the Z axis) moved through the
        // current segment, before they'll reach the next segment...
        var percent = playerComponent.Position.Z % trackComponent.Track.SegmentHeight / trackComponent.Track.SegmentHeight;

        // Now, with all this information we can smoothly interpolate between the current segments
        // Y coordinate and the next segments Y coordinate. If we set the players Y coordinate now
        // the player (and camera) will not appear to 'snap' between segments ;-)
        playerComponent.Position.Y = MathHelper.Lerp(
            currentSegment.ZMap.WorldCoordinates.Y,
            nextSegment.ZMap.WorldCoordinates.Y,
            percent);
    }
}