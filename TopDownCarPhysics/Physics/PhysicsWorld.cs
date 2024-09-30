using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace TopDownCarPhysics.Physics;

public class PhysicsWorld : World
{
    private float _displayUnitsToSimUnitsRatio = 100f;
    private float _simUnitsToDisplayUnitsRatio = 1 / 100f;

    public void SetDisplayUnitToSimUnitRatio(float displayUnitsPerSimUnit)
    {
        _displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
        _simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
    }

    public Vector2 ToDisplayUnits(Vector2 simUnits) => simUnits * _displayUnitsToSimUnitsRatio;
    public float ToSimUnits(int displayUnits) => displayUnits * _simUnitsToDisplayUnitsRatio;
    public Vector2 ToSimUnits(Vector2 displayUnits) => displayUnits * _simUnitsToDisplayUnitsRatio;
}
