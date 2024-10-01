using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using TopDownCarPhysics.Physics;

namespace TopDownCarPhysics.Entities;

internal abstract class Vehicle
{
    #region Injected services

    private readonly ContentManager _contentManager;
    private readonly PhysicsWorld _physicsWorld;
    private readonly SpriteBatch _spriteBatch;

    #endregion

    #region Private fields

    private Vector2 _origin;
    private Texture2D _texture;

    #endregion

    #region Protected fields

    protected Vector2 InputDirection = Vector2.Zero;

    #endregion

    #region Protected properties

    protected bool IsDriftingEnabled => _isDriftingEnabled;

    #endregion

    #region Vehicle physics

    private float _angularDrag;
    private float _angularDragRateOfChange;
    private float _driftFactor;
    private float _enginePower;
    private bool _isDriftingEnabled = true;
    private float _linearDrag;
    private float _linearDragRateOfChange;
    private float _maxReversingSpeed;
    private float _maxForwardSpeed;
    private Body _physicsBody;
    private float _savedDriftFactor;
    private float _turnSpeed;

    #endregion

    #region Maths helpers

    private Vector2 _forwardVector => new((float)Math.Cos(_physicsBody.Rotation), (float)Math.Sin(_physicsBody.Rotation));
    private Vector2 _forwardVelocity => _forwardVector * Vector2.Dot(_physicsBody.LinearVelocity, _forwardVector);
    private Vector2 _rightVector => new((float)Math.Cos(_physicsBody.Rotation + Math.PI / 2), (float)Math.Sin(_physicsBody.Rotation + Math.PI / 2));
    private Vector2 _rightVelocity => _rightVector * Vector2.Dot(_physicsBody.LinearVelocity, _rightVector);

    #endregion

    #region Constructors

    protected Vehicle(SpriteBatch spriteBatch, PhysicsWorld physicsWorld, ContentManager contentManager)
    {
        _spriteBatch = spriteBatch;
        _physicsWorld = physicsWorld;
        _contentManager = contentManager;
    }

    #endregion

    #region Protected methods

    protected void InitialisePhysics(
        Vector2 startingPosition, 
        float mass = 1f, 
        float turnSpeed = 15f, 
        float driftFactor = 0.92f, 
        bool enableDrifting = true)
    {
        // Create a physics body to simulate the vehicle
        _physicsBody = _physicsWorld.CreateRectangle(
            width: _physicsWorld.ToSimUnits(_texture.Width),
            height: _physicsWorld.ToSimUnits(_texture.Height),
            density: 1,
            position: _physicsWorld.ToSimUnits(startingPosition),
            rotation: MathHelper.ToRadians(0),
            bodyType: BodyType.Dynamic);

        // Set some default mass
        _physicsBody.Mass = mass;

        // Set some others if you like...
        _angularDrag = 4f;
        _angularDragRateOfChange = 0.5f;
        _driftFactor = driftFactor;
        _enginePower = 50f;
        _isDriftingEnabled = enableDrifting;
        _linearDrag = 0.5f;
        _linearDragRateOfChange = 3.5f;
        _maxReversingSpeed = 15f;
        _maxForwardSpeed = 30f;        
        _savedDriftFactor = 0f;
        _turnSpeed = turnSpeed;
    }

