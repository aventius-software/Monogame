using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutrunStyleTest.Services;

internal enum TrackSectionType
{
    LeftCurve, RightCurve, 
    LeftStraight, RightStraight,
    Straight
}

internal class TrackBuilderService
{
    public Color GrassColourDark = Color.DarkGreen;
    public Color GrassColourLight = Color.Green;
    public Color LaneColour = Color.White;
    public int NumberOfLanes = 4;
    public Color RoadColourDark = Color.DarkGray;
    public Color RoadColourLight = Color.Gray;
    public Color RumbleStripColourDark = Color.Red;
    public Color RumbleStripColourLight = Color.White;
    public int SegmentHeight = 150;
    public int SegmentWidth = 1000;
    public int SegmentsPerStrip = 2;

    private List<TrackSegment> _trackSegments = [];

    public void AddStraight(int numberOfTrackSegments)
    {
        AddSection(TrackSectionType.Straight, numberOfTrackSegments);
    }

    public void AddLeftCurve(int numberOfTrackSegments, int tightness)
    {
        AddSection(TrackSectionType.LeftCurve, numberOfTrackSegments, tightness);
    }

    public void AddLeftStraight(int numberOfTrackSegments, int tightness)
    {
        AddSection(TrackSectionType.LeftStraight, numberOfTrackSegments, tightness);
    }

    public void AddRightCurve(int numberOfTrackSegments, int tightness)
    {
        AddSection(TrackSectionType.RightCurve, numberOfTrackSegments, tightness);
    }

    public void AddRightStraight(int numberOfTrackSegments, int tightness)
    {
        AddSection(TrackSectionType.RightStraight, numberOfTrackSegments, tightness);
    }

    public TrackSegment[] Build()
    {
        return _trackSegments.ToArray();
    }

    private void AddSection(TrackSectionType trackSectionType, int numberOfTrackSegments, int tightness = 0)
    {
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
            var segmentStripIsAnEvenNumber = Math.Floor(segmentIndex / (float)SegmentsPerStrip) % 2 == 1;

            // Workout direction
            float offsetX;
            float lastOffsetX = startingSegmentCount == 0 ? 0 : _trackSegments.Last().OffsetX;

            // For calculating curves
            switch (trackSectionType)
            {
                case TrackSectionType.LeftCurve:
                    {
                        offsetX = -MathHelper.Lerp(lastOffset, lastOffset + (tightness * iterationIndex), iterationIndex);
                    }
                    break;

                case TrackSectionType.RightCurve:
                    {
                        offsetX = MathHelper.Lerp(lastOffset, lastOffset + (tightness * iterationIndex), iterationIndex);
                    }
                    break;

                case TrackSectionType.LeftStraight:
                    {
                        offsetX = -MathHelper.Lerp(lastOffset, lastOffset + tightness, (tightness * iterationIndex) * (float)Math.Pow(tightness, 4));
                    }
                    break;

                case TrackSectionType.RightStraight:
                    {
                        offsetX = MathHelper.Lerp(lastOffset, lastOffset + tightness, (tightness * iterationIndex) * (float)Math.Pow(tightness, 4));
                    }
                    break;

                default:
                    offsetX = lastOffsetX;
                    break;
            }

            // Add a segment to the track
            _trackSegments.Add(new TrackSegment
            {
                Index = segmentIndex,
                OffsetX = offsetX,
                GrassColour = segmentStripIsAnEvenNumber ? GrassColourLight : GrassColourDark,
                RoadColour = segmentStripIsAnEvenNumber ? RoadColourLight : RoadColourDark,
                RumbleColour = segmentStripIsAnEvenNumber ? RumbleStripColourLight : RumbleStripColourDark,
                Lanes = NumberOfLanes,
                LaneColour = LaneColour,
                Width = SegmentWidth,
                ZMap = new ZMap
                {
                    WorldCoordinates = new Vector3(0, 0, segmentIndex * SegmentHeight),
                    ScreenCoordinates = new Vector3(0, 0, 0),
                    Scale = -1
                },
            });
        }
    }
}
