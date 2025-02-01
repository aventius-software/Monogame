using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Basic3DCubeWithShaders.Services;

internal class Camera
{
    public float FieldOfView
    {
        get => _fieldOfView;
        set
        {
            _fieldOfView = value;
            RecalculateProjection();
        }
    }

    /// <summary>
    /// The position of the camera in world space
    /// </summary>
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            RecalculateView();
        }
    }

    public Matrix Projection { get; private set; } = Matrix.Identity;

    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            RecalculateWorld();
        }
    }

    /// <summary>
    /// The 'target' point in world space that the camera is looking at
    /// </summary>
    public Vector3 Target
    {
        get => _target;
        set
        {
            _target = value;
            RecalculateView();
        }
    }

    public Matrix View { get; private set; } = Matrix.Identity;
    public Matrix World { get; private set; } = Matrix.Identity;

    private float _farPlaneDistance = 100f;
    private float _fieldOfView = 45;
    private readonly GraphicsDevice _graphicsDevice;
    private float _nearPlaneDistance = 1f;
    private Vector3 _position = Vector3.Zero;
    private Vector3 _rotation = Vector3.Zero;
    private Vector3 _target = Vector3.Zero;

    /// <summary>
    /// The direction that is considered "up" for the camera, typically Vector3.Up (0, 1, 0)
    /// </summary>
    private Vector3 _up = Vector3.Up;

    public Camera(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        RecalculateProjection();
        RecalculateView();
        RecalculateWorld();
    }

    public void LookAt(Vector3 target) => _target = target;
    public void LookDown(float numberOfUnitsToLookDownBy) => _target += new Vector3(0, numberOfUnitsToLookDownBy, 0);
    public void LookLeft(float numberOfUnitsToLookLeftBy) => _target -= new Vector3(numberOfUnitsToLookLeftBy, 0, 0);
    public void LookRight(float numberOfUnitsToLookRightBy) => _target += new Vector3(numberOfUnitsToLookRightBy, 0, 0);
    public void LookUp(float numberOfUnitsToLookUpBy) => _target -= new Vector3(0, numberOfUnitsToLookUpBy, 0);
    public void Move(Vector3 numberOfUnitsToMoveBy) => Position += numberOfUnitsToMoveBy;
    public void MoveDown(float numberOfUnitsToMoveDownBy) => Position += new Vector3(0, numberOfUnitsToMoveDownBy, 0);
    public void MoveLeft(float numberOfUnitsToMoveLeftBy) => Position -= new Vector3(numberOfUnitsToMoveLeftBy, 0, 0);
    public void MoveRight(float numberOfUnitsToMoveRightBy) => Position += new Vector3(numberOfUnitsToMoveRightBy, 0, 0);
    public void MoveUp(float numberOfUnitsToMoveUpBy) => Position -= new Vector3(0, numberOfUnitsToMoveUpBy, 0);

    private void RecalculateProjection()
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(
            fieldOfView: MathHelper.ToRadians(FieldOfView),
            aspectRatio: (float)_graphicsDevice.Viewport.Width / (float)_graphicsDevice.Viewport.Height,
            nearPlaneDistance: _nearPlaneDistance,
            farPlaneDistance: _farPlaneDistance);
    }

    private void RecalculateView()
    {
        View = Matrix.CreateLookAt(Position, Target, _up);
    }

    private void RecalculateWorld()
    {
        var rotationAsRadians = new Vector3(
            MathHelper.ToRadians(Rotation.X),
            MathHelper.ToRadians(Rotation.Y),
            MathHelper.ToRadians(Rotation.Z));

        World = Matrix.CreateRotationX(rotationAsRadians.X)
            * Matrix.CreateRotationY(rotationAsRadians.Y)
            * Matrix.CreateRotationZ(rotationAsRadians.Z);
    }

    public void RotateAnticlockwise(Vector3 degreesToRotate) => Rotation -= degreesToRotate;
    public void RotateClockwise(Vector3 degreesToRotate) => Rotation += degreesToRotate;
    public void ZoomIn(float units) => Position += new Vector3(0, 0, units);
    public void ZoomOut(float units) => Position -= new Vector3(0, 0, units);
}
