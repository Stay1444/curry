﻿using System.Numerics;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class NewProjectEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private bool _popupOpen = false;
    private bool _open = true;
    private string _name = "My New Project";
    private string _path = Environment.CurrentDirectory;

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

        ImGui.SetNextWindowSize(new Vector2(500, 120));
        ImGui.BeginPopupModal("New Project" + id, ref _open, ImGuiWindowFlags.NoResize);

        ImGui.Text("Project Name:");
        ImGui.SameLine();
        ImGui.InputText("###name", ref _name, 100);
        
        ImGui.Text("Folder Path [TODO]:"); // TODO
        ImGui.SameLine();
        ImGui.InputText("###path", ref _path, 100);

        if (ImGui.Button("Create"))
        {
            ImGui.CloseCurrentPopup();
            Directory.CreateDirectory(Path.Combine(_path, _name));
            _renderer.Editor.Project = CurryProject.Create(Path.Combine(_path, _name), _name);
        }
        
        ImGui.EndPopup();
    }
}