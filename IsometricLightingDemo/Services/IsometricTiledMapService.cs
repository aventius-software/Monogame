using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Color = Microsoft.Xna.Framework.Color;

namespace IsometricLightingDemo.Services;

internal struct IsometricLightSource
{
    public Color Colour;
    public Vector3 Position;    
    public float Strength;
}

/// <summary>
/// A basic 2D isometric tile map service that uses Tiled maps
/// </summary>
internal class IsometricTiledMapService
{
    public Point Origin { get; set; } = Point.Zero;
    public int TileMapDepth => _tiledMap.Layers.Count;
    public int TileMapHeight => (int)_tiledMap.Height;
    public int TileMapWidth => (int)_tiledMap.Width;
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }

    private RenderTarget2D _backgroundRenderTarget;
    private readonly ContentManager _contentManager;
    private Effect _emptyShader;
    private Texture2D _lightMask;
    private Effect _lightingShader;
    private IsometricLightSource[] _lightSources;
    private RenderTarget2D _lightSourcesRenderTarget;
    private Vector3 _selectedTile;
    private readonly SpriteBatch _spriteBatch;
    private int _tileBlockHeight;
    private int _tileBlockWidth;
    private Map _tiledMap;
    private Texture2D _tilesetNormalMapTexture;
    private Texture2D _tilesetTexture;
    private Matrix _translationMatrix;
    private Matrix _transformationMatrixInverted;

    public IsometricTiledMapService(ContentManager contentManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;
    }

    private static float AngleBetween(Vector2 from, Vector2 to)
    {
        // Calculate the angle (radians of course) between 2 vectors
        return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
    }

    /// <summary>
    /// Draw the map
    /// </summary>
    public void Draw(Matrix transformMatrix)
    {
        // Draw background to the 'background' render target
        _spriteBatch.GraphicsDevice.SetRenderTarget(_backgroundRenderTarget);
        _spriteBatch.GraphicsDevice.Clear(Color.Black);
                
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: transformMatrix);

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

                    if (
                        ((int)_selectedTile.X >= x - 2 && (int)_selectedTile.X <= x + 2) &&
                        ((int)_selectedTile.Y >= y - 2 && (int)_selectedTile.Y <= y + 2) &&
                        _selectedTile.Z == elevation
                    )
                    { 
                        var from = new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, _spriteBatch.GraphicsDevice.Viewport.Height / 2);
                        //var from = new Vector2(_selectedTile.X - 1, _selectedTile.Y - 1);
                        var to = new Vector2(_lightSources[0].Position.X, _lightSources[0].Position.Y);

                        //var angle = AngleBetween(from, to) + MathHelper.ToRadians(90);
                        var angle = MathHelper.ToRadians(45);
                        Vector2 dir = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                        dir.Normalize();
                        Vector3 lightDirection = new Vector3(dir.X, dir.Y, 0f);
                        lightDirection.Normalize();


                        _lightingShader.Parameters["LightDirection"].SetValue(lightDirection);
                        _lightingShader.Parameters["LightColour"].SetValue(new Vector3(1f, 1f, 1f));
                        _lightingShader.Parameters["AmbientColour"].SetValue(new Vector3(1f, 1f, 1f) * 0.25f);
                        _lightingShader.Parameters["NormalMapTexture"].SetValue(_tilesetNormalMapTexture);

                        _lightingShader.CurrentTechnique.Passes[0].Apply();
                    }
                    else
                    {
                        _emptyShader.CurrentTechnique.Passes[0].Apply();
                    }

                    // Get the correct tile 'image' rectangle from the tileset 'atlas' image
                    var sourceRectangle = GetImageSourceRectangleForTile(tile);

                    // Draw the map tile at this position and elevation                    
                    _spriteBatch.Draw(
                        texture: _tilesetTexture,
                        position: MapToScreenCoordinates(new Point(x, y), elevation).ToVector2(),
                        sourceRectangle: sourceRectangle,
                        color: colour);
                }
            }
        }

        _spriteBatch.End();
        _spriteBatch.GraphicsDevice.SetRenderTarget(null);

        // Draw light sources
        //_spriteBatch.GraphicsDevice.SetRenderTarget(_lightSourcesRenderTarget);
        //_spriteBatch.GraphicsDevice.Clear(Color.Black);
        //_spriteBatch.Begin();

        //foreach (var lightSource in _lightSources)
        //{
        //    _spriteBatch.Draw(
        //        texture: _lightMask,
        //        position: MapToScreenCoordinates(new Point((int)lightSource.Position.X, (int)lightSource.Position.Y), (int)lightSource.Position.Z).ToVector2(),
        //        sourceRectangle: null,
        //        color: Color.White,
        //        rotation: 0,
        //        origin: new Vector2(_lightMask.Width / 2, _lightMask.Height / 2),
        //        scale: 1f,
        //        effects: SpriteEffects.None,
        //        layerDepth: 0);            
        //}

        //_spriteBatch.End();
        //_spriteBatch.GraphicsDevice.SetRenderTarget(null);

        // Pass parameters to our shader... note that the sprite batch 'draw' further down
        // below will pass the background render target 'texture' as the first texture parameter
        // to the shader, so we don't need to explicitly pass this parameter. We do need to pass
        // the light sources render target 'texture' though as the 2nd parameter ;-)
        //_lightingShader.Parameters["LightSourcesTexture"].SetValue(_lightSourcesRenderTarget);

        // If you want to give some light to the whole background, set this between 0 and 1
        // depending on the strength of the light. A value of 0 will make the background pitch
        // black, but a value of 1 will make the background completely visible and no light
        // sources will have any effect or be seen...
        //_lightingShader.Parameters["BackgroundAmbientLightStrength"].SetValue(0.15f);

        // Now, we draw the background render target to the screen, with our shader applied. The shader
        // will apply the 'lighting' by mixing the light sources texture pixels colours with the background
        // texture pixels colours and hey presto, we've got some 'fake' 2D lighting ;-)
        _spriteBatch.Begin();
        _spriteBatch.Draw(texture: _backgroundRenderTarget, position: Vector2.Zero, color: Color.White);
        _spriteBatch.End();
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

        var row = tileId / ((int)tileset.TileCount / (int)tileset.Columns);
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
    /// <param name="z"></param>
    /// <returns></returns>
    private int GetTileAtPosition(int x, int y, int z)
    {
        var tileLayer = GetLayer(z);

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
    /// <param name="screenCoordinates"></param>
    /// <returns></returns>
    public Vector3 HighlightTile(Point screenCoordinates)
    {
        _selectedTile = ScreenToMapCoordinates(screenCoordinates);

        return _selectedTile;
    }

    /// <summary>
    /// Load a Tiled map
    /// </summary>
    /// <param name="tiledMapPath"></param>
    /// <param name="tileAtlasName"></param>
    public void LoadTiledMap(string tiledMapPath, string tileAtlasName, string tileAtlasNormalMap, string lightMask)
    {
        var loader = Loader.Default();
        _tiledMap = loader.LoadMap(_contentManager.RootDirectory + "/" + tiledMapPath);
        _tilesetTexture = _contentManager.Load<Texture2D>(tileAtlasName);
        _lightMask = _contentManager.Load<Texture2D>(lightMask);

        // Load the 'normal map' for the tileset images that we'll use for lighting
        _tilesetNormalMapTexture = _contentManager.Load<Texture2D>(tileAtlasNormalMap);

        // Set tile dimensions
        var tileset = _tiledMap.Tilesets[0];
        _tileBlockWidth = (int)tileset.TileWidth;
        _tileBlockHeight = (int)tileset.TileHeight;

        // Set world dimensions
        WorldWidth = (int)_tiledMap.Width * _tileBlockWidth;
        WorldHeight = (int)_tiledMap.Height * _tileBlockHeight;

        // Get dimensions for a 'flat' version of our tile 'block'. If you end up using flat tiles then
        // the height part of this below would need to be changed appropriately
        var w = _tileBlockWidth / 2;
        var h = _tileBlockHeight / 4;

        // We'll use a matrix to translate coordinates, since translating to and from screen/map coordinates
        // is a simple transform and inversion of the matrix. See the link below for some of the maths behind this...
        // https://gamedev.stackexchange.com/questions/34787/how-to-convert-mouse-coordinates-to-isometric-indexes/34791#34791
        _translationMatrix = new Matrix(
            m11: w, m21: -w, m31: 0, m41: 0,
            m12: h, m22: h, m32: 0, m42: 0,
            m13: 0, m23: 0, m33: 1, m43: 0,
            m14: 0, m24: 0, m34: 0, m44: 1
        );

        // Get the inverse of the matrix to translate coordinates back
        _transformationMatrixInverted = Matrix.Invert(_translationMatrix);

        // Load our shader and a radial gradient texture which will act as a kind
        // of mask when we blend the background 'render target' and the light
        // sources 'render target' together to produce the final background
        _lightingShader = _contentManager.Load<Effect>("Shaders/normal map lighting shader");
        _emptyShader = _contentManager.Load<Effect>("Shaders/empty shader");
        
        // Create our render targets for the screen and another for all light sources        
        _backgroundRenderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, _spriteBatch.GraphicsDevice.Viewport.Width, _spriteBatch.GraphicsDevice.Viewport.Height);
        _lightSourcesRenderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, _spriteBatch.GraphicsDevice.Viewport.Width, _spriteBatch.GraphicsDevice.Viewport.Height);
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

        // As we're using a tile 'block' (i.e. a cube) instead of a flat tile the block is half the height
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
            if (GetTileAtPosition((int)offsetPosition.X, (int)offsetPosition.Y, z) > 0)
            {
                // Then it must be obscuring our original tile, hence we must select this
                // tile as the one at the screen position
                return new Vector3((int)offsetPosition.X, (int)offsetPosition.Y, z);
            }
        }

        // If we reached here, the best match was the original tile at zero elevation
        return flatMapCoordinates;
    }

    public void SetLightSources(IsometricLightSource[] lightSources)
    {
        _lightSources = lightSources;
    }
}
