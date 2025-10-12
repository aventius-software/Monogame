using AetherPhysicsTest.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;

namespace AetherPhysicsTest;

/// <summary>
/// Just a simple MonoGame example showing how to use the Aether.Physics2D physics engine.
/// </summary>
public class GameMain : Game
{
    private Texture2D _circle;
    private GraphicsDeviceManager _graphics;
    private List<Body> _physicsBodies = [];
    private PhysicsWorld _physicsWorld;
    private SpriteBatch _spriteBatch;
    private Texture2D _square;

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

        // Load sprites
        _circle = Content.Load<Texture2D>("circle");
        _square = Content.Load<Texture2D>("square");

        // Create the physics 'world'
        _physicsWorld = new PhysicsWorld();

        // We'll use a pixels per metre value for the simulation (as the physics engine works in metres/kilograms/etc...)
        // that kind of feels right for motion on screen/window of 800 x 480 pixels. About 32 pixels per metre seems ok
        _physicsWorld.SetPixelsPerMetre(32);

        // We want normal gravity to make things fall
        _physicsWorld.Gravity = new Vector2(0, 9.8f);

        // Create 'edges' so stuff stays within the screen/window in the simulation
        _physicsWorld.CreateBoundry(
            _physicsWorld.ToSimUnits(0),
            _physicsWorld.ToSimUnits(0),
            _physicsWorld.ToSimUnits(_graphics.PreferredBackBufferWidth),
            _physicsWorld.ToSimUnits(_graphics.PreferredBackBufferHeight));

        // Now lets create some physics objects
        var rand = new Random();

        for (int i = 0; i < 20; i++)
        {
            // Alternately create a square then a circle so we've got a mix
            if (i % 2 == 0)
            {
                var x = rand.Next(_graphics.PreferredBackBufferWidth - _square.Width);
                var y = rand.Next(_graphics.PreferredBackBufferHeight - _square.Height);

                var body = _physicsWorld.CreateRectangle(
                    width: _physicsWorld.ToSimUnits(_square.Width),
                    height: _physicsWorld.ToSimUnits(_square.Height),
                    density: 1,
                    position: _physicsWorld.ToSimUnits(new Vector2(x, y)),
                    rotation: 0,
                    bodyType: BodyType.Dynamic);

                body.Tag = "square";
                _physicsBodies.Add(body);
            }
            else
            {
                var x = rand.Next(_graphics.PreferredBackBufferWidth - _circle.Width);
                var y = rand.Next(_graphics.PreferredBackBufferHeight - _circle.Height);

                var body = _physicsWorld.CreateCircle(
                    radius: _physicsWorld.ToSimUnits(_circle.Width / 2),
                    density: 1,
                    position: _physicsWorld.ToSimUnits(new Vector2(x, y)),
                    bodyType: BodyType.Dynamic);

                body.Tag = "circle";
                _physicsBodies.Add(body);
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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

        // Draw all our objects
        for (var i = 0; i < _physicsBodies.Count; i++)
        {
            // Get the physics body
            var body = _physicsBodies[i];

            // Is this one a square?
            if (body.Tag.ToString() == "square")
            {
                _spriteBatch.Draw(
                    texture: _square,
                    position: _physicsWorld.ToDisplayUnits(body.Position),
                    sourceRectangle: new Rectangle(0, 0, _square.Width, _square.Height),
                    color: Color.White,
                    rotation: body.Rotation,
                    origin: new Vector2(_square.Width / 2, _square.Height / 2),
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
            else
            {
                _spriteBatch.Draw(
                    texture: _circle,
                    position: _physicsWorld.ToDisplayUnits(body.Position),
                    sourceRectangle: new Rectangle(0, 0, _circle.Width, _circle.Height),
                    color: Color.White,
                    rotation: body.Rotation,
                    origin: new Vector2(_circle.Width / 2, _circle.Height / 2),
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
