﻿using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotTiledTest;

internal struct MapTile
{
    public Rectangle Bounds;
    public int Column;
    public int Row;
    public string TileClass;
    public int TileId;
}

/// <summary>
/// A basic tile map management and rendering service for use with 'Tiled' maps
/// </summary>
internal class MapService
{
    private readonly ContentManager _contentManager;

    private int _numberOfVisibleTileColumns;
    private int _numberOfVisibleTileRows;
    private readonly SpriteBatch _spriteBatch;
    private Map _tiledMap;
    private Texture2D _tilesetTexture;
    private int _tileColumnPositionInTheWorld;
    private int _tileRowPositionInTheWorld;

    /// <summary>
    /// Gets or sets the active map layer to use
    /// </summary>
    public int ActiveLayer { get; set; } = 0;

    /// <summary>
    /// Gets or sets the active tileset to use
    /// </summary>
    public int ActiveTileset { get; set; } = 0;

    /// <summary>
    /// The world height (in pixels)
    /// </summary>
    public int WorldHeight => (int)_tiledMap.Height * (int)_tiledMap.TileHeight;

    /// <summary>
    /// The world width (in pixels)
    /// </summary>
    public int WorldWidth => (int)_tiledMap.Width * (int)_tiledMap.TileWidth;

    public MapService(SpriteBatch spriteBatch, ContentManager contentManager)
    {
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
    }

    /// <summary>
    /// Draw the current map (using the active layer)
    /// </summary>
    public void Draw()
    {
        // Get the layer and the tileset to draw
        var tileset = _tiledMap.Tilesets[ActiveTileset];
        var layer = GetLayer(ActiveLayer);

        // Calculate how many rows/columns we should draw, if no values have been
        // specified then we just draw all rows and columns
        var rowsToDraw = _numberOfVisibleTileRows == 0 ? (int)layer.Height : _numberOfVisibleTileRows;
        var colsToDraw = _numberOfVisibleTileColumns == 0 ? (int)layer.Width : _numberOfVisibleTileColumns;

        // We 'overdraw' the tiles so we end up drawing a tile offscreen to that a
        // tile doesn't just 'flicker' into existance at the edges of the screen
        if (_numberOfVisibleTileRows > 0 || _numberOfVisibleTileColumns > 0)
        {
            var tilesToOverDraw = 2;
            rowsToDraw += tilesToOverDraw;
            colsToDraw += tilesToOverDraw;
        }

        // Draw the relevant tiles on screen
        for (int row = _tileRowPositionInTheWorld; row <= _tileRowPositionInTheWorld + rowsToDraw; row++)
        {
            for (int column = _tileColumnPositionInTheWorld; column <= _tileColumnPositionInTheWorld + colsToDraw; column++)
            {
                // Don't bother if the row/column position is out of bounds of the map
                if (column < 0 || row < 0 || column >= layer.Width || row >= layer.Height) continue;

                // Otherwise lets find out which tile in the map is at the current row/column position                
                var tile = GetTileAtPosition(row, column);

                // If block is 0, i.e. air or nothing, then just continue...
                if (tile == 0) continue;

                // Get the correct tile 'image' rectangle from the tileset
                var sourceRectangle = GetImageSourceRectangleForTile(tile);

                // Draw this tile
                _spriteBatch.Draw(
                    texture: _tilesetTexture,
                    position: new Vector2(column * tileset.TileWidth, row * tileset.TileHeight),
                    sourceRectangle: sourceRectangle,
                    color: Microsoft.Xna.Framework.Color.White);
            }
        }
    }

    /// <summary>
    /// Helper method to work out the source rectangle for the specified tile so we can
    /// pick out the correct texture to use when drawing the tile
    /// </summary>
    /// <param name="tileset"></param>
    /// <param name="gid"></param>
    /// <returns></returns>
    private Rectangle GetImageSourceRectangleForTile(uint gid)
    {
        var tileId = (int)gid - 1;
        var tileset = _tiledMap.Tilesets[ActiveTileset];

        var row = tileId / (int)tileset.Columns;
        var column = tileId % (int)tileset.Columns;

        var tileWidth = (int)tileset.TileWidth;
        var tileHeight = (int)tileset.TileHeight;
        var x = tileWidth * column;
        var y = tileHeight * row;

        return new Rectangle(x, y, tileWidth, tileHeight);
    }

