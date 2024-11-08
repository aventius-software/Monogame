using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OutrunStyleTest.Components;
using OutrunStyleTest.Services;
using Scellecs.Morpeh;
using System.Linq;

namespace OutrunStyleTest.Systems;

internal class TrackRenderSystem : ISystem
{
    public World World { get; set; }

    private Entity _cameraEntity;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ShapeDrawingService _shapeDrawingService;
    private Entity _trackEntity;

    public TrackRenderSystem(World world, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice)
    {
        World = world;

        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Get the camera entity
        var cameraFilter = World.Filter.With<CameraComponent>().Build();
        _cameraEntity = cameraFilter.First();

        // Get the track entity
        var trackFilter = World.Filter.With<TrackComponent>().Build();
        _trackEntity = trackFilter.First();
    }

    public void OnUpdate(float deltaTime)
    {
        // We'll need some information about the track and camera
        ref var trackComponent = ref _trackEntity.GetComponent<TrackComponent>();
        ref var cameraComponent = ref _cameraEntity.GetComponent<CameraComponent>();

        // Get the current track segment for where the camera currently is along the track        
        var startingSegment = trackComponent.Track.GetSegmentAtPosition(cameraComponent.Position.Z);

        // Away we go...
        var clipBottomLine = _graphicsDevice.Viewport.Height;

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
                DrawTrackSegment(
                    _graphicsDevice.Viewport.Width,
                    thisSegment.Lanes,
                    (int)previousSegmentScreenCoordinates.X, (int)previousSegmentScreenCoordinates.Y, (int)previousSegmentScreenCoordinates.Z,
                    (int)thisSegmentScreenCoordinates.X, (int)thisSegmentScreenCoordinates.Y, (int)thisSegmentScreenCoordinates.Z,
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

    private void DrawTrackSegment(int viewPortWidth, int numberOfLanes, int x1, int y1, int w1, int x2, int y2, int w2, Color roadColour, Color grassColour, Color rumbleColour, Color laneColour, bool drawLanes)
    {
        // Draw grass first        
        _shapeDrawingService.DrawFilledRectangle(grassColour, 0, y2, viewPortWidth, y1 - y2);

        // Draw the road surface        
        _shapeDrawingService.DrawFilledQuadrilateral(roadColour, x1 - w1, y1, x1 + w1, y1, x2 + w2, y2, x2 - w2, y2);

        // Draw rumble strips
        var rumble_w1 = w1 / 5;
        var rumble_w2 = w2 / 5;

        _shapeDrawingService.DrawFilledQuadrilateral(rumbleColour, x1 - w1 - rumble_w1, y1, x1 - w1, y1, x2 - w2, y2, x2 - w2 - rumble_w2, y2);
        _shapeDrawingService.DrawFilledQuadrilateral(rumbleColour, x1 + w1 + rumble_w1, y1, x1 + w1, y1, x2 + w2, y2, x2 + w2 + rumble_w2, y2);

        // Draw lane markers if required        
        if (drawLanes)
        {
            var line_w1 = (w1 / 20) / 2;
            var line_w2 = (w2 / 20) / 2;

            var lane_w1 = (w1 * 2) / numberOfLanes;
            var lane_w2 = (w2 * 2) / numberOfLanes;

            var lane_x1 = x1 - w1;
            var lane_x2 = x2 - w2;

            for (var i = 1; i < numberOfLanes; i++)
            {
                lane_x1 += lane_w1;
                lane_x2 += lane_w2;

                _shapeDrawingService.DrawFilledQuadrilateral(laneColour,
                    lane_x1 - line_w1, y1,
                    lane_x1 + line_w1, y1,
                    lane_x2 + line_w2, y2,
                    lane_x2 - line_w2, y2
                );
            }
        }
    }
}
