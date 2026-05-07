using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using System;

namespace OpenTTDStyleIsometricMap.Services;

/// <summary>
/// A basic 2D isometric tile map service in the style of OpenTTD, which supports tile slopes and 
/// height. It provides methods for rendering the map and calculating tile map/world coordinates 
/// based on mouse position.
/// 
/// Note - For drawing at different elevations, you should set each tile layer with the Y offset which 
/// corresponds to the height of your tile multiplied by the layer index, otherwise it will not get rendered 
/// correctly (i.e. each layer will get rendered at the same elevation) and will also fail to calculate tile 
/// map/world coordinate correctly.
/// 
/// Art:
/// https://opengameart.org/content/terrain-renderer
/// 
/// Some useful references:
/// 
/// https://clintbellanger.net/articles/isometric_math/
/// https://erikonarheim.com/posts/handling-height-in-isometric/
/// https://discourse.mapeditor.org/t/half-height-isometric-maps/4545/8
/// https://stackoverflow.com/questions/21842814/mouse-position-to-isometric-tile-including-height 
/// https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
/// https://gamedev.stackexchange.com/questions/207056/selecting-tiles-with-mouse-on-isometric-map-with-height-and-slopes
/// https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
/// https://www.gamedev.net/reference/articles/article2026.asp
/// https://github.com/OpenTTD/OpenTTD/blob/master/src/landscape.cpp
/// </summary>
internal class IsometricMapService
{
    private readonly OrthographicCamera _camera;
    private readonly ContentManager _contentManager;
    private readonly SpriteBatch _spriteBatch;

    private Vector2 _activeScreenPosition;
    private Texture2DAtlas _atlas;
    private Tile[,,] _map;
    private int _tileWidth, _tileHeight, _layerDepth;
    private SizeF _tileSize;
    private Matrix _translationMatrix;

