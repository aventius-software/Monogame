using Microsoft.Xna.Framework;

namespace OutrunStyleTest.Camera;

internal class CameraComponent
{
    /// <summary>
    /// Z-distance between camera and player.
    /// </summary>
    public float DistanceToPlayer;

    /// <summary>
    /// Z-distance between camera and normalized projection plane.
    /// </summary>
    public float DistanceToProjectionPlane;

    /// <summary>
    /// Height above the player we want the camera to be at.
    /// </summary>
    public float HeightAbovePlayer;

    /// <summary>
    /// Position of the camera (duh ;-).
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Height of the viewport in pixels.
    /// </summary>
    public int ViewportHeight;

    /// <summary>
    /// Width of the viewport in pixels.
    /// </summary>
    public int ViewportWidth;
}
