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
        //private Vector2 _position;
        private SpriteFont _font;
        
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
            _mapService = new DungeonMapService(_spriteBatch, Content, new ShapeDrawingService(GraphicsDevice), GraphicsDevice);
            _mapService.LoadTiledMap("test map.tmx", "test tile atlas");
            _mapService.SetRotationAngle(MapRotationAngle.None);
            _mapService.SetDrawOffset(new Vector2(200, 200));
            _mapService.AddBlockingTileID(2);
            _mapService.AddBlockingTileID(0);
            _mapService.MoveTo(1, 28);

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

                if (keyboard.IsKeyDown(Keys.Up))// && _mapService.Position.Y > 0)
                {
                    if (!_mapService.IsBlockedAbove()) _mapService.MoveUp();
                }
                else if (keyboard.IsKeyDown(Keys.Down))// && _mapService.Position.Y < _mapService.WorldHeightInTiles -1)
                {
                    if (!_mapService.IsBlockedBelow()) _mapService.MoveDown();
                }
                else if (keyboard.IsKeyDown(Keys.Left))// && _mapService.Position.X > 0)
                {
                    if (!_mapService.IsBlockedToTheLeft()) _mapService.MoveLeft();
                }
                else if (keyboard.IsKeyDown(Keys.Right))// && _mapService.Position.X < _mapService.WorldWidthInTiles-1)
                {
                    if (!_mapService.IsBlockedToTheRight()) _mapService.MoveRight();
                }

                if (keyboard.IsKeyDown(Keys.Q))
                {                    
                    _mapService.RotateAnticlockwise();
                }
                else if (keyboard.IsKeyDown(Keys.W))
                {                    
                    _mapService.RotateClockwise();
                }

                if (keyboard.IsKeyDown(Keys.NumPad0))
                {
                    _mapService.SetRotationAngle(MapRotationAngle.None);
                }

                if (keyboard.IsKeyDown(Keys.NumPad1))
                {
                    _mapService.SetRotationAngle(MapRotationAngle.Ninety);
                }

                if (keyboard.IsKeyDown(Keys.NumPad2))
                {
                    _mapService.SetRotationAngle(MapRotationAngle.OneHundredAndEighty);
                }

                if (keyboard.IsKeyDown(Keys.NumPad3))
                {
                    _mapService.SetRotationAngle(MapRotationAngle.TwoHundredAndSeventy);
                }
            }

            _timer--;

            _tile = _mapService.GetTileAtPosition();
            _frontTile = _mapService.GetTileAbove();
            _leftTile = _mapService.GetTileToTheLeft();
            _rightTile = _mapService.GetTileToTheRight();

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

            //_spriteBatch.Begin(
            //    sortMode: SpriteSortMode.Immediate,
            //    blendState: null,
            //    samplerState: SamplerState.PointClamp,
            //    depthStencilState: null,
            //    rasterizerState: null,
            //    effect: null,
            //    transformMatrix: null);

            //_mapService.DrawTileMap();

            //_spriteBatch.End();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);

            _spriteBatch.DrawString(_font, "Position: " + _mapService.Position.X.ToString() + "," + _mapService.Position.Y.ToString(), new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Standing on: " + _tile.ToString(), new Vector2(0, 30), Color.White);
            _spriteBatch.DrawString(_font, "In front: " + _frontTile.ToString(), new Vector2(0, 60), Color.White);
            _spriteBatch.DrawString(_font, "To the left: " + _leftTile.ToString(), new Vector2(0, 90), Color.White);
            _spriteBatch.DrawString(_font, "To the right: " + _rightTile.ToString(), new Vector2(0, 120), Color.White);
            _spriteBatch.DrawString(_font, "Rotation: " + _mapService.RotationAngle.ToString(), new Vector2(0, 150), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
