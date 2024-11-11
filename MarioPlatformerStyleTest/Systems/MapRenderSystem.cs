using MarioPlatformerStyleTest.Services;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// This system handles drawing of the map tiles on screen
/// </summary>
internal class MapRenderSystem : ISystem
{
    public World World { get; set; }

    private readonly Camera _camera;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly MapService _mapService;

    public MapRenderSystem(World world, MapService mapService, Camera camera, GraphicsDevice graphicsDevice)
    {
        World = world;
        _mapService = mapService;
        _camera = camera;
        _graphicsDevice = graphicsDevice;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        // This line is optional... but its here to tell the map service 'where' the camera is currently looking
        // at and how high/wide the screen is. The map service will then ONLY draw tiles that are visible
        // on screen. If we remove this line, nothing will change on screen, however the map service will
        // just draw all tiles in the map including ones that are not on screen. So this is just for a bit
        // of efficiency
        _mapService.SetViewport(_camera.Position, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        // Now we can draw the map tiles
        _mapService.Draw();
    }
}
