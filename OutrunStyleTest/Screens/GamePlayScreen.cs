using Microsoft.Xna.Framework;
using OutrunStyleTest.Services;
using OutrunStyleTest.Systems;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Screens;

internal class GamePlayScreen : IScreen
{    
    private readonly World _ecsWorld;    
    private readonly PlayerControlSystem _playerControlSystem;
    private readonly RoadDrawingSystem _roadDrawingSystem;
    
    private SystemsGroup _renderSystemsGroup;
    private SystemsGroup _updateSystemsGroup;

    public GamePlayScreen(World ecsWorld, 
        PlayerControlSystem playerControlSystem, 
        RoadDrawingSystem roadDrawingSystem)
    {
        _ecsWorld = ecsWorld;        

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
        // Add render systems
        _renderSystemsGroup = _ecsWorld.CreateSystemsGroup();
        _renderSystemsGroup.AddSystem(_roadDrawingSystem);
        _renderSystemsGroup.Initialize();

        // Add all our update systems - order matters!
        _updateSystemsGroup = _ecsWorld.CreateSystemsGroup();        
        _updateSystemsGroup.AddSystem(_playerControlSystem);        
        _updateSystemsGroup.Initialize();

        // Create player entity
        //_vehicleFactory.Create(new Vector2(50, 50), 1.5f, true);

        // Create some enemies
        //_vehicleFactory.Create(new Vector2(150, 150), 1.5f);
        //_vehicleFactory.Create(new Vector2(400, 200), 1.5f);
        //_vehicleFactory.Create(new Vector2(50, 300), 1.5f);
        //_vehicleFactory.Create(new Vector2(600, 400), 1.5f);
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
