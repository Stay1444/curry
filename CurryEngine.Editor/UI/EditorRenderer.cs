﻿using CurryEngine.Editor.Rendering.ImGUI;
using CurryEngine.Editor.UI.Panels;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace CurryEngine.Editor.UI;

public class EditorRenderer
{
    private readonly CurryEditor _editor;
    private ImGuiRenderer _renderer;
    private List<EditorPanel> _panels = new List<EditorPanel>();
    
    public CurryEditor Editor => _editor;
    public ImGuiRenderer ImGuiRenderer => _renderer;
    public ImFontPtr Font20 { get; }
    public ImFontPtr Font16 { get; }
    
    public EditorRenderer(CurryEditor editor)
    {
        _editor = editor;
        _renderer = new ImGuiRenderer(editor);
        _renderer.RebuildFontAtlas();
        
        // Enable docking
        ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable;
       
        ImGuiUtils.SetupImGuiStyle();

        // TODO: Load fonts from memory / assembly storage.
        Font20 = ImGui.GetIO().Fonts.AddFontFromFileTTF("Ubuntu.ttf", 20);
        Font16 = ImGui.GetIO().Fonts.AddFontFromFileTTF("Ubuntu.ttf", 16);
        
        _renderer.RebuildFontAtlas();
        

        _panels.AddRange(new EditorPanel[]
        {
            new ContentBrowserEditorPanel(this),
            new LogsEditorPanel(this),
            new InspectorEditorPanel(this),
            new HierarchyEditorPanel(this),
            new ViewportEditorPanel(this)
        });
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
        
        
        ImGui.PushFont(Font20);

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
                    //TODO
                }

                if (ImGui.MenuItem("New Project..."))
                {
                    _panels.Add(new NewProjectEditorPanel(this));
                }
                
                if (ImGui.MenuItem("New Scene", "Ctrl+N"))
                {
                    // TODO
                }

                if (ImGui.MenuItem("Save Scene", "Ctrl+S"))
                {
                    // TODO
                }

                if (ImGui.MenuItem("Save Scene As...", "Ctrl+Shift+S"))
                {
                    // TODO
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
                i--;
                continue;
            }
            p.Render();
        }
        
        _renderer.AfterLayout();
    }
}