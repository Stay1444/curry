using System.Numerics;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class NewProjectEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private bool _popupOpen = false;
    private bool _open = true;
    private string _name = "My New Project";
    private string _path = Environment.CurrentDirectory;

    private bool _previousSesionPathScanned;

    public NewProjectEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        if (!_previousSesionPathScanned)
        {
            _previousSesionPathScanned = true;

            if (File.Exists(Path.Combine(CurryEditor.AppDataPath, "PREVIOUS_SESSION_PATH")))
            {
                _path = File.ReadAllText(Path.Combine(CurryEditor.AppDataPath, "PREVIOUS_SESSION_PATH"));
                _path = Directory.GetParent(_path)?.FullName ?? Environment.CurrentDirectory;
            }
        }
        
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
            #if LINUX
            Directory.CreateDirectory(Path.Combine(_path, _name), UnixFileMode.UserWrite | UnixFileMode.UserRead | UnixFileMode.GroupRead | UnixFileMode.GroupWrite);
            #else
            Directory.CreateDirectory(Path.Combine(_path, _name));
            #endif
            var project = new CurryProject();
            project.Name = _name;
            var fileName = Path.Combine(_path, _name, _name + CurryProject.STD_FS_EXTENSION);
            using var fs = File.OpenWrite(fileName);
            
            var bw = new BinaryWriter(fs);
             
            CurryProject.Write(project, bw);
             
            _renderer.Editor.SetProject(project, Path.Combine(_path, _name), fileName);
            this.ShouldBeRemoved = true;
            
            try
            {
                File.WriteAllText(Path.Combine(CurryEditor.AppDataPath, "PREVIOUS_SESSION_PATH"), _path);
            }
            catch
            {
                // ignored
            }
        }
        
        ImGui.EndPopup();
    }
}