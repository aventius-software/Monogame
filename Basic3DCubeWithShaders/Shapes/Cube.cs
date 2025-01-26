using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Basic3DCubeWithShaders.Shapes;

internal class Cube
{
    public static VertexPositionColorTexture[] GenerateVertices(float size)
    {
        // Halve the size
        size *= 0.5f;

        // Define the faces for our cube, each face consists of 2 triangle (to make a square). We
        // start at one vertex (corner) and define the following vertices counter-clockwise! The
        // Z position is smaller (and eventually negative the further away we go) assuming that the
        // camera is placed with a positive Z position. So the front face has larger Z coordinates
        // and the back face (which is further away) has smaller (or negative) Z coordinates. This
        // also means the Y coordinates from the perspective of the camera (and screen) are larger
        // (or positive) at the top of the screen and smaller (or negative) at the bottom of the screen
        // Note that the centre of our cube will be 0,0,0 so for the front we'll use +size and back -size
        // which will make the size double, but if we half the size before (see above) then we'll get a cube
        // with a width, height and depth of 'size' ;-)
        //
        // So if our camera is placed at 0,0,10 for example (with a positive Z coordinate, which is usually
        // the convention), then:-
        //
        // X axis = to left side of the screen is smaller (or negative) and to the right is larger (or positive)
        // Y axis = to the top of the screen is larger (or positive) and to the bottom of the screen is smaller (or negative)
        // Z axis = into the screen is smaller (or negative), closer to the camera/screen is larger (or positive)
        //
        // If the camera is placed with a negative Z coordinate, e.g. 0,0,-10 then the axis get reversed:-
        //
        // X axis = to left side of the screen is smaller (or negative) and to the right is larger (or positive)
        // Y axis = to the bottom of the screen is larger (or positive) and to the top of the screen is smaller (or negative)
        // Z axis = further from the camera/screen is larger (or positive), closer to the camera/screen is smaller (or positive)
        // 
        // For default convention, we'll define our vertices as if the camera is at positive Z...
        return
        [            
            // Front face (closest from the camera)
            new VertexPositionColorTexture(new Vector3(-size, size, size), Color.White, new Vector2(0, 0)),         // Front-Top-Left
            new VertexPositionColorTexture(new Vector3(size, -size, size), Color.White, new Vector2(1f/3f, 1f/2f)), // Front-Bottom-Right
            new VertexPositionColorTexture(new Vector3(size, size, size), Color.White, new Vector2(1f/3f, 0)),      // Front-Top-Right
            
            new VertexPositionColorTexture(new Vector3(-size, size, size), Color.White, new Vector2(0, 0)),         // Front-Top-Left
            new VertexPositionColorTexture(new Vector3(-size, -size, size), Color.White, new Vector2(0, 1f/2f)),    // Front-Bottom-Left
            new VertexPositionColorTexture(new Vector3(size, -size, size), Color.White, new Vector2(1f/3f, 1f/2f)), // Front-Bottom-Right

            // Back face (furthest from the camera)
            new VertexPositionColorTexture(new Vector3(-size, size, -size), Color.White, new Vector2(0, 0)),    // Back-Top-Left
            new VertexPositionColorTexture(new Vector3(size, -size, -size), Color.White, new Vector2(2f/3f, 1f/2f)),    // Back-Bottom-Right
            new VertexPositionColorTexture(new Vector3(size, size, -size), Color.White, new Vector2(2f/3f, 0)),     // Back-Top-Right
            
            new VertexPositionColorTexture(new Vector3(-size, size, -size), Color.White, new Vector2(0, 0)),    // Back-Top-Left
            new VertexPositionColorTexture(new Vector3(-size, -size, -size), Color.White, new Vector2(0, 1)),   // Back-Bottom-Left
            new VertexPositionColorTexture(new Vector3(size, -size, -size), Color.White, new Vector2(1, 1)),    // Back-Bottom-Right
            
            // Top face
            new VertexPositionColorTexture(new Vector3(-size, size, -size), Color.White, new Vector2(0, 0)),    // Back-Top-Left
            new VertexPositionColorTexture(new Vector3(size, size, -size), Color.White, new Vector2(1, 0)),     // Back-Top-Right
            new VertexPositionColorTexture(new Vector3(size, size, size), Color.White, new Vector2(1, 1)),      // Front-Top-Right

            new VertexPositionColorTexture(new Vector3(-size, size, -size), Color.White, new Vector2(0, 0)),    // Back-Top-Left
            new VertexPositionColorTexture(new Vector3(-size, size, size), Color.White, new Vector2(0, 1)),     // Front-Top-Left
            new VertexPositionColorTexture(new Vector3(size, size, size), Color.White, new Vector2(1, 1)),      // Front-Top-Right

            // Bottom face
            new VertexPositionColorTexture(new Vector3(size, -size, size), Color.White, new Vector2(1, 0)),     // Front-Bottom-Right
            new VertexPositionColorTexture(new Vector3(size, -size, -size), Color.White, new Vector2(1, 1)),    // Back-Bottom-Right
            new VertexPositionColorTexture(new Vector3(-size, -size, size), Color.White, new Vector2(0, 0)),    // Front-Bottom-Left

            new VertexPositionColorTexture(new Vector3(-size, -size, size), Color.White, new Vector2(0, 0)),    // Front-Bottom-Left
            new VertexPositionColorTexture(new Vector3(size, -size, -size), Color.White, new Vector2(1, 1)),    // Back-Bottom-Right
            new VertexPositionColorTexture(new Vector3(-size, -size, -size), Color.White, new Vector2(0, 1)),   // Back-Bottom-Left

            // Left face
            new VertexPositionColorTexture(new Vector3(-size, size, -size), Color.White, new Vector2(0, 0)),    // Back-Top-Left
            new VertexPositionColorTexture(new Vector3(-size, -size, -size), Color.White, new Vector2(0, 1)),   // Back-Bottom-Left
            new VertexPositionColorTexture(new Vector3(-size, size, size), Color.White, new Vector2(1, 0)),     // Front-Top-Left
            
            new VertexPositionColorTexture(new Vector3(-size, size, size), Color.White, new Vector2(1, 0)),     // Front-Top-Left
            new VertexPositionColorTexture(new Vector3(-size, -size, size), Color.White, new Vector2(1, 1)),    // Front-Bottom-Left
            new VertexPositionColorTexture(new Vector3(-size, -size, -size), Color.White, new Vector2(0, 1)),   // Back-Bottom-Left

            // Right face
            new VertexPositionColorTexture(new Vector3(size, size, size), Color.White, new Vector2(0, 0)),      // Front-Top-Right
            new VertexPositionColorTexture(new Vector3(size, -size, size), Color.White, new Vector2(0, 1)),     // Front-Bottom-Right
            new VertexPositionColorTexture(new Vector3(size, size, -size), Color.White, new Vector2(1, 0)),     // Back-Top-Right

            new VertexPositionColorTexture(new Vector3(size, -size, size), Color.White, new Vector2(0, 1)),     // Front-Bottom-Right
            new VertexPositionColorTexture(new Vector3(size, size, -size), Color.White, new Vector2(1, 0)),     // Back-Top-Right
            new VertexPositionColorTexture(new Vector3(size, -size, -size), Color.White, new Vector2(1, 1)),    // Back-Bottom-Right            
        ];
    }
}