﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simple2DLightingWithNormalMaps;

/// <summary>
/// A rough/simple 2D lighting effect using normal maps with shaders
/// </summary>
public class GameMain : Game
{
    private SpriteFont _font;
    private GraphicsDeviceManager _graphics;
    private Vector3 _lightPosition;
    private Effect _normalMapShader;
    private Vector2 _screenOrigin;
    private Vector2 _positionOfTextureWithoutLighting;
    private Vector2 _positionOfTextureWithLighting;
    private SpriteBatch _spriteBatch;
    private Texture2D _texture;
    private Texture2D _textureNormalMap;

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

        // Load a copy of our 'original' texture
        _texture = Content.Load<Texture2D>("Textures/tile-block");

        // Load the texture for its normal map
        _textureNormalMap = Content.Load<Texture2D>("Textures/tile-block-normal-map");

        // Load the normal map shader
        _normalMapShader = Content.Load<Effect>("Shaders/normal map lighting shader");

        // Load our font
        _font = Content.Load<SpriteFont>("Fonts/font");

        // Set positions
        _screenOrigin = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        _positionOfTextureWithoutLighting = _screenOrigin - new Vector2(_texture.Width * 2, 0);
        _positionOfTextureWithLighting = _screenOrigin;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Get the mouse position
        var mousePosition = Mouse.GetState().Position.ToVector2();

        // Place a light position at the mouse pointer        
        _lightPosition = new Vector3(mousePosition.X, mousePosition.Y, 25);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw a copy of the texture without the normal map shader
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: null);

        _spriteBatch.Draw(
            texture: _texture,
            position: _positionOfTextureWithoutLighting,
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();

        // Set the shaders general parameters that aren't individual sprite specific
        _normalMapShader.Parameters["NormalMapTexture"].SetValue(_textureNormalMap);
        _normalMapShader.Parameters["TextureSize"].SetValue(new Vector2(_texture.Width, _texture.Height));
        _normalMapShader.Parameters["AmbientColour"].SetValue(new Vector4(0.6f, 0.6f, 1f, 0.8f));
        _normalMapShader.Parameters["LightPosition"].SetValue(_lightPosition);
        _normalMapShader.Parameters["LightColour"].SetValue(new Vector4(1f, 0.8f, 0.6f, 1f));
        _normalMapShader.Parameters["LightRadius"].SetValue(300f);

        // Start batch (with our shader applied)
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: _normalMapShader,
            transformMatrix: null);

        // Set the 'world' position of our sprite so our shader knows where it is
        _normalMapShader.Parameters["WorldPosition"].SetValue(_positionOfTextureWithLighting);
        _spriteBatch.Draw(
            texture: _texture,
            position: _positionOfTextureWithLighting,
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        // Set the 'world' position of our sprite so our shader knows where it is
        _normalMapShader.Parameters["WorldPosition"].SetValue(_positionOfTextureWithLighting + new Vector2(_texture.Width, 0));
        _spriteBatch.Draw(
            texture: _texture,
            position: _positionOfTextureWithLighting + new Vector2(_texture.Width, 0),
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();

        // Draw some debugging info
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, $"Light Position: {_lightPosition}", new Vector2(0, 0), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
