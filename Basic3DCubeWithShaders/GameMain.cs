using Basic3DCubeWithShaders.Services;
using Basic3DCubeWithShaders.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Basic3DCubeWithShaders;

/// <summary>
/// Just a basic example of drawing a 3D cube with a simple camera, and 
/// with a custom vertex/pixel shader (note that we're not using BasicEffect)
/// </summary>
public class GameMain : Game
{
    private Camera _camera;
    private GraphicsDeviceManager _graphics;
    private Effect _shader;
    private Texture2D _textureAtlas;
    private VertexBuffer _vertexBuffer;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 1200;
        _graphics.ApplyChanges();
    }

    /// <summary>
    /// Initialise everything
    /// </summary>
    protected override void Initialize()
    {
        _camera = new Camera(GraphicsDevice);

        // First initialise the cube vertex data
        InitialiseVertexBuffer();

        // Next we can setup the camera world, view and project matrices
        InitialiseCamera();

        // Set the rasteriser state        
        //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;        
        GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        base.Initialize();
    }

    private void InitialiseVertexBuffer()
    {
        // Get the 3D model data for our cube
        var vertices = Cube.GenerateVertices(4);

        // Set the vertex buffer
        _vertexBuffer = new VertexBuffer(
            graphicsDevice: _graphics.GraphicsDevice,
            type: typeof(VertexPositionColorTexture),
            vertexCount: vertices.Length, 
            bufferUsage: BufferUsage.None);

        _vertexBuffer.SetData<VertexPositionColorTexture>(vertices);

        _graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
    }

    /// <summary>
    /// Initializes the transforms used for the 3D model.
    /// </summary>
    private void InitialiseCamera()
    {
        _camera.FieldOfView = 45;
        _camera.Rotation = new Vector3(-15, -25, 0);
        _camera.Position = new Vector3(0, 0, 10);
        _camera.Target = Vector3.Zero;
    }

    protected override void LoadContent()
    {
        // Load our basic custom shader
        _shader = Content.Load<Effect>("Shaders/interesting-shader");
        _textureAtlas = Content.Load<Texture2D>("Textures/texture-atlas");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var speed = 0.05f;
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Left))
        {
            _camera.MoveLeft(speed);
            _camera.LookLeft(speed);
        }

        if (keyboard.IsKeyDown(Keys.Right))
        {
            _camera.MoveRight(speed);
            _camera.LookRight(speed);
        }

        if (keyboard.IsKeyDown(Keys.Up))
        {
            _camera.MoveUp(speed);
            _camera.LookUp(speed);
        }

        if (keyboard.IsKeyDown(Keys.Down))
        {
            _camera.MoveDown(speed);
            _camera.LookDown(speed);
        }

        if (keyboard.IsKeyDown(Keys.OemPlus))
        {
            _camera.ZoomIn(speed);
        }

        if (keyboard.IsKeyDown(Keys.OemMinus))
        {
            _camera.ZoomOut(speed);
        }

        if (keyboard.IsKeyDown(Keys.Q))
        {
            _camera.RotateAnticlockwise(new Vector3(0, 0, speed * 4));
        }

        if (keyboard.IsKeyDown(Keys.E))
        {
            _camera.RotateClockwise(new Vector3(0, 0, speed * 4));
        }

        if (keyboard.IsKeyDown(Keys.A))
        {
            _camera.RotateAnticlockwise(new Vector3(speed * 4, 0, 0));
        }

        if (keyboard.IsKeyDown(Keys.D))
        {
            _camera.RotateClockwise(new Vector3(speed * 4, 0, 0));
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Pass our camera matrices to the shader
        _shader.Parameters["World"].SetValue(_camera.World);
        _shader.Parameters["View"].SetValue(_camera.View);
        _shader.Parameters["Projection"].SetValue(_camera.Projection);
        //_shader.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        _shader.Parameters["Texture"].SetValue(_textureAtlas);

        // Draw the vertex buffer        
        foreach (EffectPass pass in _shader.CurrentTechnique.Passes)
        {
            pass.Apply();
            //GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount / 2);
            GraphicsDevice.DrawPrimitives(
                primitiveType: PrimitiveType.TriangleList,
                vertexStart: 0, // always 0
                primitiveCount: _vertexBuffer.VertexCount / 2); // number of triangles
        }

        base.Draw(gameTime);
    }
}
