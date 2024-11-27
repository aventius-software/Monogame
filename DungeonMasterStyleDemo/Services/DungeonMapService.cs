using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonMasterStyleDemo.Services;

/// <summary>
/// A really rough dungeon (or maze?) map and map drawing service. You'd probably want
/// to improve things by maybe using either textures or sprites for walls instead of
/// coloured polygons. Also, everything is sized around the size of the viewport, so
/// you'd also probably want (maybe) to size things different so side/front walls look
/// similar in size depending on viewport size (or some other size)
/// </summary>
internal class DungeonMapService : MapService
{    
    private readonly int _depthPerspectiveBottomY = 20;    
    private readonly int _depthPerspectiveTopY = 10;
    private readonly int _depthPerspectiveX = 40;
    private readonly int _depthToDraw = 6;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly int _horizontalBlocksToDraw = 12; // Should calculate this dynamically really
    private readonly ShapeDrawingService _shapeDrawingService;

    public DungeonMapService(SpriteBatch spriteBatch, ContentManager contentManager, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice) : base(spriteBatch, contentManager)
    {
        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    /// <summary>
    /// Draw the tile map
    /// </summary>
    public void DrawTileMap()
    {
        base.Draw();
    }

    /// <summary>
    /// Draw the dungeon
    /// </summary>
    public override void Draw()
    {
        // First we draw the floor and ceiling first
        for (var depthOffset = _depthToDraw; depthOffset >= 0; depthOffset--)
        {
            DrawFloor(depthOffset);
            DrawCeiling(depthOffset);
        }

        // Now we draw the dungeon walls, draw from furthest away first, up to closest
        for (var depthOffset = _depthToDraw; depthOffset >= 0; depthOffset--)
        {
            // For each depth layer (further to closer) we draw a horizontal line of 'walls'
            for (var mapOffsetX = -_horizontalBlocksToDraw; mapOffsetX <= _horizontalBlocksToDraw; mapOffsetX++)
            {
                // Get map 'offset' coordinates relative to our actual world/map position                
                var mapDrawPositionX = (int)RotatedPosition.X + mapOffsetX;
                var mapDrawPositionDepth = (int)RotatedPosition.Y - depthOffset;

                // Find if the tiles around this position are 'blocking' tile types
                var currentBlock = IsBlockingTile(mapDrawPositionX, mapDrawPositionDepth);
                var blockInFront = IsBlockingTile(mapDrawPositionX, mapDrawPositionDepth - 1);
                var blockToTheLeft = IsBlockingTile(mapDrawPositionX - 1, mapDrawPositionDepth);
                var blockToTheRight = IsBlockingTile(mapDrawPositionX + 1, mapDrawPositionDepth);

                // If there is a block (wall) to the left of the player, draw a left side wall
                if (blockToTheLeft && mapOffsetX <= 0)
                {
                    DrawLeftSideWall(depthOffset, mapOffsetX);
                }

                // If there is a block (wall) to the right of the player, draw a right side wall
                if (blockToTheRight && mapOffsetX >= 0)
                {
                    DrawRightSideWall(depthOffset, mapOffsetX);
                }

                // If there is a block (wall) in front of the player draw a front facing wall
                if (blockInFront && !currentBlock)
                {
                    DrawFrontFacingWall(depthOffset, mapOffsetX);
                }
            }
        }
    }

    /// <summary>
    /// Draws the ceiling of the dungeon
    /// </summary>
    /// <param name="depth"></param>
    private void DrawCeiling(int depth)
    {
        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(Color.Red, Color.DarkRed, GetDepthPercentage(depth));

        // Draw rectangle at this 'depth'
        _shapeDrawingService.DrawFilledRectangle(colour,
            0, depth * _depthPerspectiveTopY,
            _graphicsDevice.Viewport.Width, _depthPerspectiveTopY);
    }

    /// <summary>
    /// Draws the floor of the dungeon
    /// </summary>
    /// <param name="depth"></param>
    private void DrawFloor(int depth)
    {
        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(Color.Green, Color.DarkGreen, GetDepthPercentage(depth));

        // Draw rectangle at this 'depth'
        _shapeDrawingService.DrawFilledRectangle(colour,
            0, _graphicsDevice.Viewport.Height - (_depthPerspectiveBottomY + (_depthPerspectiveBottomY * depth)),
            _graphicsDevice.Viewport.Width, _depthPerspectiveBottomY);
    }

    /// <summary>
    /// Draw a front facing wall
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="offsetX"></param>
    private void DrawFrontFacingWall(int depth, int offsetX)
    {
        // Draw the wall of the block in front, not the
        // current block depth! ;-)
        depth++;

        // Scale the default wall rectangle down to the relevant
        // size for the depth we're drawing at        
        var scaledX = _depthPerspectiveX * depth;
        var scaledY = _depthPerspectiveTopY * depth;
        var scaledWidth = _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
        var scaledHeight = _graphicsDevice.Viewport.Height - ((_depthPerspectiveTopY + _depthPerspectiveBottomY) * depth);

        // Now we can create a new front facing wall with the correct
        // size we want for the depth we're drawing at. Also as we'll
        // need to draw front facing walls at different x coordinates
        // we'll use an 'offset' to draw wall faces at different multiples
        // of the scaled width depending on the value of this x offset
        var scaledWallFace = new Rectangle(
            x: scaledX + (offsetX * scaledWidth),
            y: scaledY,
            width: scaledWidth,
            height: scaledHeight
        );

        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(Color.LightBlue, Color.DarkBlue, GetDepthPercentage(depth));

        // Finally draw the scaled (and horizonally offset) front facing wall
        _shapeDrawingService.DrawFilledRectangle(colour, scaledWallFace.X, scaledWallFace.Y, scaledWallFace.Width, scaledWallFace.Height);
    }

    /// <summary>
    /// Draws a wall to the left
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="offsetX"></param>
    private void DrawLeftSideWall(int depth, int offsetX)
    {
        // Build 'default' quad coordinates, as if the wall
        // was just to the side of the player (not taking
        // distance/depth into account...
        var topLeft = new Vector2(0, 0);
        var topRight = new Vector2(_depthPerspectiveX, _depthPerspectiveTopY);
        var bottomRight = new Vector2(_depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveBottomY);
        var bottomLeft = new Vector2(0, _graphicsDevice.Viewport.Height);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X + (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveTopY * depth));
        var scaledTopRight = new Vector2(topRight.X + (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveTopY * depth));
        var scaledBottomRight = new Vector2(bottomRight.X + (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveBottomY * depth));
        var scaledBottomLeft = new Vector2(bottomLeft.X + (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveBottomY * depth));

        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(new Color(200, 200, 200), new Color(50, 50, 50), GetDepthPercentage(depth));
        var scaledWallWidth = GetBlockHorizontalWidth(depth);

        _shapeDrawingService.DrawFilledQuadrilateral(colour,
            (int)scaledTopLeft.X + (scaledWallWidth * offsetX),
            (int)scaledTopLeft.Y,
            (int)scaledTopRight.X + (scaledWallWidth * offsetX),
            (int)scaledTopRight.Y,
            (int)scaledBottomRight.X + (scaledWallWidth * offsetX),
            (int)scaledBottomRight.Y,
            (int)scaledBottomLeft.X + (scaledWallWidth * offsetX),
            (int)scaledBottomLeft.Y);
    }

