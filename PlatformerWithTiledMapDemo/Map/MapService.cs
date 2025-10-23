using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapService
{
    private readonly ContentManager _contentManager;
    private readonly TiledMapRenderer _mapRenderer;

    public TiledMap Map { get; protected set; }
    public TiledMapRenderer MapRenderer => _mapRenderer;

    public MapService(TiledMapRenderer mapRenderer, ContentManager contentManager)
    {
        _mapRenderer = mapRenderer;
        _contentManager = contentManager;
    }

    /// <summary>
    /// Returns a rectangle with world coordinates for the specified map row/column
    /// </summary>    
    /// <param name="mapRow"></param>
    /// <param name="mapColumn"></param>
    /// <returns></returns>
    private RectangleF GetRectangleAtMapPosition(int mapRow, int mapColumn, int tileWidth, int tileHeight)
    {
        return new RectangleF(mapColumn * tileWidth, mapRow * tileHeight, tileWidth, tileHeight);
    }

    /// <summary>
    /// Get a list of the surrounding tiles at the specified position for the 
    /// specified width and height
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

        foreach (var layer in Map.TileLayers)
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

    public void LoadMap(string mapName)
    {
        Map = _contentManager.Load<TiledMap>(mapName);
        _mapRenderer.LoadMap(Map);
    }
}
