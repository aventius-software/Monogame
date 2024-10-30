using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OutrunStyleTest.Components;
using OutrunStyleTest.Services;
using Scellecs.Morpeh;
using System;
using System.Linq;

namespace OutrunStyleTest.Systems;

internal class TrackSystem : ISystem
{
    public World World { get; set; }

    private Entity _camera;
    private Filter _cameraFilter;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ShapeDrawingService _shapeDrawingService;
    private Entity _track;
    private TrackBuilderService _trackBuilderService;
    private Filter _trackFilter;
    private TrackSegment[] _trackSegments;

    public TrackSystem(World world, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice, TrackBuilderService trackBuilderService)
    {
        World = world;

        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
        _trackBuilderService = trackBuilderService;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // To get the camera entity
        _cameraFilter = World.Filter.With<CameraComponent>().Build();
        _camera = _cameraFilter.First();

        // To get the track entity
        _trackFilter = World.Filter.With<TrackComponent>().Build();
        _track = _trackFilter.First();
                
        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddStraight(25);

        _trackBuilderService.NumberOfLanes = 2;
        _trackBuilderService.SegmentWidth = 800;
        _trackBuilderService.AddLeftCurve(25, 2);

        _trackBuilderService.NumberOfLanes = 2;
        _trackBuilderService.SegmentWidth = 800;
        _trackBuilderService.AddLeftStraight(25, 2);

        _trackBuilderService.NumberOfLanes = 2;
        _trackBuilderService.SegmentWidth = 800;
        _trackBuilderService.AddRightCurve(25, 2);

        _trackBuilderService.NumberOfLanes = 2;
        _trackBuilderService.SegmentWidth = 800;
        _trackBuilderService.AddRightStraight(25, 2);

        _trackSegments = _trackBuilderService.Build();

        // Set track component values
        ref var trackComponent = ref _track.GetComponent<TrackComponent>();
        trackComponent.DrawDistance = Math.Min(200, _trackSegments.Length);
        trackComponent.SegmentHeight = _trackBuilderService.SegmentHeight;        
        trackComponent.Length = _trackBuilderService.SegmentHeight * _trackSegments.Length;
    }

    public void OnUpdate(float deltaTime)
    {
        // We'll need some information about the track and camera
        ref var trackComponent = ref _track.GetComponent<TrackComponent>();
        ref var cameraComponent = ref _camera.GetComponent<CameraComponent>();

        // Away we go...
        var clipBottomLine = _graphicsDevice.Viewport.Height;

        // Get the base track segment and its index
        var baseSegment = GetTrackSegment(cameraComponent.Position.Z, trackComponent.Length, trackComponent.SegmentHeight, _trackSegments.Length);
        var baseIndex = baseSegment.Index;

        // Draw 'n' number of track segments
        for (var n = 0; n < trackComponent.DrawDistance; n++)
        {
            var currIndex = (baseIndex + n) % _trackSegments.Length;
            var currSegment = _trackSegments[currIndex];
            var offsetZ = (currIndex < baseIndex) ? trackComponent.Length : 0;

            Project3D(ref currSegment.ZMap,
                cameraComponent.Position.X - currSegment.OffsetX,
                cameraComponent.Position.Y,
                cameraComponent.Position.Z - offsetZ,
                cameraComponent.DistanceToProjectionPlane,
                _graphicsDevice.Viewport.Width,
                _graphicsDevice.Viewport.Height,
                currSegment.Width);

            // Update current segment with projected coordinates
            _trackSegments[currIndex] = currSegment;

            var currBottomLine = currSegment.ZMap.ScreenCoordinates.Y;

            if (n > 0 && currBottomLine < clipBottomLine)
            {
                var prevIndex = currIndex > 0 ? currIndex - 1 : _trackSegments.Length - 1;
                var prevSegment = _trackSegments[prevIndex];

                var p1 = prevSegment.ZMap.ScreenCoordinates;
                var p2 = currSegment.ZMap.ScreenCoordinates;

                DrawTrackSegment(
                    _graphicsDevice.Viewport.Width,                    
                    currSegment.Lanes,
                    (int)p1.X, (int)p1.Y, (int)p1.Z,
                    (int)p2.X, (int)p2.Y, (int)p2.Z,
                    currSegment.RoadColour,
                    currSegment.GrassColour,
                    currSegment.RumbleColour,
                    currSegment.LaneColour
                );

                clipBottomLine = (int)currBottomLine;
            }
        }
    }

    private void Project3D(ref ZMap zmap, float cameraX, float cameraY, float cameraZ, float cameraDepth, int viewPortWidth, int viewPortHeight, int trackWidth)
    {
        // Translating world coordinates to camera coordinates
        var transX = zmap.WorldCoordinates.X - cameraX;
        var transY = zmap.WorldCoordinates.Y - cameraY;
        var transZ = zmap.WorldCoordinates.Z - cameraZ;

        // Scaling factor based on the law of similar triangles
        zmap.Scale = cameraDepth / transZ;

        // Projecting camera coordinates onto a normalized projection plane
        var projectedX = zmap.Scale * transX;
        var projectedY = zmap.Scale * transY;
        var projectedW = zmap.Scale * trackWidth;

        // Scaling projected coordinates to the screen coordinates
        zmap.ScreenCoordinates.X = (int)Math.Round((1 + projectedX) * (viewPortWidth / 2));
        zmap.ScreenCoordinates.Y = (int)Math.Round((1 - projectedY) * (viewPortHeight / 2));
        zmap.ScreenCoordinates.Z = (int)Math.Round(projectedW * (viewPortWidth / 2));
    }

    private void DrawTrackSegment(int viewPortWidth, int numberOfLanes, int x1, int y1, int w1, int x2, int y2, int w2, Color roadColour, Color grassColour, Color rumbleColour, Color laneColour)
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

        if (roadColour == Color.DarkGray)
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

    private TrackSegment GetTrackSegment(float z, int trackLength, int individualSegmentLength, int totalTrackSegments)
    {
        if (z < 0) z += trackLength;
        var index = (int)Math.Floor(z / individualSegmentLength) % totalTrackSegments;

        return _trackSegments[index];
    }
}
