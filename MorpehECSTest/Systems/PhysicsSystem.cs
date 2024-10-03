using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MorpehECSTest.Physics;
using Scellecs.Morpeh;

namespace MorpehECSTest.Systems;

/// <summary>
/// We need a system to 'update' the physics engine
/// </summary>
internal class PhysicsSystem : ISystem
{
    public World World { get; set; }

    private readonly GraphicsDevice _graphicsDevice;
    private readonly PhysicsWorld _physicsWorld;

    public PhysicsSystem(PhysicsWorld physicsWorld, GraphicsDevice graphicsDevice)
    {
        _physicsWorld = physicsWorld;
        _graphicsDevice = graphicsDevice;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // For this 'simulation' we don't use gravity so...
        _physicsWorld.Gravity = Vector2.Zero;

        // For the physics simulation to work correctly we need to indicate how many pixels
        // on the screen correspond to how many simulation units. So 'X' number of pixels
        // for 1 metre in the physics simulation. Our car is 64 pixels long, so if we say
        // the car is about 4 metres then 64/4 = 16. So 16 pixels will be 1 metre in the
        // physics simulation. The same thinking must also apply to other unit conversions!
        _physicsWorld.SetDisplayUnitToSimUnitRatio(16);

        // Create 'edges' for the physics engine so we don't go off screen - basically a 'box' around the screen that the
        // vehicles will 'realistically' bounce/bump off when they hit it... well, sort of ;-)
        var topLeft = new Vector2(0, 0);
        var topRight = new Vector2(_graphicsDevice.PresentationParameters.BackBufferWidth, 0);
        var bottomLeft = new Vector2(0, _graphicsDevice.PresentationParameters.BackBufferHeight);
        var bottomRight = new Vector2(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);

        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(topRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topRight), _physicsWorld.ToSimUnits(bottomRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(bottomLeft), _physicsWorld.ToSimUnits(bottomRight));
        _physicsWorld.CreateEdge(_physicsWorld.ToSimUnits(topLeft), _physicsWorld.ToSimUnits(bottomLeft));
    }

    public void OnUpdate(float deltaTime)
    {
        // Simulate our physics stuff ;-)
        _physicsWorld.Step(deltaTime);
    }
}
