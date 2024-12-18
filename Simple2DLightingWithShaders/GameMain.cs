using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simple2DLightingWithShaders;

public class GameMain : Game
{
    private RenderTarget2D _backgroundRenderTarget;
    private Texture2D _backgroundTileTexture;
    private GraphicsDeviceManager _graphics;
    private Effect _lightingShader;
    private RenderTarget2D _lightSourcesRenderTarget;
    private Texture2D _lightingTexture;
    private Point _mousePosition;
    private SpriteBatch _spriteBatch;

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

        // Load our shader and a radial gradient texture which will act as a kind
        // of mask when we blend the background 'render target' and the light
        // sources 'render target' together to produce the final background
        _lightingShader = Content.Load<Effect>("lighting shader");
        _lightingTexture = Content.Load<Texture2D>("radial gradient 256x256");

        // Load a texture for background tiles
        _backgroundTileTexture = Content.Load<Texture2D>("background tile");

        // Create our render targets for the screen and another for all light sources
        _backgroundRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _lightSourcesRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Save mouse position
        _mousePosition = Mouse.GetState().Position - new Point(_lightingTexture.Width / 2, _lightingTexture.Height / 2);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Draw the 'background' to the background render target        
        DrawTiledBackgroundToRenderTarget();

        // Now draw some light sources to the light sources render target
        DrawLightSourcesToRenderTarget();

        // Pass parameters to our shader... give it the background 'texture' and the lighting 'texture'        
        _lightingShader.Parameters["backgroundTexture"].SetValue(_backgroundRenderTarget);
        _lightingShader.Parameters["lightSourcesTexture"].SetValue(_lightSourcesRenderTarget);

        // If you want to give some light to the whole background, set this between 0 and 1
        // depending on the strength of the light. A value of 0 will make the background pitch
        // black, but a value of 1 will make the background completely visible and no light
        // sources will have any effect or be seen...
        _lightingShader.Parameters["backgroundAmbientLightStrength"].SetValue(0.15f);

        // Now, we draw the background render target to the screen, with our shader applied to it. The shader
        // will apply the 'lighting' by mixing the light sources texture pixels colours with the background
        // texture pixels colours and hey presto, we've got some 'fake' 2D lighting ;-)
        _spriteBatch.Begin(effect: _lightingShader);
        _spriteBatch.Draw(texture: _backgroundRenderTarget, position: Vector2.Zero, color: Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawLightSourcesToRenderTarget()
    {
        // Draw some light sources to the light sources render target
        GraphicsDevice.SetRenderTarget(_lightSourcesRenderTarget);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        
        // Draw a static light source
        _spriteBatch.Draw(
            texture: _lightingTexture,
            position: Vector2.Zero,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        // Draw a light source at the current mouse coordinates
        _spriteBatch.Draw(
            texture: _lightingTexture,
            position: _mousePosition.ToVector2(),
            sourceRectangle: null,
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// This just draws a load of tiles on the screen, but the background could be anything
    /// </summary>
    private void DrawTiledBackgroundToRenderTarget()
    {
        // Draw background to the 'background' render target
        GraphicsDevice.SetRenderTarget(_backgroundRenderTarget);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // Fill the screen with tiles
        for (var x = 0; x <= GraphicsDevice.Viewport.Width / _backgroundTileTexture.Width; x++)
        {
            for (var y = 0; y <= GraphicsDevice.Viewport.Height / _backgroundTileTexture.Height; y++)
            {
                var tilePosition = new Vector2(x * _backgroundTileTexture.Width, y * _backgroundTileTexture.Height);
                _spriteBatch.Draw(texture: _backgroundTileTexture, position: tilePosition, color: Color.White);
            }
        }

        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
    }
}
