using System.Numerics;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class OpenProjectEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private bool _popupOpen = false;
    private bool _open = true;
    private string _path = Environment.CurrentDirectory;

    public OpenProjectEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        const string id = "###open_project";

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

        ImGui.SetNextWindowSize(new Vector2(500, 120));
        ImGui.BeginPopupModal("Open Project" + id, ref _open, ImGuiWindowFlags.NoResize);
        
        ImGui.Text("Folder Path [TODO]:"); // TODO
        ImGui.SameLine();
        ImGui.InputText("###open_project", ref _path, 100);

        if (ImGui.Button("Open"))
        {
            _renderer.Editor.Project = CurryProject.Load(_path);
            _renderer.Editor.CreateGame();
            this.ShouldBeRemoved = true;
            ImGui.CloseCurrentPopup();
        }
        
        ImGui.EndPopup();
    }
}