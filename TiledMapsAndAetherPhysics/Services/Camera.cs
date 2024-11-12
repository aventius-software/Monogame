using Microsoft.Xna.Framework;

namespace TiledMapsAndAetherPhysics.Services;

/// <summary>
/// This is just a basic 2D camera which we can use in conjunction with 
/// SpriteBatch for viewing our game world from different positions
/// </summary>
internal class Camera
{
    private Vector2 _origin;
    private float _rotation = 0;
    private Vector2 _scale = Vector2.One;
    private Vector2 _worldDimensions;

    /// <summary>
    /// The current world position
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// The current transformation matrix
    /// </summary>
    public Matrix TransformMatrix { get; private set; }

    /// <summary>
    /// Move the camera to (or look at) the specified world position
    /// </summary>
    /// <param name="positionInTheWorld"></param>
    /// <param name="offset"></param>
    public void LookAt(Vector2 positionInTheWorld, Vector2 offset)
    {
        // Clamp position so the camera doesn't go beyond the edges of the world
        var x = MathHelper.Clamp(positionInTheWorld.X, _origin.X, _worldDimensions.X - _origin.X - offset.X);
        var y = MathHelper.Clamp(positionInTheWorld.Y, _origin.Y, _worldDimensions.Y - _origin.Y - offset.Y);

        // Save the new/current 'clamped' camera position
        Position = new Vector2(x, y);

        // Calculate our transformation matrix. Note that the camera moves in the opposite direction
        // to a character so we 'invert' the position (hence the minus signs). We're also applying
        // rotation and scaling here, so try changing some of the field values in the class to see
        // what happens when they're not set to the default values ;-)
        TransformMatrix =
            Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f)) *
            Matrix.CreateRotationZ(_rotation) *
            Matrix.CreateScale(_scale.X, _scale.Y, 1f) *
            Matrix.CreateTranslation(_origin.X, _origin.Y, 0f);
    }

    /// <summary>
    /// Set the camera origin
    /// </summary>
    /// <param name="origin"></param>
    public void SetOrigin(Vector2 origin)
    {
        _origin = origin;
    }

    /// <summary>
    /// Set dimensions for the world
    /// </summary>
    /// <param name="worldDimensions"></param>
    public void SetWorldDimensions(Vector2 worldDimensions)
    {
        _worldDimensions = worldDimensions;
    }

    /// <summary>
    /// Translate a world position to screen position
    /// </summary>
    /// <param name="positionInTheWorld"></param>
    /// <returns></returns>
    public Vector2 WorldToScreen(Vector2 positionInTheWorld)
    {
        return Vector2.Transform(positionInTheWorld, Matrix.Invert(TransformMatrix));
    }
}
