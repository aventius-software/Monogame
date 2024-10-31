using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutrunStyleTest.Services;

internal enum TrackSectionType
{
    LeftCurve, RightCurve,
    LeftStraight, RightStraight,
    Straight,
    UphillStraight, DownhillStraight
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

    private readonly List<TrackSegment> _trackSegments = [];

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

    public void AddUphillStraight(int numberOfTrackSegments, int steepness)
    {
        AddSection(TrackSectionType.UphillStraight, numberOfTrackSegments, 0, steepness);
    }

    public void AddDownhillStraight(int numberOfTrackSegments, int steepness)
    {
        AddSection(TrackSectionType.DownhillStraight, numberOfTrackSegments, 0, steepness);
    }

    public TrackSegment[] Build() => [.. _trackSegments];

    private void AddSection(TrackSectionType trackSectionType, int numberOfTrackSegments, int tightness = 0, int steepness = 0)
    {
        // Get the current segment count before we start...
        var startingSegmentCount = _trackSegments.Count;

        // Get the last x offset of the previous segment (or 0 if this is the first segment)
        var previousSegmentOffsetX = startingSegmentCount == 0 ? 0 : _trackSegments.Last().OffsetX;
        var previousSegmentOffsetY = startingSegmentCount == 0 ? 0 : _trackSegments.Last().OffsetY;

        // Add segments according to the length (number of segements specified)
        for (var segmentIndex = startingSegmentCount; segmentIndex < startingSegmentCount + numberOfTrackSegments; segmentIndex++)
        {
            // We need to know which iteration of the loop we're on...
            var iterationIndex = segmentIndex - startingSegmentCount;

            // In order to 'stripe' the track (alternating light/dark strips) we just need a calculation
            // which will give us some pattern of alternating 'true' or 'false' for each segment. This
            // way we can use it to either colour a segement (or strip of segments) a certain colour, which
            // in our case will just be alternating light/dark colours
            var segmentStripIndex = (int)Math.Floor(segmentIndex / (float)SegmentsPerStrip);
            var segmentStripIsAnEvenNumber = segmentStripIndex % 2 == 1;

            // Workout curves/hills - TODO: need to add smoother transitions between sections! 
            float dx = 0, dy = 0;

            switch (trackSectionType)
            {
                case TrackSectionType.LeftCurve:
                    {
                        dx = -MathHelper.Lerp(0, tightness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackSectionType.RightCurve:
                    {
                        dx = MathHelper.Lerp(0, tightness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackSectionType.LeftStraight:
                    {
                        dx = -MathHelper.Lerp(0, tightness, tightness * iterationIndex * (float)Math.Pow(tightness, 4));
                    }
                    break;

                case TrackSectionType.RightStraight:
                    {
                        dx = MathHelper.Lerp(0, tightness, tightness * iterationIndex * (float)Math.Pow(tightness, 4));
                    }
                    break;

                case TrackSectionType.UphillStraight:
                    {
                        dy = MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackSectionType.DownhillStraight:
                    {
                        dy = -MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                default:
                    dx = 0;
                    dy = 0;
                    break;
            }

            // Add a segment to the track
            _trackSegments.Add(new TrackSegment
            {
                Index = segmentIndex,
                OffsetX = previousSegmentOffsetX + dx,
                OffsetY = previousSegmentOffsetY + dy,
                SegmentStripIndex = segmentStripIndex,
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
