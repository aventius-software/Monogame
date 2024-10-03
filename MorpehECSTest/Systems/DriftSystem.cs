using MorpehECSTest.Components;
using Scellecs.Morpeh;

namespace MorpehECSTest.Systems;

/// <summary>
/// This system handles drifting and skidding
/// </summary>
internal class DriftSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;

    public DriftSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        _filter = World.Filter.With<DriftComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            // Get references for our components            
            ref var driftComponent = ref entity.GetComponent<DriftComponent>();
            ref var rigidBodyComponent = ref entity.GetComponent<RigidBodyComponent>();
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();

            // If drifting is enabled, set the drift factor. If drifting is not enabled then
            // we set to 0 (no drift, basically driving on rails ;-)
            var driftFactor = driftComponent.IsDriftingEnabled ? driftComponent.DriftFactor : 0;

            // Reduce 'sideways' velocity depending on drift factor
            rigidBodyComponent.Body.LinearVelocity = transformComponent.ForwardVelocity + (transformComponent.RightVelocity * driftFactor);

            // Reset skidding drift factor if we were previous drifting
            if (driftComponent.SavedDriftFactor != 0)
            {
                driftComponent.DriftFactor = driftComponent.SavedDriftFactor;
                driftComponent.SavedDriftFactor = 0;
            }

            // Handle skidding, a bit like handbrake turns
            if (driftComponent.IsSkidding)
            {
                // Save previous drift factor before skidding if we've not saved it already
                if (driftComponent.SavedDriftFactor != driftComponent.DriftFactor) driftComponent.SavedDriftFactor = driftComponent.DriftFactor;

                // Set high drift factor
                driftComponent.DriftFactor = driftComponent.SkiddingDriftFactor;
            }
        }
    }
}
