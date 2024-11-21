using DungeonMasterStyleDemo.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonMasterStyleDemo
{
    public class GameMain : Game
    {        
        private GraphicsDeviceManager _graphics;
        private DungeonMapService _mapService;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
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
            _position = new Vector2(1, 9);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the Tiled map
            _mapService = new DungeonMapService(_spriteBatch, Content, new ShapeDrawingService(GraphicsDevice), GraphicsDevice);
            _mapService.LoadTiledMap("test map.tmx", "test tile atlas");
            _mapService.SetRotationAngle(MapRotationAngle.None);

            _texture = Content.Load<Texture2D>("character");
        }

        int _tile;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var keyboard = Keyboard.GetState();
            var direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.Up))
            {
                _position.Y--;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            { 
                _position.Y++; 
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                _position.X--;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                _position.X++;
            }
                        
            _tile = _mapService.GetTileAtPosition((int)_position.Y, (int)_position.X);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

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
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
