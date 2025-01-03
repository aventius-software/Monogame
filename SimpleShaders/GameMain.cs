﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimpleShaders;

/// <summary>
/// Testing various simple shaders with Monogame ;-)
/// 
/// For Visual Studio 2022, the HLSL tools extension was useful, see this link below
/// https://marketplace.visualstudio.com/items?itemName=TimGJones.HLSLToolsforVisualStudio
/// 
/// Also you can play around using ShaderToy here which can be useful and there's lots of
/// examples. Although can be complex examples https://www.shadertoy.com/ and also try this
/// cheatsheet for shader toy https://gist.github.com/markknol/d06c0167c75ab5c6720fe9083e4319e1
/// 
/// See also:
/// 
/// https://gmjosack.github.io/posts/my-first-2d-pixel-shaders-part-1/
/// https://gamedev.net/tutorials/_/technical/apis-and-tools/2d-lighting-system-in-monogame-r4131/
/// https://thebookofshaders.com/
/// https://www.ronja-tutorials.com/
/// https://github.com/butterw/bShaders
/// https://gist.github.com/Piratkopia13/46c5dda51ed59cfe69b242deb0cf40ce
/// </summary>
public class GameMain : Game
{
    private Texture2D _backgroundTileTexture;
    private float _cycleFloatValue = 0f;
    private GraphicsDeviceManager _graphics;
    private Vector4 _mouse;
    private RenderTarget2D _renderTarget;
    private SpriteBatch _spriteBatch;
    private Texture2D _spriteTexture1;
    private Texture2D _spriteTexture2;

    private Effect _alterColourShader;
    private Effect _blendingTexturesShader;
    private Effect _chromaticAbberation;
    private Effect _colourTintShader;
    private Effect _disintegrationShader;
    private Effect _edgeGlowShader;
    private Effect _grainyShader;
    private Effect _greyscaleShader;
    private Effect _invertedColoursShader;
    private Effect _maskShader;
    private Effect _minimalShader;
    private Effect _noiseShader;
    private Effect _pixelateShader;
    private Effect _psychedelicShader;    
    private Effect _simpleEdgesShader;
    private Effect _sineWaveShader;
    private Effect _transparencyShader;
    private Effect _vividColoursShader;
    private Effect _waterRippleShader;

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
        _backgroundTileTexture = Content.Load<Texture2D>("Textures/background tile");
        _spriteTexture1 = Content.Load<Texture2D>("Textures/sprite 1");
        _spriteTexture2 = Content.Load<Texture2D>("Textures/sprite 2");

        // Load shaders
        _alterColourShader = Content.Load<Effect>("Shaders/alter colour");
        _blendingTexturesShader = Content.Load<Effect>("Shaders/blending textures");
        _chromaticAbberation = Content.Load<Effect>("Shaders/chromatic abberation");
        _colourTintShader = Content.Load<Effect>("Shaders/coloured tint");
        _disintegrationShader = Content.Load<Effect>("Shaders/disintegration");
        _edgeGlowShader = Content.Load<Effect>("Shaders/edge glow");
        _grainyShader = Content.Load<Effect>("Shaders/grainy");
        _greyscaleShader = Content.Load<Effect>("Shaders/greyscale");
        _invertedColoursShader = Content.Load<Effect>("Shaders/inverted colours");
        _maskShader = Content.Load<Effect>("Shaders/mask");
        _minimalShader = Content.Load<Effect>("Shaders/minimal shader");
        _noiseShader = Content.Load<Effect>("Shaders/noise");
        _pixelateShader = Content.Load<Effect>("Shaders/pixelate");
        _psychedelicShader = Content.Load<Effect>("Shaders/psychedelic");        
        _simpleEdgesShader = Content.Load<Effect>("Shaders/simple edges");
        _sineWaveShader = Content.Load<Effect>("Shaders/sine wave");
        _transparencyShader = Content.Load<Effect>("Shaders/transparency");
        _vividColoursShader = Content.Load<Effect>("Shaders/vivid colours");
        _waterRippleShader = Content.Load<Effect>("Shaders/water ripple");

        // Create an empty render target to use for some shaders
        _renderTarget = new RenderTarget2D(GraphicsDevice, _spriteTexture1.Width, _spriteTexture1.Height);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _cycleFloatValue += 0.005f;
        if (_cycleFloatValue > 1) _cycleFloatValue = 0;

        _mouse = new Vector4(
            Mouse.GetState().Position.ToVector2(),
            Mouse.GetState().LeftButton == ButtonState.Pressed ? 1f : 0f,
            Mouse.GetState().RightButton == ButtonState.Pressed ? 1f : 0f
        );

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        var position = Vector2.Zero;
        float time = (float)gameTime.TotalGameTime.TotalSeconds;

