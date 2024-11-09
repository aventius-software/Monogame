using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarioPlatformerStyleTest.Services;

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
    /// The world height in pixels
    /// </summary>
    public int WorldHeight => (int)_tiledMap.Height * (int)_tiledMap.TileHeight;

    /// <summary>
    /// The world width in pixels
    /// </summary>
    public int WorldWidth => (int)_tiledMap.Width * (int)_tiledMap.TileWidth;

    public MapService(SpriteBatch spriteBatch, ContentManager contentManager)
    {
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
    }

    /// <summary>
    /// Draw the current map on screen
    /// </summary>
    public void Draw()
    {
        // Get the first layer and the tileset            
        var tileset = _tiledMap.Tilesets[0];
        var firstLayer = (TileLayer)_tiledMap.Layers[0];

        // Calculate how many rows/columns we should draw, if no values have been
        // specified then we just draw all rows and columns
        var rowsToDraw = _numberOfVisibleTileRows == 0 ? (int)firstLayer.Height : _numberOfVisibleTileRows;
        var colsToDraw = _numberOfVisibleTileColumns == 0 ? (int)firstLayer.Width : _numberOfVisibleTileColumns;

        // We 'overdraw' the tiles so we end up drawing a tile offscreen to that a
        // tile doesn't just 'flicker' into existance at the edges of the screen
        if (_numberOfVisibleTileRows > 0 || _numberOfVisibleTileColumns > 0)
        {
            var tilesToOverDraw = 2;
            rowsToDraw += tilesToOverDraw;
            colsToDraw += tilesToOverDraw;
        }

        // Draw the relevant tiles on screen
        for (int row = _tileRowPositionInTheWorld; row < _tileRowPositionInTheWorld + rowsToDraw; row++)
        {
            for (int column = _tileColumnPositionInTheWorld; column < _tileColumnPositionInTheWorld + colsToDraw; column++)
            {
                // Don't bother if the row/column position is out of bounds of the map
                if (column < 0 || row < 0 || column >= firstLayer.Width || row >= firstLayer.Height) continue;

                // Otherwise lets find out which tile in the map is at the current row/column position
                var tile = firstLayer.Data.Value.GlobalTileIDs.Value[(row * firstLayer.Width) + column];

                // If block is 0, i.e. air, then continue...
                if (tile == 0) continue;

                // Get the correct tile 'image' rectangle from the tileset
                var sourceRectangle = GetSourceRect(tileset, tile);

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
    /// Helper method to work out the source rectangle for the specified tile
    /// </summary>
    /// <param name="tileset"></param>
    /// <param name="gid"></param>
    /// <returns></returns>
    private static Rectangle GetSourceRect(Tileset tileset, uint gid)
    {
        var tileId = (int)gid - 1;

        var row = tileId / ((int)tileset.TileCount / (int)tileset.Columns);
        var column = tileId % (int)tileset.Columns;

        var tileWidth = (int)tileset.TileWidth;
        var tileHeight = (int)tileset.TileHeight;
        var x = tileWidth * column;
        var y = tileHeight * row;

        return new Rectangle(x, y, tileWidth, tileHeight);
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
