using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonogameExtendedIsometricTiledMapDemo.Map;

/// <summary>
/// This service handles loading and querying the Tiled map and is kind of a
/// wrapper around MonoGame.Extended's Tiled map support. Why do this? Well, wrapping 
/// it like this means we can:
/// 
/// - easily change the implementation later if we want to
/// - allows us to add our own helper methods such as the position clamping
/// - lets us inject this as a service wherever we need it via dependency injection
/// </summary>
internal class MapService
{
    public string CollisionLayerName { get; set; } = "Collisions";
    public TiledMap Map { get; protected set; }
    public TiledMapRenderer MapRenderer => _mapRenderer;

    private readonly ContentManager _contentManager;
    private readonly TiledMapRenderer _mapRenderer;

    public MapService(TiledMapRenderer mapRenderer, ContentManager contentManager)
    {
        _mapRenderer = mapRenderer;
        _contentManager = contentManager;
    }

    /// <summary>
    /// Clamp the given position to be within the map boundaries, taking into account
    /// the viewport size so we don't show area outside the map edges. This is useful
    /// for camera positioning so the camera doesn't show area outside the map.
    /// 
    /// This function COULD be made more efficient by caching the min/max values instead of
    /// calculating them each time, but for simplicity in this demo we'll just calculate
    /// </summary>
    /// <param name="position"></param>
    /// <param name="viewPort"></param>
    /// <returns></returns>
    public Vector2 ClampPositionToMapBoundry(Vector2 position, RectangleF viewPort)
    {
        // The minimum X and Y values are half the viewport size, because the camera
        // is centered on its position. So the minimum position we can have is half the
        // viewport size, otherwise the camera would show area outside the map bounds to
        // the left and top
        var minX = viewPort.Width / 2f;
        var minY = viewPort.Height / 2f;

        // Similarly, the maximum X and Y values are the map size minus half the viewport size
        // again because the camera is centered on its position. This will stop the camera from
        // showing area outside the map bounds to the right and bottom
        var maxX = Map.WidthInPixels - minX;
        var maxY = Map.HeightInPixels - minY;

        // Now we can use these values to restrict camera position to inside the map only
        var clampedX = MathHelper.Clamp(position.X, minX, maxX);
        var clampedY = MathHelper.Clamp(position.Y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }

    /// <summary>
    /// Returns a rectangle with world coordinates for the specified map row/column
    /// </summary>    
    /// <param name="mapRow"></param>
    /// <param name="mapColumn"></param>
    /// <returns></returns>
    private static RectangleF GetRectangleAtMapPosition(int mapRow, int mapColumn, int tileWidth, int tileHeight)
    {
        return new RectangleF(mapColumn * tileWidth, mapRow * tileHeight, tileWidth, tileHeight);
    }

    /// <summary>
    /// Get a list of the surrounding tiles at the specified position for the 
    /// specified width and height. It only returns tiles that exist in the collision
    /// layer
    /// </summary>
    /// <param name="position"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>    
    /// <returns></returns>
    public List<RectangleF> GetSurroundingTiles(Vector2 position, float width, float height)
    {
        var bounds = new RectangleF(
            position.X,
            position.Y,
            width,
            height);

        // Loop through each of the surrounding tiles        
        var tiles = new List<RectangleF>();

        // For each tile layer that matches the collision layer name
        foreach (var layer in Map.TileLayers.Where(x => x.Name == CollisionLayerName))
        {
            // Find the edge tile positions
            int leftTile = (int)Math.Floor((float)bounds.Left / Map.TileWidth);
            int rightTile = (int)Math.Ceiling((float)bounds.Right / Map.TileWidth);
            int topTile = (int)Math.Floor((float)bounds.Top / Map.TileHeight);
            int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / Map.TileHeight);

            // Restrict the surrounding tiles to the map layer dimensions
            leftTile = (int)Math.Clamp(leftTile, 0, layer.Width);
            rightTile = (int)Math.Clamp(rightTile, 0, layer.Width);
            topTile = (int)Math.Clamp(topTile, 0, layer.Height);
            bottomTile = (int)Math.Clamp(bottomTile, 0, layer.Height);

            for (var row = topTile; row <= bottomTile; row++)
            {
                for (var column = leftTile; column <= rightTile; column++)
                {
                    // Get the tile id
                    var tileId = layer.GetTile((ushort)column, (ushort)row).GlobalIdentifier;

                    // Skip if no tile...
                    if (tileId == 0) continue;

                    var tileBounds = GetRectangleAtMapPosition(row, column, Map.TileWidth, Map.TileHeight);

                    if (bounds.Intersects(tileBounds))
                    {
                        tiles.Add(tileBounds);
                    }
                }
            }
        }

        return tiles;
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
