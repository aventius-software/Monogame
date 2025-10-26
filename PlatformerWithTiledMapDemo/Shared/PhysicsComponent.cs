using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace PlatformerWithTiledMapDemo.Shared;

internal class PhysicsComponent
{
    public float AirDrag = 0.75f;
    public RectangleF CollisionBoxOffsetBounds;
    public float Gravity = 800f;
    public float GroundFriction = 0.75f;
    public bool IsOnGround;
    public float JumpStrength = 300f;
    public float MaximumHorizontalSpeed = 120f;
    public float RunAcceleration = 80f;
    public Vector2 Velocity;
}
