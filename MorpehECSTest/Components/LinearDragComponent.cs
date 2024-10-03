using Scellecs.Morpeh;

namespace MorpehECSTest.Components;

internal struct LinearDragComponent : IComponent
{
    public float Drag;
    public float RateOfChange;
}
