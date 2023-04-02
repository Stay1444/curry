using System.Diagnostics;
using CurryEngine.Editor.UI;
using ImGuiNET;
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
    private CurryProject _project;
    
    private bool _sceneHasUnsavedChanges = false;
    
    
    public CurryGame? Game { get; set; }
    public GameObject? SelectedEntity { get; set; }

    public bool SceneHasUnsavedChanges
    {
        get => _sceneHasUnsavedChanges;
        set
        {
            if (value)
            {
                Window.Title = "CurryEngine | Unsaved Changes";
            }
            else
            {
                Window.Title = "CurryEngine";
            }

            _sceneHasUnsavedChanges = value;
        }
    }
    
    public CurryProject? Project
    {
        get => _project;
        set
        {
            _project = value;
            if (_project is not null)
            {
                Environment.CurrentDirectory = _project.Path;
            }
            _editorRenderer.Reload();
        }
    }
    public RenderTarget2D? GameOutputTexture { get; set; }
    public VLogger VLogger { get; } = new VLogger();
    public CurryEditor()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    public void CreateGame()
    {
        Game = CurryGame.Create(GraphicsDevice);
        Game.LoadContent();
    }

    public void ChangeScene(Scene newScene)
    {
        if (newScene.Id == Game?.ActiveScene?.Id) return;
        Game?.SwitchScene(newScene);
    }

    public void DestroyGame()
    {
        Game.Dispose();
        Game = null;
    }

    protected override void Initialize()
    {
        Window.Title = "CurryEngine";
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
        
        //Game = CurryGame.Create(this.GraphicsDevice);
        //Game.LoadContent();
        
    }

    protected override bool OnExiting(object sender, EventArgs args)
    {
        if (SceneHasUnsavedChanges)
        {
            ImGui.OpenPopup("###exit_confirmation");
            return false;
        }

        return true;
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) ImGui.CloseCurrentPopup();

        if (Game is not null)
        {
            Game.Update(gameTime);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.S))
        {
            SceneHasUnsavedChanges = false;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Red);

        if (GameOutputTexture is not null && Game is not null)
        {
            GraphicsDevice.SetRenderTarget(GameOutputTexture);
            
            Game.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
        }
        

        _spriteBatch.Begin(SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend); // TODO: Might change this later

        _editorRenderer.Render(gameTime);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}