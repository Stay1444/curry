using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CurryEngine;

public sealed class CurryGame : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch = null!;
    
    private Texture2D _texture = null!;
    private Vector2 _pos = new Vector2(0, 0);
    private CurryGame(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public static CurryGame Create(GraphicsDevice graphicsDevice)
    {
        return new CurryGame(graphicsDevice);
    }

    public void LoadContent()
    {
        using var str = File.OpenRead("Backrooms_model.jpg");
        _texture = Texture2D.FromStream(this._graphicsDevice, str);
        _spriteBatch = new SpriteBatch(_graphicsDevice);
    }

    public void Update(GameTime time)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            _pos = new Vector2(_pos.X, _pos.Y - 1);
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            _pos = new Vector2(_pos.X, _pos.Y + 1);
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            _pos = new Vector2(_pos.X + 1, _pos.Y);
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            _pos = new Vector2(_pos.X - 1, _pos.Y);
        }
    }

    public void Draw(GameTime time)
    {
        _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend); // TODO: Might change this later

        _spriteBatch.Draw(_texture, new Rectangle((int)_pos.X, (int)_pos.Y, 100, 100), Color.White);
        
        _spriteBatch.End();
    }

    public void Dispose()
    {
        
    }
}