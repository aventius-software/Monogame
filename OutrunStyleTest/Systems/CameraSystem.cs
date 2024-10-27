using Microsoft.Xna.Framework;
using OutrunStyleTest.Components;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Systems;

internal class CameraSystem : ISystem
{
    public World World { get; set; }
    
    private float _distanceToPlayer;
    private float _distanceToProjectionPlane;
    private Filter _filter;
    private Vector3 _position;

    public CameraSystem(World world)
    {
        World = world;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        // Set camera position
        _position = new Vector3(0, 1000, 0);

        // And some other stuff
        _distanceToPlayer = 500f;
        //_distanceToProjectionPlane = 1 / (_position.Y / _distanceToPlayer);

        // We want the player entity
        _filter = World.Filter.With<PlayerComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        // Get the player
        var entity = _filter.First();

        // We want the position component
        ref var playerComponent = ref entity.GetComponent<PlayerComponent>();

        // Adjust position
        _position.X = playerComponent.Position.X * 1; // * roadWidth;

        // Don't let camera Z to go negative
        _position.Z = playerComponent.Position.Z - _distanceToPlayer;        
    }
}
