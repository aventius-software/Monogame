using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using OutrunStyleTest.Player;
using OutrunStyleTest.Track;
using System.Linq;

namespace OutrunStyleTest.Camera;

/// <summary>
/// This system updates the camera position so that it follows the player.
/// </summary>
internal class CameraUpdateSystem : EntityUpdateSystem
{
    private int _cameraEntityId;
    private int _playerEntityId;
    private int _trackEntityId;

    public CameraUpdateSystem()
        : base(Aspect.One(typeof(CameraComponent), typeof(PlayerComponent), typeof(TrackComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Save the entity id's we'll need
        _cameraEntityId = ActiveEntities.Single(id => GetEntity(id).Has<CameraComponent>());
        _playerEntityId = ActiveEntities.Single(id => GetEntity(id).Has<PlayerComponent>());
        _trackEntityId = ActiveEntities.Single(id => GetEntity(id).Has<TrackComponent>());
    }

    public override void Update(GameTime gameTime)
    {
        // Get the components we'll need for the camera
        var cameraEntity = GetEntity(_cameraEntityId);
        var cameraComponent = cameraEntity.Get<CameraComponent>();

        // Get the components we'll need for the player
        var playerEntity = GetEntity(_playerEntityId);
        var playerComponent = playerEntity.Get<PlayerComponent>();

        // Get the components we'll need for the track
        var trackEntity = GetEntity(_trackEntityId);
        var trackComponent = trackEntity.Get<TrackComponent>();

        // Adjust the camera position so it follows the player position (x and y) and
        // is just a little bit above the player (z)
        cameraComponent.Position.X = playerComponent.Position.X;
        cameraComponent.Position.Y = playerComponent.Position.Y + cameraComponent.HeightAbovePlayer;
        cameraComponent.Position.Z = playerComponent.Position.Z - cameraComponent.DistanceToPlayer;

        // Don't let camera Z to go negative
        if (cameraComponent.Position.Z < 0) cameraComponent.Position.Z += trackComponent.Track.TotalLength;
    }
}
