using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MorpehECSTest.Screens;

namespace MorpehECSTest.Services;

internal class ScreenManagementService
{
    private IScreen _currentScreen;
    private readonly ScreenCollection _screens;
    private readonly SpriteBatch _spriteBatch;

    public ScreenManagementService(SpriteBatch spriteBatch, ScreenCollection screens)
    {
        _spriteBatch = spriteBatch;
        _screens = screens;
    }

    /// <summary>
    /// Change to the specified screen, unloads the existing screen if there is one
    /// </summary>
    /// <param name="screen"></param>
    public void ChangeScreen<TScreen>() where TScreen : IScreen
    {
        // Unload any current screen
        _currentScreen?.UnloadContent();

        // Get the new screen
        var screen = _screens.GetScreen<TScreen>();

        // Get the requested screen and switch            
        screen.Initialise();
        screen.LoadContent();

        // Set our new screen as the current
        _currentScreen = screen;
    }

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: null);

        // Draw the current screen here
        _currentScreen?.Draw(gameTime);

        _spriteBatch.End();
    }

    public void Update(GameTime gameTime) => _currentScreen?.Update(gameTime);
}
