using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using OutrunStyleTest.Camera;
using System;
using System.Linq;

namespace OutrunStyleTest.Track;

internal class TrackUpdateSystem : EntityUpdateSystem
{    
    private int _cameraEntityId;
    private int _trackEntityId;

    public TrackUpdateSystem() : base(Aspect.One(typeof(CameraComponent), typeof(TrackComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {        
        // Save the entity id's we'll need
        _cameraEntityId = ActiveEntities.Single(id => GetEntity(id).Has<CameraComponent>());
        _trackEntityId = ActiveEntities.Single(id => GetEntity(id).Has<TrackComponent>());
    }

    public override void Update(GameTime gameTime)
    {
        // We'll need some information about the track and camera
        var cameraComponent = GetEntity(_cameraEntityId).Get<CameraComponent>();
        var trackComponent = GetEntity(_trackEntityId).Get<TrackComponent>();        

        // Get the current track segment for where we currently are along the track
        var startingSegment = trackComponent.Track.GetSegmentAtPosition(cameraComponent.Position.Z);

        // First 'transform' (or project) the world coordinates of all the track segments to screen coordinates
        for (var segmentNumber = 0; segmentNumber < trackComponent.DrawDistance; segmentNumber++)
        {
            // Find the index of the current segment we're going to project coordinates for. Note that
            // we're going to loop back to the start if we're at the end. Obviously if you don't want
            // that kind of feature then modify this code appropriately
            var thisIndex = (startingSegment.Index + segmentNumber) % trackComponent.Track.Segments.Length;
            var thisSegment = trackComponent.Track.Segments[thisIndex];
            var offsetZ = thisIndex < startingSegment.Index ? trackComponent.Track.TotalLength : 0;

            // Transform (project) world coordinates into screen coordinates
            Project3D(ref thisSegment.ZMap,
                cameraComponent.Position.X,
                cameraComponent.Position.Y,
                cameraComponent.Position.Z - offsetZ,
                cameraComponent.DistanceToProjectionPlane,
                cameraComponent.ViewportWidth,
                cameraComponent.ViewportHeight,
                thisSegment.Width);

            // Update this segments Z map with projected coordinates for the screen
            trackComponent.Track.Segments[thisIndex].ZMap = thisSegment.ZMap;
        }
    }

    private static void Project3D(ref ZMap zmap, float cameraX, float cameraY, float cameraZ, float cameraDepth, int viewPortWidth, int viewPortHeight, int trackWidth)
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
}


/// <summary>
/// This is the update system for the track, basically it does all the initial track creation
/// and calculates (projects) all the world coordinates to screen coordinates for later rendering
/// </summary>
//internal class TrackUpdateSystem : ISystem
//{
//    private const int MinimumDrawDistance = 200;
//    public World World { get; set; }

//    private Entity _cameraEntity;
//    private readonly GraphicsDevice _graphicsDevice;
//    private Entity _trackEntity;
//    private readonly TrackBuilderService _trackBuilderService;

//    public TrackUpdateSystem(World world, GraphicsDevice graphicsDevice, TrackBuilderService trackBuilderService)
//    {
//        World = world;

//        _graphicsDevice = graphicsDevice;
//        _trackBuilderService = trackBuilderService;
//    }

//    public void Dispose()
//    {
//    }

//    public void OnAwake()
//    {
//        // Get the camera entity
//        var cameraFilter = World.Filter.With<CameraComponent>().Build();
//        _cameraEntity = cameraFilter.First();

//        // Get the track entity
//        var trackFilter = World.Filter.With<TrackComponent>().Build();
//        _trackEntity = trackFilter.First();

//        // Build a track... note that the track currently will loop when
//        // you get to the end. So you'll need to make sure that each
//        // section eventually 'adds' up to the 'x' position of the start
//        // otherwise your 'end' will abruptly 'snap' back to the start
//        // position instead of being a smooth connection back to the start ;-)        
//        _trackBuilderService.SegmentHeight = 200;

//        _trackBuilderService.NumberOfLanes = 4;
//        _trackBuilderService.SegmentWidth = 1000;
//        _trackBuilderService.AddDownhillStraight(25, 2);

//        _trackBuilderService.NumberOfLanes = 4;
//        _trackBuilderService.SegmentWidth = 1000;
//        _trackBuilderService.AddUphillStraight(25, 2);

//        _trackBuilderService.NumberOfLanes = 4;
//        _trackBuilderService.SegmentWidth = 1000;
//        _trackBuilderService.AddLeftCurve(25, 2);

//        _trackBuilderService.NumberOfLanes = 4;
//        _trackBuilderService.SegmentWidth = 1000;
//        _trackBuilderService.AddRightCurve(25, 2);

//        _trackBuilderService.NumberOfLanes = 4;
//        _trackBuilderService.SegmentWidth = 1000;
//        _trackBuilderService.AddLeftDownhillCurve(25, 2, 2);

//        _trackBuilderService.NumberOfLanes = 4;
//        _trackBuilderService.SegmentWidth = 1000;
//        _trackBuilderService.AddRightUphillCurve(25, 2, 2);

//        // Set track component values
//        ref var trackComponent = ref _trackEntity.GetComponent<TrackComponent>();
//        trackComponent.Track = _trackBuilderService.Build();
//        trackComponent.DrawDistance = Math.Min(MinimumDrawDistance, trackComponent.Track.Segments.Length);
//    }

//    public void OnUpdate(float deltaTime)
//    {
//        // We'll need some information about the track and camera
//        ref var trackComponent = ref _trackEntity.GetComponent<TrackComponent>();
//        ref var cameraComponent = ref _cameraEntity.GetComponent<CameraComponent>();

//        // Get the current track segment for where we currently are along the track
//        var startingSegment = trackComponent.Track.GetSegmentAtPosition(cameraComponent.Position.Z);

//        // First 'transform' (or project) the world coordinates of all the track segments to screen coordinates
//        for (var segmentNumber = 0; segmentNumber < trackComponent.DrawDistance; segmentNumber++)
//        {
//            // Find the index of the current segment we're going to project coordinates for. Note that
//            // we're going to loop back to the start if we're at the end. Obviously if you don't want
//            // that kind of feature then modify this code appropriately
//            var thisIndex = (startingSegment.Index + segmentNumber) % trackComponent.Track.Segments.Length;
//            var thisSegment = trackComponent.Track.Segments[thisIndex];
//            var offsetZ = thisIndex < startingSegment.Index ? trackComponent.Track.TotalLength : 0;

//            // Transform (project) world coordinates into screen coordinates
//            Project3D(ref thisSegment.ZMap,
//                cameraComponent.Position.X,
//                cameraComponent.Position.Y,
//                cameraComponent.Position.Z - offsetZ,
//                cameraComponent.DistanceToProjectionPlane,
//                _graphicsDevice.Viewport.Width,
//                _graphicsDevice.Viewport.Height,
//                thisSegment.Width);

//            // Update this segments Z map with projected coordinates for the screen
//            trackComponent.Track.Segments[thisIndex].ZMap = thisSegment.ZMap;
//        }
//    }

//    private static void Project3D(ref ZMap zmap, float cameraX, float cameraY, float cameraZ, float cameraDepth, int viewPortWidth, int viewPortHeight, int trackWidth)
//    {
//        // Translating world coordinates to camera coordinates
//        var transX = zmap.WorldCoordinates.X - cameraX;
//        var transY = zmap.WorldCoordinates.Y - cameraY;
//        var transZ = zmap.WorldCoordinates.Z - cameraZ;

//        // Scaling factor based on the law of similar triangles
//        zmap.Scale = cameraDepth / transZ;

//        // Projecting camera coordinates onto a normalized projection plane
//        var projectedX = zmap.Scale * transX;
//        var projectedY = zmap.Scale * transY;
//        var projectedW = zmap.Scale * trackWidth;

//        // Scaling projected coordinates to the screen coordinates
//        zmap.ScreenCoordinates.X = (int)Math.Round((1 + projectedX) * (viewPortWidth / 2));
//        zmap.ScreenCoordinates.Y = (int)Math.Round((1 - projectedY) * (viewPortHeight / 2));
//        zmap.ScreenCoordinates.Z = (int)Math.Round(projectedW * (viewPortWidth / 2));
//    }
//}
