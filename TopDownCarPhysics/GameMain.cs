using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownCarPhysics.Entities;
using TopDownCarPhysics.Physics;

namespace TopDownCarPhysics
{
    /// <summary>
    /// This is just a simple little arcade style 2D top down car physics demo using Aether Physics engine and Monogame. It's
    /// not entirely accurate (and isn't aiming to be), its not simulating each wheel or something clever like that. Think of 
    /// this more as just a quick/fun car simulation that can be played about with by tweaking the physics and vehicle settings.
    /// </summary>
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

            // For the physics simulation to work correctly we need to indicate how many pixels
            // on the screen correspond to how many simulation units. So 'X' number of pixels
            // for 1 metre in the physics simulation. Our car is 64 pixels long, so if we say
            // the car is about 4 metres then 64/4 = 16. So 16 pixels will be 1 metre in the
            // physics simulation. The same thinking must also apply to other unit conversions!
            _physicsWorld.SetDisplayUnitToSimUnitRatio(16);

            // Create a basic player
            _player = new Player(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), _spriteBatch, _physicsWorld, Content);
            _player.LoadContent();

            // Create an opponent
            _opponent = new Opponent(new Vector2(150, 150), _spriteBatch, _physicsWorld, Content);
            _opponent.LoadContent();

            // Create 'edges' for the physics engine so we don't go off screen - basically a 'box' around the screen that the
            // vehicles will 'realistically' bounce/bump off when they hit it... well, sort of ;-)
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

            // Update the physics 'world'
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

            // Draw the player and the opponent
            _player.Draw();
            _opponent.Draw();

            // Finish drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
