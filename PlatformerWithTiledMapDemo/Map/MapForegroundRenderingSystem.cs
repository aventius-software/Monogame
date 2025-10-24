using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapForegroundRenderingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;
    private readonly SpriteBatch _spriteBatch;

    public MapForegroundRenderingSystem(MapService mapService, OrthographicCamera camera, SpriteBatch spriteBatch)
    {
        _camera = camera;
        _mapService = mapService;
        _spriteBatch = spriteBatch;
    }

    public override void Draw(GameTime gameTime)
    {
        // As we're drawing tiles which are created at a low resolution, we
        // use PointClamp to avoid blurry pixels when the camera zooms in.
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the any foreground platforms layer (layer index 2)
        _mapService.MapRenderer.Draw(
            layerIndex: 2,
            viewMatrix: _camera.GetViewMatrix());

        // End the sprite batch
        _spriteBatch.End();
    }    
}
