using System;

namespace OutrunStyleTest.Services;

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
