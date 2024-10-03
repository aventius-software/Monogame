using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace MorpehECSTest.Physics;

internal class PhysicsWorld : World
{
    private float _displayUnitsToSimUnitsRatio = 100f;
    private float _simUnitsToDisplayUnitsRatio = 1 / 100f;

    public Body CreateRigidBodyRectangle(Vector2 position, int widthInPixels, int heightInPixels, float mass)
    {
        var body = CreateRectangle(
            width: ToSimUnits(widthInPixels),
            height: ToSimUnits(heightInPixels),
            density: 1,
            position: ToSimUnits(position),
            rotation: 0,
            bodyType: BodyType.Dynamic);

        body.Mass = mass;

        return body;
    }

    public void SetDisplayUnitToSimUnitRatio(float displayUnitsPerSimUnit)
    {
        _displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
        _simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
    }

    public Vector2 ToDisplayUnits(Vector2 simUnits) => simUnits * _displayUnitsToSimUnitsRatio;
    public float ToSimUnits(int displayUnits) => displayUnits * _simUnitsToDisplayUnitsRatio;
    public Vector2 ToSimUnits(Vector2 displayUnits) => displayUnits * _simUnitsToDisplayUnitsRatio;
}
