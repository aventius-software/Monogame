using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownCarPhysics.Entities;
using TopDownCarPhysics.Physics;

namespace TopDownCarPhysics
{
    public class GameMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private Opponent _opponent;
        private PhysicsWorld _physicsWorld;
        private Player _player;
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

            // We'll need a physics world to simulate some car stuff
            _physicsWorld = new PhysicsWorld();
            _physicsWorld.Gravity = Vector2.Zero;
            _physicsWorld.SetDisplayUnitToSimUnitRatio(8);

            // Create a basic player
            _player = new Player(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), _spriteBatch, _physicsWorld, Content);
            _player.LoadContent();

            // Create an opponent
            _opponent = new Opponent(new Vector2(150, 150), _spriteBatch, _physicsWorld, Content);
            _opponent.LoadContent();

            // Create 'edges' for the physics engine so we don't go off screen - basically a 'box' around the screen ;-)
            var topLeft = new Vector2(0, 0);
            var topRight = new Vector2(_graphics.PreferredBackBufferWidth, 0);
            var bottomLeft = new Vector2(0, _graphics.PreferredBackBufferHeight);
            var bottomRight = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(topRight));
            _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topRight), _physicsWorld.ToSimUnits(bottomRight));
            _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(bottomLeft), _physicsWorld.ToSimUnits(bottomRight));
            _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(bottomLeft));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the player/opponent
            _player.Update(gameTime);
            _opponent.Update(gameTime);

            // Update physics
            _physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

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

            _player.Draw();
            _opponent.Draw();

            // Finish drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
