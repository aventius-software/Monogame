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

    public HillSegment[] GenerateHills(int numberOfHills, int segmentsPerHill, Vector2 startPosition)
    {                
        var hillSegments = new HillSegment[segmentsPerHill * numberOfHills];

        var x = startPosition.X;
        var y = startPosition.Y;
        var step = 8;        
        var angle = MathHelper.ToRadians(360 / segmentsPerHill);        

        for (var hillIndex = 0; hillIndex < numberOfHills; hillIndex++)
        {
            var randomHeight = _random.Next(-25, 25);
            var randomDepth = _random.Next(1, 24);
            y += randomHeight;

            for (var hillSegmentIndex = 0; hillSegmentIndex < segmentsPerHill; hillSegmentIndex++)
            {
                var index = (hillIndex * segmentsPerHill) + hillSegmentIndex;
                if (index > 0) y = hillSegments[index - 1].End.Y;

                var start = new Vector2(x, y);
                x += step;

                var offsetY = (float)Math.Sin(hillSegmentIndex * angle) * randomDepth;
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
