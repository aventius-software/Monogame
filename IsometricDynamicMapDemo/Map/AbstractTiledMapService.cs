using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace IsometricDynamicMapDemo.Map;

/// <summary>
/// Useful references:
/// 
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// </summary>
internal abstract class AbstractTiledMapService
{
    protected readonly ContentManager _contentManager;

    protected DotTiled.Map _tiledMap;
    protected Texture2D _tilesetTexture;

    protected AbstractTiledMapService(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    /// <summary>
    /// Helper method to work out the source rectangle for the specified tile so we can
    /// pick out the correct texture to use when drawing the tile
    /// </summary>    
    /// <param name="gid"></param>
    /// <param name="tileSetId"></param>
    /// <returns></returns>
    protected Rectangle GetImageSourceRectangleForTile(int gid, int tileSetId = 0)
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
    protected TileLayer GetLayer(int layerNumber = 0) => (TileLayer)_tiledMap.Layers[layerNumber];

    /// <summary>
    /// Get the tile a the specified position in the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="elevation"></param>
    /// <returns></returns>
    protected int GetTileAtPosition(int x, int y, int elevation)
    {
        var tileLayer = GetLayer(elevation);
        var data = tileLayer.Data.Value.GlobalTileIDs.Value;

        // Calculate the index of the request tile in the map data
        var index = (y * tileLayer.Width) + x;

        // If the index is out of bounds then just return 'no tile' (i.e. 0)
        if (index < 0 || index >= data.Length) return 0;

        // Otherwise return the tile
        return (int)data[index];
    }

    /// <summary>
    /// Load a Tiled map from the content folder. As we're not using the content pipeline
    /// to load the map, the TMX,TSX files should NOT be added to the content manager, however
    /// in their properties you must set the 'Copy To Output Directory' to 'Copy if newer'. For
    /// the related tileset texture/image though, this SHOULD be added to the content pipeline!
    /// </summary>
    /// <param name="tiledMapContentPath">Path inside the 'Content' folder for the tile TMX map file</param>    
    public void LoadTiledMap(string tiledMapContentPath)
    {
        // Load the map using DotTiled (as we're not using the content pipeline)
        var loader = Loader.Default();
        _tiledMap = loader.LoadMap(_contentManager.RootDirectory + "/" + tiledMapContentPath);
        
        // Now load the texture atlas for the first tileset. For this we're assuming
        // only 1 tileset and that it is in the same folder as the TMX file!
        var mapFolder = Path.GetDirectoryName(tiledMapContentPath);

        // Get the file name without extension
        var tileAtlasFileName = _tiledMap.Tilesets[0].Image.Value.Source.Value;
        var tileAtlasFileWithoutExtension = Path.GetFileNameWithoutExtension(tileAtlasFileName);

        // Finally, we can build the path to the file and load it
        var contentPath = Path.Combine(mapFolder, tileAtlasFileWithoutExtension);
        _tilesetTexture = _contentManager.Load<Texture2D>(contentPath);        
    }
}
