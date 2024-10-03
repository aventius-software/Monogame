using System.Collections.Generic;
using System.Linq;

namespace MorpehECSTest.Screens;

/// <summary>
/// We can extend this class to build a collection of screens for a game, then it can be injected
/// into a screen management service (for example) by using DI and constructor injection
/// </summary>
internal abstract class ScreenCollection
{
    private readonly List<IScreen> _screens = [];

    /// <summary>
    /// Add a screen to the collection if it doesn't already exist
    /// </summary>
    /// <param name="screen"></param>
    protected void AddScreen(IScreen screen)
    {
        if (!_screens.Contains(screen)) _screens.Add(screen);
    }

    /// <summary>
    /// Add several screens to the collection
    /// </summary>
    /// <param name="screens"></param>
    protected void AddScreen(params IScreen[] screens) => _screens.AddRange(screens.Where(screen => !_screens.Contains(screen)));

    /// <summary>
    /// Get the screen instance for the requested type
    /// </summary>
    /// <typeparam name="TScreen"></typeparam>
    /// <returns></returns>
    public TScreen GetScreen<TScreen>() where TScreen : IScreen => (TScreen)_screens.Single(screen => screen is TScreen);
}
