using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CurryEngine.Editor;

public class CurryProject
{
    
    [YamlIgnore]
    public string Path { get; }

    public string Name { get; set; }
    public string InitialScene { get; set; }
    
    private CurryProject(string path)
    {
        this.Path = path;
    }

    public static CurryProject Load(string path)
    {
        return new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build().Deserialize<CurryProject>(File.ReadAllText(System.IO.Path.Combine(path, "curry.yml")));
    }

    public static CurryProject Create(string folder, string name)
    {
        var p = new CurryProject(folder)
        {
            Name = name
        };
        File.WriteAllText(System.IO.Path.Combine(folder, "curry.yml"), 
            new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build().Serialize(p)
            );
        return p;
    }

    public void Save()
    {
        File.WriteAllText(System.IO.Path.Combine(Path, "curry.yml"), 
            new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build().Serialize(this)
        );
    }   
}