using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;

namespace MorpehECSTest.Components;

internal struct SpriteComponent : IComponent
{
    public Vector2 Origin;
    public Texture2D Texture;
}
