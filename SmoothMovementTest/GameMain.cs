using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SmoothMovementTest;

/// <summary>
/// This is a simple test to show smooth movement in Monogame. There are a number
/// of 'gotchas' you need to watch out for. By default Monogame is set to use a
/// 'fixed' time step for 60 fps, in other words its locked at trying to stick to 60 
/// fps. This can be removed or changed to get various results as shown below
/// 
/// Note for cross platform projects:-
/// 
/// For 'true' super smooth movement (at least on cross platform desktop projects) you 
/// must run in full screen mode. Although there seems to be an issue where first
/// you must set the 'PreferredBackBufferWidth' and 'PreferredBackBufferHeight'
/// to the current desktop resolution otherwise the program just exits full screen
/// and minimises the program to the taskbar. This doesn't seem to be the case
/// for Windows desktop projects
/// 
/// Note for Nvidia users:-
/// 
/// If you have a frame limiter specified in the Nvidia control panel this WILL
/// cause stuttering no matter what settings you use. There doesn't seem to be
/// a workaround for this (apart from removing the frame limiter in the Nvidia
/// control panel). Might be a bug in Monogame...
/// </summary>
public class GameMain : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private int _targetFps = 60;
    private Texture2D _texture;
    private int _xDirection;
    private int _xPos;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);

        // When using full screen we need to set the screen width/height to be the same as the current
        // resolution. On cross platform projects not doing this first currently causes issues. This
        // isn't needed for Windows projects though! Not sure if this is a Monogame bug but it feels
        // like it is...
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        // Full screen mode only, windowed mode will almost always have some stutter no matter what
        // and there is NO way to stop it (AFAIK)
        _graphics.ToggleFullScreen();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // We'll move some sprites in the x-axis
        _xDirection = 1;
        _xPos = 0;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load some texture to use
        _texture = Content.Load<Texture2D>("circle");

        // By default start with the variable framerate
        UseVariableFramerate();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        var keyboard = Keyboard.GetState();

        // Change mode with V or F
        if (keyboard.IsKeyDown(Keys.V))
        {
            UseVariableFramerate();
        }
        else if (keyboard.IsKeyDown(Keys.F))
        {
            UseFixedFramerate(_targetFps);
        }

        // Move the position of the sprites
        _xPos += _xDirection;

        if (_xPos > _graphics.PreferredBackBufferWidth)
        {
            _xPos = _graphics.PreferredBackBufferWidth;
            _xDirection *= -1;
        }

        if (_xPos < 0)
        {
            _xPos = 0;
            _xDirection *= -1;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: null);

        // Draw a vertical column of sprites
        var numberOfSpritesToDraw = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / _texture.Height;

        for (var i = 0; i < numberOfSpritesToDraw; i++)
        {
            _spriteBatch.Draw(_texture, new Vector2(_xPos, i * _texture.Height), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UseFixedFramerate(int targetFps)
    {
        // For a fixed framerate we need to enable the fixed timestep
        IsFixedTimeStep = true;
        InactiveSleepTime = TimeSpan.Zero; // Helps in some configurations

        // No vsync
        _graphics.SynchronizeWithVerticalRetrace = false;

        // If we want a different target fps from the default (which in Monogame is 60), then
        // we need to set the target 'time elapsed' we want for the specified target fps
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / targetFps));

        // Apply changes
        _graphics.ApplyChanges();
    }

    private void UseVariableFramerate()
    {
        // Disable the fixed timestep
        IsFixedTimeStep = false;
        InactiveSleepTime = TimeSpan.Zero; // Helps in some configurations

        // No vsync
        _graphics.SynchronizeWithVerticalRetrace = false;

        // Apply changes
        _graphics.ApplyChanges();
    }
}
