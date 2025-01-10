using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyWingsStyleDemo.Services;

/// <summary>
/// A simple shape drawing service using BasicEffect (shader) or optional custom shader
/// </summary>
internal class ShapeDrawingService
{
    private readonly BasicEffect _basicEffect;
    private Matrix? _cameraTransformationMatrix;
    private Effect _customShader;
    private readonly GraphicsDevice _graphicsDevice;

    public ShapeDrawingService(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _graphicsDevice.RasterizerState = RasterizerState.CullNone;

        // We'll use the BasicEffect 'shader' to draw stuff by default if no custom shader is supplied
        _basicEffect = new BasicEffect(_graphicsDevice);
    }

    /// <summary>
    /// Start a batch ready for shape drawing
    /// </summary>
    /// <param name="cameraTransformationMatrix"></param>
    /// <param name="shader"></param>
    public void BeginBatch(Matrix? cameraTransformationMatrix = null, Effect shader = null)
    {
        // Set/save the transformation matrix
        _cameraTransformationMatrix = cameraTransformationMatrix;

        // Set the shader we'll use, if a custom shader is not being used we just use BasicEffect
        if (shader is null) UseDefaultShader();
        else SetCustomShader(shader);
    }

    /// <summary>
    /// Draw a filled quad
    /// </summary>
    /// <param name="colour"></param>
    /// <param name="topLeftX"></param>
    /// <param name="topLeftY"></param>
    /// <param name="topRightX"></param>
    /// <param name="topRightY"></param>
    /// <param name="bottomRightX"></param>
    /// <param name="bottomRightY"></param>
    /// <param name="bottomLeftX"></param>
    /// <param name="bottomLeftY"></param>
    public void DrawFilledQuadrilateral(Color colour, int topLeftX, int topLeftY, int topRightX, int topRightY, int bottomRightX, int bottomRightY, int bottomLeftX, int bottomLeftY)
    {
        // Coordinates
        var vertices = new VertexPositionColor[6];

        // First triangle
        var triangle1TopLeft = new VertexPositionColor
        {
            Position = new Vector3(topLeftX, topLeftY, 0f),
            Color = colour
        };

        var triangle1TopRight = new VertexPositionColor
        {
            Position = new Vector3(topRightX, topRightY, 0f),
            Color = colour
        };

        var triangle1BottomRight = new VertexPositionColor
        {
            Position = new Vector3(bottomRightX, bottomRightY, 0f),
            Color = colour
        };

        // Second triangle
        var triangle2TopLeft = new VertexPositionColor
        {
            Position = new Vector3(topLeftX, topLeftY, 0f),
            Color = colour
        };

        var triangle2BottomRight = new VertexPositionColor
        {
            Position = new Vector3(bottomRightX, bottomRightY, 0f),
            Color = colour
        };

        var triangle2BottomLeft = new VertexPositionColor
        {
            Position = new Vector3(bottomLeftX, bottomLeftY, 0f),
            Color = colour
        };

        _graphicsDevice.RasterizerState = RasterizerState.CullNone;

        // First triangle
        vertices[0] = triangle1TopLeft;
        vertices[1] = triangle1BottomRight;
        vertices[2] = triangle1TopRight;

        // Second triangle
        vertices[3] = triangle2TopLeft;
        vertices[4] = triangle2BottomLeft;
        vertices[5] = triangle2BottomRight;

        // Draw...        
        var passes = _customShader is null ? _basicEffect.CurrentTechnique.Passes : _customShader.CurrentTechnique.Passes;
        foreach (var pass in passes)
        {
            pass.Apply();
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }
    }

    /// <summary>
    /// Draw a line
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="colour"></param>
    public void DrawLine(Vector2 start, Vector2 end, Color colour)
    {
        // Coordinates
        var vertices = new VertexPositionColor[2];

        // First triangle
        vertices[0].Position = new Vector3(start.X, start.Y, 0f);
        vertices[0].Color = colour;
        vertices[1].Position = new Vector3(end.X, end.Y, 0f);
        vertices[1].Color = colour;

        // Draw...
        var passes = _customShader is null ? _basicEffect.CurrentTechnique.Passes : _customShader.CurrentTechnique.Passes;
        foreach (var pass in passes)
        {
            pass.Apply();
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }

    /// <summary>
    /// End shape drawing batch (resets transformation matrix and any custom shader
    /// </summary>
    public void EndBatch()
    {
        _cameraTransformationMatrix = null;
        _customShader = null;
    }

    /// <summary>
    /// Set a custom shader to be used, passing NULL will reset the shader
    /// </summary>
    /// <param name="shader"></param>
    public void SetCustomShader(Effect shader)
    {
        if (shader is null) UseDefaultShader();
        else UseCustomShader(shader);
    }

    /// <summary>
    /// Sets up the service to use the specified shader
    /// </summary>
    /// <param name="shader"></param>
    private void UseCustomShader(Effect shader)
    {
        _customShader = shader;

        var cameraUp = Vector3.Transform(Vector3.Down, Matrix.CreateRotationZ(0));
        var world = _cameraTransformationMatrix is null ? Matrix.Identity : (Matrix)_cameraTransformationMatrix;
        var view = _cameraTransformationMatrix is null ? Matrix.CreateLookAt(Vector3.Forward, Vector3.Zero, cameraUp) : Matrix.Identity;
        var projection = Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, 1);

        if (_cameraTransformationMatrix is null) projection *= Matrix.CreateScale(1, -1, 1);

        _customShader.Parameters["World"].SetValue(world);
        _customShader.Parameters["View"].SetValue(view);
        _customShader.Parameters["Projection"].SetValue(projection);
    }

    /// <summary>
    /// Sets up the service to use BasicEffect shader for drawing
    /// </summary>
    private void UseDefaultShader()
    {
        _customShader = null;

        var cameraUp = Vector3.Transform(Vector3.Down, Matrix.CreateRotationZ(0));
        var world = _cameraTransformationMatrix is null ? Matrix.Identity : (Matrix)_cameraTransformationMatrix;
        var view = _cameraTransformationMatrix is null ? Matrix.CreateLookAt(Vector3.Forward, Vector3.Zero, cameraUp) : Matrix.Identity;
        var projection = Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, 1);

        if (_cameraTransformationMatrix is null) projection *= Matrix.CreateScale(1, -1, 1);

        _basicEffect.World = world;
        _basicEffect.View = view;
        _basicEffect.Projection = projection;

        _basicEffect.TextureEnabled = false;
        _basicEffect.VertexColorEnabled = true;
    }
}