using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace IsometricTiledMapDemo.Services;

/// <summary>
/// A basic 2D isometric tile map service that uses Tiled maps, credits for the art and
/// tile sprites go to Clint Bellanger (they were downloaded from opengameart.org 
/// here https://opengameart.org/content/terrain-renderer which also has other 
/// colours and instructions on how to make others using Blender). Check out Clint's
/// own site for other interesting art/code tutorials here https://clintbellanger.net/
/// 
/// https://stackoverflow.com/questions/21842814/mouse-position-to-isometric-tile-including-height
/// </summary>
internal class IsometricTiledMapService
{
    public Point Origin { get; set; } = Point.Zero;
    public int TileMapDepth => _tileMap.GetLength(0);
    public int TileMapHeight => _tileMap.GetLength(2);
    public int TileMapWidth => _tileMap.GetLength(1);    
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }

    private readonly ContentManager _contentManager;
    private Vector3 _selectedTile;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _texture;
    private readonly int _tileBlockHeight;
    private readonly int _tileBlockWidth;
    private readonly int[,,] _tileMap;
    private Matrix _translationMatrix;
    private Matrix _transformationMatrixInverted;

    public IsometricTiledMapService(ContentManager contentManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;

        _texture = _contentManager.Load<Texture2D>("tile");

        // We're using a tile block sprite instead of a flat 'diamond' tile
        _tileBlockWidth = _texture.Width;
        _tileBlockHeight = _texture.Height;

        _tileMap = new int[,,]
        {
            {
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
            },
            {
                { 1,1,0,0,1 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 }
            },
            {
                { 0,0,0,0,1 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,1,0 },
                { 0,0,0,0,0 }
            }
        };

        WorldWidth = _tileMap.GetLength(0) * _tileBlockWidth;
        WorldHeight = _tileMap.GetLength(1) * _tileBlockHeight;

        // Get dimensions for a 'flat' version of our tile block
        var w = _tileBlockWidth / 2;
        var h = _tileBlockHeight / 4;

        // We'll use a matrix and its inverse to translate coordinates. See the link below for some of the maths behind this
        // https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
        _translationMatrix = new Matrix(
            m11: w, m21: -w, m31: 0, m41: 0,
            m12: h, m22: h, m32: 0, m42: 0,
            m13: 0, m23: 0, m33: 1, m43: 0,
            m14: 0, m24: 0, m34: 0, m44: 1
        );

        // Get the inverse of the matrix to translate coordinates back
        _transformationMatrixInverted = Matrix.Invert(_translationMatrix);
    }

    /// <summary>
    /// Draw the map
    /// </summary>
    public void Draw()
    {
        for (int elevation = 0; elevation < TileMapDepth; elevation++)
        {
            for (int y = 0; y < TileMapHeight; y++)
            {
                for (int x = 0; x < TileMapWidth; x++)
                {
                    // Get the tile at this map position
                    var tile = _tileMap[elevation, x, y];

                    // If there is a tile here and not empty space...
                    if (tile == 1)
                    {
                        // By default, use white
                        var colour = Color.White;

                        // If there is a 'selected' tile at these coordinates...
                        if ((int)_selectedTile.X == x && (int)_selectedTile.Y == y && _selectedTile.Z == elevation)
                        {
                            // Highlight it in red
                            colour = Color.Red;
                        }

                        // Draw the map tile at this position and elevation
                        _spriteBatch.Draw(
                            texture: _texture,
                            position: MapToScreenCoordinates(new Point(x, y), elevation).ToVector2(),
                            color: colour);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Highlights the tile at the specified screen coordinates
    /// </summary>
    /// <param name="screenCoordinates"></param>
    /// <returns></returns>
    public Vector3 HighlightTile(Point screenCoordinates)
    {
        _selectedTile = ScreenToMapCoordinates(screenCoordinates);

        return _selectedTile;
    }

    /// <summary>
    /// Translates map coordinates to screen coordinates
    /// </summary>
    /// <param name="mapCoordinates"></param>
    /// <param name="elevation"></param>
    /// <returns></returns>
    public Point MapToScreenCoordinates(Point mapCoordinates, int elevation)
    {
        // Use the matrix to transform the map coordinates into screen coordinates, not forgetting
        // to add the map origin point so the map is in the correct screen position
        var screenCoordinates = Vector2.Transform(mapCoordinates.ToVector2(), _translationMatrix) + Origin.ToVector2();

        // In isometric maps the origin point is at the middle of a tile, so to correct this
        // we offset the x coordinate by half a tile width
        screenCoordinates.X -= _tileBlockWidth / 2;

        // As we're using a tile block instead of a flat tile the block is half the height
        // of the actual sprite, so for elevation we just need multiples of this value depending
        // on the elevation level (e.g. 1,2,3 and so on...)
        screenCoordinates.Y -= elevation * (_tileBlockHeight / 2);

        // Finally, as we're not concerned with floating points for the screen
        // we return the value as a Point...
        return screenCoordinates.ToPoint();
    }

    /// <summary>
    /// Translates screen coordinates to map coordinates, taking tile elevation into account
    /// </summary>
    /// <param name="screenCoordinates"></param>
    /// <returns></returns>
    public Vector3 ScreenToMapCoordinates(Point screenCoordinates)
    {
        // Adjust the screen coordinates for the map origin
        screenCoordinates -= Origin;

        // Translate to X,Y map position as if it were a flat map with no elevation
        var flatMapCoordinates = Vector3.Transform(new Vector3(screenCoordinates.ToVector2(), 0), _transformationMatrixInverted);

        // When at a negative position in the map, the tile at say map coordinates (-1, 0) will be ranges
        // 0 to -1 from the screen coordinate translation. The problem with this is that when the calculation
        // returns say -0.138 or -0.853, this is rounded down to 0. So only until the screen coordinates move
        // further passed or equal to -1 is the tile at (-1, 0) actually what we end up with. So adjust for 
        // this we round negative values so that -0.3 or -0.7 to -1, that way the screen coordinates which are
        // inside the tile at (-1, 0) get translated to (-1, 0) instead of (0, 0)
        if (flatMapCoordinates.X < 0) flatMapCoordinates.X = (int)Math.Floor(flatMapCoordinates.X);
        if (flatMapCoordinates.Y < 0) flatMapCoordinates.Y = (int)Math.Floor(flatMapCoordinates.Y);

        // So far so good, but at this point we're effectively looking at the tile map where elevation (Z) is
        // zero (i.e. a flat map with no hills or elevation). However, if other tiles are at further coordinates than
        // where we currently are, but have higher elevation (Z) position then those tiles will appear at the same 'screen'
        // position as the tile at our current calculated map X,Y position. How do we deal with this? We need to start close
        // to the 'camera' position and check if a tile exists at an elevation which makes it appears at the same screen
        // coordinates. If none is found we work 'backwards' towards our original X,Y position checking tiles at elevations
        // that would make them appear to be at the same screen coordinates. As soon as we find one closest to the 'camera'
        // we return that tile as the tile at the screen coordinates, if none are found then we'll eventually get to our
        // original starting position and just return that. Note that we only need to check elevations from the map depth
        // down to 1 (we don't need to check elevation 0 as that is where our starting position was at)
        for (var z = TileMapDepth - 1; z >= 1; z--)
        {
            // Move to the current offset position
            var offsetPosition = flatMapCoordinates + new Vector3(z, z, 0);

            // If we're out of map bounds at this position, skip to the checking the next position...
            if (offsetPosition.X < 0 || offsetPosition.Y < 0 || offsetPosition.X >= TileMapWidth || offsetPosition.Y >= TileMapHeight) continue;

            // Otherwise, if there is a tile at this X,Y position and elevation (Z)...
            if (_tileMap[z, (int)offsetPosition.X, (int)offsetPosition.Y] > 0)
            {
                // Then it must be obscuring our original tile, hence we must select this
                // tile as the one at the screen position
                return new Vector3((int)offsetPosition.X, (int)offsetPosition.Y, z);
            }
        }

        // If we reached here, the best match was the original tile at zero elevation
        return flatMapCoordinates;
    }
}
