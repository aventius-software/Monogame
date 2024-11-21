using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMasterStyleDemo.Services;

internal class DungeonMapService : MapService
{
    private GraphicsDevice _graphicsDevice;
    private ShapeDrawingService _shapeDrawingService;

    private int _wallDepth = 30;
    private int _depthPerspective = 15;

    public DungeonMapService(SpriteBatch spriteBatch, ContentManager contentManager, ShapeDrawingService shapeDrawingService, GraphicsDevice graphicsDevice) : base(spriteBatch, contentManager)
    {
        _shapeDrawingService = shapeDrawingService;
        _graphicsDevice = graphicsDevice;
    }

    public override void Draw()
    {        
        DrawLeftWall(0, Color.LightGray);
        DrawLeftWall(1, Color.LightGray);
        DrawLeftWall(2, Color.LightGray);
        DrawLeftWall(3, Color.DarkGray);
        DrawLeftWall(4, Color.DarkGray);
        DrawLeftWall(5, Color.DarkGray);
        DrawLeftWall(6, Color.DimGray);
        DrawLeftWall(7, Color.DimGray);
        DrawLeftWall(8, Color.DimGray);

        DrawRightWall(0, Color.LightGray);
        DrawRightWall(1, Color.LightGray);
        DrawRightWall(2, Color.LightGray);
        DrawRightWall(3, Color.DarkGray);
        DrawRightWall(4, Color.DarkGray);
        DrawRightWall(5, Color.DarkGray);
        DrawRightWall(6, Color.DimGray);
        DrawRightWall(7, Color.DimGray);
        DrawRightWall(8, Color.DimGray);

        DrawBottomWall(0, Color.LightSlateGray);
        DrawBottomWall(1, Color.LightSlateGray);
        DrawBottomWall(2, Color.LightSlateGray);
        DrawBottomWall(3, Color.SlateGray);
        DrawBottomWall(4, Color.SlateGray);
        DrawBottomWall(5, Color.SlateGray);
        DrawBottomWall(6, Color.DarkSlateGray);
        DrawBottomWall(7, Color.DarkSlateGray);
        DrawBottomWall(8, Color.DarkSlateGray);

        DrawCeilingWall(0, Color.LightSlateGray);
        DrawCeilingWall(1, Color.LightSlateGray);
        DrawCeilingWall(2, Color.LightSlateGray);
        DrawCeilingWall(3, Color.SlateGray);
        DrawCeilingWall(4, Color.SlateGray);
        DrawCeilingWall(5, Color.SlateGray);
        DrawCeilingWall(6, Color.DarkSlateGray);
        DrawCeilingWall(7, Color.DarkSlateGray);
        DrawCeilingWall(8, Color.DarkSlateGray);
    }

    private void DrawLeftWall(int depthPosition, Color colour)
    {
        var nearOffsetX = depthPosition * _wallDepth;
        var nearOffsetY = depthPosition * _depthPerspective;
        var farOffsetX = (depthPosition + 1) * _wallDepth;
        var farOffsetY = (depthPosition + 1) * _depthPerspective;

        int x1, y1, x2, y2, x3, y3, x4, y4;

        x1 = nearOffsetX; y1 = nearOffsetY;
        x2 = farOffsetX; y2 = farOffsetY;
        x3 = farOffsetX; y3 = _graphicsDevice.Viewport.Height - farOffsetY;
        x4 = nearOffsetX; y4 = _graphicsDevice.Viewport.Height - nearOffsetY;

        _shapeDrawingService.DrawFilledQuadrilateral(colour, x1, y1, x2, y2, x3, y3, x4, y4);
    }

    private void DrawRightWall(int depthPosition, Color colour)
    {
        var nearOffsetX = depthPosition * _wallDepth;
        var nearOffsetY = depthPosition * _depthPerspective;
        var farOffsetX = (depthPosition + 1) * _wallDepth;
        var farOffsetY = (depthPosition + 1) * _depthPerspective;

        int x1, y1, x2, y2, x3, y3, x4, y4;
        
        x1 = _graphicsDevice.Viewport.Width - farOffsetX; y1 = farOffsetY;
        x2 = _graphicsDevice.Viewport.Width - nearOffsetX; y2 = nearOffsetY;
        x3 = _graphicsDevice.Viewport.Width - nearOffsetX; y3 = _graphicsDevice.Viewport.Height - nearOffsetY;
        x4 = _graphicsDevice.Viewport.Width - farOffsetX; y4 = _graphicsDevice.Viewport.Height - farOffsetY;

        _shapeDrawingService.DrawFilledQuadrilateral(colour, x1, y1, x2, y2, x3, y3, x4, y4);
    }

    private void DrawBottomWall(int depthPosition, Color colour)
    {
        var nearOffsetX = depthPosition * _wallDepth;
        var nearOffsetY = depthPosition * _depthPerspective;
        var farOffsetX = (depthPosition + 1) * _wallDepth;
        var farOffsetY = (depthPosition + 1) * _depthPerspective;

        int x1, y1, x2, y2, x3, y3, x4, y4;

        x1 = nearOffsetX; y1 = _graphicsDevice.Viewport.Height - nearOffsetY;
        x2 = farOffsetX; y2 = _graphicsDevice.Viewport.Height - farOffsetY;
        x3 = _graphicsDevice.Viewport.Width - farOffsetX; y3 = _graphicsDevice.Viewport.Height - farOffsetY;
        x4 = _graphicsDevice.Viewport.Width - nearOffsetX; y4 = _graphicsDevice.Viewport.Height - nearOffsetY;

        _shapeDrawingService.DrawFilledQuadrilateral(colour, x1, y1, x2, y2, x3, y3, x4, y4);
    }

    private void DrawCeilingWall(int depthPosition, Color colour)
    {
        var nearOffsetX = depthPosition * _wallDepth;
        var nearOffsetY = depthPosition * _depthPerspective;
        var farOffsetX = (depthPosition + 1) * _wallDepth;
        var farOffsetY = (depthPosition + 1) * _depthPerspective;

        int x1, y1, x2, y2, x3, y3, x4, y4;

        x1 = nearOffsetX; y1 = nearOffsetY;
        x2 = _graphicsDevice.Viewport.Width - nearOffsetX; y2 = nearOffsetY;
        x3 = _graphicsDevice.Viewport.Width - farOffsetX; y3 = farOffsetY;
        x4 = farOffsetX; y4 = farOffsetY;

        _shapeDrawingService.DrawFilledQuadrilateral(colour, x1, y1, x2, y2, x3, y3, x4, y4);
    }
}
