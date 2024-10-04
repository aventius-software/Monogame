using DependencyInjectionSample.Services;
using Microsoft.Xna.Framework;

namespace DependencyInjectionSample.Screens;

/// <summary>
/// Some example of a screen that we can inject some service into...
/// </summary>
internal class GamePlayScreen : IScreen
{
    private readonly SomeRandomService _someRandomService;

    /// <summary>
    /// Uses DI constructor injection to inject the specified service(s) for this screen
    /// </summary>
    /// <param name="someRandomService"></param>
    public GamePlayScreen(SomeRandomService someRandomService)
    {
        _someRandomService = someRandomService;
    }

    public void Draw(GameTime gameTime)
    {
        // Do drawing of everything in this screen
    }

    public void Initialise()
    {
        // Initialise anything for this screen
    }

    public void LoadContent()
    {
        // Load any content for this screen
    }

    public void UnloadContent()
    {
        // Unload any content for this screen
    }

    public void Update(GameTime gameTime)
    {
        // Update everything in this screen
    }
}
