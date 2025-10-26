using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using PlatformerWithTiledMapDemo.Map;
using Shared.Extensions;
using System;

namespace PlatformerWithTiledMapDemo.Shared.Physics;

internal class PlatformPhysicsSystem : EntityUpdateSystem
{
    private readonly MapService _mapService;

    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
    private ComponentMapper<PhysicsComponent> _physicsMapper;
    private ComponentMapper<Sprite> _spriteMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public PlatformPhysicsSystem(MapService mapService)
        : base(Aspect.All(typeof(PhysicsComponent), typeof(Transform2)).One(typeof(Sprite), typeof(AnimatedSprite)))
    {
        _mapService = mapService;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
        _physicsMapper = mapperService.GetMapper<PhysicsComponent>();
        _spriteMapper = mapperService.GetMapper<Sprite>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        var deltaTime = gameTime.GetElapsedSeconds();

        // Update each entity with physics e.g. gravity
        foreach (var entityId in ActiveEntities)
        {
            // Get required components
            var physicsComponent = _physicsMapper.Get(entityId);
            var transformComponent = _transformMapper.Get(entityId);

            // Get the sprite component (animated or static)
            var sprite = _animatedSpriteMapper.Has(entityId)
                ? _animatedSpriteMapper.Get(entityId)
                : _spriteMapper.Get(entityId);

            // For each entity, we always first reset the ground state
            physicsComponent.IsOnGround = false;

            // Apply gravity to the entity's vertical velocity and move it's
            // position based on its velocity (this will move the entity in
            // both the X and Y axis)
            physicsComponent.Velocity.Y += physicsComponent.Gravity * deltaTime;
            transformComponent.Position += physicsComponent.Velocity * deltaTime;

            // Get the entity's bounding rectangle for collision detection
            var entityBounds = GetEntityBounds(entityId);

            // Check if the entity is colliding with any of its surrounding platform tiles
            foreach (var platform in _mapService.GetSurroundingTiles(
                entityBounds.Position,
                entityBounds.Width,
                entityBounds.Height))
            {
                // Check if the entity is intersecting with this platform                
                var platformIntersectionDepth = entityBounds.GetIntersectionDepth(platform);

                // Skip if no penetration
                if (platformIntersectionDepth == Vector2.Zero)
                    continue;

                // Check shallowest penetration only
                if (Math.Abs(platformIntersectionDepth.Y) < Math.Abs(platformIntersectionDepth.X))
                {
                    // Reposition the entity outside the platform
                    transformComponent.Position += new Vector2(0, platformIntersectionDepth.Y);
                    physicsComponent.Velocity.Y = 0;

                    // If we penetrated downwards, we are on the ground
                    if (platformIntersectionDepth.Y < 0)
                        physicsComponent.IsOnGround = true;

                    // Since we've repositioned the entity, we must update the entity bounds
                    // otherwise further plaforms collision checking will be checking with
                    // the entities old position
                    entityBounds = GetEntityBounds(entityId);

                    // Skip further processing for this platform
                    continue;
                }

                // If we're here then we must be intersecting more on the horizontal axis, so just like
                // with the vertical axis, we reposition the entity outside the platform
                transformComponent.Position += new Vector2(platformIntersectionDepth.X, 0);
                physicsComponent.Velocity.X = 0;

                // Since we've repositioned the entity, we must update the entity bounds
                // otherwise further plaforms collision checking will be checking with
                // the entities old position
                entityBounds = GetEntityBounds(entityId);
            }
        }
    }

    /// <summary>
    /// Gets the current bounds of an entity based on its transform, sprite size, and physics collision box offsets.
    /// </summary>
    /// <param name="entityId"></param>
    /// <returns></returns>
    private RectangleF GetEntityBounds(int entityId)
    {
        // Get required components
        var physicsComponent = _physicsMapper.Get(entityId);
        var transformComponent = _transformMapper.Get(entityId);
        var sprite = _animatedSpriteMapper.Has(entityId)
            ? _animatedSpriteMapper.Get(entityId)
            : _spriteMapper.Get(entityId);

        // Calculate the sprite's bounding rectangle
        var spriteBounds = new RectangleF(transformComponent.Position, new SizeF(sprite.Size.X, sprite.Size.Y));

        // Apply the physics component's collision box offsets to get the final bounds. This is because
        // the collision box may be smaller or offset from the sprite's actual size.
        return spriteBounds.GetRelativeRectangle(
            physicsComponent.CollisionBoxOffsetBounds.X,
            physicsComponent.CollisionBoxOffsetBounds.Y,
            physicsComponent.CollisionBoxOffsetBounds.Width,
            physicsComponent.CollisionBoxOffsetBounds.Height);
    }
}
