using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Components;

internal struct CharacterComponent : IComponent
{    
    public bool IsOnTheGround;
    public float JumpStrength;
    public int BoundryOffset;    
    public Texture2D Texture;    
}
