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
        // Reserve an array to store the hill segment
        var hillSegments = new HillSegment[segmentsPerHill * numberOfHills];

        // Set initial starting position
        var x = startPosition.X;
        var y = startPosition.Y;

        // Per hill we gradually increase our angle per segment to cover full 360 degrees
        var angleIncrement = MathHelper.ToRadians(360 / segmentsPerHill);

        // Create requested number of hills
        for (var hillIndex = 0; hillIndex < numberOfHills; hillIndex++)
        {
            // Choose a random height offset per hill
            var randomHeight = _random.Next(-maxOffsetY, maxOffsetY);
            y += randomHeight;

            // Also, choose a random 'steepness'
            var randomSteepness = _random.Next(1, maxHillSteepness);

            // Now, generate requested number of segments per hill
            for (var hillSegmentIndex = 0; hillSegmentIndex < segmentsPerHill; hillSegmentIndex++)
            {
                // Which array index are we on?
                var index = (hillIndex * segmentsPerHill) + hillSegmentIndex;

                // If its not the first hill segment, we use the y coordinate of the previous segment
                if (index > 0) y = hillSegments[index - 1].End.Y;

                // Calculate start coordinates for this segment
                var start = new Vector2(x, y);

                // Now calculate the position of the end of the segment
                x += segmentWidth;
                var offsetY = (float)Math.Sin(hillSegmentIndex * angleIncrement) * randomSteepness;
                var end = new Vector2(x, y + offsetY);

                // Create this segment
                hillSegments[index] = new HillSegment
                {
                    Start = start,
                    End = end
                };
            }

            // Remove our 'random' offset so the next hill can have another
            y -= randomHeight;
        }

        return hillSegments;
    }
}
