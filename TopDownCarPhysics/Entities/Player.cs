using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownCarPhysics.Physics;

namespace TopDownCarPhysics.Entities;

internal class Player : Vehicle
{
    private readonly ContentManager _contentManager;
    private SpriteFont _font;
    private readonly Vector2 _initialPosition;
    private readonly SpriteBatch _spriteBatch;

    public Player(Vector2 initialPosition, SpriteBatch spriteBatch, PhysicsWorld physicsWorld, ContentManager contentManager)
        : base(spriteBatch, physicsWorld, contentManager)
    {
        _initialPosition = initialPosition;
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
    }

    public void LoadContent()
    {
        // Load and initialise the vehicle
        LoadContent("car");

        // Load a font
        _font = _contentManager.Load<SpriteFont>("font");

        // Initialise the vehicle physics, we'll make the player vehicle lighter than
        // the opponent vehicle. If the opponent is heavier, it doesn't deflect much in
        // collisions with the player. However, if we make the opponent lighter (or the
        // player heavier than the opponent) then the opponent car deflects much easier. Try
        // some different values and see what happens ;-)
        //
        // Drift factor: closer to '1' means more 'slippy', start at '0.9' for decent grip
        InitialisePhysics(_initialPosition, mass: 1f, turnSpeed: 15f, driftFactor: 0.92f, enableDrifting: true);
    }

    public override void Update(GameTime gameTime)
    {
        // Get keyboard state
        var keyboardState = Keyboard.GetState();

        // Remember to reset the input direction on each update!
        InputDirection = Vector2.Zero;

        // Acceleration and braking
        if (keyboardState.IsKeyDown(Keys.Up)) InputDirection.Y = 1;
        else if (keyboardState.IsKeyDown(Keys.Down)) InputDirection.Y = -1;

        // Turning
        if (keyboardState.IsKeyDown(Keys.Left)) InputDirection.X = -1;
        else if (keyboardState.IsKeyDown(Keys.Right)) InputDirection.X = 1;

        // Press space to skid/drift ;-)
        if (keyboardState.IsKeyDown(Keys.Space)) Skid();

        // Enable/disable drifting/skidding (when disabled it will turn like its on rails)
        if (keyboardState.IsKeyDown(Keys.D)) DisableDrifting();
        if (keyboardState.IsKeyDown(Keys.E)) EnableDrifting();

        // Change the traction/skid control level
        if (keyboardState.IsKeyDown(Keys.I)) ImproveTraction();
        if (keyboardState.IsKeyDown(Keys.R)) ReduceTraction();

        // Process/update this vehicles physics
        base.Update(gameTime);
    }

    public override void Draw()
    {
        // Draw some details about the car physics
        _spriteBatch.DrawString(_font, "Press the arrow keys to turn, brake and accelerate", new Vector2(0, 0), Color.Black);
        _spriteBatch.DrawString(_font, "Press 'I' to improve traction, 'R' to reduce traction", new Vector2(0, 18), Color.Black);
        _spriteBatch.DrawString(_font, "Press/hold space to perform handbrake skid/turn", new Vector2(0, 36), Color.Black);
        _spriteBatch.DrawString(_font, $"Drifting/skidding enabled -> {IsDriftingEnabled} (press 'E' to enable, 'D' to disable)", new Vector2(0, 54), Color.Black);        

        base.Draw();
    }
}
