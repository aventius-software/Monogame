using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace PlatformerWithTiledMapDemo.Shared;

internal class PhysicsComponent
{
    public RectangleF CollisionBoxOffsetBounds;
    public float Gravity = 22f;
    public float GroundFriction = 0.75f;
    public bool IsOnGround;
    public float JumpStrength = 400f;
    public float MoveSpeed = 100f;
    public Vector2 Velocity;
}
