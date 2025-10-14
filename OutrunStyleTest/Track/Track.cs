using System;

namespace OutrunStyleTest.Track;

/// <summary>
/// This will hold all the information about the track, including all the segments that make up the track.
/// </summary>
internal struct Track
{
    public int SegmentHeight;
    public TrackSegment[] Segments;

    public readonly TrackSegment GetSegmentAtPosition(float z)
    {
        if (z < 0) z += TotalLength;
        var index = (int)Math.Floor(z / SegmentHeight) % Segments.Length;

        return Segments[index];
    }

    public readonly int TotalLength => Segments.Length * SegmentHeight;
}
