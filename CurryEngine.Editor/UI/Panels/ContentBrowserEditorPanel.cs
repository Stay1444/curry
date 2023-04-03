using System.Text.Json;
using CurryEngine.Editor.Assets;
using CurryEngine.Editor.Rendering.ImGUI;
using CurryEngine.IO;
using IconFonts;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class ContentBrowserEditorPanel : EditorPanel
{
    private EditorRenderer _renderer;
    private readonly string? _projectPath;
    private readonly CurryProject? _project;
    private FsDialog? _fsDialog;
    
    public ContentBrowserEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
        _project = _renderer.Editor.Project;
        _projectPath = _renderer.Editor.ProjectFolderPath;
    
        if (_project is not null && _projectPath is not null)
        {
            _fsDialog = FsDialog.Create(_projectPath, FsDialogFlags.Files | FsDialogFlags.Folders | FsDialogFlags.Relative, Array.Empty<string>(), _renderer.ImGuiRenderer, _projectPath, _project.Name, _renderer.Editor);
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
                using var fs = File.OpenRead(selected!);
                var scene = SceneSerializationHelper.Deserialize(new BinaryReader(fs));
                if (!_renderer.Editor.Project!.AssetDescriptors.Any(x =>
                        x.Value.Type == AssetType.Scene && x.Value.Id == scene.Id))
                {
                    _renderer.Editor.Project.AssetDescriptors.Add(scene.Id, new AssetDescriptor()
                    {
                        Id = scene.Id,
                        Path = selected!,
                        Type = AssetType.Scene
                    });
                    
                    _renderer.Editor.SaveProject();
                }
                
                _renderer.Editor.ChangeScene(scene);
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