    /// <summary>
    /// Returns the specified tile layer, or the first if no argument value is specified
    /// </summary>
    /// <param name="layerNumber"></param>
    /// <returns></returns>
    public TileLayer GetLayer(int layerNumber = 0) => (TileLayer)_tiledMap.Layers[layerNumber];

    /// <summary>
    /// Get a list of the surrounding tiles at the specified position for the 
    /// specified width and height
    /// </summary>
    /// <param name="position"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public List<MapTile> GetSurroundingTiles(Vector2 position, int width, int height)
    {
        var tileLayer = GetLayer(ActiveLayer);
        var bounds = new Rectangle(
            (int)Math.Round(position.X),
            (int)Math.Round(position.Y),
            width,
            height);

        // Find the edge tile positions
        int leftTile = (int)Math.Floor((float)bounds.Left / _tiledMap.TileWidth);
        int rightTile = (int)Math.Ceiling((float)bounds.Right / _tiledMap.TileWidth);
        int topTile = (int)Math.Floor((float)bounds.Top / _tiledMap.TileHeight);
        int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / _tiledMap.TileHeight);

        // Restrict the surrounding tiles to the map layer dimensions
        leftTile = (int)Math.Clamp(leftTile, 0, tileLayer.Width);
        rightTile = (int)Math.Clamp(rightTile, 0, tileLayer.Width);
        topTile = (int)Math.Clamp(topTile, 0, tileLayer.Height);
        bottomTile = (int)Math.Clamp(bottomTile, 0, tileLayer.Height);

        // Loop through each of the surrounding tiles        
        var tiles = new List<MapTile>();

        for (var row = topTile; row <= bottomTile; row++)
        {
            for (var column = leftTile; column <= rightTile; column++)
            {
                // Get the tile id
                var tileId = (int)GetTileAtPosition(row, column);

                // Skip if no tile...
                if (tileId == 0) continue;

                var tileBounds = GetRectangleAtMapPosition(row, column);

                if (bounds.Intersects(tileBounds))
                {
                    // Add the tile at this position
                    tiles.Add(new MapTile
                    {
                        Bounds = tileBounds,
                        Column = column,
                        Row = row,
                        TileClass = _tiledMap.Tilesets[ActiveTileset].Tiles.Where(x => x.ID == tileId - 1)?.Single().Type ?? null,
                        TileId = tileId - 1
                    });
                }
            }
        }

        return tiles;
    }

    /// <summary>
    /// Returns a rectangle with world coordinates for the specified map row/column
    /// </summary>    
    /// <param name="mapRow"></param>
    /// <param name="mapColumn"></param>
    /// <returns></returns>
    private Rectangle GetRectangleAtMapPosition(int mapRow, int mapColumn)
    {
        var tileWidth = (int)_tiledMap.TileWidth;
        var tileHeight = (int)_tiledMap.TileHeight;

        return new Rectangle(mapColumn * tileWidth, mapRow * tileHeight, tileWidth, tileHeight);
    }

    /// <summary>
    /// Returns the tile id for the specified layer at the specified map row/column position
    /// </summary>    
    /// <param name="mapRow"></param>
    /// <param name="mapColumn"></param>
    /// <returns></returns>
    private uint GetTileAtPosition(int mapRow, int mapColumn)
    {
        // Get the active map layer
        var tileLayer = GetLayer(ActiveLayer);

        // Out of bounds?
        if (mapColumn < 0 || mapRow < 0 || mapColumn >= tileLayer.Width || mapRow >= tileLayer.Height) return 0;

        // Calculate the index of the request tile in the map data
        var index = mapRow * tileLayer.Width + mapColumn;

        // Otherwise return the tile
        return tileLayer.Data.Value.GlobalTileIDs.Value[index];
    }

    /// <summary>
    /// Load a Tiled map
    /// </summary>
    /// <param name="tiledMapPath"></param>
    /// <param name="tileAtlasName"></param>
    public void LoadTiledMap(string tiledMapPath, string tileAtlasName)
    {
        var loader = Loader.Default();
        _tiledMap = loader.LoadMap(_contentManager.RootDirectory + "/" + tiledMapPath);
        _tilesetTexture = _contentManager.Load<Texture2D>(tileAtlasName);
    }
}
