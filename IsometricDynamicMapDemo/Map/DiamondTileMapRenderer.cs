//using Dottiled.Tiled; // assumes dottiled provides this namespace; adjust if actual namespace differs
using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace IsometricDynamicMapDemo.Map;

/// <summary>
/// Renders diamond (isometric) tiled maps loaded with the dottiled library.
/// Supports per-tile elevation and simple slope interpretations driven by tile properties.
/// - Expects tile properties:
///   - "elevation" (float) : base elevation units for the tile
///   - "slope" (string) : "none", "north", "south", "east", "west", "nw", "ne", "sw", "se"
/// - Uses layer Y offset (layer.OffsetY) to contribute to elevation (if present in map data).
/// </summary>
public class DiamondTileMapRenderer
{
    private DotTiled.Map _map;
    private readonly GraphicsDevice _graphics;
    private readonly Dictionary<Tileset, Texture2D> _tilesetTextures = new();
    private readonly Dictionary<(Tileset, int), Rectangle> _tilesetSourceRects = new();
    private readonly float _elevationUnitPixels; // how many pixels one elevation unit moves vertically

    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }
    public int MapWidthTiles { get; private set; }
    public int MapHeightTiles { get; private set; }

    public DiamondTileMapRenderer(GraphicsDevice graphics, float elevationUnitPixels = 16f)
    {
        _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        _elevationUnitPixels = elevationUnitPixels;
    }

    /// <summary>
    /// Load a Tiled map (TMX) using dottiled. Also loads referenced tileset images using file paths relative to the TMX file.
    /// </summary>
    /// <param name="tmxPath">Full path to the TMX file produced by Tiled.</param>
    public void LoadMap(string tmxPath)
    {
        if (string.IsNullOrEmpty(tmxPath)) throw new ArgumentNullException(nameof(tmxPath));
        if (!File.Exists(tmxPath)) throw new FileNotFoundException("TMX file not found", tmxPath);

        // Load with dottiled. Adjust call if dottiled API differs.
        //using var fs = File.OpenRead(tmxPath);
        var loader = Loader.Default();
        _map = loader.LoadMap(tmxPath); // assume dottiled's TiledMap.Load(Stream)

        TileWidth = _map.TileWidth;
        TileHeight = _map.TileHeight;
        MapWidthTiles = _map.Width;
        MapHeightTiles = _map.Height;

        // Load tileset images and compute source rects for each tile
        _tilesetTextures.Clear();
        _tilesetSourceRects.Clear();

        var tmxDir = Path.GetDirectoryName(tmxPath) ?? string.Empty;
        foreach (var ts in _map.Tilesets)
        {
            string imagePath = ts.Image.HasValue && ts.Image.Value.Source.HasValue
                ? ts.Image.Value.Source.Value
                : null;
            if (string.IsNullOrEmpty(imagePath))
                continue;

            string resolved = Path.IsPathRooted(imagePath) ? imagePath : Path.Combine(tmxDir, imagePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(resolved))
                throw new FileNotFoundException("Tileset image not found", resolved);

            using var imgStream = File.OpenRead(resolved);
            var texture = Texture2D.FromStream(_graphics, imgStream);
            _tilesetTextures[ts] = texture;

            int cols = ts.Image.Value.Width.HasValue ? ts.Image.Value.Width.Value / ts.TileWidth : 0;
            int rows = ts.Image.Value.Height.HasValue ? ts.Image.Value.Height.Value / ts.TileHeight : 0;

            for (int localId = 0; localId < ts.TileCount; localId++)
            {
                int tx = localId % cols;
                int ty = localId / cols;
                var srcRect = new Rectangle(
                    ts.Margin + tx * (ts.TileWidth + ts.Spacing),
                    ts.Margin + ty * (ts.TileHeight + ts.Spacing),
                    ts.TileWidth,
                    ts.TileHeight);
                _tilesetSourceRects[(ts, (int)ts.FirstGID.Value + localId)] = srcRect;
            }
        }
    }

    /// <summary>
    /// Draw the loaded map. Accepts a SpriteBatch which should be begun before calling.
    /// Camera transform should be supplied via the SpriteBatch Begin; this method draws all visible tile layers.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Rectangle? screenClip = null)
    {
        if (_map == null) return;
        foreach (DotTiled.TileLayer layer in _map.Layers)
        {
            if (!layer.Visible) continue;
            //if (layer is not tileLayer) continue;

            // layer offset Y contributes to elevation
            float layerOffsetY = layer.OffsetY;

            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    // FIX: layer is already typed as TileLayer, so no need to cast
                    int gid = (int)layer.GetGlobalTileIDAtCoord(x, y);
                    if (gid == 0) continue;

                    var tileset = _map.Tilesets[0];//.GetTilesetForGid(gid);
                    if (tileset == null) continue;
                    if (!_tilesetTextures.TryGetValue(tileset, out var tex)) continue;
                    if (!_tilesetSourceRects.TryGetValue((tileset, gid), out var srcRect)) continue;

                    // compute elevation for the tile's center using tile properties and layer offset
                    float baseElevation = GetTileBaseElevation(layer, x, y);
                    float pixelElevation = (baseElevation + layerOffsetY) * _elevationUnitPixels;

                    Vector2 pos = TileToWorld(x, y);
                    pos.Y -= pixelElevation;

                    // Optionally cull by screenClip
                    if (screenClip.HasValue)
                    {
                        var clip = screenClip.Value;
                        if (pos.X + TileWidth < clip.Left || pos.X > clip.Right || pos.Y + TileHeight < clip.Top || pos.Y > clip.Bottom)
                            continue;
                    }

                    // origin: tile is normally drawn using top-center to align diamonds; adjust to your tileset arrangement; here we use (tileWidth/2, tileHeight)
                    var origin = new Vector2(srcRect.Width / 2f, srcRect.Height);

                    spriteBatch.Draw(
                        texture: tex,
                        position: pos,
                        sourceRectangle: srcRect,
                        color: Color.White,
                        rotation: 0f,
                        origin: origin,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: 0);
                }
            }
        }
    }

    /// <summary>
    /// Convert tile coordinates to world (screen) position for diamond/isometric tiles.
    /// Returns the screen pixel position where the tile's bottom-center should be placed.
    /// Formula:
    ///  screenX = (tileX - tileY) * (tileWidth / 2)
    ///  screenY = (tileX + tileY) * (tileHeight / 2)
    /// </summary>
    public Vector2 TileToWorld(int tileX, int tileY)
    {
        float halfW = TileWidth / 2f;
        float halfH = TileHeight / 2f;
        float sx = (tileX - tileY) * halfW;
        float sy = (tileX + tileY) * halfH;
        return new Vector2(sx, sy);
    }

    /// <summary>
    /// Convert a world/screen position to tile coordinates.
    /// Attempts to take elevation into account by sampling candidate tiles and their elevations.
    /// Returns tile coordinates as Point and a fractional local position within the tile (0..1,0..1).
    /// </summary>
    public (Point tile, Vector2 local) WorldToTile(Vector2 worldPosition)
    {
        // Inverse of TileToWorld (ignoring elevation)
        float halfW = TileWidth / 2f;
        float halfH = TileHeight / 2f;

        float tx = (worldPosition.X / halfW + worldPosition.Y / halfH) / 2f;
        float ty = (worldPosition.Y / halfH - worldPosition.X / halfW) / 2f;

        int ix = (int)Math.Floor(tx);
        int iy = (int)Math.Floor(ty);

        // compute local coordinates inside tile (0..1)
        Vector2 tileTopLeft = TileToWorld(ix, iy);
        // For diamond tile, top-left of tile image is offset; we'll compute local using relative coordinates within diamond bounding box
        Vector2 local = new Vector2((worldPosition.X - tileTopLeft.X) / TileWidth, (worldPosition.Y - tileTopLeft.Y) / TileHeight);

        return (new Point(ix, iy), local);
    }

    /// <summary>
    /// Compute the base elevation of a tile (in elevation units) by reading tile properties or tilelayer object.
    /// Checks:
    ///  - per-tile tile properties (property "elevation")
    ///  - per-layer offset (layer.OffsetY) used elsewhere when drawing
    /// If tile has "slope" property, elevation is interpreted when sampling per-point via GetElevationAtPoint.
    /// </summary>
    public float GetTileBaseElevation(TileLayer layer, int tileX, int tileY)
    {
        var gid = layer.GetGlobalTileIDAtCoord(tileX, tileY);
        if (gid == 0) return 0f;

        uint localTileId;
        var tileset = _map.ResolveTilesetForGlobalTileID((uint)gid, out localTileId);
        if (tileset == null) return 0f;

        // Find the tile in the tileset by localTileId
        var tile = tileset.Tiles.Find(t => t.ID == localTileId);
        if (tile == null) return 0f;

        if (tile.TryGetProperty<IProperty>("elevation", out var evProp) && float.TryParse(evProp?.ToString(), out var ev))
            return ev;

        return 0f;
    }

    /// <summary>
    /// Given a tile's position and a point within the tile (localX/localY in 0..1),
    /// compute interpolated elevation using slope metadata from tile properties.
    /// Slope values supported (tile property "slope"): "none", "north", "south", "east", "west",
    /// "nw", "ne", "sw", "se" where the first listed corner is the high corner.
    /// </summary>
    public float GetElevationAtPoint(int tileX, int tileY, Vector2 localInTile /* 0..1 */)
    {
        // Find the tile gid from topmost visible tile in stacked layers at this tile coordinate
        //foreach (TileLayer layer in _map.Layers)
        for(int i = 0;i<_map.Layers.Count;i++)
        {
            var layer = (TileLayer)_map.Layers[i];
            if (!layer.Visible) continue;
            if (layer is not TileLayer tileLayer) continue;
            int gid = (int)layer.GetGlobalTileIDAtCoord(tileX, tileY);
            if (gid == 0) continue;

            uint localTileId;
            var tileset = _map.ResolveTilesetForGlobalTileID((uint)gid, out localTileId);
            if (tileset == null) continue;
            var tile = tileset.Tiles.Find(t => t.ID == localTileId);
            if (tile == null) continue;

            float baseElevation = 0f;
            if (tile.TryGetProperty<IProperty>("elevation", out var evProp) && float.TryParse(evProp?.ToString(), out var ev))
                baseElevation = ev;

            baseElevation = i;

            string slope = "none";
            if (tile.TryGetProperty<IProperty>("slope", out var s)) slope = s?.ToString()?.ToLowerInvariant() ?? "none";

            // localInTile: (0,0) is top-left of tile bounding box. For diamond, y grows downward.
            // For slope calculations we interpret local coords in tile pixel space mapped to [0..1].
            float u = Math.Clamp(localInTile.X, 0f, 1f);
            float v = Math.Clamp(localInTile.Y, 0f, 1f);
            float height = baseElevation;

            // simple slope models: change height across one axis
            switch (slope)
            {
                case "none":
                    return height;
                case "north":
                    // higher at north edge (v==0), lower at south edge (v==1)
                    return height + (1f - v);
                case "south":
                    return height + v;
                case "west":
                    return height + (1f - u);
                case "east":
                    return height + u;
                case "nw":
                    // high at northwest corner (u~0,v~0) low at southeast (u~1,v~1)
                    return height + (1f - (u + v) / 2f);
                case "se":
                    return height + ((u + v) / 2f);
                case "ne":
                    return height + (1f - ((1f - u) + v) / 2f);
                case "sw":
                    return height + (1f - (u + (1f - v)) / 2f);
                default:
                    return height;
            }
        }

        return 0f;
    }

    /// <summary>
    /// Returns the topmost tile gid at the given tile coords (first non-zero gid in layers from top to bottom).
    /// </summary>
    public int GetTopmostGid(int tileX, int tileY)
    {
        if (_map == null) return 0;
        for (int i = _map.Layers.Count - 1; i >= 0; i--)
        {
            var layer = _map.Layers[i] as TileLayer;
            if (layer == null) continue;
            int gid = (int)layer.GetGlobalTileIDAtCoord(tileX, tileY);
            if (gid != 0) return gid;
        }
        return 0;
    }
}