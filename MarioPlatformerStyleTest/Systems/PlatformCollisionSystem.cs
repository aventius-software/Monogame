using MarioPlatformerStyleTest.Components;
using MarioPlatformerStyleTest.Services;
using Microsoft.Xna.Framework;
using Scellecs.Morpeh;
using System;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// This system handles collisions between game characters and platforms
/// </summary>
internal class PlatformCollisionSystem : ISystem
{
    public World World { get; set; }

    private Filter _filter;
    private readonly MapService _mapService;

    public PlatformCollisionSystem(World world, MapService mapService)
    {
        World = world;
        _mapService = mapService;
    }

    private static Rectangle CalculateBounds(Vector2 position, int offset, int width, int height)
    {
        return new((int)position.X + offset, (int)position.Y, width - (2 * offset), height);
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Build our filter
        _filter = World.Filter.With<CharacterComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        // Check platform collisions for each character and platforms. Credits for most of this go to gamedev quickie code which
        // I've taken and slightly modified, see https://github.com/LubiiiCZ/DevQuickie/tree/master/Quickie021-JumpingAndGravity
        foreach (var entity in _filter)
        {
            // Get the components
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var characterComponent = ref entity.GetComponent<CharacterComponent>();

            // On each update, reset the 'on ground' flag for this character
            characterComponent.IsOnTheGround = false;

            // Calculate the next position the player will be moved to (assuming we're not bumping into anything)
            var currentPosition = transformComponent.Position;
            var nextPosition = currentPosition + (transformComponent.Velocity * deltaTime);

            // Check all the tiles surrounding the player to see if we're going to be inside one with our next position change
            foreach (var tile in _mapService.GetSurroundingTileRectangles(currentPosition, transformComponent.Width, transformComponent.Height))
            {
                // If the players next position has changed in the x axis
                // then we check if they will be inside this current tile
                if (nextPosition.X != currentPosition.X)
                {
                    // They've moved in the x axis, so lets get a rectangle for the
                    // players next position in respect to the change in the x axis
                    var newPositionX = new Vector2(nextPosition.X, currentPosition.Y);
                    var newRectangleX = CalculateBounds(newPositionX, characterComponent.BoundryOffset, transformComponent.Width, transformComponent.Height);

                    // So will the player be inside this tile?
                    if (newRectangleX.Intersects(tile))
                    {
                        // Has the player moved right?
                        if (nextPosition.X > currentPosition.X)
                        {
                            // Yes, they've hit a tile to their right side, so adjust the players position
                            // to move them back outside the tile
                            nextPosition.X = tile.Left - transformComponent.Width + characterComponent.BoundryOffset;
                        }
                        else
                        {
                            // No, the player has moved left, so must have hit a tile to their left. Adjust
                            // the players position to move them back outside the tile
                            nextPosition.X = tile.Right - characterComponent.BoundryOffset;
                        }

                        continue;
                    }
                }

                // They've moved in the y axis, so lets get a rectangle for the
                // players next position in respect to the change in the y axis
                var newPositionY = new Vector2(currentPosition.X, (int)Math.Ceiling(nextPosition.Y));
                var newRectangleY = CalculateBounds(newPositionY, characterComponent.BoundryOffset, transformComponent.Width, transformComponent.Height);

                // Have we hit this this tile?
                if (newRectangleY.Intersects(tile))
                {
                    // Yes, are we falling down?
                    if (transformComponent.Velocity.Y > 0)
                    {
                        // Yes, we're falling down, so must have landed on a tile. Adjust the players
                        // position to place them outside the tile and set velocity to zero (i.e. we
                        // are now on the ground)
                        nextPosition.Y = tile.Top - transformComponent.Height;
                        characterComponent.IsOnTheGround = true;
                    }
                    else
                    {
                        // No, we're not falling downwards, we must have jumped and hit our head on a
                        // tile above us. Adjust the players position to move them back outside the tile
                        // and set velocity to zero so that they stop moving upwards and should just
                        // start falling after the next physics update
                        nextPosition.Y = tile.Bottom;
                    }

                    // Either way, we need to set the character velocity to zero (as we're either on the
                    // ground or have just hit a platform above our head)
                    transformComponent.Velocity.Y = 0;
                }
            }

            // Now update the players position
            transformComponent.Position = nextPosition;
        }
    }
}
