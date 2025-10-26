using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Tiled;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapBackgroundRenderingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;
    private readonly SpriteBatch _spriteBatch;

    private TiledMapLayer _layer;

    public MapBackgroundRenderingSystem(MapService mapService, OrthographicCamera camera, SpriteBatch spriteBatch)
    {
        _camera = camera;
        _mapService = mapService;
        _spriteBatch = spriteBatch;
    }

    public override void Initialize(World world)
    {
        // Get a reference to the background layer
        _layer = _mapService.Map.GetLayer("Background");

        base.Initialize(world);
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
            layer: _layer,
            viewMatrix: _camera.GetViewMatrix(_layer.ParallaxFactor));

        // End the sprite batch
        _spriteBatch.End();
    }
}
