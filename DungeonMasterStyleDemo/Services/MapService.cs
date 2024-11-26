using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DungeonMasterStyleDemo.Services;

internal enum MapRotationAngle
{
    None,
    Ninety,
    OneHundredAndEighty,
    TwoHundredAndSeventy
}

/// <summary>
/// A basic tile map management and rendering service for use with 'Tiled' maps
/// </summary>
internal class MapService
{
    private readonly List<int> _blockingTileIDList = [];
    private readonly ContentManager _contentManager;
    private int _drawOffset = 200;
    private int _numberOfVisibleTileColumns;
    private int _numberOfVisibleTileRows;
    private readonly Vector2 _outOfBounds = new(-1, -1);
    private Vector2 _position = Vector2.Zero;
    private MapRotationAngle _rotationAngle = MapRotationAngle.None;
    private readonly SpriteBatch _spriteBatch;
    private Map _tiledMap;
    private Texture2D _tilesetTexture;

    /// <summary>
    /// Gets or sets the active map layer to use
    /// </summary>
    public int ActiveLayer { get; set; } = 0;

    /// <summary>
    /// Gets or sets the active tileset to use
    /// </summary>
    public int ActiveTileset { get; set; } = 0;

    /// <summary>
    /// Current position in the map
    /// </summary>
    public Vector2 Position => _position;// GetRotatedMapPosition((int)_position.X, (int)_position.Y);
    public Vector2 RotatedPosition => GetRotatedMapPosition((int)_position.X, (int)_position.Y);
    /// <summary>
    /// Get the current angle of rotation for the map
    /// </summary>
    /// <returns></returns>
    public MapRotationAngle RotationAngle => _rotationAngle;

    /// <summary>
    /// The world height (in pixels)
    /// </summary>
    public int WorldHeightInPixels => (int)_tiledMap.Height * (int)_tiledMap.TileHeight;

    /// <summary>
    /// The world height (in tiles)
    /// </summary>
    public int WorldHeightInTiles => (int)_tiledMap.Height;

    /// <summary>
    /// The world width (in pixels)
    /// </summary>
    public int WorldWidthInPixels => (int)_tiledMap.Width * (int)_tiledMap.TileWidth;

    /// <summary>
    /// The world width (in tiles)
    /// </summary>
    public int WorldWidthInTiles => (int)_tiledMap.Width;

    public MapService(SpriteBatch spriteBatch, ContentManager contentManager)
    {
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
    }

    /// <summary>
    /// Add the id of tile to the list of blocking tile id's
    /// </summary>
    /// <param name="id"></param>
    public void AddBlockingTileID(int id)
    {
        if (_blockingTileIDList.Contains(id)) return;
        _blockingTileIDList.Add(id);
    }

    /// <summary>
    /// Draw the current map (using the active layer)
    /// </summary>
    public virtual void Draw()
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
        var tilesToOverDraw = 2;

