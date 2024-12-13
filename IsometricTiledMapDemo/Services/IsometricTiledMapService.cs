using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricTiledMapDemo.Services;

internal class IsometricTiledMapService
{
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }

    private readonly ContentManager _contentManager;
    private Point _selectedTile;
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
        for (int elevation = 0; elevation < _tiles.GetLength(0); elevation++)
        {
            for (int y = 0; y < _tiles.GetLength(2); y++)
            {
                for (int x = 0; x < _tiles.GetLength(1); x++)
                {
                    var tile = _tiles[elevation, x, y];

                    if (tile == 1)
                    {
                        var colour = Color.White;

                        if (_selectedTile.X == x && _selectedTile.Y == y)
                        {
                            colour = Color.Red;
                        }

                        _spriteBatch.Draw(
                            texture: _texture,
                            position: MapToScreenCoordinate(new Vector2(x, y)) - new Point(_tileWidth / 2, 0).ToVector2(),
                            color: colour);
                    }
                }
            }
        }
    }

    public void HighlightTile(Vector2 screenCoordinates)
    {
        _selectedTile = ScreenToMapCoordinate(screenCoordinates);
    }

    public Vector2 MapToScreenCoordinate(Vector2 mapCoordinates)
    {
        return Vector2.Transform(mapCoordinates, _transformationMatrix) + Origin;
    }

    public Point ScreenToMapCoordinate(Vector2 screenCoordinates)
    {
        screenCoordinates -= Origin;
        var transformed = Vector2.Transform(screenCoordinates, _transformationMatrixInverted);

        return transformed.ToPoint();
    }
}
