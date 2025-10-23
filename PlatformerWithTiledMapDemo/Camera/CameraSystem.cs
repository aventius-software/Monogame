using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using PlatformerWithTiledMapDemo.Player;
using System;

namespace PlatformerWithTiledMapDemo.Camera;

internal class CameraSystem : EntityProcessingSystem
{
    private readonly OrthographicCamera _camera;

    private ComponentMapper<PlayerComponent> _playerMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public CameraSystem(OrthographicCamera camera) : base(Aspect.All(typeof(PlayerComponent)))
    {
        _camera = camera;
    }
    
    public override void Initialize(IComponentMapperService mapperService)
    {
        _playerMapper = mapperService.GetMapper<PlayerComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        var transform = _transformMapper.Get(entityId);
        _camera.LookAt(transform.Position);
    }
}