        // Draw the relevant tiles on screen
        for (int column = -tilesToOverDraw; column < colsToDraw + (2 * tilesToOverDraw); column++)
        {
            for (int row = -tilesToOverDraw; row < rowsToDraw + (2 * tilesToOverDraw); row++)
            {
                // Calculate the tile position to fetch/draw                
                var x = (int)_position.X + column;
                var y = (int)_position.Y + row;

                // Draw it
                DrawTile(x, y, new Vector2(column * tileset.TileWidth, row * tileset.TileHeight));
            }
        }
    }

    /// <summary>
    /// Draws the map tile from the specified map coordinates at the specified position on screen
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="drawAtPositionOnScreen"></param>
    public void DrawTile(int x, int y, Vector2 drawAtPositionOnScreen)
    {
        // Find the tile at the specified position in the map
        var tile = GetTileAtPosition(x, y);

        // If block is 0, i.e. air or nothing, then just continue...
        if (tile != 0)
        {
            // Get the correct tile 'image' rectangle from the tileset
            var sourceRectangle = GetImageSourceRectangleForTile(tile);

            // Draw this tile
            _spriteBatch.Draw(
                texture: _tilesetTexture,
                position: drawAtPositionOnScreen + new Vector2(_drawOffset, _drawOffset),
                sourceRectangle: sourceRectangle,
                color: Microsoft.Xna.Framework.Color.White);
        }
    }

    /// <summary>
    /// Helper method to work out the source rectangle for the specified tile so we can
    /// pick out the correct texture to use when drawing the tile
    /// </summary>    
    /// <param name="gid"></param>
    /// <returns></returns>
    private Rectangle GetImageSourceRectangleForTile(int gid)
    {
        var tileId = gid - 1;
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
    /// Gets a list of rectangles for tiles that surround the specified rectangle
    /// </summary>
    /// <param name="tileRectangle"></param>
    /// <returns></returns>
    public List<Rectangle> GetSurroundingTileRectangles(Rectangle tileRectangle)
    {
        return GetSurroundingTileRectangles(new Vector2(tileRectangle.X, tileRectangle.Y), tileRectangle.Width, tileRectangle.Height);
    }

    /// <summary>
    /// Gets a list of rectangles for tiles that surround the specified world position and width/height
    /// </summary>
    /// <param name="position"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public List<Rectangle> GetSurroundingTileRectangles(Vector2 position, int width, int height)
    {
        // Create a list of rectangles
        var tiles = new List<Rectangle>();
        var tileLayer = GetLayer(ActiveLayer);
        var tileWidth = (int)_tiledMap.TileWidth;
        var tileHeight = (int)_tiledMap.TileHeight;

        // Find the edge tile positions
        var leftTile = (int)Math.Floor(position.X / tileWidth) - 1;
        var rightTile = (int)Math.Ceiling((position.X + width) / tileWidth) + 1;
        var topTile = (int)Math.Floor(position.Y / tileHeight) - 1;
        var bottomTile = (int)Math.Ceiling((position.Y + height) / tileHeight) + 1;

        // Restrict the surrounding tiles to the map layer dimensions
        leftTile = (int)Math.Clamp(leftTile, 0, tileLayer.Width);
        rightTile = (int)Math.Clamp(rightTile, 0, tileLayer.Width);
        topTile = (int)Math.Clamp(topTile, 0, tileLayer.Height);
        bottomTile = (int)Math.Clamp(bottomTile, 0, tileLayer.Height);

        // Loop through each of the surrounding tiles
        for (var y = topTile; y <= bottomTile; y++)
        {
            for (var x = leftTile; x <= rightTile; x++)
            {
                // Find the tile at this position
                var tile = GetTileAtPosition(x, y);

                // Only add a rectangle if there IS a tile...                
                if (tile != 0) tiles.Add(GetRectangleAtMapPosition(x, y));
            }
        }

        return tiles;
    }

    /// <summary>
    /// Returns a rectangle with world coordinates for the specified map position
    /// </summary>    
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Rectangle GetRectangleAtMapPosition(int x, int y)
    {
        var tileWidth = (int)_tiledMap.TileWidth;
        var tileHeight = (int)_tiledMap.TileHeight;

        return new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
    }

    /// <summary>
    /// Gets the map position for x and y, taking map rotation into account
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Vector2 GetRotatedMapPosition(int x, int y)
    {
        // Get the active map layer
        var tileLayer = GetLayer(ActiveLayer);

        // Out of bounds?
        if (x < 0 || y < 0 || x >= tileLayer.Width || y >= tileLayer.Height) return _outOfBounds;

        // We'll assign to new variables as we may modify due to rotation
        var newX = x;
        var newY = y;

        var mapWidth = (int)tileLayer.Width - 1;
        var mapHeight = (int)tileLayer.Height - 1;

        // Handle any requested rotation
        switch (_rotationAngle)
        {
            case MapRotationAngle.Ninety:
                {
                    // When the map is rotated 90 degrees clockwise then:
                    // x axis becomes the y axis
                    // y axis becomes the inverted x axis
                    newX = y;
                    newY = mapWidth - x;
                }
                break;

            case MapRotationAngle.OneHundredAndEighty:
                {
                    // When the map is rotated 180 degrees clockwise then:
                    // x axis becomes inverted
                    // y axis also becomes inverted
                    newX = mapWidth - x;
                    newY = mapHeight - y;
                }
                break;

            case MapRotationAngle.TwoHundredAndSeventy:
                {
                    // When the map is rotated 270 degrees clockwise then:
                    // x axis becomes the inverted y axis
                    // y axis becomes the x axis
                    newX = mapHeight - y;
                    newY = x;
                }
                break;

            case MapRotationAngle.None:
            default: break;
        }

        // Out of bounds?
        if (newX < 0 || newY < 0 || newX >= tileLayer.Width || newY >= tileLayer.Height) return _outOfBounds;

        return new Vector2(newX, newY);
    }

    /// <summary>
    /// Returns the tile id at the current map position
    /// </summary>
    /// <returns></returns>
    public int GetTileAtPosition() => GetTileAtPosition((int)_position.X, (int)_position.Y);

    /// <summary>
    /// Returns the tile id for the specified layer at the specified map row/column position
    /// </summary>    
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetTileAtPosition(int x, int y)
    {
        // Get the new position
        var position = GetRotatedMapPosition(x, y);

        // Out of bounds?
        if (position == _outOfBounds) return 0;

        // Get the active map layer
        var tileLayer = GetLayer(ActiveLayer);

        // Calculate the index of the request tile in the map data
        var index = ((int)position.Y * tileLayer.Width) + (int)position.X;

        // Otherwise return the tile
        return (int)tileLayer.Data.Value.GlobalTileIDs.Value[index];
    }

    /// <summary>
    /// Gets the tile at the offset from the current position
    /// </summary>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    public int GetTileAtOffsetFromCurrentPosition(int offsetX, int offsetY) => GetTileAtPosition((int)_position.X + offsetX, (int)_position.Y + offsetY);

    /// <summary>
    /// Get the tile above the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileAbove(int tileOffset = 1) => GetTileAtOffsetFromCurrentPosition(0, -tileOffset);

    /// <summary>
    /// Get the tile below the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileBelow(int tileOffset = 1) => GetTileAtOffsetFromCurrentPosition(0, tileOffset);

    /// <summary>
    /// Get the tile to the left of the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileToTheLeft(int tileOffset = 1) => GetTileAtOffsetFromCurrentPosition(-tileOffset, 0);

    /// <summary>
    /// Get the tile to the right of the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileToTheRight(int tileOffset = 1) => GetTileAtOffsetFromCurrentPosition(tileOffset, 0);

    /// <summary>
    /// Returns true if the tile above the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedAbove()
    {
        var numberOfTiles = 1;
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving up becomes moving left in the map
            case MapRotationAngle.Ninety:
                newX -= numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving up becomes moving down in the map
            case MapRotationAngle.OneHundredAndEighty:
                newY += numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving up is moving right in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newX += numberOfTiles;
                break;

            // For no rotation, up is just up in the map
            case MapRotationAngle.None:
            default:
                newY -= numberOfTiles;
                break;
        };

        return IsBlockingTile(GetTileAtPosition(newX, newY));
    }

    /// <summary>
    /// Returns true if the tile below the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedBelow()
    {
        var numberOfTiles = 1;
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving down becomes moving right in the map
            case MapRotationAngle.Ninety:
                newX += numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving down becomes moving up in the map
            case MapRotationAngle.OneHundredAndEighty:
                newY -= numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving down becomes moving left in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newX -= numberOfTiles;
                break;

            // For no rotation, down is just down in the map
            case MapRotationAngle.None:
            default:
                newY += numberOfTiles;
                break;
        };

        return IsBlockingTile(GetTileAtPosition(newX, newY));
    }

    /// <summary>
    /// Returns true if the tile to the left of the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedToTheLeft()
    {
        var numberOfTiles = 1;
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving left becomes moving up in the map
            case MapRotationAngle.Ninety:
                newY -= numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving left becomes moving right in the map
            case MapRotationAngle.OneHundredAndEighty:
                newX += numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving left becomes moving down in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newY += numberOfTiles;
                break;

            // For no rotation, left is just left in the map
            case MapRotationAngle.None:
            default:
                newX -= numberOfTiles;
                break;
        };

        return IsBlockingTile(GetTileAtPosition(newX, newY));
    }

    /// <summary>
    /// Returns true if the tile to the right of the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedToTheRight()
    {
        var numberOfTiles = 1;
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving right becomes moving down in the map
            case MapRotationAngle.Ninety:
                newY += numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving right becomes moving left in the map
            case MapRotationAngle.OneHundredAndEighty:
                newX -= numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving right becomes moving up in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newY -= numberOfTiles;
                break;

            // For no rotation, right is just right in the map
            case MapRotationAngle.None:
            default:
                newX += numberOfTiles;
                break;
        };

        return IsBlockingTile(GetTileAtPosition(newX, newY));
    }

    /// <summary>
    /// Returns true if the specified tile ID is a blocking tile ID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public bool IsBlockingTile(int tileID) => _blockingTileIDList.Contains(tileID);

    /// <summary>
    /// Returns true if the specified tile at the specified map position is a blocking tile
    /// </summary>
    /// <param name="tileID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsBlockingTile(int x, int y) => IsBlockingTile(GetTileAtPosition(x, y));

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
    /// Move down from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveDown(int numberOfTiles = 1)
    {
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving down becomes moving right in the map
            case MapRotationAngle.Ninety:
                newX += numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving down becomes moving up in the map
            case MapRotationAngle.OneHundredAndEighty:
                newY -= numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving down becomes moving left in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newX -= numberOfTiles;
                break;

            // For no rotation, down is just down in the map
            case MapRotationAngle.None:
            default:
                newY += numberOfTiles;
                break;
        };

        // Adjust the position
        MoveTo(newX, newY);
    }

    /// <summary>
    /// Move left from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveLeft(int numberOfTiles = 1)
    {
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving left becomes moving up in the map
            case MapRotationAngle.Ninety:
                newY -= numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving left becomes moving right in the map
            case MapRotationAngle.OneHundredAndEighty:
                newX += numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving left becomes moving down in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newY += numberOfTiles;
                break;

            // For no rotation, left is just left in the map
            case MapRotationAngle.None:
            default:
                newX -= numberOfTiles;
                break;
        };

        // Adjust the position
        MoveTo(newX, newY);
    }

    /// <summary>
    /// Move right from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveRight(int numberOfTiles = 1)
    {
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving right becomes moving down in the map
            case MapRotationAngle.Ninety:
                newY += numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving right becomes moving left in the map
            case MapRotationAngle.OneHundredAndEighty:
                newX -= numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving right becomes moving up in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newY -= numberOfTiles;
                break;

            // For no rotation, right is just right in the map
            case MapRotationAngle.None:
            default:
                newX += numberOfTiles;
                break;
        };

        // Adjust the position
        MoveTo(newX, newY);
    }

    /// <summary>
    /// Move to the specified position in the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void MoveTo(int x, int y)
    {
        if (!IsBlockingTile(x, y)) _position = new Vector2(x, y);
    }

    /// <summary>
    /// Move up from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveUp(int numberOfTiles = 1)
    {
        var newX = (int)_position.X;
        var newY = (int)_position.Y;

        switch (_rotationAngle)
        {
            // When the map is rotated 90 degrees clockwise then:
            // x axis becomes the y axis
            // y axis becomes the inverted x axis
            // So moving up becomes moving left in the map
            case MapRotationAngle.Ninety:
                newX -= numberOfTiles;
                break;

            // When the map is rotated 180 degrees clockwise then:
            // x axis becomes inverted
            // y axis also becomes inverted
            // So moving up becomes moving down in the map
            case MapRotationAngle.OneHundredAndEighty:
                newY += numberOfTiles;
                break;

            // When the map is rotated 270 degrees clockwise then:
            // x axis becomes the inverted y axis
            // y axis becomes the x axis
            // So moving up is moving right in the map
            case MapRotationAngle.TwoHundredAndSeventy:
                newX += numberOfTiles;
                break;

            // For no rotation, up is just up in the map
            case MapRotationAngle.None:
            default:
                newY -= numberOfTiles;
                break;
        };

        // Adjust the position
        MoveTo(newX, newY);
    }

    /// <summary>
    /// Rotates the map anticlockwise 90 degrees from its current angle of rotation
    /// </summary>
    public void RotateAnticlockwise()
    {
        switch (_rotationAngle)
        {
            case MapRotationAngle.None:
                _rotationAngle = MapRotationAngle.TwoHundredAndSeventy;
                break;
            case MapRotationAngle.Ninety:
                _rotationAngle = MapRotationAngle.None;
                break;
            case MapRotationAngle.OneHundredAndEighty:
                _rotationAngle = MapRotationAngle.Ninety;
                break;
            case MapRotationAngle.TwoHundredAndSeventy:
                _rotationAngle = MapRotationAngle.OneHundredAndEighty;
                break;
        };
    }

    /// <summary>
    /// Rotates the map clockwise 90 degrees from its current angle of rotation
    /// </summary>
    public void RotateClockwise()
    {
        switch (_rotationAngle)
        {
            case MapRotationAngle.None:
                _rotationAngle = MapRotationAngle.Ninety;
                break;
            case MapRotationAngle.Ninety:
                _rotationAngle = MapRotationAngle.OneHundredAndEighty;
                break;
            case MapRotationAngle.OneHundredAndEighty:
                _rotationAngle = MapRotationAngle.TwoHundredAndSeventy;
                break;
            case MapRotationAngle.TwoHundredAndSeventy:
                _rotationAngle = MapRotationAngle.None;
                break;
        };
    }

    /// <summary>
    /// Set the rotation angle in increments of 90 degrees, affects the drawing 
    /// of the map and retrieval of map tiles data from the map
    /// </summary>
    /// <param name="rotationAngle"></param>
    public void SetRotationAngle(MapRotationAngle rotationAngle)
    {
        _rotationAngle = rotationAngle;
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
        _position.X = worldX / (int)_tiledMap.TileWidth;
        _position.Y = worldY / (int)_tiledMap.TileHeight;

        // Calculate how many tiles are visible        
        _numberOfVisibleTileColumns = viewPortWidth / (int)_tiledMap.TileWidth;
        _numberOfVisibleTileRows = viewPortHeight / (int)_tiledMap.TileHeight;
    }
}
