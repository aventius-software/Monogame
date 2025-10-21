using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapRenderingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;

    public MapRenderingSystem(MapService mapService, OrthographicCamera camera)
    {
        _camera = camera;
        _mapService = mapService;
    }

    public override void Draw(GameTime gameTime)
    {
        _mapService.MapRenderer.Draw(_camera.GetViewMatrix());
    }    
}
