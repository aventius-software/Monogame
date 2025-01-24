using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Basic3DCubeWithShaders.Shapes;

internal class Cube
{
    public static VertexPositionColorTexture[] GenerateVertices(float size)
    {
        // First define the coordinates for the texture
        var topLeftTexture = new Vector2 (0, 0);
        var topRightTexture = new Vector2 (1, 0);
        var bottomLeftTexture = new Vector2 (0, 1);
        var bottomRightTexture = new Vector2 (1, 1);

        // Define the corners for the front face
        var topLeftCornerFrontFace = new Vector3(-size, -size, -size);
        var topRightCornerFrontFace = new Vector3(size, -size, -size);
        var bottomLeftCornerFrontFace = new Vector3(-size, size, -size);
        var bottomRightCornerFrontFace = new Vector3(size, size, -size);

        // Now the corners for the back face
        var topLeftCornerBackFace = new Vector3(-size, -size, size);
        var topRightCornerBackFace = new Vector3(size, -size, size);
        var bottomLeftCornerBackFace = new Vector3(-size, size, size);
        var bottomRightCornerBackFace = new Vector3(size, size, size);

        // For the left side face, we use the existing coordinates
        // from the front and back faces (the left side)
        var topLeftCornerLeftFace = topLeftCornerBackFace;
        var topRightCornerLeftFace = topLeftCornerFrontFace;
        var bottomLeftCornerLeftFace = bottomLeftCornerBackFace;
        var bottomRightCornerLeftFace = bottomLeftCornerFrontFace;

        // For the right side face, we use the existing coordinates
        // from the front and back faces (the right side)
        var topLeftCornerRightFace = topRightCornerFrontFace;
        var topRightCornerRightFace = topRightCornerBackFace;
        var bottomLeftCornerRightFace = bottomRightCornerFrontFace;
        var bottomRightCornerRightFace = bottomRightCornerBackFace;

        // For the top side face, we use the existing coordinates
        // from the front and back faces
        var topLeftCornerTopFace = topLeftCornerBackFace;
        var topRightCornerTopFace = topRightCornerBackFace;
        var bottomLeftCornerTopFace = topLeftCornerFrontFace;
        var bottomRightCornerTopFace = topRightCornerFrontFace;

        // For the bottom side face, we use the existing coordinates
        // from the front and back faces
        var topLeftCornerBottomFace = bottomLeftCornerBackFace;
        var topRightCornerBottomFace = bottomRightCornerBackFace;
        var bottomLeftCornerBottomFace = bottomLeftCornerFrontFace;
        var bottomRightCornerBottomFace = bottomRightCornerFrontFace;

        return
            [                
                // Front face - Triangle 1
                // top left corner
                new VertexPositionColorTexture(topLeftCornerFrontFace, Color.White, topLeftTexture),
                // top right corner
                new VertexPositionColorTexture(topRightCornerFrontFace, Color.White, topRightTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerFrontFace, Color.White, bottomRightTexture),

                // Front face - Triangle 2
                // top left corner
                new VertexPositionColorTexture(topLeftCornerFrontFace, Color.White, topLeftTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerFrontFace, Color.White, bottomRightTexture),
                // bottom left corner
                new VertexPositionColorTexture(bottomLeftCornerFrontFace, Color.White, bottomLeftTexture),

                // Back face - Triangle 1
                // top left corner
                new VertexPositionColorTexture(topLeftCornerBackFace, Color.White, topLeftTexture),
                // top right corner
                new VertexPositionColorTexture(topRightCornerBackFace, Color.White, topRightTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerBackFace, Color.White, bottomRightTexture),

                // Back face - Triangle 2
                // top left corner
                new VertexPositionColorTexture(topLeftCornerBackFace, Color.White, topLeftTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerBackFace, Color.White, bottomRightTexture),
                // bottom left corner
                new VertexPositionColorTexture(bottomLeftCornerBackFace, Color.White, bottomLeftTexture),

                // Left face - Triangle 1
                // top left corner
                new VertexPositionColorTexture(topLeftCornerLeftFace, Color.White, topLeftTexture),
                // top right corner
                new VertexPositionColorTexture(topRightCornerLeftFace, Color.White, topRightTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerLeftFace, Color.White, bottomRightTexture),

                // Left face - Triangle 2
                // top left corner
                new VertexPositionColorTexture(topLeftCornerLeftFace, Color.White, topLeftTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerLeftFace, Color.White, bottomRightTexture),
                // bottom left corner
                new VertexPositionColorTexture(bottomLeftCornerLeftFace, Color.White, bottomLeftTexture),

                // Right face - Triangle 1
                // top left corner
                new VertexPositionColorTexture(topLeftCornerRightFace, Color.White, topLeftTexture),
                // top right corner
                new VertexPositionColorTexture(topRightCornerRightFace, Color.White, topRightTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerRightFace, Color.White, bottomRightTexture),

                // Right face - Triangle 2
                // top left corner
                new VertexPositionColorTexture(topLeftCornerRightFace, Color.White, topLeftTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerRightFace, Color.White, bottomRightTexture),
                // bottom left corner
                new VertexPositionColorTexture(bottomLeftCornerRightFace, Color.White, bottomLeftTexture),

                // Top face - Triangle 1
                // top left corner
                new VertexPositionColorTexture(topLeftCornerTopFace, Color.White, topLeftTexture),
                // top right corner
                new VertexPositionColorTexture(topRightCornerTopFace, Color.White, topRightTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerTopFace, Color.White, bottomRightTexture),

                // Top face - Triangle 2
                // top left corner
                new VertexPositionColorTexture(topLeftCornerTopFace, Color.White, topLeftTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerTopFace, Color.White, bottomRightTexture),
                // bottom left corner
                new VertexPositionColorTexture(bottomLeftCornerTopFace, Color.White, bottomLeftTexture),

                // Bottom face - Triangle 1
                // top left corner
                new VertexPositionColorTexture(topLeftCornerBottomFace, Color.White, topLeftTexture),
                // top right corner
                new VertexPositionColorTexture(topRightCornerBottomFace, Color.White, topRightTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerBottomFace, Color.White, bottomRightTexture),

                // Bottom face - Triangle 2
                // top left corner
                new VertexPositionColorTexture(topLeftCornerBottomFace, Color.White, topLeftTexture),
                // bottom right corner
                new VertexPositionColorTexture(bottomRightCornerBottomFace, Color.White, bottomRightTexture),
                // bottom left corner
                new VertexPositionColorTexture(bottomLeftCornerBottomFace, Color.White, bottomLeftTexture),
            ];            
    }

