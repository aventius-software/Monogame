using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct TrackComponent : IComponent
{
    public int DrawDistance;                    // Number of segments to draw on screen at once                
    public int IndividualSegmentLength;         // Length of an individual segment (from top to bottom)
    public int Lanes;                           // Number of lanes for our track
    public int Length;                          // The total track length (i.e. segment length * number of segments)    
    public int RumbleSegments;                  // Number of road segments that make a rumble strip
    public int TotalTrackSegments;              // The total number of road segments in the track
    public int Width;                           // Half the width of the road (e.g. 1920px / 2 = 960px)
}
