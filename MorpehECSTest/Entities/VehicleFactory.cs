using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MorpehECSTest.Components;
using MorpehECSTest.Physics;
using Scellecs.Morpeh;

namespace MorpehECSTest.Entities;

internal class VehicleFactory
{
    private readonly ContentManager _contentManager;
    private readonly World _ecsWorld;
    private readonly PhysicsWorld _physicsWorld;

    public VehicleFactory(World ecsWorld, ContentManager contentManager, PhysicsWorld physicsWorld)
    {
        _ecsWorld = ecsWorld;
        _contentManager = contentManager;
        _physicsWorld = physicsWorld;
    }

    public Entity Create(Vector2 position, float mass, bool isPlayer = false)
    {
        var entity = _ecsWorld.CreateEntity();

        entity.AddComponent<AngularDragComponent>();
        entity.AddComponent<DriftComponent>();
        entity.AddComponent<EngineComponent>();
        entity.AddComponent<LinearDragComponent>();
        entity.AddComponent<RigidBodyComponent>();
        entity.AddComponent<SpriteComponent>();
        entity.AddComponent<SteeringComponent>();
        entity.AddComponent<TransformComponent>();

        ref var sprite = ref entity.GetComponent<SpriteComponent>();

        if (isPlayer) sprite.Texture = _contentManager.Load<Texture2D>("car");
        else sprite.Texture = _contentManager.Load<Texture2D>("opponent");

        sprite.Origin = new Vector2(sprite.Texture.Width / 2, sprite.Texture.Height / 2);

        ref var angularDrag = ref entity.GetComponent<AngularDragComponent>();
        angularDrag.Drag = 4f;
        angularDrag.RateOfChange = 0.5f;

        ref var drift = ref entity.GetComponent<DriftComponent>();
        drift.DriftFactor = 0.92f;
        drift.SavedDriftFactor = 0;
        drift.IsDriftingEnabled = true;
        drift.IsSkidding = false;
        drift.SkiddingDriftFactor = 0.99f;

        ref var engine = ref entity.GetComponent<EngineComponent>();
        engine.EnginePower = 50f;
        engine.MaxForwardSpeed = 30f;
        engine.MaxReversingSpeed = 15f;

        ref var linearDrag = ref entity.GetComponent<LinearDragComponent>();
        linearDrag.Drag = 0.5f;
        linearDrag.RateOfChange = 3.5f;

        ref var steering = ref entity.GetComponent<SteeringComponent>();
        steering.TurnSpeed = 15f;

        ref var rigidBody = ref entity.GetComponent<RigidBodyComponent>();
        rigidBody.Body = _physicsWorld.CreateRigidBodyRectangle(
            position: position,
            widthInPixels: sprite.Texture.Width,
            heightInPixels: sprite.Texture.Height,
            mass: mass);

        // And if this entity is a/the player...
        if (isPlayer) entity.AddComponent<PlayerComponent>();

        return entity;
    }
}
