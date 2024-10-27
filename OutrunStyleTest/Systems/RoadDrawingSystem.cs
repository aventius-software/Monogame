using Microsoft.Xna.Framework;
using OutrunStyleTest.Components;
using OutrunStyleTest.Services;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Systems;

internal class RoadDrawingSystem : ISystem
{
    public World World { get; set; }
    private Filter filter;
    private readonly ShapeDrawingService _shapeDrawingService;

    public RoadDrawingSystem(World world, ShapeDrawingService shapeDrawingService)
    {
        World = world;
        _shapeDrawingService = shapeDrawingService;
    }

    public void Dispose()
    {
    }

    public void OnAwake()
    {
        filter = World.Filter.With<PlayerComponent>().Build();
    }

    public void OnUpdate(float deltaTime)
    {
        //_shapeDrawingService.DrawFilledTriangle(Color.Green, new Vector2(0, 0), new Vector2(100, 0), new Vector2(50, 100));

        _shapeDrawingService.DrawFilledQuadrilateral(Color.Green, new Vector2(0, 0), new Vector2(100, 0), new Vector2(150, 100), new Vector2(50, 100));
    }
}
