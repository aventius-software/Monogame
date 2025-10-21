using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace PlatformerWithTiledMapDemo.Map;

internal class MapInitialisationSystem : EntitySystem
{
    private readonly MapService _mapService;

    public MapInitialisationSystem(MapService mapService) : base(Aspect.All(typeof(MapComponent)))
    {
        _mapService = mapService;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        // Load the Tiled map
        _mapService.LoadMap("Map/level1");        
    }    
}
