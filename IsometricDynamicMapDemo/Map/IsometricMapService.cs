using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace IsometricDynamicMapDemo.Map;

internal enum TileType
{
    Flat,
    Cube
}

/// <summary>
/// A basic 2D isometric tile map service that uses Tiled maps. 
/// 
/// Note - For drawing at different elevations, you should set each tile layer with the Y offset which 
/// corresponds to the height of your tile multiplied by the layer index, otherwise it will not get rendered 
/// correctly (i.e. each layer will get rendered at the same elevation) and will also fail to calculate tile 
/// map/world coordinate correctly.
/// 
/// Art:
/// https://opengameart.org/content/isometric-64x64-outside-tileset
/// 
/// References:
/// 
/// https://erikonarheim.com/posts/handling-height-in-isometric/
/// https://discourse.mapeditor.org/t/half-height-isometric-maps/4545/8
/// https://clintbellanger.net/articles/isometric_math/
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// https://gamedev.stackexchange.com/questions/207056/selecting-tiles-with-mouse-on-isometric-map-with-height-and-slopes
/// https://github.com/OpenTTD/OpenTTD/blob/master/src/landscape.cpp
/// </summary>
internal class IsometricMapService
{    
    public int TileMapDepth => _tiledMap.Layers.Count;
    public int TileMapHeight => (int)_tiledMap.Height;
    public int TileMapWidth => (int)_tiledMap.Width;
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }

    private readonly ContentManager _contentManager;
    
    private Vector3 _selectedTile;
    private readonly SpriteBatch _spriteBatch;
    private int _tileHeight;
    private int _tileWidth;
    //private int _tileDepth;
    private TileType _tileType;
    private DotTiled.Map _tiledMap;
    private Texture2D _tilesetTexture;
    //private Matrix _translationMatrix;
    //private Matrix _transformationMatrixInverted;

    public IsometricMapService(ContentManager contentManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;
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
                    var tile = GetTileAtPosition(x, y, elevation);

                    // If there is a tile here and not empty space...
                    if (tile == 0) continue;

                    // By default, use white
                    var colour = Color.White;

                    // If there is a 'selected' tile at these coordinates...
                    if ((int)_selectedTile.X == x && (int)_selectedTile.Y == y && _selectedTile.Z == elevation)
                    {
                        // Highlight it in red
                        colour = Color.Red;
                    }

                    // Get the correct tile 'image' rectangle from the tileset 'atlas' image
                    var sourceRectangle = GetImageSourceRectangleForTile(tile);

                    // Draw the map tile at this position and elevation
                    _spriteBatch.Draw(
                        texture: _tilesetTexture,
                        position: MapToWorldCoordinates(new Point(x, y), elevation),
                        sourceRectangle: sourceRectangle,
                        color: colour);
                }
            }
        }
    }

    /// <summary>
    /// Helper method to work out the source rectangle for the specified tile so we can
    /// pick out the correct texture to use when drawing the tile
    /// </summary>    
    /// <param name="gid"></param>
    /// <param name="tileSetId"></param>
    /// <returns></returns>
    private Rectangle GetImageSourceRectangleForTile(int gid, int tileSetId = 0)
    {
        var tileId = gid - 1;
        var tileset = _tiledMap.Tilesets[tileSetId];

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
    private TileLayer GetLayer(int layerNumber = 0) => (TileLayer)_tiledMap.Layers[layerNumber];

    /// <summary>
    /// Get the tile a the specified position in the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="elevation"></param>
    /// <returns></returns>
    private int GetTileAtPosition(int x, int y, int elevation)
    {
        var tileLayer = GetLayer(elevation);

        // Calculate the index of the request tile in the map data
        var index = (y * tileLayer.Width) + x;

        // If the index is out of bounds then just return 'no tile' (i.e. 0)
        if (index >= tileLayer.Data.Value.GlobalTileIDs.Value.Length - 1 || index < 0) return 0;

        // Otherwise return the tile
        return (int)tileLayer.Data.Value.GlobalTileIDs.Value[index];
    }

    /// <summary>
    /// Highlights the tile at the specified screen coordinates
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    public void HighlightTile(Vector2 worldCoordinates)
    {
        _selectedTile = WorldToMapCoordinates(worldCoordinates);        
    }

    /// <summary>
    /// Load a Tiled map from the content folder. As we're not using the content pipeline
    /// to load the map, the TMX,TSX files should NOT be added to the content manager, however
    /// in their properties you must set the 'Copy To Output Directory' to 'Copy if newer'. For
    /// the related tileset texture/image though, this SHOULD be added to the content pipeline!
    /// </summary>
    /// <param name="tiledMapPath">Path inside the 'Content' folder for the tile TMX map file</param>
    /// <param name="tileHeightDivisor">For flat tiles use 2, if using cube blocks should use 4</param>
    public void LoadTiledMap(string tiledMapPath, TileType tileType)
    {
        // Load the map
        var loader = Loader.Default();
        _tiledMap = loader.LoadMap(_contentManager.RootDirectory + "/" + tiledMapPath);

        // Save the tile type we're dealing with (flat or cubes)
        _tileType = tileType;

        // Now load the texture atlas for the first tileset - assuming only 1 tileset and that
        // it is in the same folder as the TMX file!
        var mapFolder = Path.GetDirectoryName(tiledMapPath);
        var tileAtlasFileName = _tiledMap.Tilesets[0].Image.Value.Source.Value;
        var tileAtlasFileWithoutExtension = Path.GetFileNameWithoutExtension(tileAtlasFileName);
        var contentPath = Path.Combine(mapFolder, tileAtlasFileWithoutExtension);
        _tilesetTexture = _contentManager.Load<Texture2D>(contentPath);

        // Set tile dimensions        
        _tileWidth = (int)_tiledMap.TileWidth;
        _tileHeight = (int)_tiledMap.TileHeight;
                
        // Set world dimensions
        WorldWidth = (int)_tiledMap.Width * _tileWidth;
        WorldHeight = (int)_tiledMap.Height * _tileHeight;

        //// Get dimensions half tiles
        //var halfTileWidth = _tileBlockWidth / 2;
        //var halfTileHeight = _tileBlockHeight / 2;
        
        //// We'll use a matrix to translate coordinates, since translating to and from screen/map coordinates
        //// is a simple transform and inversion of the matrix. See the link below for some of the maths behind this...
        //// https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
        //_translationMatrix = new Matrix(
        //    m11: halfTileWidth, m21: -halfTileWidth, m31: 0, m41: 0,
        //    m12: halfTileHeight, m22: halfTileHeight, m32: 0, m42: 0,
        //    m13: 0, m23: 0, m33: 1, m43: 0,
        //    m14: 0, m24: 0, m34: 0, m44: 1
        //);

        //// Get the inverse of the matrix to translate coordinates back
        //_transformationMatrixInverted = Matrix.Invert(_translationMatrix);
    }
    
    private Vector2 FlatMapToWorldCoordinates(Point mapCoordinates)
    {
        var halfTileWidth = _tileWidth / 2f;
        var halfTileHeight = _tileHeight / 2f;

        var tileX = mapCoordinates.X;
        var tileY = mapCoordinates.Y;

        return new Vector2(
            (tileX - tileY) * halfTileWidth,
            (tileX + tileY) * halfTileHeight
        );
    }

    /// <summary>
    /// Translates map coordinates to world coordinates
    /// </summary>
    /// <param name="mapCoordinates"></param>
    /// <param name="elevation"></param>
    /// <returns></returns>
    public Vector2 MapToWorldCoordinates(Point mapCoordinates, int elevation)
    {
        // Use the matrix to transform the map coordinates into screen coordinates
        //var worldCoordinates = Vector2.Transform(mapCoordinates.ToVector2(), _translationMatrix);
        var worldCoordinates = FlatMapToWorldCoordinates(mapCoordinates);

        // In isometric maps the origin point is at the middle of a tile, so to correct this
        // we offset the x coordinate by half a tile width
        worldCoordinates.X -= _tileWidth / 2;

        // As we're using a tile 'block' (i.e. a cube) instead of a flat tile the block is half the height
        // of the actual sprite, so for elevation we just need multiples of this value depending
        // on the elevation level (e.g. 1,2,3 and so on...)
        worldCoordinates.Y += _tiledMap.Layers[elevation].OffsetY;

        // Finally
        return worldCoordinates;
    }

    /// <summary>
    /// Takes a world position and returns the isometric tile map coordinates for that position
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    private Vector3 WorldToFlatMapCoordinates(Vector2 worldCoordinates)
    {
        var halfTileWidth = _tileWidth / 2f;
        var halfTileHeight = _tileHeight / 2f;

        int tileX;
        int tileY;

        if (_tileType == TileType.Cube)
        {
            tileX = (int)((worldCoordinates.X / halfTileWidth + worldCoordinates.Y / halfTileHeight) / 2);
            tileY = (int)((worldCoordinates.Y / halfTileHeight - worldCoordinates.X / halfTileWidth) / 2);
        }
        else
        {
            tileX = (int)Math.Floor((worldCoordinates.X / halfTileWidth + worldCoordinates.Y / halfTileHeight) / 2);
            tileY = (int)Math.Floor((worldCoordinates.Y / halfTileHeight - worldCoordinates.X / halfTileWidth) / 2);
        }

        return new Vector3(tileX, tileY, 0);
    }

    public Vector3 WorldToMapCoordinates(Vector2 worldCoordinates)
    {
        // Translate to X,Y map position as if it were a flat map with no elevation
        //var flatMapCoordinates = Vector3.Transform(new Vector3(worldCoordinates, 0), _transformationMatrixInverted);
        var flatMapCoordinates = WorldToFlatMapCoordinates(worldCoordinates);

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
        for (var elevation = _tiledMap.Layers.Count - 1; elevation >= 1; elevation--)
        {
            // Move to the current offset position
            var offsetPosition = flatMapCoordinates + new Vector3(elevation, elevation, 0);

            // If we're out of map bounds at this position, skip to the checking the next position...
            if (offsetPosition.X < 0 || offsetPosition.Y < 0 || offsetPosition.X >= TileMapWidth || offsetPosition.Y >= TileMapHeight) continue;

            // Otherwise, if there is a tile at this X,Y position and elevation (Z)...
            if (GetTileAtPosition((int)offsetPosition.X, (int)offsetPosition.Y, elevation) > 0)
            {
                // Then it must be obscuring our original tile, hence we must select this
                // tile as the one at the screen position
                return new Vector3((int)offsetPosition.X, (int)offsetPosition.Y, elevation);
            }
        }

        // If we reached here, the best match was the original tile at zero elevation
        return flatMapCoordinates;
    }

    /// <summary>
    /// Translates world coordinates to map coordinates, taking tile elevation into account
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    public Vector3 xWorldToMapCoordinates(Vector2 worldCoordinates)
    {        
        // Translate to X,Y map position as if it were a flat map with no elevation
        //var flatMapCoordinates = Vector3.Transform(new Vector3(worldCoordinates, 0), _transformationMatrixInverted);
        var flatMapCoordinates = WorldToFlatMapCoordinates(worldCoordinates);

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
        for (var elevation = TileMapDepth - 1; elevation > 0; elevation--)
        {
            // Move to the current offset position
            var offsetPosition = flatMapCoordinates + new Vector3(elevation, elevation, 0);

            // If we're out of map bounds at this position, skip to the checking the next position...
            if (offsetPosition.X < 0 || offsetPosition.Y < 0 || offsetPosition.X >= TileMapWidth || offsetPosition.Y >= TileMapHeight) continue;

            // Otherwise, if there is a tile at this X,Y position and elevation (Z)...
            if (GetTileAtPosition((int)offsetPosition.X, (int)offsetPosition.Y, elevation) > 0)
            {
                // Then it must be obscuring our original tile, hence we must select this
                // tile as the one at the screen position
                return new Vector3((int)offsetPosition.X, (int)offsetPosition.Y, elevation);
            }
        }

        // If we reached here, the best match was the original tile at zero elevation
        return flatMapCoordinates;
    }
}
