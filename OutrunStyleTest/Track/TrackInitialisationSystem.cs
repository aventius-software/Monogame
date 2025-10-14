using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;

namespace OutrunStyleTest.Track;

/// <summary>
/// This system is used to create and initialise the track when the game starts.
/// </summary>
internal class TrackInitialisationSystem : EntitySystem
{
    private const int MinimumDrawDistance = 200;

    private readonly TrackBuilderService _trackBuilderService;

    public TrackInitialisationSystem(TrackBuilderService trackBuilderService)
        : base(Aspect.All(typeof(TrackComponent)))
    {
        _trackBuilderService = trackBuilderService;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Build a track...
        //
        // Note that the track currently will loop when you get to the end. So you'll really
        // want to make sure that each section eventually 'adds' up to the 'x' and the 'Y' position
        // of the start segment otherwise your 'end' will abruptly 'snap' back to the start position
        // instead of being a smooth connection back to the start ;-)        
        _trackBuilderService.SegmentHeight = 200;

        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddDownhillStraight(25, 2);

        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddUphillStraight(25, 2);

        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddLeftCurve(25, 2);

        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddRightCurve(25, 2);

        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddLeftDownhillCurve(25, 2, 2);

        _trackBuilderService.NumberOfLanes = 4;
        _trackBuilderService.SegmentWidth = 1000;
        _trackBuilderService.AddRightUphillCurve(25, 2, 2);

        // Create the track entity
        var trackEntity = CreateEntity();
        trackEntity.Attach(new TrackComponent());

        // Set some track component values
        var trackComponent = trackEntity.Get<TrackComponent>();
        trackComponent.Track = _trackBuilderService.Build();
        trackComponent.DrawDistance = Math.Min(MinimumDrawDistance, trackComponent.Track.Segments.Length);
    }
}
