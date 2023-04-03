using CurryEngine.Editor.Assets;
using CurryEngine.Editor.UI;
using CurryEngine.IO;
using IconFonts;
using ImGuiNET;
using Serilog;
using Num = System.Numerics;

namespace CurryEngine.Editor.Rendering.ImGUI;

[Flags]
public enum FsDialogFlags
{
    None =              0x00000000,
    Files =             0x00000001,
    Folders =           0x00000002,
    ExtensionFilter =   0x00000004,
    Popup =             0x00000008,
    Relative =          0x00000010
}

public class FsDialog
{ 
    private class FsItem
    {
        public string FullName { get; set; }
        public string Name;
        public bool Directory { get; set; }
        public bool AccessDenied { get; set; }
        public List<FsItem>? Children;
        public bool WasOpen { get; set; }
    }

    private readonly ImGuiRenderer _renderer;
    private readonly string _originPath;
    private FsDialogFlags _flags;
    private readonly string[] _extensionFilter;
    private string? _renaming = null;
    private string __path;
    private readonly CurryEditor _editor;
    private string _path
    {
        get => __path;

        set
        {
            _history.Add(value);
            _historyIndex = _history.Count - 1;
            if (_historyIndex < 0) _historyIndex = 0;
            
            __path = value;
        }
    }

    private FsItem? _selectedFsItem;
    private string _fsRoot;
    private int _cWidthSet = 0;
    private string _search = "";
    private List<string> _history = new List<string>();
    private int _historyIndex = 0;
    
    private FsItem _fsModel;
    
    private FsDialog(string path, FsDialogFlags flags, string[] extensionFilter, ImGuiRenderer renderer, string rootPath, string? rootName, CurryEditor editor)
    {
        this._editor = editor;
        _flags = flags;
        _extensionFilter = extensionFilter;
        _renderer = renderer;
        this._originPath = path;
        this._path = path;
        this._fsRoot = rootPath;
        if (rootName == null) rootName = rootPath;
        
        _fsModel = new FsItem()
        {
            Name = rootName,
            FullName = rootPath,
            Directory = true
        };
    }

    public static FsDialog Create(string path, FsDialogFlags flags, CurryEditor editor)
    {
        return new FsDialog(path, flags, Array.Empty<string>(), editor.EditorRenderer.ImGuiRenderer, Path.GetPathRoot(Environment.CurrentDirectory), null, editor);
    }

    public static FsDialog Create(string path, FsDialogFlags flags, string[] extensionFilter, CurryEditor editor)
    {
        return new FsDialog(path, flags, extensionFilter, editor.EditorRenderer.ImGuiRenderer, Path.GetPathRoot(Environment.CurrentDirectory), null, editor);
    }

    public static FsDialog Create(string path, FsDialogFlags flags, string[] extensionFilter, ImGuiRenderer renderer,
        string rootPath, string rootName, CurryEditor editor)
    {
        return new FsDialog(path, flags, extensionFilter, editor.EditorRenderer.ImGuiRenderer, rootPath, rootName, editor);
    }
    
    private bool IsActive(string path)
    {
        return _path.StartsWith(path);
    }
    
    private void ScanChildren(FsItem item)
    {
        item.Children = new List<FsItem>();
        try
        {
            foreach (var dir in Directory.GetDirectories(item.FullName))
            {
                item.Children.Add(new FsItem()
                {
                    Name = new DirectoryInfo(dir).Name,
                    FullName = dir,
                    Directory = true
                });
            }

            foreach (var file in Directory.GetFiles(item.FullName))
            {
                item.Children.Add(new FsItem()
                {
                    Name = Path.GetFileName(file),
                    FullName = file
                });
            }
        }
        catch (UnauthorizedAccessException)
        {
            item.AccessDenied = true;
        }
    }
    
