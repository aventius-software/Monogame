using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RenderTargetExample.Services;

internal class CustomRenderTarget
{
    private Color? _clearScreenColour;
    private Rectangle _destinationRectangle;
    private readonly GraphicsDevice _graphicsDevice;
    private RenderTarget2D _renderTarget;
    private readonly SpriteBatch _spriteBatch;

    public CustomRenderTarget(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    public void InitialiseRenderDestination(int virtualScreenWidth, int virtualScreenHeight, Color? clearScreenColour = null)
    {
        // Create our 'virtual' render target
        _renderTarget = new RenderTarget2D(_graphicsDevice, virtualScreenWidth, virtualScreenHeight);

        // Set the screen 'clear' colour (if we're clearing the screen at all)
        _clearScreenColour = clearScreenColour;

        // Now setup scaling so everything looks right no matter the real resolution
        var screenSize = _graphicsDevice.Viewport.Bounds.Size;

        var scaleX = (float)screenSize.X / _renderTarget.Width;
        var scaleY = (float)screenSize.Y / _renderTarget.Height;
        var scale = Math.Min(scaleX, scaleY);

        var destinationWidth = (int)(_renderTarget.Width * scale);
        var destinationHeight = (int)(_renderTarget.Height * scale);

        var destinationX = (screenSize.X - destinationWidth) / 2;
        var destinationY = (screenSize.Y - destinationHeight) / 2;

        _destinationRectangle = new Rectangle(destinationX, destinationY, destinationWidth, destinationHeight);
    }

    public void Begin()
    {
        // Set the render target
        _graphicsDevice.SetRenderTarget(_renderTarget);

        // Clear the screen if we want to
        if (_clearScreenColour is not null) _graphicsDevice.Clear((Color)_clearScreenColour);
    }

    public void Draw()
    {
        // Done using render target to draw
        _graphicsDevice.SetRenderTarget(null);

        // Draw the render target to the 'real' screen now
        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTarget, _destinationRectangle, Color.White);
        _spriteBatch.End();
    }
}
