using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonogameExtendedIsometricTiledMapDemo.Map;
using MonogameExtendedIsometricTiledMapDemo.Player;

namespace MonogameExtendedIsometricTiledMapDemo.Shared.Debugging;

internal class DebugSystem : EntityDrawSystem
{
    private readonly OrthographicCamera _camera;
    private readonly ContentManager _contentManager;
    private readonly IsometricMapService _mapService;
    private readonly SpriteBatch _spriteBatch;

    private SpriteFont _font;

    public DebugSystem(ContentManager contentManager, SpriteBatch spriteBatch, IsometricMapService mapService, OrthographicCamera camera)
        : base(Aspect.All(typeof(PlayerComponent)))
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;
        _mapService = mapService;
        _camera = camera;
    }

    public override void Draw(GameTime gameTime)
    {
        var mousePosition = Mouse.GetState().Position.ToVector2();
        var worldPosition = _camera.ScreenToWorld(mousePosition);

        // Get the position in the world
        var mapPosition = _mapService.WorldToMapTilePosition(worldPosition);

        // Draw some debugging information
        _spriteBatch.Begin();

        _spriteBatch.DrawString(
            spriteFont: _font,
            text: $"Mouse position in map: {mapPosition}",
            position: new Vector2(10, 10),
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 0.75f,
            effects: SpriteEffects.None,
            layerDepth: 0);

        _spriteBatch.End();
    }

    public override void Initialize(IComponentMapperService mapperService)
    {        
        _font = _contentManager.Load<SpriteFont>("Fonts/Default");
    }
}
