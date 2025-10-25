using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace PlatformerWithTiledMapDemo.Shared;

internal class PhysicsComponent
{
    public RectangleF CollisionBoxOffsetBounds;
    public float Gravity = 800f;
    public float GroundFriction = 0.75f;
    public bool IsOnGround;
    public float JumpStrength = 300f;
    public float MaximumRunningSpeed = 120f;
    public float RunAcceleration = 80f;
    public Vector2 Velocity;
}
