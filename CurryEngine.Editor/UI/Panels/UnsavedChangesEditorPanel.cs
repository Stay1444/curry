using System.Diagnostics;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class UnsavedChangesEditorPanel : EditorPanel
{
    private bool _open;
    private EditorRenderer _renderer;

    public UnsavedChangesEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        if (_open)
        {
            ImGui.OpenPopup("Unsaved Changes");
        }
        if (ImGui.BeginPopupModal("Unsaved Changes", ref _open, ImGuiWindowFlags.Modal | ImGuiWindowFlags.Popup))
        {
            ImGui.Text("You have unsaved changes. Are you sure you want to exit?");

            if (ImGui.Button("Don't Save"))
            {
                _renderer.Editor.SceneHasUnsavedChanges = false;
                _renderer.Editor.Exit();
            }
            
            ImGui.SameLine();
            if (ImGui.Button("Save Changes And Exit"))
            {
                _renderer.Editor.SaveCurrentScene();
                _renderer.Editor.SaveProject();
                _renderer.Editor.SceneHasUnsavedChanges = false;
                _renderer.Editor.Exit();
            }
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                _open = false;
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }

    public override bool OnExiting()
    {
        if (_renderer.Editor.SceneHasUnsavedChanges)
        {
            _open = true;
            return false;
        }

        return true;
    }
}