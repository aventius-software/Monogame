using Scellecs.Morpeh;

namespace MorpehECSTest.Components;

internal struct AngularDragComponent : IComponent
{
    public float Drag;
    public float RateOfChange;
}
