using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace MorpehECSTest.Components;

internal struct TransformComponent : IComponent
{
    public Vector2 Direction;
    public float ForwardSpeed;
    public Vector2 ForwardVector;
    public Vector2 ForwardVelocity;
    public Vector2 Position;
    public Vector2 RightVector;
    public Vector2 RightVelocity;
    public float Rotation;
}
