using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonogameExtendedIsometricTiledMapDemo.Shared.Physics;

internal class PhysicsComponent
{
    public bool IsMovingDownwards => Velocity.Y > 0;

    public float AirDrag = 0.75f;
    public RectangleF CollisionBoxOffsetBounds;
    public float Gravity = 700f;
    public float GravityMultiplier = 1f;
    public float GroundFriction = 0.75f;
    public bool IsOnGround;
    public float JumpStrength = 300f;
    public float MaximumHorizontalSpeed = 120f;
    public float RunAcceleration = 0.1f;
    public Vector2 Velocity;
}
