using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace ShadersWithMonogameExtendedCamera;

/// <summary>
/// Testing various simple shaders with Monogame Extended Camera. This shows how to draw a quadrilateral 
/// using a shader and how to keep the shader 'pattern' in sync with the movement of the camera. The shaders are 
/// simple tiled patterns that repeat across the quadrilateral.
/// 
/// For Visual Studio 2022, the HLSL tools extension was useful, see this link below
/// https://marketplace.visualstudio.com/items?itemName=TimGJones.HLSLToolsforVisualStudio
/// 
/// Also you can play around using ShaderToy here which can be useful and there's lots of
/// examples. Although can be complex examples https://www.shadertoy.com/ and also try this
/// cheatsheet for shader toy https://gist.github.com/markknol/d06c0167c75ab5c6720fe9083e4319e1
/// 
/// See also:
/// 
/// https://gmjosack.github.io/posts/my-first-2d-pixel-shaders-part-1/
/// https://gamedev.net/tutorials/_/technical/apis-and-tools/2d-lighting-system-in-monogame-r4131/
/// https://thebookofshaders.com/
/// https://www.ronja-tutorials.com/
/// https://github.com/butterw/bShaders
/// https://gist.github.com/Piratkopia13/46c5dda51ed59cfe69b242deb0cf40ce
/// </summary>
public class GameMain : Game
{
    private OrthographicCamera _camera;
    private GraphicsDeviceManager _graphics;
    private Effect _shader;
    
    private Effect _pattern1;
    private Effect _pattern2;    

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Setup a viewport adapter to handle different screen sizes/aspect ratios
        var viewportAdapter = new BoxingViewportAdapter(
            Window,
            GraphicsDevice,
            _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight);

        _camera = new OrthographicCamera(viewportAdapter);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Load shaders        
        _pattern1 = Content.Load<Effect>("Shaders/Tiled Pattern 1");
        _pattern2 = Content.Load<Effect>("Shaders/Tiled Pattern 2");        

        // Set the initial shader
        _shader = _pattern1;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        MoveCamera();
        ConfigureShader();
        SelectShader();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        DrawQuadrilateral(_shader,
           x0: 0, y0: 0,
           x1: _graphics.GraphicsDevice.Viewport.Width, y1: 0,
           x2: _graphics.GraphicsDevice.Viewport.Width, y2: _graphics.GraphicsDevice.Viewport.Height,
           x3: 0, y3: _graphics.GraphicsDevice.Viewport.Height);

        base.Draw(gameTime);
    }

    private void ConfigureShader()
    {
        // Get the camera's view and projection matrices. The view matrix represents the camera's position and
        // orientation in the world, while the projection matrix defines how 3D coordinates are projected onto
        // the 2D screen.
        var view = _camera.GetViewMatrix();
        var projection = Matrix.CreateOrthographicOffCenter(0, _graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height, 0, 0, 1);

        // So that the shader knows about the camera's position and orientation, we
        // give the combined view-projection matrix to the shader.
        _shader.Parameters["ViewProjection"].SetValue(view * projection);

        // Set the tile width for the shader. This determines how big the tiles in the pattern will
        // be. In this case, we set it to half the width of the viewport, so there will be 2 tiles
        // across the width of the screen.
        _shader.Parameters["TileWidth"].SetValue(_graphics.GraphicsDevice.Viewport.Width / 2);
    }

    /// <summary>
    /// A basic method to draw a quadrilateral using the specified shader effect 
    /// and coordinates. The quadrilateral is made up of 2 triangles.
    /// </summary>
    /// <param name="effect">The shader effect to use</param>
    /// <param name="x0">Top left position of the quad</param>
    /// <param name="y0">Top left position of the quad</param>
    /// <param name="x1">Top right position of the quad</param>
    /// <param name="y1">Top right position of the quad</param>
    /// <param name="x2">Bottom right position of the quad</param>
    /// <param name="y2">Bottom right position of the quad</param>
    /// <param name="x3">Bottom left position of the quad</param>
    /// <param name="y3">Bottom left position of the quad</param>
    private void DrawQuadrilateral(Effect effect, int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3)
    {
        // Reserve an array to store the triangle coordinates that of the
        // 2 triangles that make up the quadrilateral. The first triangle
        // will be made up of the top left, bottom right and top right
        // corners of the quad, and the second triangle will be made up
        // of the top left, bottom left and bottom right corners of the
        // quad.
        var vertices = new VertexPosition[6];

        // Define the first triangle
        var triangle1TopLeft = new VertexPosition(new Vector3(x0, y0, 0f));
        var triangle1TopRight = new VertexPosition(new Vector3(x1, y1, 0f));
        var triangle1BottomRight = new VertexPosition(new Vector3(x2, y2, 0f));

        vertices[0] = triangle1TopLeft;
        vertices[1] = triangle1BottomRight;
        vertices[2] = triangle1TopRight;

        // Now the second triangle
        var triangle2TopLeft = new VertexPosition(new Vector3(x0, y0, 0f));
        var triangle2BottomRight = new VertexPosition(new Vector3(x2, y2, 0f));
        var triangle2BottomLeft = new VertexPosition(new Vector3(x3, y3, 0f));

        vertices[3] = triangle2TopLeft;
        vertices[4] = triangle2BottomLeft;
        vertices[5] = triangle2BottomRight;

        // Draw...
        _graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }
    }

    private void MoveCamera()
    {
        var keyboardState = Keyboard.GetState();

        // Move the camera with arrow keys
        if (keyboardState.IsKeyDown(Keys.Up))
            _camera.Move(new Vector2(0, -5));
        else if (keyboardState.IsKeyDown(Keys.Down))
            _camera.Move(new Vector2(0, 5));
        else if (keyboardState.IsKeyDown(Keys.Left))
            _camera.Move(new Vector2(-5, 0));
        else if (keyboardState.IsKeyDown(Keys.Right))
            _camera.Move(new Vector2(5, 0));
    }

    private void SelectShader()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.NumPad1))
            _shader = _pattern1;
        else if (keyboard.IsKeyDown(Keys.NumPad2))
            _shader = _pattern2;
    }
}
