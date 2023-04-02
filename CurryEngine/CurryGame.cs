using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CurryEngine;

public sealed class CurryGame : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch = null!;
    public Scene? ActiveScene { get; private set; }
    public bool Paused { get; set; } = true;
    
    private CurryGame(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public static CurryGame Create(GraphicsDevice graphicsDevice)
    {
        return new CurryGame(graphicsDevice);
    }

    public void SwitchScene(Scene newScene)
    {
        ActiveScene = newScene;
    }
    
    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);
    }

    public void Update(GameTime time)
    {
        if (Paused) return;
    }

    public void Draw(GameTime time)
    {
        if (ActiveScene is null) return;
        
        _graphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend); // TODO: Might change this later

        _spriteBatch.End();
    }

    public void Dispose()
    {
        this.Paused = true;
        this.ActiveScene = null;
    }
}