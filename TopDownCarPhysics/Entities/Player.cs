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
        LoadContent("car", _initialPosition);

        // Load a font
        _font = _contentManager.Load<SpriteFont>("font");
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
        if (keyboardState.IsKeyDown(Keys.T)) ReduceTraction();

        // Process/update this vehicles physics
        base.Update(gameTime);
    }

    public override void Draw()
    {
        // Draw some details about the car physics
        _spriteBatch.DrawString(_font, "Press/hold space to perform handbrake skid/turn", new Vector2(0, 0), Color.Black);
        _spriteBatch.DrawString(_font, $"Is drifting/skidding enabled: {IsDriftingEnabled} (press 'E' to enable, 'D' to disable)", new Vector2(0, 18), Color.Black);

        base.Draw();
    }
}
