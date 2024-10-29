using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct PlayerComponent : IComponent
{
    public int AccelerationRate;
    public float MaxSpeed;
    public Vector3 Position;
    public float Speed;    
}
