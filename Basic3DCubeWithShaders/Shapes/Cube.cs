using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Basic3DCubeWithShaders.Shapes;

internal class Cube
{
    private enum FaceIndex
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    public static VertexPositionTexture[] GenerateVertices(float size)
    {
        // Since the origin of our cube will be its centre, we halve the size
        // so that the distance from the origin to a side is size/2, meaning from
        // a side to the adjacent side is the 'size' value that was passed as a parameter ;-)
        size *= 0.5f;

        /*
        
        Define the faces for our cube, each face consists of 2 triangle (to make a square). Note that
        the Z position is smaller (and eventually negative the further away we go from our cube) assuming 
        that the camera is placed facing the 'front' face of the cube. So the front face has larger Z coordinates
        than the back face (which is further away) and which has smaller (or negative) Z coordinates. This
        also means the Y coordinates from the perspective of the camera (and screen) are larger
        (or positive) at the top of the screen and smaller (or negative) at the bottom of the screen
        
        Note that the centre of our cube will be (0,0,0) so for the front we'll use +size and back -size
        which will make the size double, but if we half the size before (see above) then we'll get a cube
        with a width, height and depth of 'size' ;-)
        
        So if our camera is placed at (0,0,10) for example (with a positive Z coordinate, which is usually
        the convention), then:-
        
        X axis = to left side of the screen is smaller (or negative) and to the right is larger (or positive)
        Y axis = to the top of the screen is larger (or positive) and to the bottom of the screen is smaller (or negative)
        Z axis = into the screen is smaller (or negative), closer to the camera/screen is larger (or positive)
                        
        For default convention, we'll define our vertices as if the camera is at positive Z. So, we're looking
        at the 'front' face of the cube, from the position of the camera. We can give our vertices any number to 
        identify them, so maybe we could number them as shown below (but it doesn't matter):-
        
            4 - - - - 5                    
           /|        /|
          / |       / |
         /  |      /  |
        0 - - - - 1   |
        |   7 - - | - 6
        |  /      |  / 
        | /       | /
        |/        |/
        3 - - - - 2


        Front face
        ==========
        So, with this in mind, the front face vertices would be:-

        0---1   
        |  /|
        | / |
        |/  |
        3---2

        Now, the important part is that we define our triangles which make up a face, in counter clockwise order. So
        the first triangle vertices using the numbering we decided before would be:

        1) Top left     = 0
        2) Bottom left  = 3
        3) Top right    = 1

        Then the second triangle would be:-

        1) Top right    = 1
        2) Bottom left  = 3
        3) Bottom right = 2

        One thing to note is that it doesn't matter which vertex you start at for a triangle. However, when later it
        comes to defining the texture coordinates that relate to the triangle, you need to remember which vertex of
        the triangle you defined as the top, left, bottom, right so that the texture doesn't end up the wrong way 
        around! ;-)


        Top face
        ========
        Now, rotate the cube so the top face is facing the camera, then
        the vertices we would see are as shown below:-

        4---5   
        |  /|
        | / |
        |/  |
        0---1

        Remember, we have to define them in a counter clockwise order, so
        the first triangle vertices could be:

        1) Top left     = 4
        2) Bottom left  = 0
        3) Top right    = 5

        Then the second triangle could be:-

        1) Top right    = 5
        2) Bottom left  = 0
        3) Bottom right = 1


        Back face
        ========
        Now, rotate the cube so the back face is facing the camera, then
        the vertices we would see are as shown below:-

        5---4   
        |  /|
        | / |
        |/  |
        6---7

        Remeber, we have to define them in a counter clockwise order, so
        the first triangle vertices could be:

        1) Top left     = 5
        2) Bottom left  = 6
        3) Top right    = 4

        Then the second triangle could be:-

        1) Top right    = 4
        2) Bottom left  = 6
        3) Bottom right = 7


        Bottom face
        ===========
        Now, rotate the cube so the bottom face is facing the camera, then
        the vertices we would see are as shown below:-

        3---2   
        |  /|
        | / |
        |/  |
        7---6

        Remeber, we have to define them in a counter clockwise order, so
        the first triangle vertices could be:

        1) Top left     = 3
        2) Bottom left  = 7
        3) Top right    = 2

        Then the second triangle could be:-

        1) Top right    = 2
        2) Bottom left  = 7
        3) Bottom right = 6


        Left face
        =========
        Now, rotate the cube so the left face is facing the camera, then
        the vertices we would see are as shown below:-

        4---0   
        |  /|
        | / |
        |/  |
        7---3

        Remeber, we have to define them in a counter clockwise order, so
        the first triangle vertices could be:

        1) Top left     = 4
        2) Bottom left  = 7
        3) Top right    = 0

        Then the second triangle could be:-

        1) Top right    = 0
        2) Bottom left  = 7
        3) Bottom right = 3


        Right face
        ==========
        Now, rotate the cube so the right face is facing the camera, then
        the vertices we would see are as shown below:-

        1---5   
        |  /|
        | / |
        |/  |
        2---6

        Remeber, we have to define them in a counter clockwise order, so
        the first triangle vertices could be:

        1) Top left     = 1
        2) Bottom left  = 2
        3) Top right    = 5

        Then the second triangle could be:-

        1) Top right    = 5
        2) Bottom left  = 2
        3) Bottom right = 6


        Now, if we define each vertex and give the number above, we just list the vertices as
        described to build our triangles to make up each face of the cube. As you can see, many
        vertices are re-used, so if we just define the vertices then use them to define each
        triangle, hey presto ;-)

        Define the six vertices we'll need... for a reminder, here's our cube from before
        with all the vertices numbered as we decided to number them. If you've numbered them
        differently, then obviously you'd need to use that order:-

            4 - - - - 5                    
           /|        /|
          / |       / |
         /  |      /  |
        0 - - - - 1   |
        |   7 - - | - 6
        |  /      |  / 
        | /       | /
        |/        |/
        3 - - - - 2

        */

        // Define cube vertices in order of 'face' index, so we can just refer to
        // the element index in this array to get a vertex that matches the numbers
        // we've assigned to each vertex above in the cube ;-)
        Vector3[] vertices =
            [
                new Vector3(-size, size, size),     // 0
                new Vector3(size, size, size),      // 1
                new Vector3(size, -size, size),     // 2
                new Vector3(-size, -size, size),    // 3
                new Vector3(-size, size, -size),    // 4
                new Vector3(size, size, -size),     // 5
                new Vector3(size, -size, -size),    // 6
                new Vector3(-size, -size, -size),   // 7
            ];

        // Now we can use the vertices to define each of the triangles
        // which make up all the faces of the cube ;-)
        return
            [
                // Front face, triangle 1:-
                //
                // 1) Top left     = 0
                // 2) Bottom left  = 3
                // 3) Top right    = 1
                new VertexPositionTexture(vertices[0], GetTextureCoordinates(FaceIndex.Front, 0, 0)),
                new VertexPositionTexture(vertices[3], GetTextureCoordinates(FaceIndex.Front, 0, 1)),
                new VertexPositionTexture(vertices[1], GetTextureCoordinates(FaceIndex.Front, 1, 0)),

                // Front face, triangle 2:-
                //
                // 1) Top right    = 1
                // 2) Bottom left  = 3
                // 3) Bottom right = 2
                new VertexPositionTexture(vertices[1], GetTextureCoordinates(FaceIndex.Front, 1, 0)),
                new VertexPositionTexture(vertices[3], GetTextureCoordinates(FaceIndex.Front, 0, 1)),
                new VertexPositionTexture(vertices[2], GetTextureCoordinates(FaceIndex.Front, 1, 1)),

                // Top face, triangle 1:-
                //
                // 1) Top left     = 4
                // 2) Bottom left  = 0
                // 3) Top right    = 5
                new VertexPositionTexture(vertices[4], GetTextureCoordinates(FaceIndex.Top, 0, 0)),
                new VertexPositionTexture(vertices[0], GetTextureCoordinates(FaceIndex.Top, 0, 1)),
                new VertexPositionTexture(vertices[5], GetTextureCoordinates(FaceIndex.Top, 1, 0)),

                // Top face, triangle 2:-
                //
                // 1) Top right    = 5
                // 2) Bottom left  = 0
                // 3) Bottom right = 1
                new VertexPositionTexture(vertices[5], GetTextureCoordinates(FaceIndex.Top, 1, 0)),
                new VertexPositionTexture(vertices[0], GetTextureCoordinates(FaceIndex.Top, 0, 1)),
                new VertexPositionTexture(vertices[1], GetTextureCoordinates(FaceIndex.Top, 1, 1)),

                // Back face, triangle 1:-
                //
                // 1) Top left     = 5
                // 2) Bottom left  = 6
                // 3) Top right    = 4
                new VertexPositionTexture(vertices[5], GetTextureCoordinates(FaceIndex.Back, 0, 0)),
                new VertexPositionTexture(vertices[6], GetTextureCoordinates(FaceIndex.Back, 0, 1)),
                new VertexPositionTexture(vertices[4], GetTextureCoordinates(FaceIndex.Back, 1, 0)),

                // Back face, triangle 2:-
                //
                // 1) Top right    = 4
                // 2) Bottom left  = 6
                // 3) Bottom right = 7
                new VertexPositionTexture(vertices[4], GetTextureCoordinates(FaceIndex.Back, 1, 0)),
                new VertexPositionTexture(vertices[6], GetTextureCoordinates(FaceIndex.Back, 0, 1)),
                new VertexPositionTexture(vertices[7], GetTextureCoordinates(FaceIndex.Back, 1, 1)),

                // Bottom face, triangle 1:-
                //
                // 1) Top left     = 3
                // 2) Bottom left  = 7
                // 3) Top right    = 2
                new VertexPositionTexture(vertices[3], GetTextureCoordinates(FaceIndex.Bottom, 0, 0)),
                new VertexPositionTexture(vertices[7], GetTextureCoordinates(FaceIndex.Bottom, 0, 1)),
                new VertexPositionTexture(vertices[2], GetTextureCoordinates(FaceIndex.Bottom, 1, 0)),

                // Bottom face, triangle 2:-
                //
                // 1) Top right    = 2
                // 2) Bottom left  = 7
                // 3) Bottom right = 6
                new VertexPositionTexture(vertices[2], GetTextureCoordinates(FaceIndex.Bottom, 1, 0)),
                new VertexPositionTexture(vertices[7], GetTextureCoordinates(FaceIndex.Bottom, 0, 1)),
                new VertexPositionTexture(vertices[6], GetTextureCoordinates(FaceIndex.Bottom, 1, 1)),

                // Left face, triangle 1:-
                //
                // 1) Top left     = 4
                // 2) Bottom left  = 7
                // 3) Top right    = 0
                new VertexPositionTexture(vertices[4], GetTextureCoordinates(FaceIndex.Left, 0, 0)),
                new VertexPositionTexture(vertices[7], GetTextureCoordinates(FaceIndex.Left, 0, 1)),
                new VertexPositionTexture(vertices[0], GetTextureCoordinates(FaceIndex.Left, 1, 0)),

                // Left face, triangle 2:-
                //
                // 1) Top right    = 0
                // 2) Bottom left  = 7
                // 3) Bottom right = 3
                new VertexPositionTexture(vertices[0], GetTextureCoordinates(FaceIndex.Left, 1, 0)),
                new VertexPositionTexture(vertices[7], GetTextureCoordinates(FaceIndex.Left, 0, 1)),
                new VertexPositionTexture(vertices[3], GetTextureCoordinates(FaceIndex.Left, 1, 1)),

                // Right face, triangle 1:-
                //
                // 1) Top left     = 1
                // 2) Bottom left  = 2
                // 3) Top right    = 5
                new VertexPositionTexture(vertices[1], GetTextureCoordinates(FaceIndex.Right, 0, 0)),
                new VertexPositionTexture(vertices[2], GetTextureCoordinates(FaceIndex.Right, 0, 1)),
                new VertexPositionTexture(vertices[5], GetTextureCoordinates(FaceIndex.Right, 1, 0)),

                // Right face, triangle 2:-
                //
                // 1) Top right    = 5
                // 2) Bottom left  = 2
                // 3) Bottom right = 6
                new VertexPositionTexture(vertices[5], GetTextureCoordinates(FaceIndex.Right, 1, 0)),
                new VertexPositionTexture(vertices[2], GetTextureCoordinates(FaceIndex.Right, 0, 1)),
                new VertexPositionTexture(vertices[6], GetTextureCoordinates(FaceIndex.Right, 1, 1))
            ];
    }

    private static Vector2 GetTextureCoordinates(FaceIndex faceIndex, float x, float y)
    {
        // Since a cube has 6 faces/sides, the texture 'atlas' is 6 images (one for each
        // face) on a single horizontal row. So we need to divide the width of the whole
        // texture by 6 to get the 'start' of the real 'x' coordinate to use for the
        // texture. Since there is only a single row of images (not 2 or 3 rows), we don't
        // need to do anything to the 'y' texture coordinate ;-)
        return new Vector2(((float)faceIndex + x) / 6f, y);
    }
}