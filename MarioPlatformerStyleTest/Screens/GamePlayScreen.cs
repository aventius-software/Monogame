using MarioPlatformerStyleTest.Components;
using MarioPlatformerStyleTest.Services;
using MarioPlatformerStyleTest.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;

namespace MarioPlatformerStyleTest.Screens;

internal class GamePlayScreen : IScreen
{
    private readonly Camera _camera;
    private readonly CameraSystem _cameraSystem;
    private readonly ContentManager _contentManager;
    private readonly World _ecsWorld;
    private readonly MapRenderSystem _mapRenderSystem;
    private readonly MapService _mapService;
    private readonly PlatformCollisionSystem _platformCollisionSystem;
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly PlayerRenderSystem _playerRenderSystem;
    private SystemsGroup _renderSystemsGroup;
    private readonly SpriteBatch _spriteBatch;
    private SystemsGroup _updateSystemsGroup;

    public GamePlayScreen(World ecsWorld, 
        MapRenderSystem mapRenderSystem, 
        MapService mapService, 
        SpriteBatch spriteBatch, 
        Camera camera, 
        PlayerControlSystem playerControlSystem,
        CameraSystem cameraSystem,
        PlayerRenderSystem playerRenderSystem,
        ContentManager contentManager,
        PlatformCollisionSystem platformCollisionSystem)
    {
        _ecsWorld = ecsWorld;
        _mapRenderSystem = mapRenderSystem;
        _mapService = mapService;
        _spriteBatch = spriteBatch;
        _camera = camera;
        _playerControlSystem = playerControlSystem;
        _cameraSystem = cameraSystem;
        _playerRenderSystem = playerRenderSystem;
        _contentManager = contentManager;
        _platformCollisionSystem = platformCollisionSystem;
    }

    public void Draw(GameTime gameTime)
    {
        // Clear the screen first
        _spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Start the sprite batch (using our camera to set the transform matrix)
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.TransformMatrix);

        // Draw everything
        _renderSystemsGroup.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        // Done...
        _spriteBatch.End();
    }

    public void Initialise()
    {
        // Add all our render systems - order matters!
        _renderSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _renderSystemsGroup.AddSystem(_mapRenderSystem);
        _renderSystemsGroup.AddSystem(_playerRenderSystem);

        // Add all our update systems - order matters!
        _updateSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _updateSystemsGroup.AddSystem(_playerControlSystem);
        _updateSystemsGroup.AddSystem(_platformCollisionSystem);
        _updateSystemsGroup.AddSystem(_cameraSystem);
        
        // Load the map
        _mapService.LoadTiledMap("test map.tmx", "test tile atlas");
        
        // Create the player entity
        var player = _ecsWorld.CreateEntity();
        player.AddComponent<PlayerComponent>();

        ref var playerComponent = ref player.GetComponent<PlayerComponent>();
        playerComponent.Texture = _contentManager.Load<Texture2D>("character");
        playerComponent.Width = playerComponent.Texture.Width;
        playerComponent.Height = playerComponent.Texture.Height;

        // Now we can initialise the systems
        _updateSystemsGroup.Initialize();
        _renderSystemsGroup.Initialize();
    }

    public void LoadContent()
    {
    }

    public void UnloadContent()
    {
        _updateSystemsGroup.Dispose();
        _renderSystemsGroup.Dispose();
    }

    public void Update(GameTime gameTime)
    {
        _updateSystemsGroup.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
}