    protected void LoadContent(string texturePath)
    {
        // Set up the texture for the sprite
        _texture = _contentManager.Load<Texture2D>(texturePath);

        // Set the origin to be the centre of the car
        _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);        
    }

    #endregion

    #region Public methods

    public virtual void Draw()
    {
        // Draw this vehicle
        _spriteBatch.Draw(
            texture: _texture,
            position: _physicsWorld.ToDisplayUnits(_physicsBody.Position),
            sourceRectangle: null,
            color: Color.White,
            rotation: _physicsBody.Rotation,
            origin: _origin,
            scale: 1,
            effects: SpriteEffects.None,
            layerDepth: 0);
    }

    public virtual void Update(GameTime gameTime)
    {
        // Apply physics to the vehicle
        ApplyEngineForce(gameTime);
        KillOrthogonalVelocity();
        ApplySteering(gameTime);

        // Reset skidding drift factor
        UpdateDriftFactor();
    }

    #endregion

    #region Physics methods

    private void ApplyEngineForce(GameTime gameTime)
    {
        // Calculate the forward speed (i.e. not including sideways speed)
        var forwardSpeed = Vector2.Dot(_physicsBody.LinearVelocity, _forwardVector);

        // If we've reached max forward speed, do nothing more - just return...
        if (forwardSpeed > _maxForwardSpeed && InputDirection.Y > 0) return;
        if (forwardSpeed < -_maxReversingSpeed && InputDirection.Y < 0) return;
        if (_physicsBody.LinearVelocity.LengthSquared() > _maxForwardSpeed * _maxForwardSpeed && InputDirection.Y > 0) return;

        // If the car is accelerating or braking we don't apply any drag/friction (yes, not totally accurate), but if
        // the car isn't accelerating or braking we do apply linear 'drag' (friction) so it will bring the car to a
        // halt if no input (accelerate/brake) is pressed
        if (InputDirection.Y == 0)
        {
            // We're NOT accelerating or braking, so apply a 'gradual' drag to slow us down and bring the vehicle to a halt
            _physicsBody.LinearDamping = MathHelper.Lerp(_physicsBody.LinearDamping, _linearDrag, _linearDragRateOfChange * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        else
        {
            // We ARE accelerating or braking, so don't apply any drag
            _physicsBody.LinearDamping = 0;
        }

        // Finally apply the engine force in the 'forward' direction
        _physicsBody.ApplyForce(_forwardVector * _enginePower * InputDirection.Y);
    }

    private void ApplySteering(GameTime gameTime)
    {
        // Steers left or right depending on the X direction
        if (InputDirection.X == 0)
        {
            // We're not turning left or right, so slowly reduce the angular damping so that the vehicle doesn't keep turning forever
            _physicsBody.AngularDamping = MathHelper.Lerp(_physicsBody.AngularDamping, _angularDrag, _angularDragRateOfChange * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        else
        {
            // We're turning one way or the other, so apply some angular drag
            _physicsBody.AngularDamping = _angularDrag;
        }

        // Apply steering to (increase) angular velocity, note we take into account the forward/linear velocity
        // as we want to turn less if we are moving forward/reverse slower. A very slow moving car doesn't have
        // a high turning speed
        _physicsBody.AngularVelocity += InputDirection.X * MathHelper.ToRadians(_turnSpeed) * (Math.Abs(_physicsBody.LinearVelocity.Length()) / _maxForwardSpeed);
    }
    
    private void KillOrthogonalVelocity()
    {
        // If drifting is enabled... otherwise set to 0 (no drift, basically driving on rails ;-)
        var driftFactor = _isDriftingEnabled ? _driftFactor : 0;

        // Reduce 'sideways' velocity depending on drift factor
        _physicsBody.LinearVelocity = _forwardVelocity + (_rightVelocity * driftFactor);
    }

    private void UpdateDriftFactor()
    {
        // Reset skidding drift factor
        if (_savedDriftFactor != 0)
        {
            _driftFactor = _savedDriftFactor;
            _savedDriftFactor = 0;
        }
    }

    #endregion

    #region Vehicle control    

    /// <summary>
    /// Disable drifting
    /// </summary>
    public void DisableDrifting() => _isDriftingEnabled = false;

    /// <summary>
    /// Enable drifting
    /// </summary>
    public void EnableDrifting() => _isDriftingEnabled = true;

    /// <summary>
    /// Improve the traction of the vehicle
    /// </summary>
    /// <param name="factor"></param>
    public void ImproveTraction(float factor = 0.01f)
    {
        if (_driftFactor > 0.91f) _driftFactor -= factor;
    }

    /// <summary>
    /// Reduce the traction of the vehicle
    /// </summary>
    /// <param name="factor"></param>
    public void ReduceTraction(float factor = 0.01f)
    {
        if (_driftFactor < 0.98f) _driftFactor += factor;
    }

    /// <summary>
    /// Calling this lets the vehicle skid/drift by setting a high drift factor. Note
    /// that if it is not called in the next update then drift factor will be returned
    /// to its previous value. This might be useful if you have a 'drift key' which
    /// the player keeps pressed in order to drift, but then stops drifting if the key
    /// is released
    /// </summary>
    public void Skid()
    {
        // Save previous drift factor before skidding if we've not saved it already
        if (_savedDriftFactor != _driftFactor) _savedDriftFactor = _driftFactor;

        // Set high drift factor
        _driftFactor = 0.99f;
    }

    #endregion
}