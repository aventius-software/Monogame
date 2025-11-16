using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TTDStyleIsometricTileMap.Services;

namespace TTDStyleIsometricTileMap;

public class GameMain : Game
{
    private GraphicsDeviceManager _graphics;
    private IsometricDiamondTileMapService _isometricDiamondTileMapService;
    private SpriteBatch _spriteBatch;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _isometricDiamondTileMapService = new IsometricDiamondTileMapService(Content);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _isometricDiamondTileMapService.LoadTileAtlas("Map/tiles_grass");        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Start drawing...
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw map
        _isometricDiamondTileMapService.Draw(_spriteBatch);

        // Done
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
