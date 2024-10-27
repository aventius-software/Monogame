using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct PlayerComponent : IComponent
{
    public Vector3 Position;
    public float Speed;
    public float MaxSpeed;
}
