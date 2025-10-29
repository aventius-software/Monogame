using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace Shared.Extensions;

/// <summary>
/// Various rectangle extensions - some of methods in this class were originally
/// part of the XNA samples
/// </summary>
public static class RectangleExtensions
{    
    /// <summary>
    /// Calculates the signed depth of intersection between two rectangles.
    /// </summary>
    /// <returns>
    /// The amount of overlap between two intersecting rectangles. These
    /// depth values can be negative depending on which wides the rectangles
    /// intersect. This allows callers to determine the correct direction
    /// to push objects in order to resolve collisions. If the rectangles 
    /// are not intersecting, Vector2.Zero is returned.
    /// </returns>
    public static Vector2 GetIntersectionDepth(this RectangleF sourceRectangle, RectangleF otherRectangle)
    {        
        // Calculate current distances between centers.
        var distanceX = sourceRectangle.Center.X - otherRectangle.Center.X;
        var distanceY = sourceRectangle.Center.Y - otherRectangle.Center.Y;

        // Calculate the minimum-non-intersecting distances between centers.
        var minDistanceX = sourceRectangle.HalfWidth() + otherRectangle.HalfWidth();
        var minDistanceY = sourceRectangle.HalfHeight() + otherRectangle.HalfHeight();

        // If we are not intersecting at all, return (0, 0).
        if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            return Vector2.Zero;

        // Calculate and return intersection depths.
        var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

        return new Vector2(depthX, depthY);
    }

    public static float HalfHeight(this RectangleF rectangle)
    {
        return rectangle.Height / 2f;
    }

    public static float HalfWidth(this RectangleF rectangle)
    {
        return rectangle.Width / 2f;
    }
}
