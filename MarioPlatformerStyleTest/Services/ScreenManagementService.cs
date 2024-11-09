using Microsoft.Xna.Framework;

namespace MarioPlatformerStyleTest.Services;

internal class ScreenManagementService
{
    private IScreen _currentScreen;
    private readonly ScreenCollection _screens;

    public ScreenManagementService(ScreenCollection screens)
    {
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
        _currentScreen?.Draw(gameTime);
    }

    public void Update(GameTime gameTime) => _currentScreen?.Update(gameTime);
}
