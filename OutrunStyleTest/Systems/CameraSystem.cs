using Microsoft.Xna.Framework;
using OutrunStyleTest.Components;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Systems;

internal class CameraSystem : ISystem
{
    public World World { get; set; }

    private Filter _cameraFilter;
    private Filter _playerFilter;
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

        // Setup the camera
        var camera = _cameraFilter.First();

        // We want the camera component
        ref var cameraComponent = ref camera.GetComponent<CameraComponent>();

        cameraComponent.Position = new Vector3(0, 1000, 0);
        cameraComponent.DistanceToPlayer = 500;
        cameraComponent.DistanceToProjectionPlane = 1 / (cameraComponent.Position.Y / cameraComponent.DistanceToPlayer);
    }

    public void OnUpdate(float deltaTime)
    {
        var camera = _cameraFilter.First();
        ref var cameraComponent = ref camera.GetComponent<CameraComponent>();

        var player = _playerFilter.First();
        ref var playerComponent = ref player.GetComponent<PlayerComponent>();

        var track = _trackFilter.First();
        ref var trackComponent = ref track.GetComponent<TrackComponent>();

        // Adjust camera position
        cameraComponent.Position.X = playerComponent.Position.X * trackComponent.Width;
        cameraComponent.Position.Z = playerComponent.Position.Z - cameraComponent.DistanceToPlayer;

        // Don't let camera Z to go negative
        if (cameraComponent.Position.Z < 0) cameraComponent.Position.Z += trackComponent.Length;
    }
}
