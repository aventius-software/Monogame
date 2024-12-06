using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;

namespace DungeonMasterStyleDemo.Services;

internal enum MapRotationAngle
{
    None, Ninety, OneHundredAndEighty, TwoHundredAndSeventy
}

/// <summary>
/// A really rough dungeon (or maze?) map and map drawing service. You'd probably want
/// to improve things by maybe using either textures or sprites for walls instead of
/// coloured polygons. Also, everything is sized around the size of the viewport, so
/// you'd also probably want (maybe) to size things different so side/front walls look
/// similar in size depending on viewport size (or some other size)
/// </summary>
internal class DungeonMapService
{
    /// <summary>
    /// Current position in the map
    /// </summary>
    public Vector2 Position => _position;

    /// <summary>
    /// Get the current angle of rotation for the map
    /// </summary>
    /// <returns></returns>
    public MapRotationAngle RotationAngle => _rotationAngle;

    private int _activeLayer = 0;
    private readonly List<int> _blockingTileIDList = [];
    private readonly ContentManager _contentManager;
    private readonly int _depthPerspectiveBottomY = 20;
    private readonly int _depthPerspectiveTopY = 10;
    private readonly int _depthPerspectiveX = 40;
    private readonly int _depthToDraw = 6;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly int _horizontalBlocksToDraw = 12; // Should calculate this dynamically really
    private Vector2 _position = Vector2.Zero;
    private MapRotationAngle _rotationAngle = MapRotationAngle.None;
    private readonly ShapeDrawingService _shapeDrawingService;
    private Map _tiledMap;

