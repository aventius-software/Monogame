using AStarPathFindUsingRoyT.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AStarPathFindUsingRoyT;

/// <summary>
/// Just a simple test of the RoyT A-star pathfinding library with a Tiled map. See https://github.com/roy-t/AStar
/// </summary>
public class GameMain : Game
{
    private Camera _camera;
    private Texture2D _characterTexture;
    private GraphicsDeviceManager _graphics;
    private Grid _grid;
    private MapService _mapService;
    private List<IEdge> _pathToTraverse = [];
    private PathFinder _pathFinder;
    private Vector2 _position;
    private SpriteBatch _spriteBatch;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load a texture for a 'character'
        _characterTexture = Content.Load<Texture2D>("Textures/circle");

        // Load map
        _mapService = new MapService(_spriteBatch, Content);
        _mapService.LoadTiledMap("Maps/test map.tmx", "Maps/test tile atlas");

        // Create a camera
        _camera = new Camera();

        // Tell the camera the dimensions of the world
        _camera.SetWorldDimensions(new Vector2(_mapService.WorldWidth, _mapService.WorldHeight));

        // Set the camera origin to the middle of the viewport, also note the offset for the size of the character sprite
        _camera.SetOrigin(new Vector2(
            GraphicsDevice.Viewport.Width / 2 - _characterTexture.Width / 2,
            GraphicsDevice.Viewport.Height / 2 - _characterTexture.Height / 2));

        // Place the character at some 'world' position coordinates
        _position = new Vector2(0, 0);

        // Setup RoyT A* pathfinding service   
        var gridSize = new GridSize(columns: _mapService.Tiles.GetLength(1), rows: _mapService.Tiles.GetLength(0));
        var cellSize = new Size(Distance.FromMeters(10), Distance.FromMeters(10));
        var traversalVelocity = Velocity.FromMetersPerSecond(10);

        // Create a new grid, each cell is laterally connected (like how a rook moves over a chess board, other options are available)       
        _grid = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

        // Add impassable 'tiles'
        for (var row = 0; row < _mapService.Tiles.GetLength(0); row++)
        {
            for (var col = 0; col < _mapService.Tiles.GetLength(1); col++)
            {
                if (_mapService.Tiles[row, col] > 1) _grid.DisconnectNode(new GridPosition(col, row));
            }
        }

        _pathFinder = new PathFinder();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Check for mouse clicks on the map        
        if (Mouse.GetState().LeftButton == ButtonState.Pressed && _pathToTraverse.Count == 0)
        {
            // Get the screen mouse position
            var mousePosition = Mouse.GetState().Position.ToVector2();
            var worldPosition = _camera.ScreenToWorld(mousePosition);
            var tileMapPosition = new Point((int)worldPosition.X / _mapService.TileWidth, (int)worldPosition.Y / _mapService.TileHeight);
            
            // Set the A* details and try and find a path
            var currentAStarGridPosition = new GridPosition((int)_position.X / _mapService.TileWidth, (int)_position.Y / _mapService.TileHeight);
            var destinationAStarGridPosition = new GridPosition(tileMapPosition.X, tileMapPosition.Y);
            var path = _pathFinder.FindPath(currentAStarGridPosition, destinationAStarGridPosition, _grid);

            // Set actual path to traverse
            _pathToTraverse = [.. path.Edges];            
        }

        if (_pathToTraverse.Count > 0)
        {
            var nextPosition = new Vector2(_pathToTraverse[0].End.Position.X / 10f, _pathToTraverse[0].End.Position.Y / 10f);
            nextPosition.X *= _mapService.TileWidth;
            nextPosition.Y *= _mapService.TileHeight;

            if (_position == nextPosition)
            {
                // Remove from list
                _pathToTraverse.RemoveAt(0);
            }
            else
            {
                // Keep moving towards next position
                _position.X += nextPosition.X - _position.X;
                _position.Y += nextPosition.Y - _position.Y;
            }
        }

        // Move the player
        //var keyboard = Keyboard.GetState();
        //var direction = Vector2.Zero;
        //var speed = 4;

        //if (keyboard.IsKeyDown(Keys.Up)) direction.Y = -speed;
        //if (keyboard.IsKeyDown(Keys.Down)) direction.Y = speed;
        //if (keyboard.IsKeyDown(Keys.Left)) direction.X = -speed;
        //if (keyboard.IsKeyDown(Keys.Right)) direction.X = speed;

        //_position += direction;

        // Restrict movement to the world
        //if (_position.X < 0) _position.X = speed;
        //if (_position.X > _gameWorld.WorldWidth - _character.Width) _position.X = _gameWorld.WorldWidth - speed - _character.Width;
        //if (_position.Y < 0) _position.Y = speed;
        //if (_position.Y > _gameWorld.WorldHeight - _character.Height) _position.Y = _gameWorld.WorldHeight - speed - _character.Height;

        // Set camera to the player position, set offset so we account for the character sprite origin
        // being the top left corner of the sprite, this makes the camera constrain to the end of the
        // map 'minus' the width/height of the character. Otherwise we'd get a gap at the end of the map
        _camera.LookAt(_position, new Vector2(_characterTexture.Width, _characterTexture.Height));

        // (Optionally) tell map where the player is, this helps the world map
        // drawing restrict tile drawing to only the tiles visible at the
        // specified position within the viewport area also specified. If we
        // comment this line out, the map service will draw all map tiles
        _mapService.SetViewport(_camera.Position, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Start drawing, notice the 'transformMatrix' which is from our camera
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: null,
            samplerState: SamplerState.PointClamp,
            depthStencilState: null,
            rasterizerState: null,
            effect: null,
            transformMatrix: _camera.TransformMatrix);

        // First draw the map (so it will be under the character)
        _mapService.Draw();

        // Now draw character after, this way it will be on top of the map
        _spriteBatch.Draw(
            texture: _characterTexture,
            position: _position,
            color: Color.White);

        // We're done...
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