    /// <summary>
    /// Draws a wall to the right
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="offsetX"></param>
    private void DrawRightSideWall(int depth, int offsetX)
    {
        // Build 'default' quad coordinates, as if the wall
        // was just to the side of the player (not taking
        // distance/depth into account...
        var topLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _depthPerspectiveTopY);
        var topRight = new Vector2(_graphicsDevice.Viewport.Width, 0);
        var bottomRight = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        var bottomLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveBottomY);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X - (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveTopY * depth));
        var scaledTopRight = new Vector2(topRight.X - (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveTopY * depth));
        var scaledBottomRight = new Vector2(bottomRight.X - (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveBottomY * depth));
        var scaledBottomLeft = new Vector2(bottomLeft.X - (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveBottomY * depth));

        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(new Color(200, 200, 200), new Color(50, 50, 50), GetDepthPercentage(depth));
        var scaledWallWidth = GetBlockHorizontalWidth(depth);

        _shapeDrawingService.DrawFilledQuadrilateral(colour,
            (int)scaledTopLeft.X + (scaledWallWidth * offsetX),
            (int)scaledTopLeft.Y,
            (int)scaledTopRight.X + (scaledWallWidth * offsetX),
            (int)scaledTopRight.Y,
            (int)scaledBottomRight.X + (scaledWallWidth * offsetX),
            (int)scaledBottomRight.Y,
            (int)scaledBottomLeft.X + (scaledWallWidth * offsetX),
            (int)scaledBottomLeft.Y);
    }

    /// <summary>
    /// Gets the horizontal width of a wall 'block' at the specified depth
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int GetBlockHorizontalWidth(int depth)
    {
        return _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
    }

    /// <summary>
    /// Returns a 0 to 1 value for how deep (depth) we are into the full draw depth
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    private float GetDepthPercentage(int depth)
    {
        return ((1f * depth) % (_depthToDraw + 2)) / (_depthToDraw + 2);
    }
}
