using OutrunStyleTest.Services;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct TrackComponent : IComponent
{
    public int DrawDistance;    // Number of track segments to draw on screen at once    
    public Track Track;         // The actual track
}
