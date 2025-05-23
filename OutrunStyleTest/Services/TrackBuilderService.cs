﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutrunStyleTest.Services;

internal enum TrackTurnType
{
    EaseInLeftTurn, EaseOutLeftTurn,
    EaseInAndOutLeftTurn,
    EaseInRightTurn, EaseOutRightTurn,
    EaseInAndOutRightTurn,
    LeftStraight, RightStraight,
    Straight
}

internal enum TrackHillType
{
    EaseInUphill, EaseOutUphill,
    EaseInAndOutUphill,
    EaseInDownhill, EaseOutDownhill,
    EaseInAndOutDownhill,
    StraightUphill, StraightDownhill,
    Flat
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
    public int SegmentHeight = 200;
    public int SegmentWidth = 1000;
    public int SegmentsPerStrip = 2;

    private readonly List<TrackSegment> _trackSegments = [];

    public void AddStraight(int numberOfTrackSegments)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.Straight, 0);
    }

    public void AddLeftCurve(int numberOfTrackSegments, int tightness)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.EaseInAndOutLeftTurn, tightness);
    }

    public void AddLeftDownhillCurve(int numberOfTrackSegments, int tightness, int steepness)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.EaseInAndOutLeftTurn, tightness, TrackHillType.EaseInAndOutDownhill, steepness);
    }

    public void AddLeftStraight(int numberOfTrackSegments, int tightness)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.LeftStraight, tightness);
    }

    public void AddRightCurve(int numberOfTrackSegments, int tightness)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.EaseInAndOutRightTurn, tightness);
    }

    public void AddRightStraight(int numberOfTrackSegments, int tightness)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.RightStraight, tightness);
    }

    public void AddRightUphillCurve(int numberOfTrackSegments, int tightness, int steepness)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.EaseInAndOutRightTurn, tightness, TrackHillType.EaseInAndOutUphill, steepness);
    }

    public void AddUphillStraight(int numberOfTrackSegments, int steepness)
    {
        AddSection(numberOfTrackSegments, TrackHillType.EaseInAndOutUphill, steepness);
    }

    public void AddDownhillStraight(int numberOfTrackSegments, int steepness)
    {
        AddSection(numberOfTrackSegments, TrackHillType.EaseInAndOutDownhill, steepness);
    }

    /// <summary>
    /// Build the current track
    /// </summary>
    /// <returns></returns>
    public Track Build()
    {
        return new Track
        {
            SegmentHeight = SegmentHeight,            
            Segments = [.. _trackSegments]
        };
    }

    /// <summary>
    /// Add just a turn section, no hills
    /// </summary>
    /// <param name="numberOfTrackSegments"></param>
    /// <param name="turnType"></param>
    /// <param name="tightness"></param>
    private void AddSection(int numberOfTrackSegments, TrackTurnType turnType, int tightness = 0)
    {
        AddSection(numberOfTrackSegments, turnType, tightness, TrackHillType.Flat, 0);
    }

    /// <summary>
    /// Add just a hill section, no turns
    /// </summary>
    /// <param name="numberOfTrackSegments"></param>
    /// <param name="turnType"></param>
    /// <param name="tightness"></param>
    private void AddSection(int numberOfTrackSegments, TrackHillType hillType, int steepness = 0)
    {
        AddSection(numberOfTrackSegments, TrackTurnType.Straight, 0, hillType, steepness);
    }

    /// <summary>
    /// Add a section to the track
    /// </summary>
    /// <param name="numberOfTrackSegmentsToAdd"></param>
    /// <param name="turnType"></param>
    /// <param name="tightness"></param>
    /// <param name="hillType"></param>
    /// <param name="steepness"></param>
    private void AddSection(int numberOfTrackSegmentsToAdd, TrackTurnType turnType, int tightness, TrackHillType hillType, int steepness)
    {
        // Get the current segment count before we start...
        var startingSegmentCount = _trackSegments.Count;
        var lastSegmentIndex = startingSegmentCount + numberOfTrackSegmentsToAdd;

        // Get the last x offset of the previous segment (or 0 if this is the first segment)
        var previousSegmentWorldX = startingSegmentCount == 0 ? 0 : _trackSegments.Last().ZMap.WorldCoordinates.X;
        var previousSegmentWorldY = startingSegmentCount == 0 ? 0 : _trackSegments.Last().ZMap.WorldCoordinates.Y;

        // Add segments according to the length (number of segements specified)
        for (var segmentIndex = startingSegmentCount; segmentIndex < lastSegmentIndex; segmentIndex++)
        {
            // We need to know which iteration of the loop we're on...
            var iterationIndex = segmentIndex - startingSegmentCount;

            // In order to 'stripe' the track (alternating light/dark strips) we just need a calculation
            // which will give us some pattern of alternating 'true' or 'false' for each segment. This
            // way we can use it to either colour a segement (or strip of segments) a certain colour, which
            // in our case will just be alternating light/dark colours
            var segmentStripIndex = (int)Math.Floor(segmentIndex / (float)SegmentsPerStrip);
            var segmentStripIsAnEvenNumber = segmentStripIndex % 2 == 1;

            // Workout turns/curves            
            float dx;

            switch (turnType)
            {
                case TrackTurnType.EaseInLeftTurn:
                    {
                        dx = -MathHelper.Lerp(0, tightness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackTurnType.EaseInRightTurn:
                    {
                        dx = MathHelper.Lerp(0, tightness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackTurnType.LeftStraight:
                    {
                        dx = -MathHelper.Lerp(0, tightness, tightness * iterationIndex * (float)Math.Pow(tightness, 4));
                    }
                    break;

                case TrackTurnType.RightStraight:
                    {
                        dx = MathHelper.Lerp(0, tightness, tightness * iterationIndex * (float)Math.Pow(tightness, 4));
                    }
                    break;

                case TrackTurnType.EaseOutLeftTurn:
                    {
                        dx = -MathHelper.Lerp(tightness * iterationIndex, 0, iterationIndex);
                    }
                    break;

                case TrackTurnType.EaseOutRightTurn:
                    {
                        dx = MathHelper.Lerp(tightness * iterationIndex, 0, iterationIndex);
                    }
                    break;

                case TrackTurnType.EaseInAndOutLeftTurn:
                    {
                        var start = 0f;
                        var end = 1f;
                        var incrementPerIteration = (end - start) / numberOfTrackSegmentsToAdd;
                        var offset = iterationIndex * (1f * incrementPerIteration);

                        dx = -tightness * 1000 * MathHelper.SmoothStep(start, end, offset);
                    }
                    break;

                case TrackTurnType.EaseInAndOutRightTurn:
                    {
                        var start = 0f;
                        var end = 1f;
                        var incrementPerIteration = (end - start) / numberOfTrackSegmentsToAdd;
                        var offset = iterationIndex * (1f * incrementPerIteration);

                        dx = tightness * 1000 * MathHelper.SmoothStep(start, end, offset);
                    }
                    break;

                default:
                    dx = 0;
                    break;
            }

            // Now hills
            float dy;

            switch (hillType)
            {
                case TrackHillType.EaseInUphill:
                    {
                        dy = MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackHillType.EaseInDownhill:
                    {
                        dy = -MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackHillType.StraightUphill:
                    {
                        dy = MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackHillType.StraightDownhill:
                    {
                        dy = -MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackHillType.EaseOutUphill:
                    {
                        dy = MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackHillType.EaseOutDownhill:
                    {
                        dy = -MathHelper.Lerp(0, steepness * iterationIndex, iterationIndex);
                    }
                    break;

                case TrackHillType.EaseInAndOutUphill:
                    {
                        var start = 0f;
                        var end = 1f;
                        var incrementPerIteration = (end - start) / numberOfTrackSegmentsToAdd;
                        var offset = iterationIndex * (1f * incrementPerIteration);

                        dy = steepness * 1000 * MathHelper.SmoothStep(start, end, offset);
                    }
                    break;

                case TrackHillType.EaseInAndOutDownhill:
                    {
                        var start = 0f;
                        var end = 1f;
                        var incrementPerIteration = (end - start) / numberOfTrackSegmentsToAdd;
                        var offset = iterationIndex * (1f * incrementPerIteration);

                        dy = -steepness * 1000 * MathHelper.SmoothStep(start, end, offset);
                    }
                    break;

                default:
                    dy = 0;
                    break;
            }

            // Add a segment to the track
            _trackSegments.Add(new TrackSegment
            {
                Index = segmentIndex,                
                SegmentStripIndex = segmentStripIndex,
                GrassColour = segmentStripIsAnEvenNumber ? GrassColourLight : GrassColourDark,
                RoadColour = segmentStripIsAnEvenNumber ? RoadColourLight : RoadColourDark,
                RumbleColour = segmentStripIsAnEvenNumber ? RumbleStripColourLight : RumbleStripColourDark,
                Lanes = NumberOfLanes,
                LaneColour = LaneColour,
                Width = SegmentWidth,
                ZMap = new ZMap
                {
                    WorldCoordinates = new Vector3(previousSegmentWorldX + dx, previousSegmentWorldY + dy, segmentIndex * SegmentHeight),
                    ScreenCoordinates = new Vector3(0, 0, 0),
                    Scale = -1
                },
            });
        }
    }
}
