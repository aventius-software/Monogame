using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutrunStyleTest.Services;

internal class TrackBuilderService
{
    private Color grassColourDark = Color.DarkGreen;
    private Color grassColourLight = Color.Green;
    private Color laneColour = Color.White;
    private Color roadColourDark = Color.DarkGray;
    private Color roadColourLight = Color.Gray;
    private Color rumbleStripColourDark = Color.Red;
    private Color rumbleStripColourLight = Color.White;
    private int _segmentLength = 100;
    private int _segmentsPerStrip = 2;

    private List<TrackSegment> _trackSegments = [];

    public void AddCurve(bool isLeft, int tightness, int numberOfTrackSegments)
    {
        // Workout direction
        var direction = isLeft ? -1 : 1;

        // Get the current segment count before we start...
        var startingSegmentCount = _trackSegments.Count;
        var lastOffset = startingSegmentCount == 0 ? 0 : _trackSegments.Last().OffsetX;

        // Add segments according to the length (number of segements specified)
        for (var segmentIndex = startingSegmentCount; segmentIndex < startingSegmentCount + numberOfTrackSegments; segmentIndex++)
        {
            // We need to know which iteration of the loop we're on...
            var iterationIndex = segmentIndex - startingSegmentCount;
            
            // In order to 'stripe' the track (alternating light/dark strips) we just need a calculation
            // which will give us some pattern of alternating 'true' or 'false' for each segment. This
            // way we can use it to either colour a segement (or strip of segments) a certain colour, which
            // in our case will just be alternating light/dark colours
            var segmentStripIsAnEvenNumber = Math.Floor(segmentIndex / (float)_segmentsPerStrip) % 2 == 1;

            // For calculating curves
            var curve = direction * MathHelper.Lerp(lastOffset, lastOffset + (tightness * iterationIndex), iterationIndex);

            // Add a segment to the track
            _trackSegments.Add(new TrackSegment
            {
                Index = segmentIndex,
                OffsetX = curve,
                GrassColour = segmentStripIsAnEvenNumber ? grassColourLight : grassColourDark,
                RoadColour = segmentStripIsAnEvenNumber ? roadColourLight : roadColourDark,
                RumbleColour = segmentStripIsAnEvenNumber ? rumbleStripColourLight : rumbleStripColourDark,
                LaneColour = Color.White,
                ZMap = new ZMap
                {
                    WorldCoordinates = new Vector3(0, 0, segmentIndex * _segmentLength),
                    ScreenCoordinates = new Vector3(0, 0, 0),
                    Scale = -1
                },
            });
        }
    }

    public void AddStraight(int numberOfTrackSegments)
    {
        // Get the current segment count before we start...
        var startingSegmentCount = _trackSegments.Count;
        var lastOffset = startingSegmentCount == 0 ? 0 : _trackSegments.Last().OffsetX;

        // Add segments according to the length (number of segements specified)
        for (var segmentIndex = startingSegmentCount; segmentIndex < startingSegmentCount + numberOfTrackSegments; segmentIndex++)
        {
            // In order to 'stripe' the track (alternating light/dark strips) we just need a calculation
            // which will give us some pattern of alternating 'true' or 'false' for each segment. This
            // way we can use it to either colour a segement (or strip of segments) a certain colour, which
            // in our case will just be alternating light/dark colours
            var segmentStripIsAnEvenNumber = Math.Floor(segmentIndex / (float)_segmentsPerStrip) % 2 == 1;

            _trackSegments.Add(new TrackSegment
            {
                Index = segmentIndex,
                OffsetX = lastOffset,
                GrassColour = segmentStripIsAnEvenNumber ? grassColourLight : grassColourDark,
                RoadColour = segmentStripIsAnEvenNumber ? roadColourLight : roadColourDark,
                RumbleColour = segmentStripIsAnEvenNumber ? rumbleStripColourLight : rumbleStripColourDark,
                LaneColour = Color.White,
                ZMap = new ZMap
                {
                    WorldCoordinates = new Vector3(0, 0, segmentIndex * _segmentLength),
                    ScreenCoordinates = new Vector3(0, 0, 0),
                    Scale = -1
                },
            });
        }
    }

    public TrackSegment[] Build()
    {
        return _trackSegments.ToArray();
    }

    private float[] CurveLeft(int tightness, int length, int offset)
    {
        var curves = new float[length];

        for (var n = 0; n < length; n++)
        {
            curves[n] = MathHelper.Lerp(offset, offset + (tightness * n), n);
        }

        return curves;
    }

    private float[] CurveRight(int tightness, int length, int offset)
    {
        var curves = new float[length];

        for (var n = 0; n < length; n++)
        {
            curves[n] = MathHelper.Lerp(offset + (tightness * n), offset, n);
        }

        return curves;
    }

    private float[] LeftStraight(int tightness, int length, int offset)
    {
        var curves = new float[length];

        for (var n = 0; n < length; n++)
        {
            curves[n] = MathHelper.Lerp(offset, offset + tightness, n * tightness * tightness);
        }

        return curves;
    }

    private float[] RightStraight(int tightness, int length, int offset)
    {
        var curves = new float[length];

        for (var n = 0; n < length; n++)
        {
            curves[n] = MathHelper.Lerp(offset + tightness, offset, n * tightness * tightness);
        }

        return curves;
    }
}
