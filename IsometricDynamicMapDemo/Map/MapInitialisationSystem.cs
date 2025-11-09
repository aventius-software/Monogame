using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace IsometricDynamicMapDemo.Map;

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
        //_mapService.LoadTiledMap("Map/tiles_cubes.tmx", TileType.Cube);
        _mapService.LoadTiledMap("Map/tiles_grass.tmx", TileType.Flat);
    }    
}
