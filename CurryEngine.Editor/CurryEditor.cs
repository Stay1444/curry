using System.Diagnostics;
using CurryEngine.Editor.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Serilog.Events;

namespace CurryEngine.Editor;

public sealed class CurryEditor : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private SpriteBatch _spriteBatch = null!;
    private EditorRenderer _editorRenderer = null!;
    
    public CurryGame? Game { get; set; }
    public CurryProject? Project { get; set; }
    public RenderTarget2D? GameOutputTexture { get; set; }
    public VLogger VLogger { get; } = new VLogger();
    public CurryEditor()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void Initialize()
    {
        Log.Information("Initializing editor");
        IsMouseVisible = true;
        Window.IsBorderlessEXT = false;
        Window.AllowUserResizing = true;
        base.Initialize();
        Log.Information("Editor ready");
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / 200));
    }

    protected override void LoadContent()
    {
        Log.Information("Loading content");
        base.LoadContent();
        _spriteBatch = new SpriteBatch(this.GraphicsDevice);
        _editorRenderer = new EditorRenderer(this);
        _graphicsDeviceManager.PreferredBackBufferWidth = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1.2);
        _graphicsDeviceManager.PreferredBackBufferHeight = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.2);
        _graphicsDeviceManager.ApplyChanges();
        Log.Information("Content ready");
        
        //_game = CurryGame.Create(this.GraphicsDevice);
        //_game.LoadContent();
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        // TODO: Save things
        
        #if DEBUG
        // TODO: This is garbage. FNA hangs indefinitely after calling Exit() so we have to do this terrible fix.
        // https://github.com/FNA-XNA/FNA/issues/416
        Process.GetCurrentProcess().Kill(); 
        #endif
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        //_game.Update(gameTime);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Red);

        if (GameOutputTexture is not null)
        {
            GraphicsDevice.SetRenderTarget(GameOutputTexture);
            
            //_game.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
        }
        

        _spriteBatch.Begin(SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend); // TODO: Might change this later

        _editorRenderer.Render(gameTime);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}