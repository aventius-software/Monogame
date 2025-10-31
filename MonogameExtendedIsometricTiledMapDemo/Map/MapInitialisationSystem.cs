using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace MonogameExtendedIsometricTiledMapDemo.Map;

internal class MapInitialisationSystem : EntitySystem
{
    private readonly IsometricMapService _mapService;

    public MapInitialisationSystem(IsometricMapService mapService) : base(Aspect.All(typeof(MapComponent)))
    {
        _mapService = mapService;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Load the Tiled map
        _mapService.LoadMap("Map/isometric map");
    }    
}
