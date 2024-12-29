using Microsoft.Xna.Framework;
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
    private Vector2 _spriteOrigin;
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
        _spriteOrigin = new Vector2(_texture.Width / 2, _texture.Height / 2);

        // Load the texture for its normal map
        _textureNormalMap = Content.Load<Texture2D>("Textures/tile-block-normal-map");

        // Load the normal map shader
        _normalMapShader = Content.Load<Effect>("Shaders/normal map lighting shader");

        // Load our font
        _font = Content.Load<SpriteFont>("Fonts/font");

        // Set positions
        _screenOrigin = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        _positionOfTextureWithoutLighting = _screenOrigin - new Vector2(_texture.Width, 0);
        _positionOfTextureWithLighting = _screenOrigin;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Get the mouse position
        var mousePosition = Mouse.GetState().Position.ToVector2();

        // Place the light position at the mouse pointer
        var lightPosition = mousePosition - _screenOrigin;
        _lightPosition = new Vector3(lightPosition.X, -lightPosition.Y, 0);

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
            origin: _spriteOrigin,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();

        // Set the shaders parameters        
        _normalMapShader.Parameters["LightPosition"].SetValue(_lightPosition);
        _normalMapShader.Parameters["LightColour"].SetValue(new Vector3(1f, 1f, 1f) * 0.75f);
        _normalMapShader.Parameters["AmbientColour"].SetValue(new Vector3(1f, 1f, 1f) * 0.25f);
        _normalMapShader.Parameters["LightRadius"].SetValue(500f);
        _normalMapShader.Parameters["NormalMapTexture"].SetValue(_textureNormalMap);

        // Start batch (with our shader applied)
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: _normalMapShader,
            transformMatrix: null);

        // Draw the texture with the normal map shader applied
        var offset = Vector2.Zero;
        var spritePosition = _positionOfTextureWithLighting + offset;
        var worldPosition = new Vector3(spritePosition - _screenOrigin, 0);
        _normalMapShader.Parameters["WorldPosition"].SetValue(worldPosition);

        _spriteBatch.Draw(
            texture: _texture,
            position: spritePosition,
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: _spriteOrigin,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        offset = new Vector2(_texture.Width, 0);
        spritePosition = _positionOfTextureWithLighting + offset;
        worldPosition = new Vector3(spritePosition - _screenOrigin, 0);
        _normalMapShader.Parameters["WorldPosition"].SetValue(worldPosition);

        _spriteBatch.Draw(
            texture: _texture,
            position: spritePosition,
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: _spriteOrigin,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        offset = new Vector2(_texture.Width / 2, _texture.Height / 4);
        spritePosition = _positionOfTextureWithLighting + offset;
        worldPosition = new Vector3(spritePosition - _screenOrigin, 0);
        _normalMapShader.Parameters["WorldPosition"].SetValue(worldPosition);

        _spriteBatch.Draw(
            texture: _texture,
            position: spritePosition,
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: _spriteOrigin,
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
