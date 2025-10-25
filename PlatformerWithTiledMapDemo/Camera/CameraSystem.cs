using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using PlatformerWithTiledMapDemo.Map;
using PlatformerWithTiledMapDemo.Player;
using System.Linq;

namespace PlatformerWithTiledMapDemo.Camera;

internal class CameraSystem : EntityProcessingSystem
{
    private readonly OrthographicCamera _camera;
    private readonly MapService _mapService;

    private Vector2? _positionToTrack = null;
    private Vector2 _playerSpriteSizeOffset;
    private ComponentMapper<Transform2> _transformMapper;

    public CameraSystem(OrthographicCamera camera, MapService mapService)
        : base(Aspect.All(typeof(PlayerComponent), typeof(Transform2)))
    {
        _camera = camera;
        _mapService = mapService;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();

        // Get the player entity to determine the sprite size offset to center the camera. We
        // divide by 2 as we want half the size to offset from the center. Then we can add this
        // to the player's position to get the center point of the sprite. If we don't do this, the
        // camera position will be offset by the entire sprite size, meaning the player will appear
        // slightly off-center in the viewport.
        var playerEntity = GetEntity(ActiveEntities.First());
        _playerSpriteSizeOffset = playerEntity.Get<AnimatedSprite>().Size.ToVector2() / 2f;

        // Initialize the position to track to the camera's current position
        _positionToTrack ??= _camera.Position;
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        // Get the player's transform, which contains the players current position
        var transform = _transformMapper.Get(entityId);

        // Interpolate the camera position for smooth movement between position
        // changes, using a simple linear interpolation (lerp). This gives us a
        // kind of an acceleration/deccelaration effect for the camera movement
        // as it tracks towards the players position.        
        _positionToTrack = Vector2.Lerp((Vector2)_positionToTrack, transform.Position + _playerSpriteSizeOffset, 0.2f);

        // Clamp the camera position to be within the map boundaries
        var clampedPosition = _mapService.ClampPositionToMapBoundry((Vector2)_positionToTrack, _camera.BoundingRectangle);

        // Update the camera to look at the new position
        _camera.LookAt(clampedPosition);
    }
}
