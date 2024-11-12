using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using TiledMapsAndAetherPhysics.Components;
using TiledMapsAndAetherPhysics.Services;
using TiledMapsAndAetherPhysics.Systems;

namespace TiledMapsAndAetherPhysics.Screens;

/// <summary>
/// This is the main gameplay screen, in a real game you would have probably other screens
/// such as a splash screen and/or a title screen and a game over screen
/// </summary>
internal class GamePlayScreen : IScreen
{
    private readonly Camera _camera;
    private readonly CameraSystem _cameraSystem;
    private readonly World _ecsWorld;
    private readonly MapRenderSystem _mapRenderSystem;
    private readonly MapService _mapService;
    private readonly PhysicsSystem _physicsSystem;
    private Entity _player;
    private readonly PlayerInitialiser _playerInitialiser;
    private readonly PlayerControlSystem _playerControlSystem;
    private SystemsGroup _renderSystemsGroup;
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteRenderSystem _spriteRenderSystem;
    private SystemsGroup _updateSystemsGroup;

    public GamePlayScreen(World ecsWorld,
        MapRenderSystem mapRenderSystem,
        MapService mapService,
        SpriteBatch spriteBatch,
        Camera camera,
        PlayerControlSystem playerControlSystem,
        CameraSystem cameraSystem,
        PhysicsSystem physicsSystem,
        PlayerInitialiser playerInitialiser,
        SpriteRenderSystem spriteRenderSystem)
    {
        _ecsWorld = ecsWorld;
        _mapRenderSystem = mapRenderSystem;
        _mapService = mapService;
        _spriteBatch = spriteBatch;
        _camera = camera;
        _playerControlSystem = playerControlSystem;
        _cameraSystem = cameraSystem;
        _physicsSystem = physicsSystem;
        _playerInitialiser = playerInitialiser;
        _spriteRenderSystem = spriteRenderSystem;
    }

    public void Draw(GameTime gameTime)
    {
        // Clear the screen first
        _spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Start the sprite batch, note that we're using our camera to set the transform matrix
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
        _renderSystemsGroup.AddSystem(_spriteRenderSystem);

        // Add all our update systems - order matters!
        _updateSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _updateSystemsGroup.AddInitializer(_playerInitialiser);
        _updateSystemsGroup.AddSystem(_playerControlSystem);
        _updateSystemsGroup.AddSystem(_physicsSystem);
        _updateSystemsGroup.AddSystem(_cameraSystem);

        // Load the Tiled map
        _mapService.LoadTiledMap("test map.tmx", "test tile atlas");

        // Create the player entity (if they don't already exist)
        _player ??= _ecsWorld.CreateEntity();
        _player.AddComponent<PlayerComponent>();

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
