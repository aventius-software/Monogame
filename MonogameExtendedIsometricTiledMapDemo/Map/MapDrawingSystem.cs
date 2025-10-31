using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace MonogameExtendedIsometricTiledMapDemo.Map;

internal class MapDrawingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly IsometricMapService _mapService;
    private readonly SpriteBatch _spriteBatch;

    public MapDrawingSystem(IsometricMapService mapService, OrthographicCamera camera, SpriteBatch spriteBatch)
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
       
        // Draw the any other items that are the same depth
        // as the platforms layer but are still behind the player
        _mapService.MapRenderer.Draw(viewMatrix: _camera.GetViewMatrix());

        // End the sprite batch
        _spriteBatch.End();
    }    
}
