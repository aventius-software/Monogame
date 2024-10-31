using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct TrackComponent : IComponent
{
    public int DrawDistance;    // Number of track segments to draw on screen at once                
    public int SegmentHeight;   // Length of an individual track segment (from top to bottom)    
    public int Length;          // The total track length (i.e. segment length * number of segments)            
}
