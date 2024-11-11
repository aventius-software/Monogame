using Microsoft.Xna.Framework;
using OutrunStyleTest.Components;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Systems;

internal class CameraSystem : ISystem
{
    public World World { get; set; }

    private Entity _cameraEntity;
    private Entity _playerEntity;
    private Entity _trackEntity;

    public CameraSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Get the entities
        var cameraFilter = World.Filter.With<CameraComponent>().Build();
        _cameraEntity = cameraFilter.First();

        var playerFilter = World.Filter.With<PlayerComponent>().Build();
        _playerEntity = playerFilter.First();

        var trackFilter = World.Filter.With<TrackComponent>().Build();
        _trackEntity = trackFilter.First();

        // We want the camera component of the camera to set some initial values
        ref var cameraComponent = ref _cameraEntity.GetComponent<CameraComponent>();

        // Initialise the camera
        cameraComponent.Position = new Vector3(0, 1000, 0);
        cameraComponent.DistanceToPlayer = 500;
        cameraComponent.DistanceToProjectionPlane = 1 / (cameraComponent.Position.Y / cameraComponent.DistanceToPlayer);
        cameraComponent.HeightAbovePlayer = 800;
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the components we'll need
        ref var cameraComponent = ref _cameraEntity.GetComponent<CameraComponent>();
        ref var playerComponent = ref _playerEntity.GetComponent<PlayerComponent>();
        ref var trackComponent = ref _trackEntity.GetComponent<TrackComponent>();

        // Adjust the camera position so it follows the player position (x and y) and
        // is just a little bit above the player (z)
        cameraComponent.Position.X = playerComponent.Position.X;
        cameraComponent.Position.Y = playerComponent.Position.Y + cameraComponent.HeightAbovePlayer;
        cameraComponent.Position.Z = playerComponent.Position.Z - cameraComponent.DistanceToPlayer;

        // Don't let camera Z to go negative
        if (cameraComponent.Position.Z < 0) cameraComponent.Position.Z += trackComponent.Track.TotalLength;
    }
}