        // Draw normal texture
        _spriteBatch.Begin();
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture but with altered colours
        _spriteBatch.Begin(effect: _alterColourShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture with a coloured tint
        _colourTintShader.Parameters["Colour"].SetValue(Color.Green.ToVector4());
        _spriteBatch.Begin(effect: _colourTintShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw blended textures        
        _blendingTexturesShader.Parameters["BlendingAmount"].SetValue(_cycleFloatValue);
        _blendingTexturesShader.Parameters["BlendingTexture"].SetValue(_spriteTexture2);
        _spriteBatch.Begin(effect: _blendingTexturesShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture with greyscale effect
        _greyscaleShader.Parameters["GreyscaleLevel"].SetValue(0.5f);
        _spriteBatch.Begin(effect: _greyscaleShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;
        
        // Draw texture but entire texture pixels changed to same colour
        _spriteBatch.Begin(effect: _minimalShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Next row of sprites... ;-)
        position.X = 0;
        position.Y += _spriteTexture1.Height;

        // Draw noise
        _noiseShader.Parameters["Resolution"].SetValue(new Vector2(2, 2));
        _noiseShader.Parameters["Time"].SetValue(time);
        _noiseShader.Parameters["Mouse"].SetValue(_mouse);
        _spriteBatch.Begin(effect: _noiseShader);
        _spriteBatch.Draw(texture: _renderTarget, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture with a pixelated effect
        _pixelateShader.Parameters["PixelSize"].SetValue(_cycleFloatValue * 10);
        _pixelateShader.Parameters["TextureDimensions"].SetValue(new Vector2(_spriteTexture1.Width, _spriteTexture1.Height));
        _spriteBatch.Begin(effect: _pixelateShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture with a sine wave effect
        _sineWaveShader.Parameters["Frequency"].SetValue(18.5f);
        _sineWaveShader.Parameters["Amplitude"].SetValue(0.05f);
        _sineWaveShader.Parameters["Time"].SetValue(time);
        _spriteBatch.Begin(effect: _sineWaveShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture but make partly transparent, first draw a background
        _spriteBatch.Begin();
        _spriteBatch.Draw(texture: _backgroundTileTexture, position: position, color: Color.White);
        _spriteBatch.End();

        // Now draw a sprite on top of the background with transparent effect
        _transparencyShader.Parameters["TransparencyLevel"].SetValue(0.5f);
        _spriteBatch.Begin(effect: _transparencyShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw texture with a water ripple (stone dropped in pond) type effect
        _waterRippleShader.Parameters["RippleCenter"].SetValue(new Vector2(0.5f, 0.5f));
        _waterRippleShader.Parameters["Time"].SetValue(time * 6);
        _waterRippleShader.Parameters["Amplitude"].SetValue(0.25f);
        _waterRippleShader.Parameters["Frequency"].SetValue(30.0f);
        _spriteBatch.Begin(effect: _waterRippleShader);
        _spriteBatch.Draw(texture: _backgroundTileTexture, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;
        
        // Draw texture and disintegrate
        _disintegrationShader.Parameters["DisintegrationThreshold"].SetValue(_cycleFloatValue);
        _spriteBatch.Begin(effect: _disintegrationShader);
        _spriteBatch.Draw(texture: _backgroundTileTexture, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Next row of sprites... ;-)
        position.X = 0;
        position.Y += _spriteTexture1.Height;

        // Draw a texture, but try and find any edges and add a glow to them
        _edgeGlowShader.Parameters["GlowIntensity"].SetValue(0.75f);
        _edgeGlowShader.Parameters["GlowThreshold"].SetValue(0.2f);
        _edgeGlowShader.Parameters["TextureDimensions"].SetValue(new Vector2(_spriteTexture1.Width, _spriteTexture1.Height));
        _spriteBatch.Begin(effect: _edgeGlowShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw a texture, but add an animated 'grainy' effect. You could set
        // the 'time' to 1 if you don't want any animation of the effect
        _grainyShader.Parameters["Time"].SetValue(time);
        _grainyShader.Parameters["GrainIntensity"].SetValue(0.15f);
        _edgeGlowShader.Parameters["TextureDimensions"].SetValue(new Vector2(_spriteTexture1.Width, _spriteTexture1.Height));
        _spriteBatch.Begin(effect: _grainyShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw a texture, but with a spinning psychedelic or trippy effect
        _psychedelicShader.Parameters["Time"].SetValue(time);
        _psychedelicShader.Parameters["Strength"].SetValue(18.0f);
        _spriteBatch.Begin(effect: _psychedelicShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw a texture, but with inverted colours     
        _spriteBatch.Begin(effect: _invertedColoursShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw a texture, but with masked effect
        _spriteBatch.Begin(effect: _maskShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;        
        
        // Draw a texture, but with a simple edge effect
        _spriteBatch.Begin(effect: _simpleEdgesShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Next row of sprites... ;-)
        position.X = 0;
        position.Y += _spriteTexture1.Height;

        // Draw a texture, but with more 'vivid' colours
        _spriteBatch.Begin(effect: _vividColoursShader);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        // Draw a texture, but with chromatic abberation applied to the colours
        _chromaticAbberation.Parameters["Amount"].SetValue(0.025f);
        _spriteBatch.Begin(effect: _chromaticAbberation);
        _spriteBatch.Draw(texture: _spriteTexture1, position: position, color: Color.White);
        _spriteBatch.End();
        position.X += _spriteTexture1.Width;

        base.Draw(gameTime);
    }
}
