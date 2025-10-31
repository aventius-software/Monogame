using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MonogameExtendedIsometricTiledMapDemo.Map;

/// <summary>
/// This service handles loading and querying the Tiled map and is kind of a wrapper
/// around MonoGame.Extended's Tiled map support. Why do this? Well, wrapping it like 
/// this means we can:
/// 
/// - easily change the implementation later if we want to
/// - allows us to add our own helper methods such as the position clamping
/// - lets us inject this as a service wherever we need it via dependency injection
/// </summary>
internal class IsometricMapService
{    
    public TiledMap Map { get; protected set; }
    public TiledMapRenderer MapRenderer => _mapRenderer;

    private readonly ContentManager _contentManager;
    private readonly TiledMapRenderer _mapRenderer;

    public IsometricMapService(TiledMapRenderer mapRenderer, ContentManager contentManager)
    {
        _mapRenderer = mapRenderer;
        _contentManager = contentManager;
    }

    /// <summary>
    /// Takes a world position and returns the tile coordinates for that position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Point WorldToMapTilePosition(Vector2 worldPosition)
    {
        var halfTileWidth = Map.TileWidth / 2f;
        var halfTileHeight = Map.TileHeight / 2f;

        var tileX = (int)((worldPosition.X / halfTileWidth + worldPosition.Y / halfTileHeight) / 2);
        var tileY = (int)((worldPosition.Y / halfTileHeight - worldPosition.X / halfTileWidth) / 2);

        return new Point(tileX, tileY);
    }

    /// <summary>
    /// Load the specified Tiled map content
    /// </summary>
    /// <param name="mapName"></param>
    public void LoadMap(string mapName)
    {
        Map = _contentManager.Load<TiledMap>(mapName);
        _mapRenderer.LoadMap(Map);
    }
}
