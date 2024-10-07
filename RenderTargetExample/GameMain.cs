using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RenderTargetExample.Services;

namespace RenderTargetExample;

/// <summary>
/// A simple example of the use of (and how to use) RenderTarget2D. It's useful when dealing with different
/// potential resolutions and keeping the scale of the game the same and preserving aspect ratio
/// </summary>
public class GameMain : Game
{
    private CustomRenderTarget _customRenderTarget;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _texture;

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

        // Load some images
        _texture = Content.Load<Texture2D>("circle");

        // Create our custom render target
        _customRenderTarget = new CustomRenderTarget(GraphicsDevice, _spriteBatch);

        // Choose an option to try the outcome
        var option = 0;

        switch (option)
        {
            // Scale to a third current screen width, but half current screen height
            case 0:
                {
                    var virtualWidth = _graphics.PreferredBackBufferWidth / 3;
                    var virtualHeight = _graphics.PreferredBackBufferHeight / 2;

                    // Set the render target resolution
                    _customRenderTarget.InitialiseRenderDestination(virtualWidth, virtualHeight, Color.CornflowerBlue);
                }
                break;

            // Scale to a half current screen width, but a third the current screen height
            case 1:
                {
                    var virtualWidth = _graphics.PreferredBackBufferWidth / 2;
                    var virtualHeight = _graphics.PreferredBackBufferHeight / 3;

                    // Set the render target resolution
                    _customRenderTarget.InitialiseRenderDestination(virtualWidth, virtualHeight, Color.CornflowerBlue);
                }
                break;

            // Scale width and height to half, so full screen, but effectively everything is twice as big
            default:
                {
                    var virtualWidth = _graphics.PreferredBackBufferWidth / 2;
                    var virtualHeight = _graphics.PreferredBackBufferHeight / 2;

                    // Set the render target resolution
                    _customRenderTarget.InitialiseRenderDestination(virtualWidth, virtualHeight, Color.CornflowerBlue);
                }
                break;
        }
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
        // Begin rendering with our custom render target, must be BEFORE 'SpriteBatch.Begin()'
        _customRenderTarget.Begin();

        // Sprite begin as normal...
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: null);

        // Draw sprites as normal (these will be drawn to the render target, not the screen!)
        _spriteBatch.Draw(
            texture: _texture,
            position: new Vector2(0, 0),
            color: Color.White);

        // End sprite batch as normal
        _spriteBatch.End();

        // Finally, AFTER 'SpriteBatch.End()' we 'draw' the render target to the real screen
        _customRenderTarget.Draw();

        base.Draw(gameTime);
    }
}
