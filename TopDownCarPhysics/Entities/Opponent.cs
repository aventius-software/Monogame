using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDownCarPhysics.Physics;

namespace TopDownCarPhysics.Entities;

internal class Opponent : Vehicle
{
    private readonly ContentManager _contentManager;
    private readonly Vector2 _initialPosition;
    private readonly SpriteBatch _spriteBatch;

    public Opponent(Vector2 initialPosition, SpriteBatch spriteBatch, PhysicsWorld physicsWorld, ContentManager contentManager)
        : base(spriteBatch, physicsWorld, contentManager)
    {
        _initialPosition = initialPosition;
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
    }

    public void LoadContent()
    {
        // Load and initialise the vehicle
        LoadContent("opponent");

        // Initialise the vehicle physics, we'll make the opponent vehicle heavier than
        // the player. If the opponent is heavier, it doesn't deflect as much in collisions with the
        // player. However, if we make the opponent lighter then it deflects much easier when the
        // player collides with it. Try some different values and see what happens ;-)
        //
        // Drift factor: a value closer to '1' means more 'slippy', start at '0.9' for decent grip. We'll start
        // with it a little slippy at 0.96 so it can slide around when the player hits it
        InitialisePhysics(_initialPosition, mass: 1.5f, turnSpeed: 15f, driftFactor: 0.96f, enableDrifting: true);
    }
}
