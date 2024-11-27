using DungeonMasterStyleDemo.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonMasterStyleDemo
{
    public class GameMain : Game
    {
        private int _tileBehind, _currentTile, _tileInFront, _tileToTheLeft, _tileToTheRight;
        private GraphicsDeviceManager _graphics;
        private DungeonMapService _dungeonMapService;
        private SpriteFont _font;
        private int _inputDelayTimer;
        private SpriteBatch _spriteBatch;

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
            var shapeDrawingService = new ShapeDrawingService(GraphicsDevice);

            // Load the Tiled map
            _dungeonMapService = new DungeonMapService(_spriteBatch, Content, shapeDrawingService, GraphicsDevice);
            _dungeonMapService.LoadTiledMap("test map.tmx", "test tile atlas");
            _dungeonMapService.SetRotationAngle(MapRotationAngle.None);
            _dungeonMapService.SetDrawOffset(new Vector2(200, 200));
            _dungeonMapService.AddBlockingTileID(2);
            _dungeonMapService.AddBlockingTileID(0);

            // Must start on a clear block!
            _dungeonMapService.MoveTo(1, 28);

            // Load the font
            _font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Use a timer to slow down the input
            if (_inputDelayTimer <= 0)
            {
                // Reset timer
                _inputDelayTimer = 5;

                // Get the current keyboard state
                var keyboard = Keyboard.GetState();

                // Move around the map
                if (keyboard.IsKeyDown(Keys.Up))
                {
                    if (!_dungeonMapService.IsBlockedAbove()) _dungeonMapService.MoveUp();
                }
                else if (keyboard.IsKeyDown(Keys.Down))
                {
                    if (!_dungeonMapService.IsBlockedBelow()) _dungeonMapService.MoveDown();
                }
                else if (keyboard.IsKeyDown(Keys.Left))
                {
                    if (!_dungeonMapService.IsBlockedToTheLeft()) _dungeonMapService.MoveLeft();
                }
                else if (keyboard.IsKeyDown(Keys.Right))
                {
                    if (!_dungeonMapService.IsBlockedToTheRight()) _dungeonMapService.MoveRight();
                }

                // Rotate the map
                if (keyboard.IsKeyDown(Keys.Q))
                {
                    _dungeonMapService.RotateAnticlockwise();
                }
                else if (keyboard.IsKeyDown(Keys.W))
                {
                    _dungeonMapService.RotateClockwise();
                }
            }

            // Reduce the timer
            _inputDelayTimer--;

            // Get the tiles types around the players position
            _currentTile = _dungeonMapService.GetTileAtPosition();
            _tileInFront = _dungeonMapService.GetTileAbove();
            _tileBehind = _dungeonMapService.GetTileBelow();
            _tileToTheLeft = _dungeonMapService.GetTileToTheLeft();
            _tileToTheRight = _dungeonMapService.GetTileToTheRight();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawDungeon();
            DrawInfo();

            base.Draw(gameTime);
        }

        public void DrawDungeon()
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);

            _dungeonMapService.Draw();

            _spriteBatch.End();
        }

        public void DrawInfo()
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);

            _spriteBatch.DrawString(_font, "Current map position: " + _dungeonMapService.Position.X.ToString() + "," + _dungeonMapService.Position.Y.ToString(), new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Standing on: " + _currentTile.ToString(), new Vector2(0, 30), Color.White);
            _spriteBatch.DrawString(_font, "To the front: " + _tileInFront.ToString(), new Vector2(0, 60), Color.White);
            _spriteBatch.DrawString(_font, "To the rear: " + _tileBehind.ToString(), new Vector2(0, 90), Color.White);
            _spriteBatch.DrawString(_font, "To the left: " + _tileToTheLeft.ToString(), new Vector2(0, 120), Color.White);
            _spriteBatch.DrawString(_font, "To the right: " + _tileToTheRight.ToString(), new Vector2(0, 150), Color.White);
            _spriteBatch.DrawString(_font, "Map rotation angle: " + _dungeonMapService.RotationAngle.ToString(), new Vector2(0, 180), Color.White);

            _spriteBatch.End();
        }

        public void DrawTileMap()
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: null,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null);

            _dungeonMapService.DrawTileMap();

            _spriteBatch.End();
        }
    }
}
