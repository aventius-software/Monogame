using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonMasterStyleDemo.Services;

internal class DungeonMapService : MapService
{
    private readonly int _depthPerspectiveX = 40;
    private readonly int _depthPerspectiveY = 20;
    private readonly int _depthToDraw = 6;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly int _horizontalBlocksToDraw = 12;
    private readonly ShapeDrawingService _shapeDrawingService;

    public DungeonMapService(SpriteBatch spriteBatch, ContentManager contentManager, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice) : base(spriteBatch, contentManager)
    {
        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    public void DrawTileMap()
    {
        base.Draw();
    }

    public override void Draw()
    {
        // First we draw the floor and ceiling
        for (var depthOffset = _depthToDraw; depthOffset >= 0; depthOffset--)
        {
            DrawFloor(depthOffset);
            DrawCeiling(depthOffset);
        }

        // Draw from furthest away first, up to closest
        for (var depthOffset = _depthToDraw; depthOffset >= 0; depthOffset--)
        {
            for (var mapOffsetX = -_horizontalBlocksToDraw; mapOffsetX <= _horizontalBlocksToDraw; mapOffsetX++)
            {
                // Now get map 'offset' coordinates relative to our actual world/map position                
                var mapDrawPositionX = (int)RotatedPosition.X + mapOffsetX;
                var mapDrawPositionDepth = (int)RotatedPosition.Y - depthOffset;                

                // Find the tile at this relative position in the map                
                var currentBlock = IsBlockingTile(mapDrawPositionX, mapDrawPositionDepth);
                var blockInFront = IsBlockingTile(mapDrawPositionX, mapDrawPositionDepth - 1);
                var blockOnTheLeft = IsBlockingTile(mapDrawPositionX - 1, mapDrawPositionDepth);
                var blockOnTheRight = IsBlockingTile(mapDrawPositionX + 1, mapDrawPositionDepth);

                if (blockOnTheLeft && mapOffsetX <= 0)
                {
                    DrawLeftSideWall(depthOffset, mapOffsetX);
                }

                if (blockOnTheRight && mapOffsetX >= 0)
                {
                    DrawRightSideWall(depthOffset, mapOffsetX);
                }

                if (blockInFront && !currentBlock)
                {
                    DrawFrontFacingWall(depthOffset, mapOffsetX);
                }
            }
        }
    }
    
    private void DrawFloor(int depth)
    {
        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(Color.Green, Color.DarkGreen, GetDepthPercentage(depth));

        _shapeDrawingService.DrawFilledRectangle(colour,
            0, _graphicsDevice.Viewport.Height - (_depthPerspectiveY + (_depthPerspectiveY * depth)),
            _graphicsDevice.Viewport.Width, _depthPerspectiveY);
    }

    private void DrawCeiling(int depth)
    {
        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(Color.Red, Color.DarkRed, GetDepthPercentage(depth));

        _shapeDrawingService.DrawFilledRectangle(colour,
            0, depth * _depthPerspectiveY,
            _graphicsDevice.Viewport.Width, _depthPerspectiveY);
    }

    private void DrawFrontFacingWall(int depth, int offsetX)
    {
        // Draw the wall of the block in front, not the
        // current block depth! ;-)
        depth++;

        // Scale the default wall rectangle down to the relevant
        // size for the depth we're drawing at        
        var scaledX = _depthPerspectiveX * depth;
        var scaledY = _depthPerspectiveY * depth;
        var scaledWidth = _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
        var scaledHeight = _graphicsDevice.Viewport.Height - (2 * _depthPerspectiveY * depth);

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

    private void DrawLeftSideWall(int depth, int offsetX)
    {
        // Build 'default' quad coordinates, as if the wall
        // was just to the side of the player (not taking
        // distance/depth into account...
        var topLeft = new Vector2(0, 0);
        var topRight = new Vector2(_depthPerspectiveX, _depthPerspectiveY);
        var bottomRight = new Vector2(_depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveY);
        var bottomLeft = new Vector2(0, _graphicsDevice.Viewport.Height);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X + (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveY * depth));
        var scaledTopRight = new Vector2(topRight.X + (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveY * depth));
        var scaledbottomRight = new Vector2(bottomRight.X + (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveY * depth));
        var scaledbottomLeft = new Vector2(bottomLeft.X + (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveY * depth));

        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(new Color(200, 200, 200), new Color(50, 50, 50), GetDepthPercentage(depth));
        var scaledWallWidth = GetBlockHorizontalWidth(depth);

        _shapeDrawingService.DrawFilledQuadrilateral(colour,
            (int)scaledTopLeft.X + (scaledWallWidth * offsetX),
            (int)scaledTopLeft.Y,
            (int)scaledTopRight.X + (scaledWallWidth * offsetX),
            (int)scaledTopRight.Y,
            (int)scaledbottomRight.X + (scaledWallWidth * offsetX),
            (int)scaledbottomRight.Y,
            (int)scaledbottomLeft.X + (scaledWallWidth * offsetX),
            (int)scaledbottomLeft.Y);
    }

    private void DrawRightSideWall(int depth, int offsetX)
    {
        // Build 'default' quad coordinates, as if the wall
        // was just to the side of the player (not taking
        // distance/depth into account...
        var topLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _depthPerspectiveY);
        var topRight = new Vector2(_graphicsDevice.Viewport.Width, 0);
        var bottomRight = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        var bottomLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveY);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X - (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveY * depth));
        var scaledTopRight = new Vector2(topRight.X - (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveY * depth));
        var scaledbottomRight = new Vector2(bottomRight.X - (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveY * depth));
        var scaledbottomLeft = new Vector2(bottomLeft.X - (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveY * depth));

        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(new Color(200, 200, 200), new Color(50, 50, 50), GetDepthPercentage(depth));
        var scaledWallWidth = GetBlockHorizontalWidth(depth);

        _shapeDrawingService.DrawFilledQuadrilateral(colour,
            (int)scaledTopLeft.X + (scaledWallWidth * offsetX),
            (int)scaledTopLeft.Y,
            (int)scaledTopRight.X + (scaledWallWidth * offsetX),
            (int)scaledTopRight.Y,
            (int)scaledbottomRight.X + (scaledWallWidth * offsetX),
            (int)scaledbottomRight.Y,
            (int)scaledbottomLeft.X + (scaledWallWidth * offsetX),
            (int)scaledbottomLeft.Y);
    }

    private int GetBlockHorizontalWidth(int depth)
    {
        return _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
    }

    private float GetDepthPercentage(int depth)
    {
        return ((1f * depth) % (_depthToDraw + 2)) / (_depthToDraw + 2);
    }    
}