    public DungeonMapService(ContentManager contentManager, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice)
    {
        _contentManager = contentManager;
        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    /// <summary>
    /// Add the id of tile to the list of blocking tile id's
    /// </summary>
    /// <param name="id"></param>
    public void AddBlockingTileID(int id)
    {
        if (!_blockingTileIDList.Contains(id)) _blockingTileIDList.Add(id);
    }

    /// <summary>
    /// Draw the dungeon
    /// </summary>
    public void Draw()
    {
        // First we draw the floor and ceiling first
        for (var depthOffset = _depthToDraw; depthOffset >= 0; depthOffset--)
        {
            DrawFloor(depthOffset);
            DrawCeiling(depthOffset);
        }

        // Now we draw the dungeon walls, draw from furthest away first, up to closest
        for (var distanceOffset = _depthToDraw; distanceOffset >= 0; distanceOffset--)
        {
            // For each depth layer (further to closer) we draw a horizontal line of 'walls'
            for (var mapOffsetX = -_horizontalBlocksToDraw; mapOffsetX <= _horizontalBlocksToDraw; mapOffsetX++)
            {
                // Get map 'offset' coordinates relative to our actual world/map position
                var currentPosition = GetNewPositionFromOffset(Position, mapOffsetX, -distanceOffset);
                var currentBlock = IsBlockingTile((int)currentPosition.X, (int)currentPosition.Y);

                var frontPosition = GetNewPositionFromOffset(Position, mapOffsetX, -distanceOffset - 1);
                var blockInFront = IsBlockingTile((int)frontPosition.X, (int)frontPosition.Y);

                var leftPosition = GetNewPositionFromOffset(Position, mapOffsetX - 1, -distanceOffset);
                var blockToTheLeft = IsBlockingTile((int)leftPosition.X, (int)leftPosition.Y);

                var rightPosition = GetNewPositionFromOffset(Position, mapOffsetX + 1, -distanceOffset);
                var blockToTheRight = IsBlockingTile((int)rightPosition.X, (int)rightPosition.Y);

                // If there is a block (wall) to the left of the player, draw a left side wall
                if (blockToTheLeft && mapOffsetX <= 0)
                {
                    DrawLeftSideWall(distanceOffset, mapOffsetX);
                }

                // If there is a block (wall) to the right of the player, draw a right side wall
                if (blockToTheRight && mapOffsetX >= 0)
                {
                    DrawRightSideWall(distanceOffset, mapOffsetX);
                }

                // If there is a block (wall) in front of the player draw a front facing wall
                if (blockInFront && !currentBlock)
                {
                    DrawFrontFacingWall(distanceOffset, mapOffsetX);
                }
            }
        }
    }

    /// <summary>
    /// Draws the ceiling of the dungeon
    /// </summary>
    /// <param name="depth"></param>
    private void DrawCeiling(int depth)
    {
        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(Color.Red, Color.DarkRed, GetDepthPercentage(depth));

        // Draw rectangle at this 'depth'
        _shapeDrawingService.DrawFilledRectangle(colour,
            0, depth * _depthPerspectiveTopY,
            _graphicsDevice.Viewport.Width, _depthPerspectiveTopY);
    }

    /// <summary>
    /// Draws the floor of the dungeon
    /// </summary>
    /// <param name="depth"></param>
    private void DrawFloor(int depth)
    {
        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(Color.Green, Color.DarkGreen, GetDepthPercentage(depth));

        // Draw rectangle at this 'depth'
        _shapeDrawingService.DrawFilledRectangle(colour,
            0, _graphicsDevice.Viewport.Height - (_depthPerspectiveBottomY + (_depthPerspectiveBottomY * depth)),
            _graphicsDevice.Viewport.Width, _depthPerspectiveBottomY);
    }

    /// <summary>
    /// Draw a front facing wall
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="offsetX"></param>
    private void DrawFrontFacingWall(int depth, int offsetX)
    {
        // Draw the wall of the block in front, not the
        // current block depth! ;-)
        depth++;

        // Scale the default wall rectangle down to the relevant
        // size for the depth we're drawing at        
        var scaledX = _depthPerspectiveX * depth;
        var scaledY = _depthPerspectiveTopY * depth;
        var scaledWidth = _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
        var scaledHeight = _graphicsDevice.Viewport.Height - ((_depthPerspectiveTopY + _depthPerspectiveBottomY) * depth);

        // Now we can create a new front facing wall with the correct
        // size we want for the depth we're drawing at. Also as we'll
        // need to draw front facing walls at different x coordinates
        // we'll use an 'offset' to draw wall faces at different multiples
        // of the scaled width depending on the value of this x offset
        var scaledWallFace = new Rectangle(
            x: scaledX + (offsetX * scaledWidth),
            y: scaledY,
            width: scaledWidth,
            height: scaledHeight
        );

        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(Color.LightBlue, Color.DarkBlue, GetDepthPercentage(depth));

        // Finally draw the scaled (and horizonally offset) front facing wall
        _shapeDrawingService.DrawFilledRectangle(colour, scaledWallFace.X, scaledWallFace.Y, scaledWallFace.Width, scaledWallFace.Height);
    }

    /// <summary>
    /// Draws a wall to the left
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="offsetX"></param>
    private void DrawLeftSideWall(int depth, int offsetX)
    {
        // Build 'default' quad coordinates, as if the wall
        // was just to the side of the player (not taking
        // distance/depth into account...
        var topLeft = new Vector2(0, 0);
        var topRight = new Vector2(_depthPerspectiveX, _depthPerspectiveTopY);
        var bottomRight = new Vector2(_depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveBottomY);
        var bottomLeft = new Vector2(0, _graphicsDevice.Viewport.Height);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X + (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveTopY * depth));
        var scaledTopRight = new Vector2(topRight.X + (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveTopY * depth));
        var scaledBottomRight = new Vector2(bottomRight.X + (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveBottomY * depth));
        var scaledBottomLeft = new Vector2(bottomLeft.X + (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveBottomY * depth));

        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(new Color(200, 200, 200), new Color(50, 50, 50), GetDepthPercentage(depth));
        var scaledWallWidth = GetBlockHorizontalWidth(depth);

        _shapeDrawingService.DrawFilledQuadrilateral(colour,
            (int)scaledTopLeft.X + (scaledWallWidth * offsetX),
            (int)scaledTopLeft.Y,
            (int)scaledTopRight.X + (scaledWallWidth * offsetX),
            (int)scaledTopRight.Y,
            (int)scaledBottomRight.X + (scaledWallWidth * offsetX),
            (int)scaledBottomRight.Y,
            (int)scaledBottomLeft.X + (scaledWallWidth * offsetX),
            (int)scaledBottomLeft.Y);
    }

