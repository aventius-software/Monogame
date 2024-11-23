using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonMasterStyleDemo.Services;

internal class DungeonMapService : MapService
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ShapeDrawingService _shapeDrawingService;
    
    private int _depthPerspectiveX = 40;
    private int _depthPerspectiveY = 20;
    private int _depthToDraw = 8;
    private int _horizontalBlocksToDraw = 2;

    public DungeonMapService(SpriteBatch spriteBatch, ContentManager contentManager, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice) : base(spriteBatch, contentManager)
    {
        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    public override void Draw()
    {
        // First we draw the floor and ceiling
        for (var depth = _depthToDraw; depth > 0; depth--)
        {
            DrawFloor(depth);
            DrawCeiling(depth);
        }
        
        // Draw from furthest away first, up to closest
        for (var mapDepthOffset = _depthToDraw; mapDepthOffset > 0; mapDepthOffset--)
        {            
            for (var mapColumnOffset = -_horizontalBlocksToDraw / 2; mapColumnOffset < _horizontalBlocksToDraw / 2; mapColumnOffset++)
            {
                // Now get map 'offset' coordinates relative to our actual world/map position
                var mapDrawPositionDepth = _tileRowPositionInTheWorld - mapDepthOffset;
                var mapDrawPositionX = _tileColumnPositionInTheWorld + mapColumnOffset;

                // Find the tile at this relative position in the map                
                var blockInFront = GetTileAtPosition(mapDrawPositionDepth - 1, mapDrawPositionX);
                var blockOnTheLeft = GetTileAtPosition(mapDrawPositionDepth, mapDrawPositionX - 1);
                var blockOnTheRight = GetTileAtPosition(mapDrawPositionDepth, mapDrawPositionX + 1);

                if (blockInFront == 2)
                {
                    DrawFrontFacingWall(mapDepthOffset, mapColumnOffset);
                }

                if (blockOnTheLeft == 2)
                {
                    DrawLeftSideWall(mapDepthOffset - 1, mapColumnOffset);
                }
                
                if (blockOnTheRight == 2)
                {
                    DrawRightSideWall(mapDepthOffset - 1, mapColumnOffset);
                }                               
            }
        }                    
    }

    private int GetBlockHorizontalWidth(int depth)
    {
        return _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);
    }

    private float GetDepthPercentage(int depth)
    {
        return ((1f * depth) % (_depthToDraw + 1)) / (_depthToDraw + 1);
    }

    private void DrawFloor(int depth)
    {
        var yOffset = depth * _depthPerspectiveY;

        // Scale the wall colour so that walls further away get darker        
        var colour = Color.Lerp(Color.Green, Color.DarkGreen, GetDepthPercentage(depth));

        _shapeDrawingService.DrawFilledRectangle(colour, 
            0, _graphicsDevice.Viewport.Height - yOffset,
            _graphicsDevice.Viewport.Width, yOffset);
    }

    private void DrawCeiling(int depth)
    {
        var yOffset = depth * _depthPerspectiveY;

        // Scale the wall colour so that walls further away get darker
        var colour = Color.Lerp(Color.Red, Color.DarkRed, GetDepthPercentage(depth));

        _shapeDrawingService.DrawFilledRectangle(colour, 0, 0,
            _graphicsDevice.Viewport.Width, yOffset);
    }

    private void DrawFrontFacingWall(int depth, int offsetX)
    {        
        // Start with a default wall face, which is a rectangle covering the
        // viewport. We'll then scale this down depending on the 'depth' we
        // are drawing at
        var defaultWallFace = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        // Scale the default wall rectangle down to the relevant size for
        // the depth we're drawing at        
        var scaledX = defaultWallFace.X + (_depthPerspectiveX * depth);
        var scaledY = defaultWallFace.Y + (_depthPerspectiveY * depth);
        var scaledWidth = defaultWallFace.Width - (2 * _depthPerspectiveX * depth);
        var scaledHeight = defaultWallFace.Height - (2 * _depthPerspectiveY * depth);

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
        var colour = Color.Lerp(Color.DarkGray, Color.Gray, GetDepthPercentage(depth));
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
        var colour = Color.Lerp(Color.DarkGray, Color.Gray, GetDepthPercentage(depth));
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
}
