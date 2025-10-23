using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapForegroundRenderingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;

    public MapForegroundRenderingSystem(MapService mapService, OrthographicCamera camera)
    {
        _camera = camera;
        _mapService = mapService;
    }

    public override void Draw(GameTime gameTime)
    {
        _mapService.MapRenderer.Draw(
            layerIndex: 2,
            viewMatrix: _camera.GetViewMatrix());
    }    
}
