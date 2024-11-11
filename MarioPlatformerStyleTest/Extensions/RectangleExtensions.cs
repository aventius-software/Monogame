using Microsoft.Xna.Framework;
using System;

namespace MarioPlatformerStyleTest.Extensions;

internal static class RectangleExtensions
{
    public static Rectangle FromPosition(Vector2 position, int width, int height)
    {
        return new Rectangle((int)position.X, (int)position.Y, width, height);
    }

    public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
    {
        // Calculate half sizes.
        float halfWidthA = rectA.Width / 2.0f;
        float halfHeightA = rectA.Height / 2.0f;
        float halfWidthB = rectB.Width / 2.0f;
        float halfHeightB = rectB.Height / 2.0f;

        // Calculate centers.
        var centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
        var centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

        // Calculate current and minimum-non-intersecting distances between centers.
        float distanceX = centerA.X - centerB.X;
        float distanceY = centerA.Y - centerB.Y;
        float minDistanceX = halfWidthA + halfWidthB;
        float minDistanceY = halfHeightA + halfHeightB;

        // If we are not intersecting at all, return (0, 0).
        if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY) return Vector2.Zero;

        // Calculate and return intersection depths.
        float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

        return new Vector2(depthX, depthY);
    }
}
