using Microsoft.Xna.Framework;

namespace OutrunStyleTest.Player;

internal class PlayerComponent
{
    /// <summary>
    /// Acceleration rate.
    /// </summary>
    public int AccelerationRate;

    /// <summary>
    /// Limit on the maximum speed.
    /// </summary>
    public float MaxSpeed;

    /// <summary>
    /// Position of the player in the world.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Current speed of the player.
    /// </summary>
    public float Speed;

    /// <summary>
    /// The rate at which the player can steer left/right.
    /// </summary>
    public float SteeringStrength;
}
