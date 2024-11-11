using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Components;

internal struct TransformComponent : IComponent
{
    public readonly bool IsMovingDownwards => Velocity.Y > 0;
    public readonly bool IsMovingUpwards => Velocity.Y < 0;    

    public int Height;    
    public Vector2 Position;
    public float Speed;
    public Vector2 Velocity;
    public int Width;
}
