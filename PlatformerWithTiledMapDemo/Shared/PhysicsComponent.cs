using Microsoft.Xna.Framework;

namespace PlatformerWithTiledMapDemo.Shared;

internal class PhysicsComponent
{
    public float Gravity = 22f;
    public bool IsOnGround;
    public Vector2 Velocity;
}
