using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OutrunStyleTest.Components;
using OutrunStyleTest.Services;
using Scellecs.Morpeh;
using System;
using System.Collections.Generic;

namespace OutrunStyleTest.Systems;

internal class TrackSystem : ISystem
{
    public World World { get; set; }

    private Filter _cameraFilter;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ShapeDrawingService _shapeDrawingService;
    private Filter _trackFilter;
    private List<TrackSegment> _trackSegments;

    public TrackSystem(World world, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice)
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
        // To get the camera entity
        _cameraFilter = World.Filter.With<CameraComponent>().Build();

        // To get the track entity
        _trackFilter = World.Filter.With<TrackComponent>().Build();

        // Initialise the track
        var track = _trackFilter.First();

        ref var trackComponent = ref track.GetComponent<TrackComponent>();
        trackComponent.DrawDistance = 200;
        trackComponent.IndividualSegmentLength = 100;
        trackComponent.Lanes = 3;
        trackComponent.Width = 1000;
        trackComponent.RumbleSegments = 5;
        trackComponent.TotalTrackSegments = 300;
        trackComponent.Length = trackComponent.IndividualSegmentLength * trackComponent.TotalTrackSegments;

        // Create (pre-populate) the track data        
        _trackSegments = CreateTrack(
            numberOfTrackSegments: trackComponent.TotalTrackSegments,
            individualSegmentLength: trackComponent.IndividualSegmentLength,
            numberOfRumbleSegments: trackComponent.RumbleSegments);
    }

    public void OnUpdate(float deltaTime)
    {
        var track = _trackFilter.First();
        ref var trackComponent = ref track.GetComponent<TrackComponent>();

        var camera = _cameraFilter.First();
        ref var cameraComponent = ref track.GetComponent<CameraComponent>();

        var clipBottomLine = _graphicsDevice.Viewport.Height;
        var baseSegment = GetTrackSegment(cameraComponent.Position.Z, trackComponent.Length, trackComponent.IndividualSegmentLength, trackComponent.TotalTrackSegments);
        var baseIndex = baseSegment.Index;

        for (var n = 0; n < trackComponent.DrawDistance; n++)
        {
            var currIndex = (baseIndex + n) % trackComponent.TotalTrackSegments;
            var currSegment = _trackSegments[currIndex];

            var offsetZ = (currIndex < baseIndex) ? trackComponent.Length : 0;

            Project3D(ref currSegment.ZMap, 
                cameraComponent.Position.X, 
                cameraComponent.Position.Y, 
                cameraComponent.Position.Z - offsetZ,
                cameraComponent.DistanceToProjectionPlane, 
                _graphicsDevice.Viewport.Width, 
                _graphicsDevice.Viewport.Height, 
                trackComponent.Width);

            _trackSegments[currIndex] = currSegment;

            var currBottomLine = currSegment.ZMap.ScreenCoordinates.Y;

            if (n > 0 && currBottomLine < clipBottomLine)
            {
                var prevIndex = currIndex > 0 ? currIndex - 1 : trackComponent.TotalTrackSegments - 1;
                var prevSegment = _trackSegments[prevIndex];

                var p1 = prevSegment.ZMap.ScreenCoordinates;
                var p2 = currSegment.ZMap.ScreenCoordinates;

                DrawTrackSegment(
                    _graphicsDevice.Viewport.Width,
                    trackComponent.Lanes,
                    p1.X, p1.Y, p1.W,
                    p2.X, p2.Y, p2.W,
                    currSegment.RoadColour,
                    currSegment.GrassColour,
                    currSegment.RumbleColour,
                    Color.DarkGray
                );

                clipBottomLine = currBottomLine;
            }
        }
    }

    private List<TrackSegment> CreateTrack(int numberOfTrackSegments, int individualSegmentLength, int numberOfRumbleSegments)
    {
        // Lets build a track
        var track = new List<TrackSegment>();

        // Create all the track segments
        for (var segmentNumber = 0; segmentNumber < numberOfTrackSegments; segmentNumber++)
        {
            // Add a segment to the track
            track.Add(CreateTrackSegment(segmentNumber, individualSegmentLength, numberOfRumbleSegments));
        }

        // Colour the start/end of the track
        //for (var i = 0; i < numberOfRumbleSegments; i++)
        //{
            // Start segment...
            var startSegment = track[0];
            startSegment.RoadColour = Color.White;// Color.FromArgb(255, 255, 255);
            track[0] = startSegment;

            // Last segment...
            var finishSegment = track[^1];
            finishSegment.RoadColour = Color.Silver;// Color.FromArgb(50, 50, 50);
            track[^1] = finishSegment;
        //}

        return track;
    }    
    
    private TrackSegment CreateTrackSegment(int index, int individualSegmentLength, int numberOfRumbleSegments)
    {        
        return new TrackSegment
        {
            Index = index,
            ZMap = new ZMap
            {
                WorldCoordinates = new WorldCoordinate
                {
                    X = 0,
                    Y = 0,
                    Z = index * individualSegmentLength
                },
                ScreenCoordinates = new ScreenCoordinate
                {
                    X = 0,
                    Y = 0,
                    W = 0
                },
                Scale = -1
            },
            GrassColour = Math.Floor(index / (float)numberOfRumbleSegments) % 2 == 1 ? Color.LightGreen : Color.DarkGreen, // GrassColourLight : GrassColourDark,
            RoadColour = Math.Floor(index / (float)numberOfRumbleSegments) % 2 == 1 ? Color.LightGray : Color.DarkGray, // RoadColourLight : RoadColourDark,
            RumbleColour = Math.Floor(index / (float)numberOfRumbleSegments) % 2 == 1 ? Color.White : Color.Red // RumbleColourLight : RumbleColourDark
        };
    }

    //private void Project2D(ref ZMap zmap, int viewPortWidth, int viewPortHeight, int trackWidth)
    //{
    //    zmap.ScreenCoordinates.X = viewPortWidth / 2;
    //    zmap.ScreenCoordinates.Y = (int)(viewPortHeight - zmap.WorldCoordinates.Z);
    //    zmap.ScreenCoordinates.W = trackWidth;
    //}

    private void Project3D(ref ZMap zmap, float cameraX, float cameraY, float cameraZ, float cameraDepth, int viewPortWidth, int viewPortHeight, int trackWidth)
    {
        // translating world coordinates to camera coordinates
        var transX = zmap.WorldCoordinates.X - cameraX;
        var transY = zmap.WorldCoordinates.Y - cameraY;
        var transZ = zmap.WorldCoordinates.Z - cameraZ;

        // scaling factor based on the law of similar triangles
        zmap.Scale = cameraDepth / transZ;

        // projecting camera coordinates onto a normalized projection plane
        var projectedX = zmap.Scale * transX;
        var projectedY = zmap.Scale * transY;
        var projectedW = zmap.Scale * trackWidth;

        // scaling projected coordinates to the screen coordinates
        zmap.ScreenCoordinates.X = (int)Math.Round((1 + projectedX) * (viewPortWidth / 2));
        zmap.ScreenCoordinates.Y = (int)Math.Round((1 - projectedY) * (viewPortHeight / 2));
        zmap.ScreenCoordinates.W = (int)Math.Round(projectedW * (viewPortWidth / 2));
    }
    
    private void DrawTrackSegment(int viewPortWidth, int numberOfLanes, int x1, int y1, int w1, int x2, int y2, int w2, Color roadColour, Color grassColour, Color rumbleColour, Color laneColour)
    {        
        // Draw grass first
        //await GraphicsService.DrawFilledRectangleAsync(ColorTranslator.ToHtml(grassColour), 0, y2, GraphicsService.CanvasWidth, y1 - y2);
        _shapeDrawingService.DrawFilledRectangle(grassColour, 0, y2, viewPortWidth, y1 - y2);

        // Draw the road surface
        //await GraphicsService.DrawQuadrilateralAsync(ColorTranslator.ToHtml(roadColour), x1 - w1, y1, x1 + w1, y1, x2 + w2, y2, x2 - w2, y2);
        _shapeDrawingService.DrawFilledQuadrilateral(roadColour, x1 - w1, y1, x1 + w1, y1, x2 + w2, y2, x2 - w2, y2);

        // Draw rumble strips
        var rumble_w1 = w1 / 5;
        var rumble_w2 = w2 / 5;

        //await GraphicsService.DrawQuadrilateralAsync(ColorTranslator.ToHtml(rumbleColour), x1 - w1 - rumble_w1, y1, x1 - w1, y1, x2 - w2, y2, x2 - w2 - rumble_w2, y2);
        //await GraphicsService.DrawQuadrilateralAsync(ColorTranslator.ToHtml(rumbleColour), x1 + w1 + rumble_w1, y1, x1 + w1, y1, x2 + w2, y2, x2 + w2 + rumble_w2, y2);

        //if (true)//roadColour == RoadColourDark)
        //{
        //    var line_w1 = (w1 / 20) / 2;
        //    var line_w2 = (w2 / 20) / 2;

        //    var lane_w1 = (w1 * 2) / numberOfLanes;
        //    var lane_w2 = (w2 * 2) / numberOfLanes;

        //    var lane_x1 = x1 - w1;
        //    var lane_x2 = x2 - w2;

        //    for (var i = 1; i < numberOfLanes; i++)
        //    {
        //        lane_x1 += lane_w1;
        //        lane_x2 += lane_w2;

        //        _shapeDrawingService.DrawFilledQuadrilateral(laneColour,
        //            new Vector2(lane_x1 - line_w1, y1),
        //            new Vector2(lane_x1 + line_w1, y1),
        //            new Vector2(lane_x2 + line_w2, y2),
        //            new Vector2(lane_x2 - line_w2, y2)
        //        );
        //    }
        //}
    }

    private TrackSegment GetTrackSegment(float z, int trackLength, int individualSegmentLength, int totalTrackSegments)
    {
        if (z < 0) z += trackLength;
        var index = (int)Math.Floor(z / individualSegmentLength) % totalTrackSegments;

        return _trackSegments[index];
    }
}
