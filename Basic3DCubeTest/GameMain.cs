using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Basic3DCubeTest;

/// <summary>
/// A migrated (slightly modified) version of the original XNA cube demo code
/// </summary>
public class GameMain : Game
{
    private BasicEffect _basicEffect;
    private Vector3 _cameraPosition;
    private float _cameraRotationX;
    private float _cameraRotationY;
    private Vector3 _cameraTarget;
    private Vector3 _cameraUpVector;
    private float _fieldOfView;
    private GraphicsDeviceManager _graphics;
    private Matrix _projectionMatrix;
    private VertexBuffer _vertexBuffer;
    private Matrix _viewMatrix;
    private Matrix _worldMatrix;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    /// Returns a set of vertices for a basic 3D cube
    /// </summary>
    /// <returns></returns>
    private static VertexPositionColor[] BuildCubeVertices()
    {
        Vector3 LeftTopFront = new Vector3(-1.0f, 1.0f, 1.0f);
        Vector3 LeftBottomFront = new Vector3(-1.0f, -1.0f, 1.0f);
        Vector3 LeftTopBack = new Vector3(-1.0f, 1.0f, -1.0f);
        Vector3 LeftBottomBack = new Vector3(-1.0f, -1.0f, -1.0f);

        Vector3 RightTopFront = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 RightBottomFront = new Vector3(1.0f, -1.0f, 1.0f);
        Vector3 RightTopBack = new Vector3(1.0f, 1.0f, -1.0f);
        Vector3 RightBottomBack = new Vector3(1.0f, -1.0f, -1.0f);

        return
        [
            // Front face.
            new(LeftTopFront, Color.DarkGray),
            new(LeftBottomFront, Color.DarkGray),
            new(RightTopFront, Color.DarkGray),

            new(LeftBottomFront, Color.Gray),
            new(RightBottomFront, Color.Gray),
            new(RightTopFront, Color.Gray),

            // Back face.
            new(LeftTopBack, Color.DarkGreen),
            new(RightTopBack, Color.DarkGreen),
            new(LeftBottomBack, Color.DarkGreen),

            new(LeftBottomBack, Color.Green),
            new(RightTopBack, Color.Green),
            new(RightBottomBack, Color.Green),

            // Top face.
            new(LeftTopFront, Color.DarkBlue),
            new(RightTopBack, Color.DarkBlue),
            new(LeftTopBack, Color.DarkBlue),

            new(LeftTopFront, Color.Blue),
            new(RightTopFront, Color.Blue),
            new(RightTopBack, Color.Blue),

            // Bottom face. 
            new(LeftBottomFront, Color.DarkOrange),
            new(LeftBottomBack, Color.DarkOrange),
            new(RightBottomBack, Color.DarkOrange),

            new(LeftBottomFront, Color.Orange),
            new(RightBottomBack, Color.Orange),
            new(RightBottomFront, Color.Orange),

            // Left face.
            new(LeftTopFront, Color.DarkRed),
            new(LeftBottomBack, Color.DarkRed),
            new(LeftBottomFront, Color.DarkRed),

            new(LeftTopBack, Color.Red),
            new(LeftBottomBack, Color.Red),
            new(LeftTopFront, Color.Red),

            // Right face. 
            new(RightTopFront, Color.DarkViolet),
            new(RightBottomFront, Color.DarkViolet),
            new(RightBottomBack, Color.DarkViolet),

            new(RightTopBack, Color.Violet),
            new(RightTopFront, Color.Violet),
            new(RightBottomBack, Color.Violet)
        ];
    }

    /// <summary>
    /// Initialise everything
    /// </summary>
    protected override void Initialize()
    {
        // First initialise the cube vertex data
        InitialiseVertexBuffer();

        // Next we can setup the world, view and project matrices
        InitializeTransform();

        // Finally, after we've setup our matrices, we can create a
        // BasicEffect and add the matrices to it etc...
        InitialiseBasicEffect();

        // Set the rasteriser state
        var rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;
        GraphicsDevice.RasterizerState = rasterizerState;

        base.Initialize();
    }

    /// <summary>
    /// Setup the basic effect
    /// </summary>
    private void InitialiseBasicEffect()
    {
        _basicEffect = new BasicEffect(GraphicsDevice);

        // Set the matrices
        _basicEffect.World = _worldMatrix;
        _basicEffect.View = _viewMatrix;
        _basicEffect.Projection = _projectionMatrix;

        // For just coloured vertices we need to enable colour
        // otherwise the shape will just be white
        _basicEffect.VertexColorEnabled = true;

        // Disable lighting otherwise it will alter the flat
        // colours we want to use for our basic test
        _basicEffect.LightingEnabled = false;
    }

    private void InitialiseVertexBuffer()
    {
        // Get the 3D model data for our cube
        var cubeVertices = BuildCubeVertices();

        // Set the vertex buffer
        _vertexBuffer = new VertexBuffer(_graphics.GraphicsDevice, typeof(VertexPositionColor), cubeVertices.Length, BufferUsage.None);
        _vertexBuffer.SetData<VertexPositionColor>(cubeVertices);

        _graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
    }

    /// <summary>
    /// Initializes the transforms used for the 3D model.
    /// </summary>
    private void InitializeTransform()
    {
        // Build the world matrix
        _cameraRotationX = MathHelper.ToRadians(45);
        _cameraRotationY = MathHelper.ToRadians(45);
        _worldMatrix = Matrix.CreateRotationX(_cameraRotationX) * Matrix.CreateRotationY(_cameraRotationY);

        // Set the initial starting view matrix
        _cameraPosition = new Vector3(0, 0, 10);
        _cameraTarget = Vector3.Zero;
        _cameraUpVector = Vector3.Up;
        _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, _cameraUpVector);

        // Set the projection matrix
        _fieldOfView = MathHelper.ToRadians(45);
        _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            fieldOfView: _fieldOfView,
            aspectRatio: (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
            nearPlaneDistance: 1.0f,
            farPlaneDistance: 100.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var speed = 0.1f;
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Left))
        {
            _cameraPosition.X -= speed;
            _cameraTarget.X -= speed;
        }

        if (keyboard.IsKeyDown(Keys.Right))
        {
            _cameraPosition.X += speed;
            _cameraTarget.X += speed;
        }

        if (keyboard.IsKeyDown(Keys.Up))
        {
            _cameraPosition.Y -= speed;
            _cameraTarget.Y -= speed;
        }

        if (keyboard.IsKeyDown(Keys.Down))
        {
            _cameraPosition.Y += speed;
            _cameraTarget.Y += speed;
        }

        if (keyboard.IsKeyDown(Keys.OemPlus))
        {
            _cameraPosition.Z += speed;
        }

        if (keyboard.IsKeyDown(Keys.OemMinus))
        {
            _cameraPosition.Z -= speed;
        }

        // Change the view
        _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, _cameraUpVector);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Update our view
        _basicEffect.View = _viewMatrix;

        // Draw the vertex buffer        
        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount / 2);
        }

        base.Draw(gameTime);
    }
}
