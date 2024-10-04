using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DotTiledTest
{
    /// <summary>
    /// Simple test of the DotTiled 'Tiled' map loader. See https://dcronqvist.github.io/DotTiled/docs/quickstart.html
    /// </summary>
    public class GameMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private Map _tiledMap;
        private SpriteBatch _spriteBatch;
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

            var loader = Loader.Default();
            _tiledMap = loader.LoadMap($"{Content.RootDirectory}/test map.tmx");
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
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            // Begin drawing for sprites
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);
            
            // Get the first layer and the tileset            
            Tileset tileset = _tiledMap.Tilesets[0];
            TileLayer firstLayer = (TileLayer)_tiledMap.Layers[0];

            for (int y = 0; y < firstLayer.Height; y++)
            {
                for (int x = 0; x < firstLayer.Width; x++)
                {
                    uint tile = firstLayer.Data.Value.GlobalTileIDs.Value[(y * firstLayer.Width) + x];
                    if (tile == 0) continue; // If block is 0, i.e. air, then continue

                    var sourceRectangle = GetSourceRect(tileset, tile);
                    
                    _spriteBatch.Draw(
                        texture: _tilesetTexture,
                        position: new Vector2(x * tileset.TileWidth, y * tileset.TileHeight),
                        sourceRectangle: sourceRectangle,
                        color: Microsoft.Xna.Framework.Color.White);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private static Rectangle GetSourceRect(Tileset tileset, uint gid)
        {            
            var tileId = (int)gid - 1;

            var row = tileId / ((int)tileset.TileCount / (int)tileset.Columns);
            var column = tileId % (int)tileset.Columns;

            var tileWidth = (int)tileset.TileWidth;
            var tileHeight = (int)tileset.TileHeight;
            var x = tileWidth * column;
            var y = tileHeight * row;            

            return new Rectangle(x, y, tileWidth, tileHeight);                       
        }
    }
}
