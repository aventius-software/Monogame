using MarioPlatformerStyleTest.Components;
using MarioPlatformerStyleTest.Services;
using Microsoft.Xna.Framework;
using Scellecs.Morpeh;
using System;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// This system handles simple physics such as gravity and related stuff 
/// like collisions with the world (i.e. tiles or platforms). Note that it
/// doesn't handle collisions between characters and other characters
/// </summary>
internal class PhysicsSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;
    private float _gravity;
    private readonly MapService _mapService;

    public PhysicsSystem(World world, MapService mapService)
    {
        World = world;
        _mapService = mapService;
    }

    /// <summary>
    /// Credits for this function go to the gamedev quickie code which I've taken from this link
    /// here https://github.com/LubiiiCZ/DevQuickie/tree/master/Quickie021-JumpingAndGravity
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private static Rectangle CalculateBounds(Vector2 position, int offset, int width, int height)
    {
        return new((int)position.X + offset, (int)position.Y, width - (2 * offset), height);
    }

    /// <summary>
    /// This helps the player make fine adjustments to their jump. If they are holding
    /// down the jump button then they make a normal jump, but if they just tap and 
    /// release the jump button, we make the character do a smaller jump
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private static float CalculateGravityAdjustmentForPlayer(Entity entity)
    {
        // Get the components we need
        ref var playerComponent = ref entity.GetComponent<PlayerComponent>();
        ref var transformComponent = ref entity.GetComponent<TransformComponent>();

        // If the player is jumping...
        if (transformComponent.IsMovingUpwards && !playerComponent.IsJumpPressed)
        {
            // If the player is jumping and NOT holding the jump button, reduce
            // the jump a bit quicker so they just do a small/low jump. Then
            // if they are holding the jump button they'll do a normal 'full' jump
            return playerComponent.LowJumpGravityMultiplier;
        }
        else if (transformComponent.IsMovingDownwards)
        {
            // When falling, we can tweak the fall speed a little to make
            // the player drop faster than normal gravity would make them...
            return playerComponent.FallingGravityMultiplier;
        }
        else
        {
            // If we're here then either the player is holding the jump button
            // down (in which case, we do a normal jump). So, return a multiplier
            // of '1' which will make NO change to the gravity value
            return 1f;
        }
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Build our filter
        _filter = World.Filter.With<TransformComponent>().Build();

        // Set initial gravity
        _gravity = 500f;
    }

    public void OnUpdate(float deltaTime)
    {
        // Apply physics (like gravity) to all entities
        foreach (var entity in _filter)
        {
            // We may want to modify gravity for some characters
            var modifiedGravity = _gravity;

            // Special case for the player character, we make some adjustments to normal gravity
            if (entity.Has<PlayerComponent>()) modifiedGravity *= CalculateGravityAdjustmentForPlayer(entity);

            // Update the characters velocity in the Y axis according to gravity
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();            
            transformComponent.Velocity.Y += modifiedGravity * deltaTime;

            // Update the characters position
            UpdateCharacterPosition(entity, deltaTime);
        }
    }

    /// <summary>
    /// Check platform collisions for each character and platforms. Credits for most of this function 
    /// go to the gamedev quickie code which I've taken and slightly modified a little bit, see 
    /// this link https://github.com/LubiiiCZ/DevQuickie/tree/master/Quickie021-JumpingAndGravity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deltaTime"></param>
    public void UpdateCharacterPosition(Entity entity, float deltaTime)
    {
        // Check platform collisions for each character and platforms                   
        ref var transformComponent = ref entity.GetComponent<TransformComponent>();
        ref var characterComponent = ref entity.GetComponent<CharacterComponent>();

        // On each update, reset the 'on ground' flag for this character
        characterComponent.IsOnTheGround = false;

        // Calculate the next position the character will be moved to
        var currentPosition = transformComponent.Position;
        var nextPosition = currentPosition + (transformComponent.Velocity * deltaTime);

        // Check all the tiles surrounding the character to see if we're going to be inside one with our next position change
        foreach (var tile in _mapService.GetSurroundingTileRectangles(currentPosition, transformComponent.Width, transformComponent.Height))
        {
            // If the characters next position has changed in the x axis
            // then we check if they will be inside this current tile
            if (nextPosition.X != currentPosition.X)
            {
                // They've moved in the x axis, so lets get a rectangle for the
                // characters next position in respect to the change in the x axis
                var newPositionX = new Vector2(nextPosition.X, currentPosition.Y);
                var newRectangleX = CalculateBounds(newPositionX, characterComponent.BoundryOffset, transformComponent.Width, transformComponent.Height);

                // So will the character be inside this tile?
                if (newRectangleX.Intersects(tile))
                {
                    // If we're here, the character is inside this tile. So, has the character moved right?
                    if (nextPosition.X > currentPosition.X)
                    {
                        // Yes, they've hit a tile to their right side, so adjust the characters
                        // position to move them back outside the tile
                        nextPosition.X = tile.Left - transformComponent.Width + characterComponent.BoundryOffset;
                    }
                    else
                    {
                        // The character has moved left, so they must have hit a tile to their
                        // left. Adjust the characters position to move them back outside the tile
                        nextPosition.X = tile.Right - characterComponent.BoundryOffset;
                    }
                }
            }

            // They've moved in the y axis, so lets get a rectangle for the
            // characters next position in respect to the change in the y axis
            var newPositionY = new Vector2(currentPosition.X, (int)Math.Ceiling(nextPosition.Y));
            var newRectangleY = CalculateBounds(newPositionY, characterComponent.BoundryOffset, transformComponent.Width, transformComponent.Height);

            // Have we hit this this tile?
            if (newRectangleY.Intersects(tile))
            {
                // Yes, are we falling down?
                if (transformComponent.Velocity.Y > 0)
                {
                    // Yes, we're falling down, so must have landed on a tile. Adjust the characters
                    // position to place them outside the tile and set velocity to zero (i.e. we
                    // are now on the ground)
                    nextPosition.Y = tile.Top - transformComponent.Height;
                    characterComponent.IsOnTheGround = true;
                }
                else
                {
                    // No, we're not falling downwards, we must have jumped and hit our head on a
                    // tile above us. Adjust the characters position to move them back outside the tile
                    // and set velocity to zero so that they stop moving upwards and should just
                    // start falling after the next physics update
                    nextPosition.Y = tile.Bottom;
                }

                // Either way, we need to set the character velocity to zero (as we're either on
                // the ground or have just hit a platform above our head)
                transformComponent.Velocity.Y = 0;
            }
        }

        // Now we can update the character position
        transformComponent.Position = nextPosition;
    }
}
