using DotTiled;
using MarioPlatformerStyleTest.Components;
using MarioPlatformerStyleTest.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// This system handles collisions between game characters with platforms
/// </summary>
internal class PlatformCollisionSystem : ISystem
{
    public World World { get; set; }
    
    private readonly MapService _mapService;
    private Entity _playerEntity;

    public PlatformCollisionSystem(World world, MapService mapService)
    {
        World = world;        
        _mapService = mapService;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // We'll need the player entity to find out where they are
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        _playerEntity = playerFilter.First();        
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the player component
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();        
        var OFFSET =0; // TODO

        playerComponent.IsOnTheGround = false;
        var newPos = playerComponent.Position + (playerComponent.Velocity * deltaTime);
        Rectangle newRect = CalculateBounds(newPos, OFFSET, playerComponent.Width, playerComponent.Height);
        var layer = _mapService.GetLayer();

        foreach (var collider in _mapService.GetSurroundingTileRectangles(layer, playerComponent.Position, playerComponent.Width, playerComponent.Height))
        {
            if (newPos.X != playerComponent.Position.X)
            {
                newRect = CalculateBounds(new(newPos.X, playerComponent.Position.Y), OFFSET, playerComponent.Width, playerComponent.Height);
                if (newRect.Intersects(collider))
                {
                    if (newPos.X > playerComponent.Position.X) newPos.X = collider.Left - playerComponent.Width + OFFSET;
                    else newPos.X = collider.Right - OFFSET;
                    continue;
                }
            }

            newRect = CalculateBounds(new(playerComponent.Position.X, newPos.Y), OFFSET, playerComponent.Width, playerComponent.Height);
            if (newRect.Intersects(collider))
            {
                if (playerComponent.Velocity.Y > 0)
                {
                    newPos.Y = collider.Top - playerComponent.Height;
                    playerComponent.IsOnTheGround = true;
                    playerComponent.Velocity.Y = 0;
                }
                else
                {
                    newPos.Y = collider.Bottom;
                    playerComponent.Velocity.Y = 0;
                }
            }
        }

        playerComponent.Position = newPos;
    }

    private static Rectangle CalculateBounds(Vector2 pos, int offset, int width, int height)
    {
        return new((int)pos.X + offset, (int)pos.Y, width - (2 * offset), height);
    }

    //public static List<Rectangle> GetNearestColliders(Rectangle bounds)
    //{
    //    int leftTile = (int)Math.Floor((float)bounds.Left / TILE_SIZE);
    //    int rightTile = (int)Math.Ceiling((float)bounds.Right / TILE_SIZE) - 1;
    //    int topTile = (int)Math.Floor((float)bounds.Top / TILE_SIZE);
    //    int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / TILE_SIZE) - 1;

    //    leftTile = MathHelper.Clamp(leftTile, 0, tiles.GetLength(1));
    //    rightTile = MathHelper.Clamp(rightTile, 0, tiles.GetLength(1));
    //    topTile = MathHelper.Clamp(topTile, 0, tiles.GetLength(0));
    //    bottomTile = MathHelper.Clamp(bottomTile, 0, tiles.GetLength(0));

    //    List<Rectangle> result = new();

    //    for (int x = topTile; x <= bottomTile; x++)
    //    {
    //        for (int y = leftTile; y <= rightTile; y++)
    //        {
    //            if (tiles[x, y] != 0) result.Add(Colliders[x, y]);
    //        }
    //    }

    //    return result;
    //}
}
