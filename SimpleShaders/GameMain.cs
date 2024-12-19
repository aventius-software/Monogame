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
/// examples. Although can be complex examples https://www.shadertoy.com/
/// 
/// See also:
/// 
/// https://gmjosack.github.io/posts/my-first-2d-pixel-shaders-part-1/
/// https://gamedev.net/tutorials/_/technical/apis-and-tools/2d-lighting-system-in-monogame-r4131/
/// https://thebookofshaders.com/
/// https://www.ronja-tutorials.com/
/// </summary>
public class GameMain : Game
{
    private float _cycleValue = 0f;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _spriteTexture1;
    private Texture2D _spriteTexture2;

    private Effect _alterColourShader;
    private Effect _blendingTexturesShader;
    private Effect _greyscaleShader;
    private Effect _minimalShader;
    private Effect _noiseShader;
    private Effect _pixelateShader;
    private Effect _slideTransitionShader;
    private Effect _transparencyShader;
    private Effect _transparentSectionShader;
    private Effect _tunnelShader;
    private Effect _wavyShader;

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
        _spriteTexture1 = Content.Load<Texture2D>("Textures/sprite 1");
        _spriteTexture2 = Content.Load<Texture2D>("Textures/sprite 2");

        // Load shaders
        _alterColourShader = Content.Load<Effect>("Shaders/alter colour");
        _blendingTexturesShader = Content.Load<Effect>("Shaders/blending textures");
        _greyscaleShader = Content.Load<Effect>("Shaders/greyscale");
        _minimalShader = Content.Load<Effect>("Shaders/minimal shader");
        _noiseShader = Content.Load<Effect>("Shaders/noise");
        _pixelateShader = Content.Load<Effect>("Shaders/pixelate");
        _slideTransitionShader = Content.Load<Effect>("Shaders/slide transition");
        _transparencyShader = Content.Load<Effect>("Shaders/transparency");
        _transparentSectionShader = Content.Load<Effect>("Shaders/transparent section");
        _tunnelShader = Content.Load<Effect>("Shaders/tunnel");
        _wavyShader = Content.Load<Effect>("Shaders/wavy");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _cycleValue += 0.005f;
        if (_cycleValue > 1) _cycleValue = 0;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        var position = Vector2.Zero;

        // 1. Draw normal texture
        _spriteBatch.Begin();
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // 2. Draw texture but with altered colours
        _spriteBatch.Begin(effect: _alterColourShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // 3. Draw blended textures        
        _blendingTexturesShader.Parameters["blendingAmount"].SetValue(_cycleValue);
        _blendingTexturesShader.Parameters["blendWithTexture"].SetValue(_spriteTexture2);
        _spriteBatch.Begin(effect: _blendingTexturesShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // 4. Draw texture with greyscale effect
        _spriteBatch.Begin(effect: _greyscaleShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // 5. Draw texture but entire texture pixels changed to same colour
        _spriteBatch.Begin(effect: _minimalShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // 6. Draw texture but make partly transparent
        _transparencyShader.Parameters["transparencyLevel"].SetValue(0.25f);
        _spriteBatch.Begin(effect: _transparencyShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;
        
        // 7. Draw texture but add a sine wave style wobble to the pixels
        _spriteBatch.Begin(effect: _wavyShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Next row of sprites... ;-)
        position.X = 0;
        position.Y += _spriteTexture1.Height;

        base.Draw(gameTime);
    }
}
