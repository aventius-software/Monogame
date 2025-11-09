using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace IsometricDynamicMapDemo.Map;

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
/// https://stackoverflow.com/questions/21842814/mouse-position-to-isometric-tile-including-height
/// https://github.com/OpenTTD/OpenTTD/blob/master/src/landscape.cpp
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// https://gamedev.stackexchange.com/questions/207056/selecting-tiles-with-mouse-on-isometric-map-with-height-and-slopes
/// https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
/// https://www.gamedev.net/reference/articles/article2026.asp
/// </summary>
internal class IsometricMapService
{
    private readonly ContentManager _contentManager;

    private Vector3 _selectedTile;
    private readonly SpriteBatch _spriteBatch;
    private TileType _tileType;
    private DotTiled.Map _tiledMap;
    private Texture2D _tilesetTexture;

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
        for (int elevation = 0; elevation < _tiledMap.Layers.Count; elevation++)
        {
            for (int y = 0; y < _tiledMap.Height; y++)
            {
                for (int x = 0; x < _tiledMap.Width; x++)
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

                    if ((int)_selectedTile.X == x && (int)_selectedTile.Y == y && _selectedTile.Z == elevation)
                    {
                        var pos = MapToWorldCoordinates(new Point(x, y), elevation);
                        _spriteBatch.DrawRectangle(new RectangleF(pos.X, pos.Y, 64, 48), Color.White);
                    }                    
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

        var data = tileLayer.Data.Value.GlobalTileIDs.Value;

        // If the index is out of bounds then just return 'no tile' (i.e. 0)
        if (index < 0 || index >= data.Length) return 0;

        // Otherwise return the tile
        return (int)data[index];
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
    /// <param name="tileType">For flat or cube tile types</param>
    public void LoadTiledMap(string tiledMapPath, TileType tileType)
    {
        // Load the map using DotTiled (as we're not using the content pipeline)
        var loader = Loader.Default();
        _tiledMap = loader.LoadMap(_contentManager.RootDirectory + "/" + tiledMapPath);

        // Save the tile type we're dealing with (flat or cubes)
        _tileType = tileType;

        // Now load the texture atlas for the first tileset. For this we're assuming
        // only 1 tileset and that it is in the same folder as the TMX file!
        var mapFolder = Path.GetDirectoryName(tiledMapPath);

        // Get the file name without extension
        var tileAtlasFileName = _tiledMap.Tilesets[0].Image.Value.Source.Value;
        var tileAtlasFileWithoutExtension = Path.GetFileNameWithoutExtension(tileAtlasFileName);

        // Finally, we can build the path to the file and load it
        var contentPath = Path.Combine(mapFolder, tileAtlasFileWithoutExtension);
        _tilesetTexture = _contentManager.Load<Texture2D>(contentPath);
    }

    /// <summary>
    /// Takes a flat map position and returns the relevant world coordinates
    /// </summary>
    /// <param name="mapCoordinates"></param>
    /// <returns></returns>
    private Vector2 FlatMapToWorldCoordinates(Point mapCoordinates)
    {
        var halfTileWidth = _tiledMap.TileWidth / 2f;
        var halfTileHeight = _tiledMap.TileHeight / 2f;

        return new Vector2(
            (mapCoordinates.X - mapCoordinates.Y) * halfTileWidth,
            (mapCoordinates.X + mapCoordinates.Y) * halfTileHeight
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
        // First get the world coordinates as if it were on a flat map
        var worldCoordinates = FlatMapToWorldCoordinates(mapCoordinates);

        // In isometric maps the origin point is at the middle of a tile, so to correct this
        // we offset the x coordinate by half a tile width
        worldCoordinates.X -= _tiledMap.TileWidth / 2;

        // Adjust the Y coordinate for the elevation
        worldCoordinates.Y += _tiledMap.Layers[elevation].OffsetY;

        // Finally
        return worldCoordinates;
    }

    /// <summary>
    /// Takes a world position and returns the isometric tile map 
    /// coordinates for that position in a flat map
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    private Vector2 WorldToFlatMapCoordinates(Vector2 worldCoordinates)
    {
        var halfTileWidth = _tiledMap.TileWidth / 2f;
        var halfTileHeight = _tiledMap.TileHeight / 2f;

        // preserve fractional coordinates for precision; floor only when selecting final tile
        var tileX = (worldCoordinates.X / halfTileWidth + worldCoordinates.Y / halfTileHeight) / 2f;
        var tileY = (worldCoordinates.Y / halfTileHeight - worldCoordinates.X / halfTileWidth) / 2f;

        return new Vector2(tileX, tileY);
    }

    /// <summary>
    /// Translates world coordinates to map coordinates, taking 
    /// tile elevation into account
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    public Vector3 WorldToMapCoordinates(Vector2 worldCoordinates)
    {
        if (_tileType == TileType.Cube) return WorldToCubeTileMapCoordinates(worldCoordinates);
        else return WorldToFlatTileMapCoordinates(worldCoordinates);
    }

    /// <summary>
    /// Translates world coordinates to map coordinates when using 
    /// cube tiles, taking tile elevation into account
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    private Vector3 WorldToCubeTileMapCoordinates(Vector2 worldCoordinates)
    {
        // Translate to X,Y map position as if it were a flat map with no elevation        
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
            var offsetPosition = flatMapCoordinates + new Vector2(elevation, elevation);

            // If we're out of map bounds at this position, skip to the checking the next position...
            if (offsetPosition.X < 0
                || offsetPosition.Y < 0
                || offsetPosition.X >= _tiledMap.Width
                || offsetPosition.Y >= _tiledMap.Height) continue;

            // Otherwise, if there is a tile at this X,Y position and elevation (Z)...
            if (GetTileAtPosition((int)offsetPosition.X, (int)offsetPosition.Y, elevation) > 0)
            {
                // Then it must be obscuring our original tile, hence we must select this
                // tile as the one at the screen position
                return new Vector3((int)offsetPosition.X, (int)offsetPosition.Y, elevation);
            }
        }

        // If we reached here, the best match was the original tile at zero elevation
        return new Vector3(flatMapCoordinates, 0);
    }

    /// <summary>
    /// Translates world coordinates to map coordinates when using
    /// diamond/flat (half-height) tiles, taking tile elevation into account.
    /// 
    /// Improved precision: this method now performs per-layer inversion as before but
    /// additionally performs per-tile hit-tests using the tileset image alpha (pixel-perfect)
    /// when available, and also falls back to geometric diamond tests. It also checks a small
    /// neighbourhood of candidate tile coordinates to handle border cases.
    /// </summary>
    /// <param name="worldCoordinates"></param>
    /// <returns></returns>
    private Vector3 WorldToFlatTileMapCoordinates(Vector2 worldCoordinates)
    {
        if (_tiledMap == null || _tiledMap.Layers.Count == 0) return Vector3.Zero;

        // neighbour offsets to test around candidate (covers border clicks)
        var neighbourOffsets = new Point[]
        {
            new Point(0,0),
            new Point(-1,0),
            new Point(1,0),
            new Point(0,-1),
            new Point(0,1),
            new Point(-1,-1),
            new Point(1,1),
            new Point(-1,1),
            new Point(1,-1),
        };

        // Iterate from topmost layer down so we find the tile visually closest to the camera first
        for (var elevation = _tiledMap.Layers.Count - 1; elevation >= 0; elevation--)
        {
            // Undo MapToWorldCoordinates adjustments for this elevation:
            // - MapToWorldCoordinates subtracted tileWidth/2 from X, so add it back.
            // - MapToWorldCoordinates added layer.OffsetY to Y, so subtract it here.
            var adjusted = new Vector2(
                worldCoordinates.X + _tiledMap.TileWidth / 2f,
                worldCoordinates.Y - _tiledMap.Layers[elevation].OffsetY
            );

            // Convert the adjusted world coordinates back to flat map coordinates
            var candidate = WorldToFlatMapCoordinates(adjusted);

            // Correct negative rounding (same logic as for cube tiles)
            if (candidate.X < 0) candidate.X = (int)Math.Floor(candidate.X);
            if (candidate.Y < 0) candidate.Y = (int)Math.Floor(candidate.Y);

            var baseX = (int)candidate.X;
            var baseY = (int)candidate.Y;

            // Test the candidate tile and neighbours for a hit.
            foreach (var off in neighbourOffsets)
            {
                var tx = baseX + off.X;
                var ty = baseY + off.Y;

                // Bounds check
                if (tx < 0 || ty < 0 || tx >= _tiledMap.Width || ty >= _tiledMap.Height) continue;

                if (DoesScreenPointHitTile(new Point(tx, ty), elevation, worldCoordinates))
                {
                    return new Vector3(tx, ty, elevation);
                }
            }
        }

        // If nothing was found, return best-effort base tile (elevation zero) by inverting elevation 0 adjustments
        var baseAdjusted = new Vector2(
            worldCoordinates.X + _tiledMap.TileWidth / 2f,
            worldCoordinates.Y - _tiledMap.Layers[0].OffsetY
        );

        var baseCoords = WorldToFlatMapCoordinates(baseAdjusted);
        if (baseCoords.X < 0) baseCoords.X = (int)Math.Floor(baseCoords.X);
        if (baseCoords.Y < 0) baseCoords.Y = (int)Math.Floor(baseCoords.Y);

        return new Vector3((int)baseCoords.X, (int)baseCoords.Y, 0);
    }

    /// <summary>
    /// Checks whether a screen world point hits a tile at map coordinate (mapCoords, elevation).
    /// Uses pixel-perfect alpha test against the tileset image when possible; otherwise falls back
    /// to a diamond geometric test.
    /// </summary>
    private bool DoesScreenPointHitTile(Point mapCoords, int elevation, Vector2 screenWorldPoint)
    {
        var gid = GetTileAtPosition(mapCoords.X, mapCoords.Y, elevation);
        if (gid == 0) return false;

        // Tile sprite top-left in world coordinates
        var tileWorldPos = MapToWorldCoordinates(mapCoords, elevation);

        // local coordinates within tile sprite
        var localXf = screenWorldPoint.X - tileWorldPos.X;
        var localYf = screenWorldPoint.Y - tileWorldPos.Y;

        // Get source rectangle for tile within tileset
        var srcRect = GetImageSourceRectangleForTile(gid);
        var tileW = srcRect.Width;
        var tileH = srcRect.Height;

        var localXi = (int)Math.Floor(localXf);
        var localYi = (int)Math.Floor(localYf);

        // quick bounds test
        if (localXi < 0 || localYi < 0 || localXi >= tileW || localYi >= tileH) return false;

        // Try pixel-perfect alpha test if texture available
        if (_tilesetTexture != null)
        {
            try
            {
                var sampleRect = new Rectangle(srcRect.X + localXi, srcRect.Y + localYi, 1, 1);
                var pixel = new Color[1];
                _tilesetTexture.GetData(0, sampleRect, pixel, 0, 1);

                // If pixel has significant alpha then this click hits the visible part of the tile
                if (pixel[0].A > 10) return true;
                // otherwise fall through to geometric test
            }
            catch
            {
                // If GetData fails for any reason, fall back to geometric test.
            }
        }

        // Geometric diamond test fallback.
        // Diamond center is at (tileW/2, tileH/2).
        var halfW = tileW / 2f;
        var halfH = tileH / 2f;
        var dx = Math.Abs(localXf - halfW) / halfW;
        var dy = Math.Abs(localYf - halfH) / halfH;

        return dx + dy <= 1.0f;
    }
}
