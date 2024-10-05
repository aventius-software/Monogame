using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using TiledCS;

namespace TiledCSTest;

/// <summary>
/// A quick test of TiledCS to load and deal with Tiled maps, see https://github.com/TheBoneJarmer/TiledCS and
/// although its no longer actively maintained it still functions well (at the time of writing) with some
/// features lacking in other .NET Tiled libraries
/// </summary>
public class GameMain : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private TiledMap _tileMap;
    private Dictionary<int, TiledTileset> _tilesets;
    private Texture2D _tilesetTexture;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Tiled maps aren't part of the content pipeline so we have to load
        // them 'manually' like shown below. Some Tiled libs (like Monogame.Extended)
        // provide a content pipeline processor, but this requires some 'configuration' so
        // as long as you're not storing your assets in another project this works ok...
        _tileMap = new TiledMap($"{Content.RootDirectory}/test map.tmx");
        _tilesets = _tileMap.GetTiledTilesets($"{Content.RootDirectory}/");

        // Tile atlas is part of the content pipeline so we can load as normal
        _tilesetTexture = Content.Load<Texture2D>("test tile atlas");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin drawing for sprites
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: null);

        // Get the first layer
        var firstLayer = _tileMap.Layers.First(x => x.type == TiledLayerType.TileLayer);

        // Loop through the rows and columns
        for (int row = 0; row < firstLayer.height; row++)
        {
            for (int column = 0; column < firstLayer.width; column++)
            {
                // Fetch the tile at the row/column position we're at ;-)
                var gid = firstLayer.data[(row * firstLayer.width) + column];

                // Gid 0 is used to tell there is no tile set, so don't draw anything. Or 
                // do something else, whatever you would like ;-)
                if (gid == 0)
                {
                    continue;
                }

                // Helper method to fetch the right TieldMapTileset instance. This is a connection object
                // Tiled uses for linking the correct tileset to the gid value using the firstgid property
                var mapTileset = _tileMap.GetTiledMapTileset(gid);

                // Retrieve the actual tileset based on the firstgid property of the connection
                // object we retrieved just now
                var tileset = _tilesets[mapTileset.firstgid];

                // Use the connection object as well as the tileset to figure out the source rectangle
                var tileSourceRectangle = _tileMap.GetSourceRect(mapTileset, tileset, gid);

                // Create source rectangle to get the tile from the texture
                var sourceRectangle = new Rectangle(tileSourceRectangle.x, tileSourceRectangle.y, tileSourceRectangle.width, tileSourceRectangle.height);

                // Draw this tile
                _spriteBatch.Draw(
                    texture: _tilesetTexture,
                    position: new Vector2(column * tileset.TileWidth, row * tileset.TileHeight),
                    sourceRectangle: sourceRectangle,
                    color: Color.White);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
