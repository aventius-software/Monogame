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
    public int MapDepth => _tiles.GetLength(0);
    public int MapHeight => _tiles.GetLength(2);
    public int MapWidth => _tiles.GetLength(1);
    public Point Origin { get; set; } = Point.Zero;
    public Vector3 TileOver => _selectedTile;
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }

    private readonly ContentManager _contentManager;
    private Vector3 _selectedTile;    
    private readonly SpriteBatch _spriteBatch;
    private Texture2D _texture;
    private readonly int[,,] _tiles;
    private int _tileHeight;
    private int _tileWidth;
    private Matrix _transformationMatrix;
    private Matrix _transformationMatrixInverted;

    public IsometricTiledMapService(ContentManager contentManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;

        _texture = _contentManager.Load<Texture2D>("tile");
        _tileWidth = _texture.Width;
        _tileHeight = _texture.Height;

        _tiles = new int[,,]
        {
            {
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
            },
            {
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 }
            },
            {
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 }
            }
        };

        WorldWidth = _tiles.GetLength(0) * _tileWidth;
        WorldHeight = _tiles.GetLength(1) * _tileHeight;

        // Create a translation matrix to translate map coordinates
        // to screen coordinates
        // See https://gist.github.com/jordwest/8a12196436ebcf8df98a2745251915b5        
        _transformationMatrix = new Matrix(
            _tileWidth / 2, _tileHeight / 4, 0, 0,
            -_tileWidth / 2, _tileHeight / 4, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        var w = _tileWidth / 2;
        var h = _tileHeight / 4;
        
        //https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
        _transformationMatrix = new Matrix(
            m11: w, m21: -w, m31: 0, m41: 0,
            m12: h, m22: h, m32: 0, m42: 0,
            m13: 0, m23: 0, m33: 1, m43: 0,
            m14: 0, m24: 0, m34: 0, m44: 1
        );

        // Get the inverse of the matrix to translate coordinates back
        _transformationMatrixInverted = Matrix.Invert(_transformationMatrix);
    }

    public void Draw()
    {
        for (int elevation = 0; elevation < MapDepth; elevation++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    var tile = _tiles[elevation, x, y];

                    if (tile == 1)
                    {
                        var colour = Color.White;

                        if (_selectedTile.X == x && _selectedTile.Y == y)// && _selectedTile.Z == elevation)
                        {
                            colour = Color.Red;
                        }
                                               
                        _spriteBatch.Draw(
                            texture: _texture,
                            position: MapToScreenCoordinates(new Point(x, y), elevation).ToVector2(),
                            color: colour);
                    }
                }
            }
        }
       
        _spriteBatch.Draw(
            texture: _texture,
            position: MapToScreenCoordinates(new Point((int)_selectedTile.X, (int)_selectedTile.Y), 0).ToVector2(),
            color: Color.Blue);
    }

    public Vector3 HighlightTile(Point screenCoordinates)
    {
        _selectedTile = ScreenToMapCoordinates(screenCoordinates);
        
        return _selectedTile;
    }

    public Point MapToScreenCoordinates(Point mapCoordinates, int elevation)
    {
        var screenCoordinates = Vector2.Transform(mapCoordinates.ToVector2(), _transformationMatrix) + Origin.ToVector2();
        screenCoordinates.X -= _tileWidth / 2;
        screenCoordinates.Y -= elevation * (_tileHeight / 2);

        return screenCoordinates.ToPoint();
    }
    
    public Vector3 ScreenToMapCoordinates(Point screenCoordinates)
    {
        // Adjust the screen coordinates for the map origin
        screenCoordinates -= Origin;

        // Translate to X,Y map position as if it were a flat map with no elevation
        var flatMapCoordinates = Vector3.Transform(new Vector3(screenCoordinates.ToVector2(), 0), _transformationMatrixInverted);
        
        // When at a negative position we're offset by, so -0.1 is actually coordinate -1, so adjust for this
        if (flatMapCoordinates.X < 0) flatMapCoordinates.X = (int)Math.Floor(flatMapCoordinates.X);
        if (flatMapCoordinates.Y < 0) flatMapCoordinates.Y = (int)Math.Floor(flatMapCoordinates.Y);
        
        // At this point we're effectively looking at the tile map where elevation (Z) is zero. However, if
        // other tiles are at further coordinates X,Y but higher elevation (Z) position then those tiles will
        // appear at the same screen position as the tile at our current calculated map X,Y position, so we
        // need to move closer to the camera in the X,Y and check for higher tiles (Z) at those X,Y coordinates
        // then if we find one with the same screen position, then that 'higher' elevation tile is the one that
        // is selected (as its hiding the lower tiles behind it)                
        //_selectedTileOffset = new Vector3(flatMapCoordinates.ToVector2(), 0);// + (Vector2.One * (MapDepth - 1)), 0);//new Vector3(newY, newX, 0);

        // Work backwards from new starting position, checking the elevation
        // at each position to see if a tile is there...
        //for (var z = MapDepth - 1; z > 1; z--)
        //{
        //    // Move to the current offset position
        //    var offsetPosition = new Vector3(flatMapCoordinates.ToVector2() + (Vector2.One * z), 0);

        //    // If we're out of map bounds at this position, skip to the checking the next position...
        //    if (offsetPosition.X < 0 || offsetPosition.Y < 0 || offsetPosition.X >= MapWidth || offsetPosition.Y >= MapHeight) continue;

        //    // Otherwise, if there is a tile at this X,Y position and elevation (Z)...
        //    if (_tiles[z, (int)offsetPosition.X, (int)offsetPosition.Y] != 0)
        //    {
        //        // Then we selected it ;-)
        //        return new Vector3((int)offsetPosition.X, (int)offsetPosition.Y, z);
        //    }
        //}

        // If we reached here, the best match was the original tile at zero elevation
        return flatMapCoordinates;
    }

    /*
    public Point MapToScreenCoordinates(Point mapCoordinates, int elevation) 
    {
        // https://gamedev.stackexchange.com/questions/30566/how-would-i-translate-screen-coordinates-to-isometric-coordinates
        var screenX = (mapCoordinates.X - mapCoordinates.Y) * _tileWidth / 2;
        var screenY = (mapCoordinates.X + mapCoordinates.Y) * _tileHeight / 4;

        screenY -= (_tileHeight / 2) * elevation;

        return new Point(screenX, screenY) + Origin;
    }

    public Vector3 ScreenToMapCoordinates(Point screenCoordinates)
    {
        // Adjust the screen coordinates for the map origin
        screenCoordinates -= Origin;

        //var mapX = screenCoordinates.Y / (_tileHeight / 2) + screenCoordinates.X / (_tileWidth);
        //var mapY = screenCoordinates.Y / (_tileHeight / 2) - screenCoordinates.X / (_tileWidth);

        var mapX = 0.5f * (screenCoordinates.X / (_tileWidth / 1) + screenCoordinates.Y / (_tileHeight / 2));
        var mapY = 0.5f * (-screenCoordinates.X / (_tileWidth / 1) + screenCoordinates.Y / (_tileHeight / 2));

        return new Vector3(mapX, mapY, 0);
    }*/
}
