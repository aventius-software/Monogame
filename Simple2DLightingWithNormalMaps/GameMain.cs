using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Simple2DLightingWithNormalMaps;

public class GameMain : Game
{
    private SpriteFont _font;
    private GraphicsDeviceManager _graphics;
    private float _lightingAngle;
    private float _lightingAngleInDegrees;
    private Vector3 _lightDirection;
    private Effect _normalMapShader;
    private Vector2 _origin;
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
        _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

        // Load the texture for its normal map
        _textureNormalMap = Content.Load<Texture2D>("Textures/tile-block-normal-map");

        // Load the normal map shader
        _normalMapShader = Content.Load<Effect>("Shaders/normal map lighting shader");

        // Load our font
        _font = Content.Load<SpriteFont>("Fonts/font");

        // Set positions
        var screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        _positionOfTextureWithoutLighting = screenCenter - new Vector2(_texture.Width, 0);
        _positionOfTextureWithLighting = screenCenter + new Vector2(_texture.Width, 0);
    }

    public static float AngleBetween(Vector2 from, Vector2 to)
    {
        // Calculate the angle (radians of course) between 2 vectors
        return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Get the mouse position
        var mousePosition = Mouse.GetState().Position.ToVector2();

        // Calculate angle between the sprite with lighting and the mouse
        _lightingAngle = AngleBetween(_positionOfTextureWithLighting, mousePosition) + MathHelper.ToRadians(90);
        _lightingAngleInDegrees = MathHelper.ToDegrees(_lightingAngle);

        // Now calculate the direction vector for the light from the angle
        var directionVector = new Vector2((float)Math.Sin(_lightingAngle), (float)Math.Cos(_lightingAngle));
        directionVector.Normalize();

        // Set the light direction, keep Z as 0 ;-)
        _lightDirection = new Vector3(directionVector.X, directionVector.Y, 0f);
        _lightDirection.Normalize();

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
            origin: _origin,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();

        // Set the shaders parameters
        _normalMapShader.Parameters["LightDirection"].SetValue(_lightDirection);
        _normalMapShader.Parameters["LightColour"].SetValue(new Vector3(1f, 1f, 1f) * 1f);
        _normalMapShader.Parameters["AmbientColour"].SetValue(new Vector3(1f, 1f, 1f) * 0.25f);
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
        _spriteBatch.Draw(
            texture: _texture,
            position: _positionOfTextureWithLighting,
            sourceRectangle: new Rectangle(0, 0, _texture.Width, _texture.Height),
            color: Color.White,
            rotation: 0,
            origin: _origin,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();

        // Draw some debugging info
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, $"Angle: {_lightingAngleInDegrees}", new Vector2(0, 0), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
