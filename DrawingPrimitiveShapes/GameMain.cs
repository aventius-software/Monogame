using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DrawingPrimitiveShapes;

/// <summary>
/// This is one way to draw 2D shapes, by using 'BasicEffect' default shader in Monogame. See the link
/// here https://docs.monogame.net/articles/getting_to_know/howto/graphics/HowTo_Create_a_BasicEffect.html
/// 
/// Alternatively you can use another method which involves creating your own 'texture' and drawing lines
/// to get an outline, but this is tricky to draw filled shapes
/// </summary>
public class GameMain : Game
{
    private BasicEffect _basicEffect;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // We'll use the BasicEffect 'shader' to draw stuff
        _basicEffect = new BasicEffect(_graphics.GraphicsDevice);

        // Setup the world matrix for effectively the screen as a 2D surface
        _basicEffect.World = Matrix.CreateOrthographicOffCenter(
            left: 0,
            right: GraphicsDevice.Viewport.Width,
            bottom: GraphicsDevice.Viewport.Height,
            top: 0,
            zNearPlane: 0,
            zFarPlane: 1);

        // The following MUST be enabled if you want to color your vertices
        _basicEffect.VertexColorEnabled = true;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw a triangle outline
        DrawTriangle(Color.Red, new Vector2(10, 10), new Vector2(100, 10), new Vector2(10, 100));

        // Draw a filled triangle
        DrawFilledTriangle(Color.Green, new Vector2(200, 200), new Vector2(350, 200), new Vector2(150, 350));

        // Other 'hacky' method, not as efficient as the above methods
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: null);

        DrawLineUsingTexture(new Vector2(50, 400), new Vector2(600, 400), 10, Color.Blue);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawTriangle(Color colour, Vector2 point1, Vector2 point2, Vector2 point3)
    {
        // Coordinates
        var triangleVertices = new VertexPositionColor[4];

        triangleVertices[0].Position = new Vector3(point1.X, point1.Y, 0f);
        triangleVertices[0].Color = colour;
        triangleVertices[1].Position = new Vector3(point2.X, point2.Y, 0f);
        triangleVertices[1].Color = colour;
        triangleVertices[2].Position = new Vector3(point3.X, point3.Y, 0f);
        triangleVertices[2].Color = colour;

        // When we're drawing a 'LineStrip' w need to remember a line back to the start coordinates!
        triangleVertices[3].Position = new Vector3(point1.X, point1.Y, 0f);
        triangleVertices[3].Color = colour;

        // Draw...
        RasterizerState rasterizerState1 = new RasterizerState();
        rasterizerState1.CullMode = CullMode.None;
        GraphicsDevice.RasterizerState = rasterizerState1;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();

            GraphicsDevice.DrawUserPrimitives(
                primitiveType: PrimitiveType.LineStrip,
                vertexData: triangleVertices,
                vertexOffset: 0,
                primitiveCount: 3,
                vertexDeclaration: VertexPositionColor.VertexDeclaration
            );
        }
    }

    private void DrawFilledTriangle(Color colour, Vector2 point1, Vector2 point2, Vector2 point3)
    {
        // Coordinates
        var triangleVertices = new VertexPositionColor[3];

        triangleVertices[0].Position = new Vector3(point1.X, point1.Y, 0f);
        triangleVertices[0].Color = colour;
        triangleVertices[1].Position = new Vector3(point2.X, point2.Y, 0f);
        triangleVertices[1].Color = colour;
        triangleVertices[2].Position = new Vector3(point3.X, point3.Y, 0f);
        triangleVertices[2].Color = colour;

        // Draw...
        RasterizerState rasterizerState1 = new RasterizerState();
        rasterizerState1.CullMode = CullMode.None;
        GraphicsDevice.RasterizerState = rasterizerState1;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();

            GraphicsDevice.DrawUserPrimitives(
                primitiveType: PrimitiveType.TriangleList,
                vertexData: triangleVertices,
                vertexOffset: 0,
                primitiveCount: 1,
                vertexDeclaration: VertexPositionColor.VertexDeclaration
            );
        }
    }

    private void DrawLineUsingTexture(Vector2 startPos, Vector2 endPos, int thickness, Color color)
    {
        // I copied this originally from some post, but can't remember. So credits to whoever created
        // this originally. Create a texture as wide as the distance between two points and as high as
        // the desired thickness of the line.
        var distance = (int)Vector2.Distance(startPos, endPos);

        // We'll need to dispose of this manually created texture
        using var texture = new Texture2D(_spriteBatch.GraphicsDevice, distance, thickness);

        // Fill texture with given color.
        var data = new Color[distance * thickness];

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = color;
        }

        texture.SetData(data);

        // Rotate about the beginning middle of the line.
        var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
        var origin = new Vector2(0, thickness / 2);

        // Now draw...
        _spriteBatch.Draw(
            texture: texture,
            position: startPos,
            sourceRectangle: null,
            color: Color.White,
            rotation: rotation,
            origin: origin,
            scale: 1.0f,
            effects: SpriteEffects.None,
            layerDepth: 1.0f);
    }
}
