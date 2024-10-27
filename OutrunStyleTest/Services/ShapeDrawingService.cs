using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OutrunStyleTest.Services;

internal class ShapeDrawingService
{
    private BasicEffect _basicEffect;
    private readonly GraphicsDevice _graphicsDevice;

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
    }

    private void Draw(VertexPositionColor[] vertices, PrimitiveType primitiveType, int primitiveCount)
    {
        RasterizerState rasterizerState1 = new RasterizerState();
        rasterizerState1.CullMode = CullMode.None;
        _graphicsDevice.RasterizerState = rasterizerState1;

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

    public void DrawFilledQuadrilateral(Color colour, Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
    {
        // Coordinates
        var vertices = new VertexPositionColor[6];

        // First triangle
        vertices[0].Position = new Vector3(point1.X, point1.Y, 0f);
        vertices[0].Color = colour;
        vertices[1].Position = new Vector3(point2.X, point2.Y, 0f);
        vertices[1].Color = colour;
        vertices[2].Position = new Vector3(point3.X, point3.Y, 0f);
        vertices[2].Color = colour;

        // Second triangle
        vertices[3].Position = new Vector3(point1.X, point1.Y, 0f);
        vertices[3].Color = colour;
        vertices[4].Position = new Vector3(point3.X, point3.Y, 0f);
        vertices[4].Color = colour;
        vertices[5].Position = new Vector3(point4.X, point4.Y, 0f);
        vertices[5].Color = colour;

        // Draw...
        Draw(vertices, PrimitiveType.TriangleList, 2);
    }
}
