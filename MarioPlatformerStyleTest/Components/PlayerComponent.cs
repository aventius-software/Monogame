using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Components;

internal struct PlayerComponent : IComponent
{
    public int Height;
    public bool IsOnTheGround;
    public Vector2 Position;
    public Texture2D Texture;
    public Vector2 Velocity;
    public int Width;
}
