using System.Numerics;
using System.Text.Json;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class NewSceneEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private bool _popupOpen = false;
    private bool _open = true;
    private string _name = "Scene";

    public NewSceneEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        const string id = "###new_scene";

        if (ImGui.IsPopupOpen(id) && !_popupOpen)
        {
            ShouldBeRemoved = true;
            return;
        }else if (!ImGui.IsPopupOpen(id) && _popupOpen)
        {
            this.ShouldBeRemoved = true;
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
        ImGui.BeginPopupModal("New Scene" + id, ref _open, ImGuiWindowFlags.NoResize);

        ImGui.Text("Scene Name:");
        ImGui.SameLine();
        ImGui.InputText("###name", ref _name, 100);

        if (File.Exists($"{_name}.cscn"))
        {
            ImGui.TextColored(new Vector4(1,0,0,1), $"Scene with name '{_name}' already exists!");
        }
        else
        {
            if (ImGui.Button("Create"))
            {
                var scene = new Scene();
                scene.Name = _name;
                File.WriteAllText(_name + ".cscn", JsonSerializer.Serialize(scene));
                _renderer.Editor.ChangeScene(scene);
                ImGui.CloseCurrentPopup();
            }    
        }
        
        ImGui.EndPopup();
    }
}