    public IsometricMapService(ContentManager contentManager, SpriteBatch spriteBatch, OrthographicCamera camera)
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;
        _camera = camera;
    }

    public void Draw()
    {
        // Draw from the bottom up so that tiles at higher elevations
        // get drawn on top of those at lower elevations
        for (int elevation = 0; elevation < _map.GetLength(0); elevation++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(2); x++)
                {
                    // Get the tile at this map position
                    var tile = _map[elevation, y, x];

                    // Skip if no tile at this position
                    if (tile.SlopeType == SlopeType.NONE)
                        continue;

                    // Skip if the tile is outside of the camera view, otherwise we end up doing
                    // a lot of unnecessary drawing and calculations for tiles that aren't even
                    // visible. Note - this is a simple check to see if the center of the tile
                    // is within the camera view, but you could make this more complex if you
                    // wanted to check if any part of the tile is within the camera view (e.g.
                    // using Intersects instead of Contains)
                    if (_camera.Contains(tile.Bounds.Center) == ContainmentType.Intersects)
                        continue;

                    // Tile is in the view, so draw it
                    DrawTile(tile.SlopeType, tile.Bounds.TopLeft, tile.IsHighlighted);
                }
            }
        }
    }

    public void DrawTile(SlopeType slopeType, Vector2 position, bool selected)
    {
        // If the tile is selected/active then we draw it in a different
        // colour to highlight it, otherwise we draw it in white
        var colour = selected ? Color.Red : Color.White;

        // Get the source rectangle for this tile type from the atlas and draw it
        // at the specified position
        var sourceRect = _atlas.GetRegion((int)slopeType);
        _spriteBatch.Draw(sourceRect, position, colour);
    }

    public void LoadTileTextureAtlas(string textureAtlasPath, int tileWidth = 64, int tileHeight = 48, int layerDepth = 8)
    {
        // Store the tile dimensions and layer depth for use in drawing and coordinate calculations
        _tileWidth = tileWidth;
        _tileHeight = tileHeight;
        _tileSize = new SizeF(_tileWidth, _tileHeight);
        _layerDepth = layerDepth;

        // Load the tile texture atlas and create a Texture2DAtlas instance to manage the tile regions
        // within the atlas. The atlas is expected to be arranged in a grid where each tile type corresponds
        // to a specific region based on its SlopeType value (e.g., SLOPE_FLAT = 0 corresponds to the first
        // tile region, SLOPE_W = 4 corresponds to the fifth tile region, and so on). The tileWidth and
        // tileHeight parameters are used to calculate the size of each tile region within the atlas.
        var texture = _contentManager.Load<Texture2D>(textureAtlasPath);
        _atlas = Texture2DAtlas.Create("TileAtlas", texture, tileWidth, tileHeight);

        var w = tileWidth / 2;
        var h = tileHeight / 3;

        // We'll use a matrix to translate coordinates, since translating to and from screen/map coordinates
        // is a simple transform and inversion of the matrix. See the link below for some of the maths behind this...
        // https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
        _translationMatrix = new Matrix(
            m11: w, m21: -w, m31: 0, m41: 0,
            m12: h, m22: h, m32: 0, m42: 0,
            m13: 0, m23: 0, m33: 1, m43: 0,
            m14: 0, m24: 0, m34: 0, m44: 1
        );
    }

    public void SetMap(Tile[,,] map) => _map = map;

    public void Update()
    {
        // Start at the lowest elevation and loop through each tile, calculating the
        // screen position and bounds for each tile, and checking if the active/select
        // position is over the tile to determine if it should be highlighted. We also
        // check if the tile is hidden under tiles at higher elevations, and if so we
        // set it to NONE so it doesn't get drawn by the 'Draw' method.
        for (int elevation = 0; elevation < _map.GetLength(0); elevation++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(2); x++)
                {
                    // Get the tile at this map position                    
                    var tile = _map[elevation, y, x];

                    // Skip if no tile at this position
                    if (tile.SlopeType == SlopeType.NONE)
                        continue;

                    // Get the screen position of the tile so we can calculate if the mouse is over it
                    var tileScreenPosition = MapToScreenCoordinates(new Vector2(x, y), elevation);

                    // Set the tile bounds based on the screen position and tile size, and also set the world position
                    tile.Bounds = new RectangleF(tileScreenPosition, _tileSize);
                    tile.WorldPosition = new Vector3(x, y, elevation);

                    // Default to NOT highlighted, but we'll check this below based on the active/select position
                    tile.IsHighlighted = false;

                    // Check if the active/select position is over this tiles drawing rectangle, and
                    // if so check we then check if the pixel (in the source texture) under the position
                    // is transparent or not. If its NOT transparent, then we're over the actual tile
                    // pixels not just the tile bounds, so we can highlight the tile to show it is
                    // selected/active
                    if (tile.Bounds.Contains(_activeScreenPosition))
                    {
                        // The tile rectangle contains the position... but now we also
                        // need to check if the pixel at the specified position inside
                        // the tile texture is part of the tile or just the background
                        // transparent area, since the tile textures are not rectangular
                        // (e.g. they are diamond shaped) and we only want to highlight
                        // the tile if we're actually over the tile pixels, not just the
                        // tiles rectangular bounds
                        tile.IsHighlighted = PixelIsInsideTheTile(
                            tile.SlopeType,
                            (int)(_activeScreenPosition.X - tileScreenPosition.X),
                            (int)(_activeScreenPosition.Y - tileScreenPosition.Y));
                    }

                    // Check if this tile is hidden under tiles at higher elevations
                    bool isHidden = false;

                    for (int higherElevation = elevation + 1; higherElevation < _map.GetLength(0); higherElevation++)
                    {
                        var checkY = (int)MathF.Max(0, y + elevation);
                        var tileAbove = _map[higherElevation, checkY, x];

                        if (tileAbove.SlopeType != SlopeType.NONE)
                        {
                            var tileAboveScreenPosition = MapToScreenCoordinates(new Vector2(x, checkY), higherElevation);
                            var boundsAbove = new RectangleF(tileAboveScreenPosition, _tileSize);

                            // Check if this tile's bounds are completely covered by the tile above
                            bool completelyContained =
                                tile.Bounds.X >= boundsAbove.X &&
                                tile.Bounds.Y >= boundsAbove.Y &&
                                (tile.Bounds.X + tile.Bounds.Width) <= (boundsAbove.X + boundsAbove.Width) &&
                                (tile.Bounds.Y + tile.Bounds.Height) <= (boundsAbove.Y + boundsAbove.Height);

                            if (completelyContained)
                            {
                                isHidden = true;
                                break;
                            }
                        }
                    }

                    if (isHidden)
                        tile.SlopeType = SlopeType.NONE;

                    // Update the tile in the map with the new properties (e.g., bounds, world position, highlighted state)
                    _map[elevation, y, x] = tile;
                }
            }
        }
    }

    /// <summary>
    /// Set the active screen coordinates (e.g., from the mouse position) which can 
    /// then be used to calculate which tile is under the mouse position for example
    /// </summary>
    /// <param name="screenPosition"></param>
    public void SetActiveScreenCoordinates(Vector2 screenPosition)
    {
        _activeScreenPosition = _camera.ScreenToWorld(screenPosition);
    }

    /// <summary>
    /// Determines whether the specified pixel within a tile's texture region is 
    /// part of the tile or just a transparent background 'pixel'
    /// </summary>
    /// <remarks>
    /// Out-of-bounds coordinates are treated as transparent. This method checks the 
    /// alpha channel of the pixel to determine transparency.
    /// </remarks>
    /// <param name="slopeType">The type of slope used to select the corresponding tile region within the texture atlas.</param>
    /// <param name="textureCoordinateX">The horizontal coordinate, relative to the tile's region, of the pixel to check. Must be greater than or equal
    /// to 0 and less than the width of the tile region.</param>
    /// <param name="textureCoordinateY">The vertical coordinate, relative to the tile's region, of the pixel to check. Must be greater than or equal to
    /// 0 and less than the height of the tile region.</param>
    /// <returns>true if the specified pixel is fully transparent or the coordinates are out of bounds; otherwise, false.</returns>    
    private bool PixelIsInsideTheTile(SlopeType slopeType, int textureCoordinateX, int textureCoordinateY)
    {
        // Get the region of the texture atlas for this tile
        var sourceRect = _atlas.GetRegion((int)slopeType);

        // Check if the x,y coordinates are within the bounds of the tile
        if (textureCoordinateX < 0 || textureCoordinateX >= sourceRect.Width || textureCoordinateY < 0 || textureCoordinateY >= sourceRect.Height)
        {
            // Treat out-of-bounds as outside the tile
            return false;
        }

        // Calculate the absolute position in the atlas texture
        var absoluteX = sourceRect.X + textureCoordinateX;
        var absoluteY = sourceRect.Y + textureCoordinateY;

        // Extract the pixel data from the atlas texture
        var texture = _atlas.Texture;
        var pixelData = new Color[texture.Width * texture.Height];
        texture.GetData(pixelData);

        // Get the pixel color at the absolute position
        var pixelIndex = absoluteY * texture.Width + absoluteX;
        Color pixelColor = pixelData[pixelIndex];

        // Return true if the pixel is inside the tile (alpha != 0), false otherwise
        return pixelColor.A != 0;
    }

    /// <summary>
    /// Translates map coordinates to screen coordinates taking elevation into account
    /// </summary>
    /// <param name="mapCoordinates"></param>
    /// <param name="elevation"></param>
    /// <returns></returns>
    private Vector2 MapToScreenCoordinates(Vector2 mapCoordinates, int elevation)
    {
        // Use the matrix to transform the map coordinates into screen coordinates
        var screenCoordinates = Vector2.Transform(mapCoordinates, _translationMatrix) + _camera.Origin;

        // In isometric maps the origin point is at the middle of a tile, so to correct this
        // we offset the x coordinate by half a tile width
        screenCoordinates.X -= _tileWidth / 2;

        // Taking elevation into account since on screen, higher elevation means smaller Y screen
        // coordinate. So, we just need multiples of the layer depth (or offset) between layers
        // depending on the elevation level (e.g. 1,2,3 and so on...)
        screenCoordinates.Y -= elevation * _layerDepth;

        // Done...
        return screenCoordinates;
    }
}
