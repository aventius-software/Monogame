using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Basic3DCubeWithShaders.Shapes;

internal class Cube
{
    /// <summary>
    /// Returns a set of vertices for a basic 3D cube
    /// </summary>
    /// <returns></returns>
    public static VertexPositionColor[] BuildVertices()
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
}

/*
internal class Cube
{
    public VertexPositionColor[] Vertices { get; set; }
    public short[] Indices { get; set; }
    public Matrix WorldMatrix { get; set; }

    public Cube(Vector3 position, Color color)
    {
        Vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(-1, -1, -1), color),
            new VertexPositionColor(new Vector3(-1, 1, -1), color),
            new VertexPositionColor(new Vector3(1, 1, -1), color),
            new VertexPositionColor(new Vector3(1, -1, -1), color),
            new VertexPositionColor(new Vector3(-1, -1, 1), color),
            new VertexPositionColor(new Vector3(-1, 1, 1), color),
            new VertexPositionColor(new Vector3(1, 1, 1), color),
            new VertexPositionColor(new Vector3(1, -1, 1), color)
        };

        Indices = new short[]
        {
            0, 1, 2, 2, 3, 0, // Front face
            4, 5, 6, 6, 7, 4, // Back face
            0, 1, 5, 5, 4, 0, // Left face
            2, 3, 7, 7, 6, 2, // Right face
            1, 2, 6, 6, 5, 1, // Top face
            0, 3, 7, 7, 4, 0  // Bottom face
        };

        WorldMatrix = Matrix.CreateTranslation(position);
    }
}
*/