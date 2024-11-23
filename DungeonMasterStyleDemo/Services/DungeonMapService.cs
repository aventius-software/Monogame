using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMasterStyleDemo.Services;

internal class DungeonMapService : MapService
{
    private GraphicsDevice _graphicsDevice;
    private ShapeDrawingService _shapeDrawingService;

    private int _wallDepth = 100;
    private int _depthPerspectiveX = 60;
    private int _depthPerspectiveY = 60;
    private int _tileBlocksToDraw = 4;

    public DungeonMapService(SpriteBatch spriteBatch, ContentManager contentManager, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice) : base(spriteBatch, contentManager)
    {
        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    public override void Draw()
    {
        // First we draw the floor and ceiling
        for (var depth = _tileBlocksToDraw - 1; depth > 0; depth--)
        {
            DrawFloor(depth);
            DrawCeiling(depth);
        }

        // Now all the walls
        for (var mapColumnOffset = -_tileBlocksToDraw / 2; mapColumnOffset < _tileBlocksToDraw / 2; mapColumnOffset++)
        {
            // Draw from furthest away first, up to closest
            for (var mapDepthOffset = _tileBlocksToDraw - 1; mapDepthOffset > 0; mapDepthOffset--)
            {                
                // Now get map 'offset' coordinates relative to our actual world/map position
                var mapDrawPositionDepth = _tileRowPositionInTheWorld - mapDepthOffset;
                var mapDrawPositionX = _tileColumnPositionInTheWorld + mapColumnOffset;
                
                // Find the tile at this relative position in the map
                //var tileAtDrawPosition = GetTileAtPosition(mapDrawPositionDepth, mapDrawPositionX);
                var tileToTheFront = GetTileAtPosition(mapDrawPositionDepth, mapDrawPositionX);
                var tileToTheLeft = GetTileAtPosition(mapDrawPositionDepth, mapDrawPositionX - 1);
                var tileToTheRight = GetTileAtPosition(mapDrawPositionDepth, mapDrawPositionX + 1);
                
                if (tileToTheLeft == 2)
                {
                    DrawLeftSideWall(mapDepthOffset - 1, mapColumnOffset);
                }

                if (tileToTheRight == 2)
                {
                    DrawRightSideWall(mapDepthOffset - 1, mapColumnOffset);
                }

                if (tileToTheFront == 2)
                {
                    DrawFrontFacingWall(mapDepthOffset, mapColumnOffset);
                }
            }
        }                       
    }

    private void DrawFloor(int depth)
    {
        var yOffset = depth * _depthPerspectiveY;

        // Scale the wall colour so that walls further away get darker
        var percent = ((1f * depth) % _tileBlocksToDraw) / _tileBlocksToDraw;
        var colour = Color.Lerp(Color.LightGreen, Color.DarkGreen, percent);

        _shapeDrawingService.DrawFilledRectangle(colour, 
            0, _graphicsDevice.Viewport.Height - yOffset,
            _graphicsDevice.Viewport.Width, yOffset);
    }

    private void DrawCeiling(int depth)
    {
        var yOffset = depth * _depthPerspectiveY;

        // Scale the wall colour so that walls further away get darker
        var percent = ((1f * depth) % _tileBlocksToDraw) / _tileBlocksToDraw;
        var colour = Color.Lerp(Color.Red, Color.DarkRed, percent);

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
        var percent = ((1f * depth) % _tileBlocksToDraw) / _tileBlocksToDraw;
        var colour = Color.Lerp(Color.LightBlue, Color.DarkBlue, percent);

        // Finally draw the scaled (and horizonally offset) front facing wall
        _shapeDrawingService.DrawFilledRectangle(colour, scaledWallFace.X, scaledWallFace.Y, scaledWallFace.Width, scaledWallFace.Height);
    }

    private void DrawLeftSideWall(int depth, int offsetX)
    {        
        // Build 'default' quad coordinates, as if the left wall
        // was just to the left of the player (not taking distance/depth
        // into account...
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
        var percent = ((1f * depth) % _tileBlocksToDraw) / _tileBlocksToDraw;
        var colour = Color.Lerp(Color.LightGray, Color.DimGray, percent);
        var scaledWallWidth = _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);

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
        // Build 'default' quad coordinates, as if the left wall
        // was just to the left of the player (not taking distance/depth
        // into account...
        var topLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _depthPerspectiveY);
        var topRight = new Vector2(_graphicsDevice.Viewport.Width, 0);
        var bottomRight = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        var bottomLeft = new Vector2(_graphicsDevice.Viewport.Width - _depthPerspectiveX, _graphicsDevice.Viewport.Height - _depthPerspectiveY);

        // Scale the down to the relevant size for
        // the depth we're drawing at                
        var scaledTopLeft = new Vector2(topLeft.X + (_depthPerspectiveX * depth), topLeft.Y + (_depthPerspectiveY * depth));
        var scaledTopRight = new Vector2(topRight.X + (_depthPerspectiveX * depth), topRight.Y + (_depthPerspectiveY * depth));
        var scaledbottomRight = new Vector2(bottomRight.X + (_depthPerspectiveX * depth), bottomRight.Y - (_depthPerspectiveY * depth));
        var scaledbottomLeft = new Vector2(bottomLeft.X + (_depthPerspectiveX * depth), bottomLeft.Y - (_depthPerspectiveY * depth));

        // Scale the wall colour so that walls further away get darker
        var percent = ((1f * depth) % _tileBlocksToDraw) / _tileBlocksToDraw;
        var colour = Color.Lerp(Color.LightGray, Color.DimGray, percent);
        var scaledWallWidth = _graphicsDevice.Viewport.Width - (2 * _depthPerspectiveX * depth);

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
