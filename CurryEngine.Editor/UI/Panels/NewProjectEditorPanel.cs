using System.Numerics;
using CurryEngine.Editor.Rendering.ImGUI;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class NewProjectEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private bool _popupOpen = false;
    private bool _open = true;
    private string _name = "My New Project";
    public NewProjectEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        const string id = "###new_project";

        if (ImGui.IsPopupOpen(id) && !_popupOpen)
        {
            ShouldBeRemoved = true;
            return;
        }

        if (!_open)
        {
            ShouldBeRemoved = true;
            return;
        }
        
        if (!_popupOpen)
        {
            ImGui.OpenPopup(id);
            _popupOpen = true;
        }

        ImGui.SetNextWindowSize(new Vector2(500, 300));
        ImGui.BeginPopupModal("New Project" + id, ref _open, ImGuiWindowFlags.NoResize);

        ImGui.Text("Project Name:");
        ImGui.SameLine();
        ImGui.InputText("", ref _name, 100);
        FSDialog.FolderPicker("Select a Folder for the Project");

        if (ImGui.Button("Create"))
        {
        }
        
        ImGui.EndPopup();
    }
}