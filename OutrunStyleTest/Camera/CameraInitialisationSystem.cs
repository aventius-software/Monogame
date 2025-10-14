using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace OutrunStyleTest.Camera;

/// <summary>
/// This creates and initialises the camera entity and sets up the camera component.
/// </summary>
internal class CameraInitialisationSystem : EntitySystem
{
    private readonly GraphicsDevice _graphicsDevice;

    public CameraInitialisationSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(CameraComponent)))
    {
        _graphicsDevice = graphicsDevice;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Create the camera entity
        var cameraEntity = CreateEntity();
        cameraEntity.Attach(new CameraComponent());

        // Get the camera component
        var cameraComponent = cameraEntity.Get<CameraComponent>();

        // Initialise the camera
        cameraComponent.Position = new Vector3(0, 1000, 0);
        cameraComponent.DistanceToPlayer = 500;
        cameraComponent.DistanceToProjectionPlane = 1 / (cameraComponent.Position.Y / cameraComponent.DistanceToPlayer);
        cameraComponent.HeightAbovePlayer = 800;
        cameraComponent.ViewportWidth = _graphicsDevice.Viewport.Width;
        cameraComponent.ViewportHeight = _graphicsDevice.Viewport.Height;
    }
}