    /// <summary>
    /// Returns a set of vertices for a basic 3D cube
    /// </summary>
    /// <returns></returns>
    public static VertexPositionColorTexture[] BuildVertices()
    {
        Vector3 LeftTopFront = new Vector3(-1.0f, 1.0f, 1.0f);
        Vector3 LeftBottomFront = new Vector3(-1.0f, -1.0f, 1.0f);
        Vector3 LeftTopBack = new Vector3(-1.0f, 1.0f, -1.0f);
        Vector3 LeftBottomBack = new Vector3(-1.0f, -1.0f, -1.0f);

        Vector3 RightTopFront = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 RightBottomFront = new Vector3(1.0f, -1.0f, 1.0f);
        Vector3 RightTopBack = new Vector3(1.0f, 1.0f, -1.0f);
        Vector3 RightBottomBack = new Vector3(1.0f, -1.0f, -1.0f);

        var leftTopTexture = new Vector2(0, 0);
        var leftBottomTexture = new Vector2(0, 1);
        var rightTopTexture = new Vector2(1, 0);
        var rightBottomTexture = new Vector2(0, 0);

        return
        [
            // Front face.
            new(LeftTopFront, Color.DarkGray, leftTopTexture),
            new(LeftBottomFront, Color.DarkGray, leftBottomTexture),
            new(RightTopFront, Color.DarkGray, rightTopTexture),

            new(LeftBottomFront, Color.Gray, leftBottomTexture),
            new(RightBottomFront, Color.Gray, rightBottomTexture),
            new(RightTopFront, Color.Gray, rightTopTexture),

            // Back face.
            new(LeftTopBack, Color.DarkGreen, leftTopTexture),
            new(RightTopBack, Color.DarkGreen, rightTopTexture),
            new(LeftBottomBack, Color.DarkGreen, leftBottomTexture),

            new(LeftBottomBack, Color.Green, leftBottomTexture),
            new(RightTopBack, Color.Green, rightTopTexture),
            new(RightBottomBack, Color.Green, rightBottomTexture),

            // Top face.
            new(LeftTopFront, Color.DarkBlue, leftTopTexture),
            new(RightTopBack, Color.DarkBlue, rightTopTexture),
            new(LeftTopBack, Color.DarkBlue, leftTopTexture),

            new(LeftTopFront, Color.Blue, leftTopTexture),
            new(RightTopFront, Color.Blue, rightTopTexture),
            new(RightTopBack, Color.Blue, rightTopTexture),

            // Bottom face. 
            new(LeftBottomFront, Color.DarkOrange, leftBottomTexture),
            new(LeftBottomBack, Color.DarkOrange, leftBottomTexture),
            new(RightBottomBack, Color.DarkOrange, rightBottomTexture),

            new(LeftBottomFront, Color.Orange, leftBottomTexture),
            new(RightBottomBack, Color.Orange, rightBottomTexture),
            new(RightBottomFront, Color.Orange, rightBottomTexture),

            // Left face.
            new(LeftTopFront, Color.DarkRed, leftTopTexture),
            new(LeftBottomBack, Color.DarkRed, leftBottomTexture),
            new(LeftBottomFront, Color.DarkRed, leftBottomTexture),

            new(LeftTopBack, Color.Red, leftTopTexture),
            new(LeftBottomBack, Color.Red, leftBottomTexture),
            new(LeftTopFront, Color.Red, leftTopTexture),

            // Right face. 
            new(RightTopFront, Color.DarkViolet, rightTopTexture),
            new(RightBottomFront, Color.DarkViolet, rightBottomTexture),
            new(RightBottomBack, Color.DarkViolet, rightBottomTexture),

            new(RightTopBack, Color.Violet, rightTopTexture),
            new(RightTopFront, Color.Violet, rightTopTexture),
            new(RightBottomBack, Color.Violet, rightBottomTexture)
        ];
    }
}

internal class CubeIndexed
{
    public VertexPositionColor[] Vertices { get; set; }
    public short[] Indices { get; set; }
    public Matrix WorldMatrix { get; set; }

    public CubeIndexed(Color color)
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

        //WorldMatrix = Matrix.CreateTranslation(position);
    }
}
