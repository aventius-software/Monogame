using Microsoft.Xna.Framework;
using OutrunStyleTest.Components;
using OutrunStyleTest.Services;
using OutrunStyleTest.Systems;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Screens;

internal class GamePlayScreen : IScreen
{
    private readonly CameraSystem _cameraSystem;
    private readonly World _ecsWorld;    
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly TrackSystem _roadDrawingSystem;
    
    private SystemsGroup _renderSystemsGroup;
    private SystemsGroup _updateSystemsGroup;

    public GamePlayScreen(
        World ecsWorld,
        CameraSystem cameraSystem,
        PlayerControlSystem playerControlSystem, 
        TrackSystem roadDrawingSystem)
    {
        _ecsWorld = ecsWorld;        

        _cameraSystem = cameraSystem;
        _playerControlSystem = playerControlSystem;
        _roadDrawingSystem = roadDrawingSystem;
    }

    public void Draw(GameTime gameTime)
    {
        // Update all the 'draw/render' systems
        _renderSystemsGroup.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public void Initialise()
    {        
        // Add all our update systems - order matters!
        _updateSystemsGroup = _ecsWorld.CreateSystemsGroup();        
        _updateSystemsGroup.AddSystem(_playerControlSystem);
        _updateSystemsGroup.AddSystem(_cameraSystem);        

        // Add render systems
        _renderSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _renderSystemsGroup.AddSystem(_roadDrawingSystem);                       

        // Create entities
        var track = _ecsWorld.CreateEntity();
        track.AddComponent<TrackComponent>();

        var player = _ecsWorld.CreateEntity();
        player.AddComponent<PlayerComponent>();

        var camera = _ecsWorld.CreateEntity();
        camera.AddComponent<CameraComponent>();

        _updateSystemsGroup.Initialize();
        _renderSystemsGroup.Initialize();
    }

    public void LoadContent()
    {
    }

    public void UnloadContent()
    {
    }

    public void Update(GameTime gameTime)
    {
        // Update all the 'update' systems
        _updateSystemsGroup.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
}
