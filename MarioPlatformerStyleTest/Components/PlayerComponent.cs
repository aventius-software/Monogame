using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Components;

internal struct PlayerComponent : IComponent
{
    public float FallingGravityMultiplier;
    public bool IsJumpPressed;
    public float LowJumpGravityMultiplier;
}
