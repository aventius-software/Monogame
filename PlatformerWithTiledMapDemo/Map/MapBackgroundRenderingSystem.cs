using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapBackgroundRenderingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;
    private readonly SpriteBatch _spriteBatch;

    private readonly Vector2 _parallaxFactor = new(0.5f, 0.5f);

    public MapBackgroundRenderingSystem(MapService mapService, OrthographicCamera camera, SpriteBatch spriteBatch)
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

        // Draw the background layer (layer index 0). Note the use of a parallax factor
        // to create a parallax scrolling effect and make the background move/scroll
        // slower than the foreground layer.
        _mapService.MapRenderer.Draw(
            layerIndex: 0,
            viewMatrix: _camera.GetViewMatrix(_parallaxFactor));

        // End the sprite batch
        _spriteBatch.End();
    }
}
