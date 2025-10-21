using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;

namespace PlatformerWithTiledMapDemo.Camera;

internal class CameraSystem : EntityProcessingSystem
{
    private readonly OrthographicCamera _camera;

    public CameraSystem(OrthographicCamera camera) 
        : base(Aspect.All(typeof(CameraComponent)))
    {
        _camera = camera;
    }
    
    public override void Initialize(IComponentMapperService mapperService)
    {
        var cameraEntity = CreateEntity();
        cameraEntity.Attach(new CameraComponent());
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        //throw new NotImplementedException();
    }
}
