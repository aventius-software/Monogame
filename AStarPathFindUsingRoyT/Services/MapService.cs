using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AStarPathFindUsingRoyT.Services;

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
    /// Returns all the tiles in the map
    /// </summary>
    public int[,] Tiles
    {
        get
        {
            var tileLayer = GetLayer(ActiveLayer);
            var numberOfColumns = tileLayer.Width;
            var numberOfRows = tileLayer.Height;            
            var rows = new int[numberOfRows, numberOfColumns];

            for (var row = 0; row < tileLayer.Height; row++) 
            {
                for (var col = 0; col < tileLayer.Width; col++)
                {
                    rows[row, col] = (int)GetTileAtPosition(row, col);
                }
            }

            return rows;
        }
    }

    public int TileHeight => (int)_tiledMap.TileHeight;

    public int TileWidth => (int)_tiledMap.TileWidth;

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

        var row = tileId / ((int)tileset.TileCount / (int)tileset.Columns);
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
    /// Returns the tile id for the specified layer at the specified map row/column position
    /// </summary>    
    /// <param name="mapRow"></param>
    /// <param name="mapColumn"></param>
    /// <returns></returns>
    private uint GetTileAtPosition(int mapRow, int mapColumn)
    {
        var tileLayer = GetLayer(ActiveLayer);

        // Calculate the index of the request tile in the map data
        var index = (mapRow * tileLayer.Width) + mapColumn;

        // If the index is out of bounds then just return 'no tile' (i.e. 0)
        if (index >= tileLayer.Data.Value.GlobalTileIDs.Value.Length - 1 || index < 0) return 0;

        // Otherwise return the tile
        return tileLayer.Data.Value.GlobalTileIDs.Value[(mapRow * tileLayer.Width) + mapColumn];
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

    /// <summary>
    /// Set details about the viewport and current position (e.g. of a camera). Setting this
    /// allows the world drawing to restrict the tile drawing to only the tiles visible at the
    /// specified position within the viewport area. Not calling this will result in the draw
    /// method just drawing all tiles in the map
    /// </summary>
    /// <param name="position">Relevant drawing position in the world</param>
    /// <param name="viewPortWidth">Width of the viewport</param>
    /// <param name="viewPortHeight">Height of the viewport</param>
    public void SetViewport(Vector2 position, int viewPortWidth, int viewPortHeight)
    {
        // Get the world position, offset to the centre of the viewport
        var worldX = (int)Math.Floor(position.X) - (viewPortWidth / 2);
        var worldY = (int)Math.Floor(position.Y) - (viewPortHeight / 2);

        // Get the current position in the world, but in tile position not world/pixels
        _tileColumnPositionInTheWorld = worldX / (int)_tiledMap.TileWidth;
        _tileRowPositionInTheWorld = worldY / (int)_tiledMap.TileHeight;

        // Calculate how many tiles are visible        
        _numberOfVisibleTileColumns = viewPortWidth / (int)_tiledMap.TileWidth;
        _numberOfVisibleTileRows = viewPortHeight / (int)_tiledMap.TileHeight;
    }
}
