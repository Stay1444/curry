using System.Text.Json;
using CurryEngine.Editor.Rendering.ImGUI;
using IconFonts;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class ContentBrowserEditorPanel : EditorPanel
{
    private EditorRenderer _renderer;
    private readonly CurryProject? _project;

    private FsDialog? _fsDialog;
    
    public ContentBrowserEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
        _project = renderer.Editor.Project;
    
        if (_project is not null)
        {
            _fsDialog = FsDialog.Create(_project.Path, FsDialogFlags.Files | FsDialogFlags.Folders | FsDialogFlags.Relative, Array.Empty<string>(), _renderer.ImGuiRenderer, _project.Path, _project.Name);
        }
    }

    public override void Render()
    {
        if (_project is null) return;

        ImGui.Begin($"{FontAwesome4.FolderOpen} Content Browser###content-browser");

        if (_fsDialog?.Wait(out var selected) ?? false)
        {
            if (Path.GetExtension(selected)?.Contains("cscn") ?? false)
            {
                _renderer.Editor.ChangeScene(JsonSerializer.Deserialize<Scene>(File.ReadAllText(selected)));
            }
        }
        
        ImGui.End();
    }

    private void RenderTree()
    {
        ImGui.BeginChild("Tree");
        
        ImGui.EndChild();
    }
}