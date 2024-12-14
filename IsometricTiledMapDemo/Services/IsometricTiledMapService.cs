using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }

    private readonly ContentManager _contentManager;
    private Vector3 _selectedTile;
    private Vector3 _selectedTileOffset;
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
                { 1,1,1,1,1 },
                { 1,1,1,1,1 },
                { 0,0,1,0,0 },
                { 0,0,1,0,0 },
                { 0,0,0,0,0 }
            },
            {
                { 1,1,1,1,1 },
                { 0,0,1,0,0 },
                { 0,0,1,0,0 },
                { 0,0,0,0,0 },
                { 0,0,0,0,0 }
            }
        };

        WorldWidth = _tiles.GetLength(0) * _tileWidth;
        WorldHeight = _tiles.GetLength(1) * _tileHeight;

        // Create a translation matrix to translate between coordinate systems
        // See https://gist.github.com/jordwest/8a12196436ebcf8df98a2745251915b5        
        _transformationMatrix = new Matrix(
            _tileWidth / 2, _tileHeight / 4, 0, 0,
            -_tileWidth / 2, _tileHeight / 4, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
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

                        if (_selectedTile.X == x && _selectedTile.Y == y && _selectedTile.Z == elevation)
                        {
                            colour = Color.Red;
                        }

                        if (_selectedTileOffset.X == x && _selectedTileOffset.Y == y && _selectedTileOffset.Z == elevation)
                        {
                            colour = Color.Red;
                        }

                        _spriteBatch.Draw(
                            texture: _texture,
                            position: MapToScreenCoordinate(new Vector2(x, y), elevation) - new Vector2(_tileWidth / 2, 0),
                            color: colour);
                    }
                }
            }
        }
    }

    public Vector3 HighlightTile(Vector2 screenCoordinates)
    {
        _selectedTile = ScreenToMapCoordinate(screenCoordinates);
        
        return _selectedTile;
    }

    public Vector2 MapToScreenCoordinate(Vector2 mapCoordinates, int elevation)
    {
        var screenCoordinates = Vector2.Transform(mapCoordinates, _transformationMatrix) + Origin;
        screenCoordinates.Y -= elevation * (_tileHeight / 2);

        return screenCoordinates;
    }

    public Vector3 ScreenToMapCoordinate(Vector2 screenCoordinates)
    {
        // Adjust the screen coordinates for the map origin
        screenCoordinates -= Origin;

        // Translate to X,Y map position as if it were a flat map with no elevation
        var flatMapCoordinates = Vector2.Transform(screenCoordinates, _transformationMatrixInverted).ToPoint();

        // At this point we're effectively looking at the tile map where elevation (Z) is zero. However, if
        // other tiles are at further coordinates X,Y but higher elevation (Z) position then those tiles will
        // appear at the same screen position as the tile at our current calculated map X,Y position, so we
        // need to move closer to the camera in the X,Y and check for higher tiles (Z) at those X,Y coordinates
        // then if we find one with the same screen position, then that 'higher' elevation tile is the one that
        // is selected (as its hiding the lower tiles behind it)        
        //var iterationsFromMapEdge = MathHelper.Min(MapWidth - flatMapCoordinates.X, MapHeight - flatMapCoordinates.Y);
        //iterationsFromMapEdge = MathHelper.Min(MapDepth, iterationsFromMapEdge);

        // Calculate new starting position to work back from
        var newX = flatMapCoordinates.X + MapDepth - 1;
        var newY = flatMapCoordinates.Y + MapDepth - 1;

        //_selectedTileOffset = new Vector3(flatMapCoordinates.X, flatMapCoordinates.Y, 0);

        // Work backwards from new starting position, checking elevation
        // at each position to see if a tile is there...
        for (var z = MapDepth - 1; z > 0; z--)
        {            
            // Check if we're out of map bounds
            if (newX < 0 || newY < 0 || newX >= MapWidth || newY >= MapHeight) continue;
            
            // If there is a tile at this X,Y position and elevation (Z)...
            if (_tiles[z, newX, newY] != 0)
            {
                // Then we selected it ;-)
                return new Vector3(newX, newY, z);
            }            

            // Move backwards, towards our 'flat' starting tile position
            newX--;
            newY--;
        }

        // If we reached here, the best match was the original tile at zero elevation
        return new Vector3(flatMapCoordinates.ToVector2(), 0);
    }
}
