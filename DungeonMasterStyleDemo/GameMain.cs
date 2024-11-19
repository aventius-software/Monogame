using DungeonMasterStyleDemo.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonMasterStyleDemo
{
    public class GameMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private MapService _mapService;
        private SpriteBatch _spriteBatch;
        private Vector2 _position;

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

            // Load the Tiled map
            _mapService = new MapService(_spriteBatch, Content);
            _mapService.LoadTiledMap("test map.tmx", "test tile atlas");
            _mapService.SetRotationAngle(MapRotationAngle.None);
        }

        int _tile;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _position = new Vector2(0, 2);

            _tile = _mapService.GetTileAtPosition((int)_position.Y, (int)_position.X);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start the sprite batch, note that we're using our camera to set the transform matrix
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);

            _mapService.Draw();

            _mapService.DrawTile((int)_position.Y, (int)_position.X, new Vector2(0, 400));
            _mapService.DrawTile((int)_position.Y - 1, (int)_position.X, new Vector2(0, 400 - 32));
            _mapService.DrawTile((int)_position.Y - 2, (int)_position.X, new Vector2(0, 400 - 64));

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
