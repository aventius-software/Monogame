using Microsoft.Xna.Framework;
using OutrunStyleTest.Components;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Systems;

internal class CameraSystem : ISystem
{
    public World World { get; set; }

    private Entity _camera;
    private Filter _cameraFilter;
    private Entity _player;
    private Filter _playerFilter;
    private Entity _track;
    private Filter _trackFilter;

    public CameraSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Set filters for each of our entities we need
        _cameraFilter = World.Filter.With<CameraComponent>().Build();
        _playerFilter = World.Filter.With<PlayerComponent>().Build();
        _trackFilter = World.Filter.With<TrackComponent>().Build();

        // Get the entities (only ever one of each so 'First()' will do)
        _camera = _cameraFilter.First();
        _player = _playerFilter.First();
        _track = _trackFilter.First();

        // We want the camera component
        ref var cameraComponent = ref _camera.GetComponent<CameraComponent>();

        // Initialise the camera
        cameraComponent.Position = new Vector3(0, 1000, 0);
        cameraComponent.DistanceToPlayer = 500;
        cameraComponent.DistanceToProjectionPlane = 1 / (cameraComponent.Position.Y / cameraComponent.DistanceToPlayer);
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the components we'll need
        ref var cameraComponent = ref _camera.GetComponent<CameraComponent>();
        ref var playerComponent = ref _player.GetComponent<PlayerComponent>();
        ref var trackComponent = ref _track.GetComponent<TrackComponent>();

        // Adjust the camera position so it follows the player position
        cameraComponent.Position.X = playerComponent.Position.X * trackComponent.Width;
        cameraComponent.Position.Z = playerComponent.Position.Z - cameraComponent.DistanceToPlayer;

        // Don't let camera Z to go negative
        if (cameraComponent.Position.Z < 0) cameraComponent.Position.Z += trackComponent.Length;
    }
}
