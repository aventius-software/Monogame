using Scellecs.Morpeh;

namespace MorpehECSTest.Components;

internal struct DriftComponent : IComponent
{    
    public float DriftFactor;
    public bool IsDriftingEnabled;
    public bool IsSkidding;
    public float SavedDriftFactor;
    public float SkiddingDriftFactor;
}
