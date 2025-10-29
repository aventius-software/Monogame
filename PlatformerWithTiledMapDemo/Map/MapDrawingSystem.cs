using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapDrawingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;
    private readonly SpriteBatch _spriteBatch;

    public MapDrawingSystem(MapService mapService, OrthographicCamera camera, SpriteBatch spriteBatch)
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

        // Draw the background layer. Note the use of a parallax factor
        // to create a parallax scrolling effect and make the background move/scroll
        // slower than the foreground layer.
        _mapService.MapRenderer.Draw(
            layer: _mapService.Map.GetLayer("Background"),
            viewMatrix: _camera.GetViewMatrix(_mapService.Map.GetLayer("Background").ParallaxFactor));

        // Draw the platforms layer
        _mapService.MapRenderer.Draw(
            layer: _mapService.Map.GetLayer("Platforms"),
            viewMatrix: _camera.GetViewMatrix());

        // Draw the any other items that are the same depth
        // as the platforms layer but are still behind the player
        _mapService.MapRenderer.Draw(
            layer: _mapService.Map.GetLayer("Foreground"),
            viewMatrix: _camera.GetViewMatrix());

        // End the sprite batch
        _spriteBatch.End();
    }
}
