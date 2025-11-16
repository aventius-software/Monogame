using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using nkast.Aether.Physics2D.Collision.Shapes;
using System.Collections.Generic;

namespace TTDStyleIsometricTileMap.Services;

/// <summary>
/// Tile types as used in OpenTTD:
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// </summary>
internal enum SlopeType
{
    SLOPE_FLAT = 0,
    SLOPE_W = 1,
    SLOPE_S = 2,
    SLOPE_E = 3,
    SLOPE_N = 4,
    SLOPE_NW = 5,
    SLOPE_SW = 6,
    SLOPE_SE = 7,
    SLOPE_NE = 8,
    SLOPE_EW = 9,
    SLOPE_NS = 10,
    SLOPE_NWS = 11,
    SLOPE_WSE = 12,
    SLOPE_SEN = 13,
    SLOPE_ENW = 14,
    SLOPE_STEEP_W = 15,
    SLOPE_STEEP_S = 16,
    SLOPE_STEEP_E = 17,
    SLOPE_STEEP_N = 18
}

internal struct Tile
{
    public int X;
    public int Y;
    public int Elevation;
    public SlopeType Slope;    
}

/// <summary>
/// Tile types as used in OpenTTD:
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// </summary>
internal class IsometricDiamondTileMapService
{
    private readonly ContentManager _contentManager;

    private Tile[,] _map;
    private int _mapWidth = 20;
    private int _mapHeight = 12;    
    private Texture2D _texture;
    private Texture2DAtlas _textureAtlas;
    private Dictionary<SlopeType, int> _textureAtlasRegions;
    private int _tileWidth = 64;
    private int _tileHeight = 48;

    public IsometricDiamondTileMapService(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    public void Draw(SpriteBatch spriteBatch)
    {        
        for (var x = 0; x < _mapWidth; x++)
        {
            for (var y = 0; y < _mapHeight; y++)
            {
                var slope = _map[x, y].Slope;
                var region = _textureAtlasRegions[slope];
                spriteBatch.Draw(_textureAtlas[region], new Vector2(x * _tileWidth, y * _tileHeight), Color.White);
            }
        }
    }
    
    public void LoadTileAtlas(string contentPath, int tileWidth = 64, int tileHeight = 48)
    {
        // Load the tiles
        _texture = _contentManager.Load<Texture2D>(contentPath);
        _tileWidth = tileWidth;
        _tileHeight = tileHeight;

        // Create tile atlas, see https://www.monogameextended.net/docs/features/texture-handling/texture2datlas/
        _textureAtlas = Texture2DAtlas.Create("Atlas/Tiles", _texture, _tileWidth, _tileHeight);

        // TODO: need to make these custom mappable!
        _textureAtlasRegions = new Dictionary<SlopeType, int>()
        {
            { SlopeType.SLOPE_FLAT, 0 },
            { SlopeType.SLOPE_W, 4 },
            { SlopeType.SLOPE_S, 5 },
            { SlopeType.SLOPE_E, 6 },
            { SlopeType.SLOPE_N, 7 },
            { SlopeType.SLOPE_NW, 8 },
            { SlopeType.SLOPE_SW, 9 },
            { SlopeType.SLOPE_SE, 10 },
            { SlopeType.SLOPE_NE, 11 },
            { SlopeType.SLOPE_EW, 21 },
            { SlopeType.SLOPE_NS, 20 },
            { SlopeType.SLOPE_NWS, 12 },
            { SlopeType.SLOPE_WSE, 13 },
            { SlopeType.SLOPE_SEN, 14 },
            { SlopeType.SLOPE_ENW, 15 },
            { SlopeType.SLOPE_STEEP_W, 17 },
            { SlopeType.SLOPE_STEEP_S, 16 },
            { SlopeType.SLOPE_STEEP_E, 19 },
            { SlopeType.SLOPE_STEEP_N, 18 }
        };

        _map = new Tile[_mapWidth, _mapHeight];

        for (var x = 0; x < _mapWidth; x++)
        {
            for (var y = 0; y < _mapHeight; y++)
            {
                var tile = new Tile
                {
                    X = x,
                    Y = y,
                    Elevation = 0,
                    Slope = SlopeType.SLOPE_FLAT
                };

                _map[x, y] = tile;
            }
        }
    }
}
