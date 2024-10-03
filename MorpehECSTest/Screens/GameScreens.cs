namespace MorpehECSTest.Screens;

/// <summary>
/// We can collate all our game screens here (obviously in this example we've just got one), then 
/// this can be 'constructor' injected into the screen management service by the DI container ;-)
/// </summary>
internal class GameScreens : ScreenCollection
{
    public GameScreens(GamePlayScreen gamePlayScreen)
    {
        // Add all our game screens (well, just the one for this demo/test ;-)
        AddScreen(gamePlayScreen);
    }
}
