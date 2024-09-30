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
        LoadContent("opponent", _initialPosition);        
    }    
}
