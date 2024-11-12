using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace TiledMapsAndAetherPhysics.Components;

internal struct TransformComponent : IComponent
{    
    public int Height;    
    public Vector2 Position;
    public float Rotation;
    public int Width;
}
