using Microsoft.Xna.Framework;

namespace DungeonMasterStyleDemo.Services;

/// <summary>
/// This helps manage our game screens and transitioning between them. This could be improved
/// with fancy features like fade or swipe style transitions between screens, but for demo
/// purposes this does the trick
/// </summary>
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

    /// <summary>
    /// Draw the current screen
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime) => _currentScreen?.Draw(gameTime);

    /// <summary>
    /// Update the current screen
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime) => _currentScreen?.Update(gameTime);
}
