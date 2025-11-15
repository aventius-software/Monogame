using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace IsometricDynamicMapDemo.Map;

internal class MapInitialisationSystem : EntitySystem
{
    private readonly DiamondTileMapRenderer _diamondTileMapRenderer;
    //private readonly IsometricMapService _mapService;

    public MapInitialisationSystem(DiamondTileMapRenderer diamondTileMapRenderer) : base(Aspect.All(typeof(MapComponent)))
    {
        _diamondTileMapRenderer = diamondTileMapRenderer;
        //_mapService = mapService;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Load the Tiled map
        //_mapService.LoadTiledMap("Map/tiles_cubes.tmx", TileType.Cube);
        //_mapService.LoadTiledMap("Map/tiles_grass.tmx", TileType.Flat);
        _diamondTileMapRenderer.LoadMap("C:\\Users\\Ben\\source\\repos\\aventius-software\\Monogame\\IsometricDynamicMapDemo\\Content\\Map\\tiles_grass.tmx");

    }    
}
