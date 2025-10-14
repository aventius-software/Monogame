using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using OutrunStyleTest.Camera;
using System.Linq;

namespace OutrunStyleTest.Track;

/// <summary>
/// This system is responsible for rendering the track to the screen.
/// </summary>
internal class TrackRenderSystem : EntityDrawSystem
{
    private readonly TrackDrawingService _trackDrawingService;

    private int _cameraEntityId;
    private int _trackEntityId;

    public TrackRenderSystem(TrackDrawingService trackDrawingService)
        : base(Aspect.One(typeof(CameraComponent), typeof(TrackComponent)))
    {
        _trackDrawingService = trackDrawingService;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Save the entity id's we'll need
        _cameraEntityId = ActiveEntities.Single(id => GetEntity(id).Has<CameraComponent>());
        _trackEntityId = ActiveEntities.Single(id => GetEntity(id).Has<TrackComponent>());
    }

    public override void Draw(GameTime gameTime)
    {
        // We'll need to reference the some components
        var cameraComponent = GetEntity(_cameraEntityId).Get<CameraComponent>();
        var trackComponent = GetEntity(_trackEntityId).Get<TrackComponent>();

        // Get the current track segment for where the camera currently is along the track        
        var startingSegment = trackComponent.Track.GetSegmentAtPosition(cameraComponent.Position.Z);

        // Away we go...
        var clipBottomLine = cameraComponent.ViewportHeight;

        // Now draw the segments
        for (var drawPosition = 0; drawPosition < trackComponent.DrawDistance; drawPosition++)
        {
            var thisIndex = (startingSegment.Index + drawPosition) % trackComponent.Track.Segments.Length;
            var thisSegment = trackComponent.Track.Segments[thisIndex];

            // Only draw if its on screen
            var currBottomLine = thisSegment.ZMap.ScreenCoordinates.Y;

            if (currBottomLine < clipBottomLine && drawPosition > 0)
            {
                // Get the previous segment
                var previousIndex = thisIndex > 0 ? thisIndex - 1 : trackComponent.Track.Segments.Length - 1;
                var previousSegment = trackComponent.Track.Segments[previousIndex];

                // Get the previous/current screen coordinates to draw the track segment
                var previousSegmentScreenCoordinates = previousSegment.ZMap.ScreenCoordinates;
                var thisSegmentScreenCoordinates = thisSegment.ZMap.ScreenCoordinates;

                // We only need (or want) to draw lane markers on every other strip, so basically we
                // check if the current segments 'strip' index (defining a strip as a few segments, in
                // other words a 'strip' as a small number of segments) is an odd/even number. This way
                // we get an alternating flag saying when to draw lanes or not. If you want continuous
                // lanes without breaks then just set this to 'true' ;-)
                var drawLanes = thisSegment.SegmentStripIndex % 2 == 1;

                // Finally, draw this segment
                _trackDrawingService.DrawTrackSegment(
                    cameraComponent.ViewportWidth,
                    thisSegment.Lanes,
                    (int)previousSegmentScreenCoordinates.X,
                    (int)previousSegmentScreenCoordinates.Y,
                    (int)previousSegmentScreenCoordinates.Z,
                    (int)thisSegmentScreenCoordinates.X,
                    (int)thisSegmentScreenCoordinates.Y,
                    (int)thisSegmentScreenCoordinates.Z,
                    thisSegment.RoadColour,
                    thisSegment.GrassColour,
                    thisSegment.RumbleColour,
                    thisSegment.LaneColour,
                    drawLanes
                );

                clipBottomLine = (int)currBottomLine;
            }
        }
    }
}
