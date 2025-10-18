using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace Shared.Services;

/// <summary>
/// This is just a simple wrapper around the Aether Physics 2d 'World' class to add some helper methods. Probably
/// the most useful is the ability to convert between display units (pixels) and simulation units (metres).
/// </summary>
public class PhysicsService : World
{
    private float _displayUnitsPerSimUnit = 1f;
    private float _simUnitsToDisplayUnitsRatio = 1f;

    /// <summary>
    /// The number of pixels per metre. The default is 1 pixel per metre unless changed.
    /// </summary>
    public float PixelsPerMetre => _displayUnitsPerSimUnit;

    /// <summary>
    /// Creates a boundry (edges)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void CreateBoundry(float x, float y, float width, float height)
    {
        var topLeft = new Vector2(x, y);
        var topRight = new Vector2(width, y);
        var bottomLeft = new Vector2(x, height);
        var bottomRight = new Vector2(width, height);

        CreateEdge(topLeft, topRight);
        CreateEdge(topRight, bottomRight);
        CreateEdge(bottomLeft, bottomRight);
        CreateEdge(topLeft, bottomLeft);
    }

    /// <summary>
    /// Set the ratio of simulation units (i.e. 1 metre) to pixel units. In
    /// other words, how many pixels is 1 metre?
    /// </summary>
    /// <param name="displayUnitsPerSimUnit"></param>
    public void SetPixelsPerMetre(float displayUnitsPerSimUnit)
    {
        _displayUnitsPerSimUnit = displayUnitsPerSimUnit;
        _simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
    }

    /// <summary>
    /// Converts simulation units (metres) to display units (pixels)
    /// </summary>
    /// <param name="simUnits"></param>
    /// <returns></returns>
    public Vector2 ToDisplayUnits(Vector2 simUnits) => simUnits * _displayUnitsPerSimUnit;

    /// <summary>
    /// Converts display units (pixels) to simulation units (metres)
    /// </summary>
    /// <param name="displayUnits"></param>
    /// <returns></returns>
    public float ToSimUnits(int displayUnits) => displayUnits * _simUnitsToDisplayUnitsRatio;

    /// <summary>
    /// Converts display units (pixels) to simulation units (metres)
    /// </summary>
    /// <param name="displayUnits"></param>
    /// <returns></returns>
    public Vector2 ToSimUnits(Vector2 displayUnits) => displayUnits * _simUnitsToDisplayUnitsRatio;
}
