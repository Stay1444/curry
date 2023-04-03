using System.Numerics;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class OpenProjectEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private bool _popupOpen = false;
    private bool _open = true;
    private string _path = Environment.CurrentDirectory;
    
    private bool _previousSesionPathScanned;
    
    public OpenProjectEditorPanel(EditorRenderer renderer)
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
            }
        }
        
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
            var fileName = _path;
            
            if (!File.Exists(_path) && Directory.Exists(_path) && Directory.GetFiles(_path).Any(x => Path.GetExtension(x).Contains(CurryProject.STD_FS_EXTENSION)))
            {
                fileName = Directory.GetFiles(_path)
                    .First(x => Path.GetExtension(x).Contains(CurryProject.STD_FS_EXTENSION));
            }
            else if (!File.Exists(_path))
            {
                return;
            }
            
            using var fs = File.OpenRead(fileName);
            var project = CurryProject.Read(new BinaryReader(fs));

            _renderer.Editor.DestroyGame();

            _renderer.Editor.SetProject(project, _path, fileName);
            
            _renderer.Editor.CreateGame();
            
            this.ShouldBeRemoved = true;
            ImGui.CloseCurrentPopup();

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