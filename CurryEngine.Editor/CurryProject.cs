using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CurryEngine.Editor;

public class CurryProject
{
    
    [YamlIgnore]
    public string Path { get; }

    public string Name { get; set; }
    public string? InitialScene { get; set; }
    
    public CurryProject(string path)
    {
        this.Path = path;
    }

    public static CurryProject Load(string path)
    {
        return JsonSerializer.Deserialize<CurryProject>(File.ReadAllText(System.IO.Path.Combine(path, "curry.cprj")));
    }

    public static CurryProject Create(string folder, string name)
    {
        var p = new CurryProject(folder)
        {
            Name = name,
            InitialScene = null
        };
        File.WriteAllText(System.IO.Path.Combine(folder, "curry.cprj"), 
            JsonSerializer.Serialize(p)
            );
        return p;
    }

    public void Save()
    {
        File.WriteAllText(System.IO.Path.Combine(Path, "curry.cprj"), JsonSerializer.Serialize(this));
    }   
}