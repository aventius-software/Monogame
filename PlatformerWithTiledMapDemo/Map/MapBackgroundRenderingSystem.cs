using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapBackgroundRenderingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;

    public MapBackgroundRenderingSystem(MapService mapService, OrthographicCamera camera)
    {
        _camera = camera;
        _mapService = mapService;
    }

    public override void Draw(GameTime gameTime)
    {
        _mapService.MapRenderer.Draw(
            layerIndex: 0,
            viewMatrix: _camera.GetViewMatrix(new Vector2(0.5f, 0.5f)));
    }    
}