    private void RenderTree(FsItem item)
    {
        if (item.FullName == _path) _selectedFsItem = item;
        
        var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;

        if (IsActive(item.FullName))
        {
            flags |= ImGuiTreeNodeFlags.DefaultOpen;
        }
        
        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1, 1, 1f, _path == item.FullName ? 1 : 0.5f));

        if (item.AccessDenied)
        {
            flags |= ImGuiTreeNodeFlags.Leaf;
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1, 0.5f, 0.2f, 0.7f));
        }

        var icon = item.WasOpen switch
        {
            true => FontAwesome4.FolderOpen,
            false => FontAwesome4.Folder
        };

        void Interact()
        {
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _path = item.FullName;
                _selectedFsItem = item;
            }
        }

        string name;

        name = item.AccessDenied ? $"{FontAwesome4.Lock} {item.Name}###{item.FullName}" : $"{icon} {item.Name}###{item.FullName}";
        
        if (ImGui.TreeNodeEx(name, flags))
        {
            if (item.Directory) item.WasOpen = true;
            
            if (item.AccessDenied)
            {
                ImGui.PopStyleColor();
            }
            
            ImGui.PopStyleColor();
            
            if (item is {Directory: true, Children: null})
            {
                ScanChildren(item);
            }

            Interact();
            
            if (item.Children is not null)
            {
                foreach (var child in item.Children.Where(x => x.Directory))
                {
                    RenderTree(child);
                }
            }
            ImGui.TreePop();
        }
        else
        {
            Interact();

            if (item.Directory) item.WasOpen = false;
            
            if (item.AccessDenied)
            {
                ImGui.PopStyleColor();
            }
            
            ImGui.PopStyleColor();
        }
    }

    private void RenderView(FsItem item, ref string? selection)
    {
        if (item.Children is null)
        {
            ScanChildren(item);
        }
        
        var cellSize = 64 + 16; // image size + padding

        var panelWidth = ImGui.GetContentRegionAvail().X;

        var columnCount = (int) (panelWidth / cellSize);
        if (columnCount < 1) columnCount = 1;

        ImGui.Columns(columnCount, "0", false);
        
        foreach (var file in item.Children!)
        {
            if (!string.IsNullOrWhiteSpace(_search) && !file.Name.ToLower().Contains(_search.ToLower()))
            {
                continue;    
            }
            
            if (file.Directory)
            {
                ImGui.PushID(file.FullName);

                var icon = IconManager.FS_Folder_Icon.GetImGuiId(_renderer);
                
                ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(0,0,0,0));
                ImGui.ImageButton(file.FullName + "##button", icon, new Num.Vector2(64, 64));
                
                if (ImGui.BeginPopupContextItem(file.FullName +"###right-click"))
                {
                    if (ImGui.MenuItem("Into"))
                    {
                        _path = file.FullName;
                    }
                    
                    if (ImGui.MenuItem("Rename"))
                    {
                        _renaming = _renaming == file.FullName ? null : file.FullName;
                    }
                    
                    ImGui.Separator();

                    if (ImGui.MenuItem("Delete"))
                    {
                        Directory.Delete(file.FullName, true);
                        item.Children = null;
                    }
                    
                    ImGui.EndPopup();
                }
                
                ImGui.PopStyleColor();

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    _path = file.FullName;
                }

                if (_renaming != file.FullName)
                {
                    var name = file.Name;
                    if (name.Length > 12)
                    {
                        name = name.Substring(0, 12) + "...";
                    }
                    
                    ImGui.TextWrapped(name);
                }
                else
                {
                    if (ImGui.InputText("", ref file.Name, 100, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        var repl = file.FullName.Replace(Path.GetFileName(file.FullName)!, file.Name);
                        Directory.Move(file.FullName, repl);
                        item.Children = null;
                        _renaming = null;
                    }
                }
                
                ImGui.NextColumn();
                
                ImGui.PopID();
            }
            else
            {
                ImGui.PushID(file.FullName);

                var extension = Path.GetExtension(file.FullName);
                var icon = FileExtensionImageProvider.GetFileIcon(extension.Replace(".", ""));

                ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(0,0,0,0));
                ImGui.ImageButton(file.FullName + "##button", icon, new Num.Vector2(64, 64));
                
                if (ImGui.BeginPopupContextItem(file.FullName +"###right-click"))
                {
                    if (ImGui.MenuItem("Rename"))
                    {
                        if (_renaming == file.FullName)
                        {
                            _renaming = null;
                        }
                        else
                        {
                            _renaming = file.FullName;
                        }
                    }
                    
                    ImGui.Separator();

                    if (ImGui.MenuItem("Delete"))
                    {
                        File.Delete(file.FullName);
                        item.Children = null;
                    }
                    
                    ImGui.EndPopup();
                }
                
                ImGui.PopStyleColor();

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    selection = file.FullName;
                }

                if (_renaming != file.FullName)
                {
                    var name = file.Name;
                    if (name.Length > 12)
                    {
                        name = name.Substring(0, 12) + "...";
                    }

                    ImGui.TextWrapped(name);
                }
                else
                {
                    if (ImGui.InputText("", ref file.Name, 100, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                    {
                        File.Move(file.FullName, file.FullName.Replace(Path.GetFileName(file.FullName), file.Name));
                        item.Children = null;
                        _renaming = null;
                    }
                }
                
                ImGui.NextColumn();
                
                ImGui.PopID();
            }
        }
    }
    
    public bool Wait(out string? selection)
    {
        if (_flags.HasFlag(FsDialogFlags.Relative))
        {
            if (__path.StartsWith(Environment.CurrentDirectory))
            {
                __path = __path.Replace(Environment.CurrentDirectory, "." + Path.DirectorySeparatorChar);
            }

            if (_fsModel.FullName.StartsWith(_fsRoot))
            {
                _fsModel.FullName = _fsModel.FullName.Replace(_fsRoot, "." + Path.DirectorySeparatorChar);
            }
        }

        selection = null;

        bool open = true;

        if (_flags.HasFlag(FsDialogFlags.Popup))
        {
            ImGui.BeginPopupModal($"File System Browser###fsbrowser", ref open,
                ImGuiWindowFlags.Modal | ImGuiWindowFlags.Popup | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking);
        }

        if (ImGui.ImageButton("###backward", IconManager.I_CaretLeft_Icon.GetImGuiId(_renderer),
                new Num.Vector2(16, 16)))
        {
            _historyIndex--;
            if (_historyIndex < _history.Count && _historyIndex >= 0)
            {
                __path = _history[_historyIndex];
            }
        }
        
        ImGui.SameLine();
        if (ImGui.ImageButton("###forward", IconManager.I_CaretRight_Icon.GetImGuiId(_renderer),
                new Num.Vector2(16, 16)))
        {
            _historyIndex--;
            if (_historyIndex < _history.Count && _historyIndex >= 0)
            {
                __path = _history[_historyIndex];
            }
        }
        
        ImGui.SameLine();
        if (ImGui.ImageButton("###upward", IconManager.I_CaretUp_Icon.GetImGuiId(_renderer), new Num.Vector2(16, 16)))
        {
            try
            {
                var rel = Path.GetRelativePath(Environment.CurrentDirectory, _fsRoot) + Path.DirectorySeparatorChar;
                if (_path != _fsRoot && _path != rel)
                {
                    var parentFullName = new DirectoryInfo(_path).Parent?.FullName;
                    if (parentFullName != null)
                    {
                        if (_flags.HasFlag(FsDialogFlags.Relative))
                        {
                            var target = Path.GetRelativePath(Environment.CurrentDirectory, parentFullName) +
                                         Path.DirectorySeparatorChar; // TODO UGLY CODE AHEAD
                            
                            if (!target.StartsWith("." + Path.DirectorySeparatorChar))
                            {
                                _path = $".{Path.DirectorySeparatorChar}{target}";
                            }
                            else
                            {
                                _path = target;
                            }
                        }
                        else
                        {
                            _path = parentFullName;
                        }
                    }
                }
            }
            catch
            {
                // ignore XD
            }
        }
        
        ImGui.SameLine();
        var width = ImGui.GetContentRegionAvail().X;
        ImGui.SetNextItemWidth(width - width / 3);

        ImGui.InputText("###path", ref __path, 999);

        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        ImGui.InputTextWithHint("###search", "Search...", ref _search, 999);
        
        ImGui.Separator();
        ImGui.Columns(2);
        
        if (_cWidthSet < 15)
        {
            _cWidthSet++;
            ImGui.SetColumnWidth(0, 350.0f);
        }
        
        ImGui.BeginChild("###left");

        RenderTree(_fsModel);        
        
        ImGui.EndChild();

        
        ImGui.NextColumn();

        ImGui.BeginChild("###view", new Num.Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y));

        if (ImGui.BeginPopupContextWindow("###wcr"))
        {
            if (ImGui.BeginMenu("New"))
            {
                if (ImGui.MenuItem($"{FontAwesome4.Film} Scene"))
                {
                    var scene = new Scene(Guid.NewGuid());
                    scene.Name = "Scene";
                    
                    var fileName = $"{scene.Name}.cscn";
                    var iter = 0;
                    
                    while (File.Exists(Path.Combine(_path, fileName)))
                    {
                        fileName = $"{scene.Name}{++iter}.cscn";
                    }


                    using var fs = File.OpenWrite(Path.Combine(_path, fileName));
                    
                    SceneSerializationHelper.Serialize(scene, new BinaryWriter(fs));
                    if (_selectedFsItem is not null)
                    {
                        _selectedFsItem.Children = null;
                    }
                }

                if (ImGui.MenuItem($"{FontAwesome4.Code} Script"))
                {
                    
                }
                
                ImGui.Separator();

                if (ImGui.MenuItem("Directory"))
                {
                    int id = 0;
                    
                    while (Directory.Exists("New Directory" + id))
                    {
                        id++;
                    }

                    Directory.CreateDirectory(Path.Combine(_path, "New Directory" + id));
                    _selectedFsItem!.Children = null;
                }
                
                ImGui.EndMenu();
            }

            ImGui.Separator();
            
            if (ImGui.MenuItem("Reload"))
            {
                _selectedFsItem!.Children = null;
            }
            
            ImGui.EndPopup();
        }
        
        if (_selectedFsItem is not null && _selectedFsItem.Directory)
        {
            RenderView(_selectedFsItem, ref selection);
        }

        ImGui.EndChild();

        if (_flags.HasFlag(FsDialogFlags.Popup))
        {
            ImGui.End();
            
            if (!ImGui.IsPopupOpen("###fsbrowser"))
            {
                ImGui.OpenPopup("###fsbrowser");
            }
        }
        

             
        return selection != null;
    }
}