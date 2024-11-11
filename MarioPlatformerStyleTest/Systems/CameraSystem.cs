using MarioPlatformerStyleTest.Components;
using MarioPlatformerStyleTest.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scellecs.Morpeh;
using System.Linq;

namespace MarioPlatformerStyleTest.Systems;

/// <summary>
/// The camera system effectively makes the camera 'follow' the player. We 
/// could improve this with a more elaborate player tracking system, for example, say by
/// having some smoother camera movement instead of instant position changes, maybe 
/// using some kind of interpolation to gradually bring the camera movement to a halt
/// </summary>
internal class CameraSystem : ISystem
{
    public World World { get; set; }

    private readonly Camera _camera;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly MapService _mapService;
    private Entity _playerEntity;

    public CameraSystem(World world, Camera camera, GraphicsDevice graphicsDevice, MapService mapService)
    {
        World = world;
        _camera = camera;
        _graphicsDevice = graphicsDevice;
        _mapService = mapService;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // We'll need the player entity to find out where they are
        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        _playerEntity = playerFilter.First();

        // Set the cameras 'origin' to the middle of the viewport, also note the offset for the size of the character sprite
        _camera.SetOrigin(new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2));

        // We need to tell the camera the dimensions of the map as it will restrict its movement
        // to within the confines of the map and won't 'scroll' outside the map
        _camera.SetWorldDimensions(new Vector2(_mapService.WorldWidth, _mapService.WorldHeight));
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the players transform component
        ref var transformComponent = ref _playerEntity.GetComponent<TransformComponent>();

        // Make the camera 'look' at the current position of the player in the world
        _camera.LookAt(transformComponent.Position, new Vector2(transformComponent.Width, transformComponent.Height));
    }
}
