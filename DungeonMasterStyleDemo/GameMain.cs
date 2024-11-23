using DungeonMasterStyleDemo.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace DungeonMasterStyleDemo
{
    public class GameMain : Game
    {        
        private GraphicsDeviceManager _graphics;
        private DungeonMapService _mapService;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _position;
        private SpriteFont _font;

        const int _wallTileId = 2;
        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _position = new Vector2(1, 8);
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
            _font = Content.Load<SpriteFont>("font");
        }

        int _tile;
        int _frontTile;
        int _leftTile;
        int _rightTile;
        int _timer;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (_timer <= 0)
            {
                _timer = 5;
                var keyboard = Keyboard.GetState();
                var direction = Vector2.Zero;

                if (keyboard.IsKeyDown(Keys.Up) && _position.Y > 0)
                {
                    if (_mapService.GetTileAtPosition((int)_position.Y - 1, (int)_position.X) != _wallTileId)
                        _position.Y--;
                }
                else if (keyboard.IsKeyDown(Keys.Down) && _position.Y < _mapService.WorldHeightInTiles -1)
                {
                    if (_mapService.GetTileAtPosition((int)_position.Y + 1, (int)_position.X) != _wallTileId)
                        _position.Y++;
                }
                else if (keyboard.IsKeyDown(Keys.Left) && _position.X > 0)
                {
                    if (_mapService.GetTileAtPosition((int)_position.Y, (int)_position.X - 1) != _wallTileId)
                        _position.X--;
                }
                else if (keyboard.IsKeyDown(Keys.Right) && _position.X < _mapService.WorldWidthInTiles-1)
                {
                    if (_mapService.GetTileAtPosition((int)_position.Y, (int)_position.X + 1) != _wallTileId)
                        _position.X++;
                }
            }

            _timer--;    
            
            _tile = _mapService.GetTileAtPosition((int)_position.Y, (int)_position.X);
            _frontTile = _mapService.GetTileAtPosition((int)_position.Y - 1, (int)_position.X);
            _leftTile = _mapService.GetTileAtPosition((int)_position.Y, (int)_position.X - 1);
            _rightTile = _mapService.GetTileAtPosition((int)_position.Y, (int)_position.X + 1);

            _mapService.SetTilePosition((int)_position.X, (int)_position.Y);

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

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);

            _spriteBatch.DrawString(_font, "Position: " + _position.X.ToString() + "," + _position.Y.ToString(), new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Standing on: " + _tile.ToString(), new Vector2(0, 30), Color.White);
            _spriteBatch.DrawString(_font, "In front: " + _frontTile.ToString(), new Vector2(0, 60), Color.White);
            _spriteBatch.DrawString(_font, "To the left: " + _leftTile.ToString(), new Vector2(0, 90), Color.White);
            _spriteBatch.DrawString(_font, "To the right: " + _rightTile.ToString(), new Vector2(0, 120), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
