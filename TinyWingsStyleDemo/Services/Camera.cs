using Microsoft.Xna.Framework;

namespace TinyWingsStyleDemo.Services;

/// <summary>
/// This is just a basic 2D camera which we can use in conjunction with 
/// SpriteBatch for viewing our game world from different positions
/// </summary>
internal class Camera
{
    private Vector2 _origin = Vector2.Zero;
    private float _rotation = 0;
    private Vector2 _scale = Vector2.One;
    private Vector2 _worldDimensions = Vector2.Zero;

    /// <summary>
    /// The current origin position
    /// </summary>
    public Vector2 Origin => _origin;

    /// <summary>
    /// The current world position
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// The previous world position
    /// </summary>
    public Vector2 PreviousPosition { get; private set; }

    /// <summary>
    /// The previous transformation matrix
    /// </summary>
    public Matrix PreviousTransformMatrix { get; private set; }

    /// <summary>
    /// The current rotation matrix
    /// </summary>
    public Matrix RotationMatrix => Matrix.CreateRotationZ(_rotation);

    /// <summary>
    /// Current scale (or zoom) factor
    /// </summary>
    public Vector2 Scale => _scale;

    /// <summary>
    /// The current scaling matrix
    /// </summary>
    public Matrix ScalingMatrix => Matrix.CreateScale(_scale.X, _scale.Y, 1f);

    /// <summary>
    /// The current translation matrix
    /// </summary>
    public Matrix TranslationMatrix => Matrix.CreateTranslation(_origin.X, _origin.Y, 0f) *
        Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f));

    /// <summary>
    /// The current world dimensions
    /// </summary>
    public Vector2 WorldDimensions => _worldDimensions;

    /// <summary>
    /// The current transformation matrix
    /// </summary>
    public Matrix TransformMatrix { get; private set; }

    /// <summary>
    /// Calculate transformation for current state
    /// </summary>
    /// <returns></returns>
    private Matrix CalculateMatrix() => TranslationMatrix * RotationMatrix * ScalingMatrix;

    /// <summary>
    /// Move the camera to (or look at) the specified world position
    /// </summary>
    /// <param name="positionInTheWorld"></param>
    /// <param name="offset"></param>
    public void LookAt(Vector2 positionInTheWorld, Vector2 offset)
    {
        // Save previous position
        PreviousPosition = Position;
        PreviousTransformMatrix = TransformMatrix;

        // Clamp position so the camera doesn't go beyond the edges of the world
        var x = MathHelper.Clamp(positionInTheWorld.X, _origin.X, _worldDimensions.X - _origin.X - offset.X);
        var y = MathHelper.Clamp(positionInTheWorld.Y, _origin.Y, _worldDimensions.Y - _origin.Y - offset.Y);

        // Save the new/current 'clamped' camera position
        Position = new Vector2(x, y);

        // Calculate our transformation matrix. Note that the camera moves in the opposite direction
        // to a character so we 'invert' the position (hence the minus signs). We're also applying
        // rotation and scaling here, so try changing some of the field values in the class to see
        // what happens when they're not set to the default values ;-)
        TransformMatrix = CalculateMatrix();
    }

    /// <summary>
    /// Translate a screen position to world position
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, Matrix.Invert(TransformMatrix));
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
    /// Translates a world position to a screen position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, TransformMatrix);
    }

    /// <summary>
    /// Set the level of zoom (or scale)
    /// </summary>
    /// <param name="scale"></param>
    public void Zoom(float scale)
    {
        _scale = new Vector2(scale, scale);
    }
}
