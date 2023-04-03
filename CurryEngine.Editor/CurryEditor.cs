using System.Diagnostics;
using CurryEngine.Editor.UI;
using CurryEngine.IO;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;

namespace CurryEngine.Editor;

public sealed class CurryEditor : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private SpriteBatch _spriteBatch = null!;
    private EditorRenderer _editorRenderer = null!;
    private CurryProject? _project;
    private string? _projectFilePath;
    private string? _projectFolderPath;
    
    private bool _sceneHasUnsavedChanges = false;
    
    public static string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CurryEngine");
    
    public CurryGame? Game { get; set; }
    public GameObject? SelectedEntity { get; set; }
    public EditorRenderer EditorRenderer => _editorRenderer;
    public bool SceneHasUnsavedChanges
    {
        get => _sceneHasUnsavedChanges;
        set
        {
            if (value && !_sceneHasUnsavedChanges)
            {
                Window.Title = "CurryEngine | Unsaved Changes";
            }
            else if (_sceneHasUnsavedChanges)
            {
                Window.Title = "CurryEngine";
            }

            _sceneHasUnsavedChanges = value;
        }
    }

    public CurryProject? Project => _project;
    public string? ProjectFilePath => _projectFilePath;
    public string? ProjectFolderPath => _projectFolderPath;

    public void SetProject(CurryProject project, string folderPath, string filePath)
    {

        this._project = project;
        this._projectFolderPath = folderPath;
        this._projectFilePath = filePath;
        
        Environment.CurrentDirectory = folderPath;
        
        _editorRenderer.Reload();
    }

    public void SaveProject()
    {
        if (_project is null) return;
        using var fs = File.OpenWrite(_projectFilePath!);
        CurryProject.Write(_project, new BinaryWriter(fs));
    }

    public void SaveCurrentScene()
    {
        if (_project is null) return;

        if (!_sceneHasUnsavedChanges) return;
            
        if (Game?.ActiveScene is null) return;
            
        var assetDescriptor = _project.AssetDescriptors.Values.FirstOrDefault(x => x.Id == Game?.ActiveScene?.Id);

        if (assetDescriptor is null) return;

        using var fs = File.OpenWrite(assetDescriptor.Path);
            
        SceneSerializationHelper.Serialize(Game.ActiveScene, new BinaryWriter(fs));
            
        SceneHasUnsavedChanges = false;
    }
    
    public RenderTarget2D? GameOutputTexture { get; set; }
    public VLogger VLogger { get; } = new VLogger();
    
    public CurryEditor()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Directory.CreateDirectory(AppDataPath);
    }

    public void CreateGame()
    {
        DestroyGame();
        
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
        GameOutputTexture?.Dispose();

        Game?.Dispose();
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
        if (!_editorRenderer.OnExiting())
        {
            return false;
        }
        
        ImGui.SaveIniSettingsToDisk(Path.Combine(AppDataPath, "curry-imgui.ini"));
        Log.Information("Exiting");
        new Thread(() =>
        {
            Thread.Sleep(500);
            Log.Debug("Killing process.");
            Log.Debug("https://github.com/FNA-XNA/FNA/issues/416");
            Process.GetCurrentProcess().Kill();
        }).Start();
        this.Dispose();
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
            SaveCurrentScene();
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