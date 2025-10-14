using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared.Services;

/// <summary>
/// A simple shape drawing service using BasicEffect (shader)
/// </summary>
public class ShapeDrawingService
{
    private readonly BasicEffect _basicEffect;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly RasterizerState _rasterizerState;

    public ShapeDrawingService(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        // We'll use the BasicEffect 'shader' to draw stuff
        _basicEffect = new BasicEffect(_graphicsDevice);

        // Setup the world matrix for effectively the screen as a 2D surface
        _basicEffect.World = Matrix.CreateOrthographicOffCenter(
            left: 0,
            right: _graphicsDevice.Viewport.Width,
            bottom: _graphicsDevice.Viewport.Height,
            top: 0,
            zNearPlane: 0,
            zFarPlane: 1);

        // The following MUST be enabled if you want to color your vertices
        _basicEffect.VertexColorEnabled = true;

        // Build rasterizer state
        _rasterizerState = new RasterizerState();
        _rasterizerState.CullMode = CullMode.None;
    }

    private void Draw(VertexPositionColor[] vertices, PrimitiveType primitiveType, int primitiveCount)
    {
        _graphicsDevice.RasterizerState = _rasterizerState;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();

            _graphicsDevice.DrawUserPrimitives(
                primitiveType: primitiveType,
                vertexData: vertices,
                vertexOffset: 0,
                primitiveCount: primitiveCount,
                vertexDeclaration: VertexPositionColor.VertexDeclaration
            );
        }
    }

    public void DrawFilledQuadrilateral(Color colour, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        // Coordinates
        var vertices = new VertexPositionColor[6];

        // First triangle
        vertices[0].Position = new Vector3(x1, y1, 0f);
        vertices[0].Color = colour;
        vertices[1].Position = new Vector3(x2, y2, 0f);
        vertices[1].Color = colour;
        vertices[2].Position = new Vector3(x3, y3, 0f);
        vertices[2].Color = colour;

        // Second triangle
        vertices[3].Position = new Vector3(x1, y1, 0f);
        vertices[3].Color = colour;
        vertices[4].Position = new Vector3(x3, y3, 0f);
        vertices[4].Color = colour;
        vertices[5].Position = new Vector3(x4, y4, 0f);
        vertices[5].Color = colour;

        // Draw...
        Draw(vertices, PrimitiveType.TriangleList, 2);
    }

    public void DrawFilledRectangle(Color colour, int x, int y, int width, int height)
    {
        // Coordinates
        var vertices = new VertexPositionColor[6];

        // First triangle
        vertices[0].Position = new Vector3(x, y, 0f);
        vertices[0].Color = colour;
        vertices[1].Position = new Vector3(x + width, y, 0f);
        vertices[1].Color = colour;
        vertices[2].Position = new Vector3(x + width, y + height, 0f);
        vertices[2].Color = colour;

        // Second triangle
        vertices[3].Position = new Vector3(x, y, 0f);
        vertices[3].Color = colour;
        vertices[4].Position = new Vector3(x + width, y + height, 0f);
        vertices[4].Color = colour;
        vertices[5].Position = new Vector3(x, y + height, 0f);
        vertices[5].Color = colour;

        // Draw...
        Draw(vertices, PrimitiveType.TriangleList, 2);
    }
}
