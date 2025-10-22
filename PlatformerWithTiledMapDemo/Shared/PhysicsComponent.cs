using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace PlatformerWithTiledMapDemo.Shared;

internal class PhysicsComponent
{    
    public RectangleF CollisionBoxOffsetBounds;
    public bool IsFalling => Velocity.Y > 0;
    public bool IsJumping => Velocity.Y < 0;
    public bool IsMovingLeft => Velocity.X < 0;
    public bool IsMovingRight => Velocity.X > 0;

    public float Gravity = 22f;
    public bool IsOnGround;
    public Vector2 Velocity;
}
