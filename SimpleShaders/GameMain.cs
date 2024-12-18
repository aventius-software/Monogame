using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimpleShaders;

/// <summary>
/// Testing various simple shaders
/// 
/// For Visual Studio 2022, the HLSL tools extension was useful, see this link below
/// https://marketplace.visualstudio.com/items?itemName=TimGJones.HLSLToolsforVisualStudio
/// 
/// Also you can play around using ShaderToy here which can be useful and there's lots of
/// examples https://www.shadertoy.com/
/// 
/// See also:
/// 
/// https://gmjosack.github.io/posts/my-first-2d-pixel-shaders-part-1/
/// https://gamedev.net/tutorials/_/technical/apis-and-tools/2d-lighting-system-in-monogame-r4131/
/// https://thebookofshaders.com/
/// </summary>
public class GameMain : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _spriteTexture1;
    private Texture2D _spriteTexture2;

    private Effect _alterColourShader;
    private Effect _greyscaleShader;
    private Effect _minimalShader;
    private Effect _wobblyShader;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load textures
        _spriteTexture1 = Content.Load<Texture2D>("sprite 1");
        _spriteTexture2 = Content.Load<Texture2D>("sprite 2");

        // Load shaders
        _alterColourShader = Content.Load<Effect>("alter colour");
        _greyscaleShader = Content.Load<Effect>("greyscale texture");
        _minimalShader = Content.Load<Effect>("minimal shader");
        _wobblyShader = Content.Load<Effect>("wobbly");
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

        var position = Vector2.Zero;

        // Draw normal texture
        _spriteBatch.Begin();
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += 150;

        // Draw texture but with changed colours
        _spriteBatch.Begin(effect: _alterColourShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += 150;

        // Draw texture with greyscale effect
        _spriteBatch.Begin(effect: _greyscaleShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += 150;

        // Draw texture but entire texture pixels changed to same colour
        _spriteBatch.Begin(effect: _minimalShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += 150;

        // Draw texture but wobble the pixels
        _spriteBatch.Begin(effect: _wobblyShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += 150;

        base.Draw(gameTime);
    }
}
