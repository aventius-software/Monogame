using Microsoft.Xna.Framework;
using System;

namespace TinyWingsStyleDemo.Services;

internal struct HillSegment
{
    public Vector2 Start, End;
}

internal class HillGeneratorService
{
    private readonly Random _random = new();

    public HillSegment[] GenerateHills(Vector2 startPosition, int numberOfHills, int segmentsPerHill = 32, int segmentWidth = 16, int maxOffsetY = 25, int maxHillSteepness = 25)
    {                
        var hillSegments = new HillSegment[segmentsPerHill * numberOfHills];

        var x = startPosition.X;
        var y = startPosition.Y;             
        var angleIncrement = MathHelper.ToRadians(360 / segmentsPerHill);        

        for (var hillIndex = 0; hillIndex < numberOfHills; hillIndex++)
        {
            var randomHeight = _random.Next(-maxOffsetY, maxOffsetY);
            var randomSteepness = _random.Next(1, maxHillSteepness);
            y += randomHeight;

            for (var hillSegmentIndex = 0; hillSegmentIndex < segmentsPerHill; hillSegmentIndex++)
            {
                var index = (hillIndex * segmentsPerHill) + hillSegmentIndex;
                if (index > 0) y = hillSegments[index - 1].End.Y;

                var start = new Vector2(x, y);
                x += segmentWidth;

                var offsetY = (float)Math.Sin(hillSegmentIndex * angleIncrement) * randomSteepness;
                var end = new Vector2(x, y + offsetY);

                hillSegments[index] = new HillSegment
                {
                    Start = start,
                    End = end
                };
            }

            y -= randomHeight;
        }

        return hillSegments;
    }
}
