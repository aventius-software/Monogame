using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;

namespace IsometricDynamicMapDemo.Map;

internal class MapDrawingSystem : DrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly DiamondTileMapRenderer _diamondTileMapRenderer;
    //private readonly IsometricMapService _mapService;
    private readonly SpriteBatch _spriteBatch;    

    public MapDrawingSystem(OrthographicCamera camera, SpriteBatch spriteBatch,
        DiamondTileMapRenderer diamondTileMapRenderer)
    {
        _camera = camera;
        //_mapService = mapService;
        _spriteBatch = spriteBatch;
        _diamondTileMapRenderer = diamondTileMapRenderer;
    }

    public override void Draw(GameTime gameTime)
    {        
        // As we're drawing tiles which are created at a low resolution, we
        // use PointClamp to avoid blurry pixels when the camera zooms in.
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

        // Draw the any other items that are the same depth
        // as the platforms layer but are still behind the player
        //_mapService.Draw();
        _diamondTileMapRenderer.Draw(_spriteBatch);

        // End the sprite batch
        _spriteBatch.End();
    }    
}
