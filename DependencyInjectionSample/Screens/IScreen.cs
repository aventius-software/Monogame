using Microsoft.Xna.Framework;

namespace DependencyInjectionSample.Screens;

public interface IScreen
{
    public void Draw(GameTime gameTime);    
    public void Initialise();
    public void LoadContent();
    public void UnloadContent();
    public void Update(GameTime gameTime);
}