    /// <summary>
    /// Draws a wall to the right
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="offsetX"></param>
    private void DrawRightSideWall(int depth, int offsetX)
    {
        // Build 'default' quad coordinates, as if the wall
        // was just to the side of the player (not taking
        // distance/depth into account...
        var topLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _depthPerspectiveTopY);
        var topRight = new Vector2(_graphicsDevice.Viewport.Width, 0);
        var bottomRight = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        var bottomLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveBottomY);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X - (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveTopY * depth));
        var scaledTopRight = new Vector2(topRight.X - (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveTopY * depth));
        var scaledBottomRight = new Vector2(bottomRight.X - (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveBottomY * depth));
        var scaledBottomLeft = new Vector2(bottomLeft.X - (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveBottomY * depth));

        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(new Color(200, 200, 200), new Color(50, 50, 50), GetDepthPercentage(depth));
        var scaledWallWidth = GetBlockHorizontalWidth(depth);

        _shapeDrawingService.DrawFilledQuadrilateral(colour,
            (int)scaledTopLeft.X + (scaledWallWidth * offsetX),
            (int)scaledTopLeft.Y,
            (int)scaledTopRight.X + (scaledWallWidth * offsetX),
            (int)scaledTopRight.Y,
            (int)scaledBottomRight.X + (scaledWallWidth * offsetX),
            (int)scaledBottomRight.Y,
            (int)scaledBottomLeft.X + (scaledWallWidth * offsetX),
            (int)scaledBottomLeft.Y);
    }

    /// <summary>
    /// Gets the horizontal width of a wall 'block' at the specified depth
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int GetBlockHorizontalWidth(int depth)
    {
        return _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
    }

    /// <summary>
    /// Returns a 0 to 1 value for how deep (depth) we are into the full draw depth
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    private float GetDepthPercentage(int depth)
    {
        return ((1f * depth) % (_depthToDraw + 2)) / (_depthToDraw + 2);
    }

    /// <summary>
    /// Returns the specified tile layer, or the first if no argument value is specified
    /// </summary>
    /// <param name="layerNumber"></param>
    /// <returns></returns>
    private TileLayer GetLayer(int layerNumber = 0) => (TileLayer)_tiledMap.Layers[layerNumber];

    /// <summary>
    /// Returns a new position relative to the specified position, calculated using the offsets for x and y
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    private Vector2 GetNewPositionFromOffset(Vector2 position, int offsetX, int offsetY)
    {
        var newX = (int)position.X;
        var newY = (int)position.Y;

        switch (_rotationAngle)
        {
            case MapRotationAngle.Ninety:
                {
                    // When the map is rotated 90 degrees clockwise then:
                    // x axis becomes the y axis
                    // y axis becomes the inverted x axis
                    newX -= offsetY;
                    newY += offsetX;
                }
                break;

            case MapRotationAngle.OneHundredAndEighty:
                {
                    // When the map is rotated 180 degrees clockwise then:
                    // x axis becomes inverted
                    // y axis also becomes inverted
                    newX -= offsetX;
                    newY -= offsetY;
                }
                break;

            case MapRotationAngle.TwoHundredAndSeventy:
                {
                    // When the map is rotated 270 degrees clockwise then:
                    // x axis becomes the inverted y axis
                    // y axis becomes the x axis
                    newX += offsetY;
                    newY -= offsetX;
                }
                break;

            case MapRotationAngle.None:
                {
                    // No rotation, so just add the offsets
                    newX += offsetX;
                    newY += offsetY;
                }
                break;

            default: break;
        }

        return new Vector2(newX, newY);
    }

    /// <summary>
    /// Returns the tile id at the current map position
    /// </summary>
    /// <returns></returns>
    public int GetTileAtPosition() => GetTileAtPosition((int)_position.X, (int)_position.Y);

    /// <summary>
    /// Returns the tile id for the specified layer at the specified map position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetTileAtPosition(Vector2 position) => GetTileAtPosition((int)position.X, (int)position.Y);

    /// <summary>
    /// Returns the tile id for the specified layer at the specified map position
    /// </summary>    
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetTileAtPosition(int x, int y)
    {
        // Get the active map layer
        var tileLayer = GetLayer(_activeLayer);

        // Out of bounds?
        if (x < 0 || y < 0 || x >= tileLayer.Width || y >= tileLayer.Height) return 0;

        // Calculate the index of the request tile in the map data
        var index = (y * tileLayer.Width) + x;

        // Otherwise return the tile
        return (int)tileLayer.Data.Value.GlobalTileIDs.Value[index];
    }

    /// <summary>
    /// Get the tile above the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileAbove(int tileOffset = 1)
    {
        return GetTileAtPosition(GetNewPositionFromOffset(_position, 0, -tileOffset));
    }

    /// <summary>
    /// Get the tile below the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileBelow(int tileOffset = 1)
    {
        return GetTileAtPosition(GetNewPositionFromOffset(_position, 0, tileOffset));
    }

    /// <summary>
    /// Get the tile to the left of the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileToTheLeft(int tileOffset = 1)
    {
        return GetTileAtPosition(GetNewPositionFromOffset(_position, -tileOffset, 0));
    }

    /// <summary>
    /// Get the tile to the right of the current position, optionally offset by the specified number of tiles
    /// </summary>
    /// <param name="tileOffset"></param>
    /// <returns></returns>
    public int GetTileToTheRight(int tileOffset = 1)
    {
        return GetTileAtPosition(GetNewPositionFromOffset(_position, tileOffset, 0));
    }

    /// <summary>
    /// Returns true if the tile above the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedAbove()
    {
        return IsBlockingTile(GetTileAbove());
    }

    /// <summary>
    /// Returns true if the tile below the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedBelow()
    {
        return IsBlockingTile(GetTileBelow());
    }

    /// <summary>
    /// Returns true if the tile to the left of the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedToTheLeft()
    {
        return IsBlockingTile(GetTileToTheLeft());
    }

    /// <summary>
    /// Returns true if the tile to the right of the current position is a blocking tile
    /// </summary>
    /// <returns></returns>
    public bool IsBlockedToTheRight()
    {
        return IsBlockingTile(GetTileToTheRight());
    }

    /// <summary>
    /// Returns true if the specified tile ID is a blocking tile ID
    /// </summary>
    /// <param name="tileID"></param>
    /// <returns></returns>
    private bool IsBlockingTile(int tileID) => _blockingTileIDList.Contains(tileID);

    /// <summary>
    /// Returns true if the specified tile at the specified map position is a blocking tile
    /// </summary>
    /// <param name="tileID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsBlockingTile(int x, int y) => IsBlockingTile(GetTileAtPosition(x, y));

    /// <summary>
    /// Load a Tiled map for the dungeon
    /// </summary>
    /// <param name="tiledMapPath"></param>    
    public void LoadDungeonTiledMap(string tiledMapPath)
    {
        var loader = Loader.Default();
        _tiledMap = loader.LoadMap(_contentManager.RootDirectory + "/" + tiledMapPath);
    }

    /// <summary>
    /// Move down from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveDown(int numberOfTiles = 1)
    {
        MoveTo(GetNewPositionFromOffset(_position, 0, numberOfTiles));
    }

    /// <summary>
    /// Move left from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveLeft(int numberOfTiles = 1)
    {
        MoveTo(GetNewPositionFromOffset(_position, -numberOfTiles, 0));
    }

    /// <summary>
    /// Move right from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveRight(int numberOfTiles = 1)
    {
        MoveTo(GetNewPositionFromOffset(_position, numberOfTiles, 0));
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
    /// Move to the specified position in the map
    /// </summary>
    /// <param name="position"></param>
    public void MoveTo(Vector2 position)
    {
        if (!IsBlockingTile((int)position.X, (int)position.Y)) _position = position;
    }

    /// <summary>
    /// Move up from the current position by the specified number of tiles
    /// </summary>
    /// <param name="numberOfTiles"></param>
    public void MoveUp(int numberOfTiles = 1)
    {
        MoveTo(GetNewPositionFromOffset(_position, 0, -numberOfTiles));
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
}
