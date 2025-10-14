using Microsoft.Xna.Framework;

namespace OutrunStyleTest.Track;

internal struct TrackSegment
{
    /// <summary>
    /// The colour of the grass on either side of the road.
    /// </summary>
    public Color GrassColour;

    /// <summary>
    /// The index of this segment in the track.
    /// </summary>
    public int Index;

    /// <summary>
    /// Number of lanes in this segment of the track.
    /// </summary>
    public int Lanes;

    /// <summary>
    /// Colour of the lane markers on the road.
    /// </summary>
    public Color LaneColour;

    /// <summary>
    /// Colour of the road surface for this segment.
    /// </summary>
    public Color RoadColour;

    /// <summary>
    /// Colour of the rumble strips on the edge of the road for this segment.
    /// </summary>
    public Color RumbleColour;

    /// <summary>
    /// Which 'strip' this segment is part of. A 'strip' is a small number of segments grouped together
    /// </summary>
    public int SegmentStripIndex;

    /// <summary>
    /// Width of the road at this segment. Note that this is half the total width of the road.
    /// </summary>
    public int Width;

    /// <summary>
    /// The Z map for this segment, which holds all the precalculated information we need to draw.
    /// </summary>
    public ZMap ZMap;
}
