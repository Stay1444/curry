using System.Runtime.InteropServices;
using System.Text;
using CurryEngine.Editor.Rendering.ImGUI;
using CurryEngine.Editor.UI.Panels;
using IconFonts;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace CurryEngine.Editor.UI;

public class EditorRenderer : IDisposable
{
    private readonly CurryEditor _editor;
    private ImGuiRenderer _renderer;
    private List<EditorPanel> _panels = new List<EditorPanel>();
    
    public CurryEditor Editor => _editor;
    public ImGuiRenderer ImGuiRenderer => _renderer;
    public ImFontPtr Font18 { get; }
    public ImFontPtr Font16 { get; }
    public ImFontPtr IconFont { get; }
    
    public EditorRenderer(CurryEditor editor)
    {
        _editor = editor;
        _renderer = new ImGuiRenderer(editor);
        _renderer.RebuildFontAtlas();
        
        // Enable docking
        ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable;

        unsafe
        {
            var path = Path.Combine(CurryEditor.AppDataPath, "curry-imgui.ini");

            ImGui.GetIO().NativePtr->IniFilename = null;

            if (File.Exists(path))
            {
                ImGui.LoadIniSettingsFromDisk(path);
            }
        }
        
        ImGuiUtils.SetupImGuiStyle();

        // TODO: Load fonts from memory / assembly storage.
        unsafe
        {
            ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();

            config.MergeMode = true;
            config.PixelSnapH = true;
            config.GlyphMinAdvanceX = 16.0f;
            
            var rangeHandle = GCHandle.Alloc(new ushort[]
            {
                FontAwesome4.IconMin,
                FontAwesome4.IconMax,
                0
            }, GCHandleType.Pinned);

            try
            {
                //IconFont = ImGui.GetIO().Fonts.AddFontFromAssemblyResource("CurryEngine.Editor.Resources.Fonts");
                //Font18 = ImGui.GetIO().Fonts
                //    .AddFontFromFileTTF("Ubuntu.ttf", 16, config, rangeHandle.AddrOfPinnedObject());

                Font18 = ImGui.GetIO().Fonts
                    .AddFontFromAssemblyResource("CurryEngine.Editor.Resources.Fonts.Ubuntu.ttf", 18);

                IconFont = ImGui.GetIO().Fonts.AddFontFromAssemblyResource(
                    "CurryEngine.Editor.Resources.Fonts.fontawesome-webfont.ttf", 16, config, rangeHandle.AddrOfPinnedObject());
                

                Font16 = ImGui.GetIO().Fonts
                    .AddFontFromAssemblyResource("CurryEngine.Editor.Resources.Fonts.Ubuntu.ttf", 16);
                
                IconFont = ImGui.GetIO().Fonts.AddFontFromAssemblyResource(
                    "CurryEngine.Editor.Resources.Fonts.fontawesome-webfont.ttf", 16, config, rangeHandle.AddrOfPinnedObject());
            }
            finally
            {
                config.Destroy();

                if (rangeHandle.IsAllocated)
                {
                    rangeHandle.Free();
                }
            }
        }
        
        _renderer.RebuildFontAtlas();

        FileExtensionImageProvider.Register(_renderer);
        DynamicIconManager.Initialize(_renderer);
        
        Reload();
    }

    public void Reload()
    {
        foreach (var panel in _panels)
        {
            if (panel is IDisposable disposable)
                disposable.Dispose();
        }

        _panels = new List<EditorPanel>();
        
        _panels.AddRange(new EditorPanel[]
        {
            new LogsEditorPanel(this),
            new ContentBrowserEditorPanel(this),
            new InspectorEditorPanel(this),
            new HierarchyEditorPanel(this),
            new ViewportEditorPanel(this),
            new UnsavedChangesEditorPanel(this)
        });
    }

    public bool OnExiting()
    {
        foreach (var panel in _panels)
        {
            if (!panel.OnExiting())
            {
                return false;
            }
        }

        return true;
    }
    
    public void Render(GameTime gameTime)
    {
        _renderer.BeforeLayout(gameTime);

        // Draw parent window

        var size = new Vector2(_editor.GraphicsDevice.PresentationParameters.BackBufferWidth,
            _editor.GraphicsDevice.PresentationParameters.BackBufferHeight);
        
        ImGui.SetNextWindowSize(size);
        ImGui.SetNextWindowPos(new Vector2(0,0));
        
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0,0));
        
        
        ImGui.PushFont(Font18);

        ImGui.Begin("##master", 
            ImGuiWindowFlags.NoResize
            | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.MenuBar
            | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus);

        var dockSpaceId = ImGui.GetID("MyDockSpace");
        ImGui.DockSpace(dockSpaceId, new Vector2(0, 0), ImGuiDockNodeFlags.None);

        ImGui.PopFont();
        ImGui.PushFont(Font16);

        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                
                if (ImGui.MenuItem("Open Project..."))
                {
                    _panels.Add(new OpenProjectEditorPanel(this));
                }

                if (ImGui.MenuItem("New Project..."))
                {
                    _panels.Add(new NewProjectEditorPanel(this));
                }

                if (_editor.Project is not null)
                {
                    if (ImGui.MenuItem("New Scene", "Ctrl+N"))
                    {
                        _panels.Add(new NewSceneEditorPanel(this));
                    }

                    if (ImGui.MenuItem("Save Scene", "Ctrl+S", false, _editor.Game?.ActiveScene is not null))
                    {
                        _editor.SaveCurrentScene();
                    }

                    if (ImGui.MenuItem("Save Scene As...", "Ctrl+Shift+S", false, false))
                    {
                        // TODO
                    }    
                }
                
                ImGui.Separator();

                if (ImGui.MenuItem("Exit"))
                {
                    _editor.Exit();
                }
                
                ImGui.EndMenu();
            }
            
            ImGui.EndMenuBar();
        }
        
        ImGui.End();
        
        ImGui.PopStyleVar();
        ImGui.PopStyleVar();
        ImGui.PopStyleVar();

        for (var i = 0; i < _panels.Count; i++)
        {
            var p = _panels[i];
            if (p.ShouldBeRemoved)
            {
                _panels.RemoveAt(i);
                if (p is IDisposable disposable)
                    disposable.Dispose();
                i--;
                continue;
            }
            p.Render();
        }
        
        _renderer.AfterLayout();
    }

    public void Dispose()
    {
        foreach (var panel in _panels)
        {
            if (panel is IDisposable disposable) disposable.Dispose();
        }        
    }
}