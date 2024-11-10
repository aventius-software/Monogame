using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Components;

internal struct TransformComponent : IComponent
{
    public int Height;
    public Vector2 Position;
    public float Speed;
    public Vector2 Velocity;
    public int Width;
}
