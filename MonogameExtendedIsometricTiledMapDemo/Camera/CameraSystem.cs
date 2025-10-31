using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonogameExtendedIsometricTiledMapDemo.Player;

namespace MonogameExtendedIsometricTiledMapDemo.Camera;

internal class CameraSystem : EntityProcessingSystem
{
    private readonly OrthographicCamera _camera;

    private ComponentMapper<Transform2> _transformMapper;

    public CameraSystem(OrthographicCamera camera) : base(Aspect.All(typeof(PlayerComponent), typeof(Transform2)))
    {
        _camera = camera;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();        
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        // Get the player's transform, which contains the players current position
        var transform = _transformMapper.Get(entityId);
        
        // Update the camera to look at the new position
        _camera.LookAt(transform.Position);
    }
}
