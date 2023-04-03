using CurryEngine.Editor.Assets;
using CurryEngine.Editor.IO;

namespace CurryEngine.Editor;

public class CurryProject
{
    public const string STD_FS_EXTENSION = ".cprj";
    public string Name { get; set; } = null!;

    public Guid DefaultScene { get; set; }

    public Dictionary<Guid, AssetDescriptor> AssetDescriptors { get; set; }

    public CurryProject()
    {
        AssetDescriptors = new Dictionary<Guid, AssetDescriptor>();
    }
    
    public static CurryProject Read(BinaryReader reader)
    {
        var project = new CurryProject();
        
        project.Name = reader.ReadString();
        project.DefaultScene = new Guid(reader.ReadBytes(16));
        project.AssetDescriptors = new Dictionary<Guid, AssetDescriptor>();

        AssetSerializationHelper.Deserialize(project.AssetDescriptors, reader);

        return project;
    }

    public static void Write(CurryProject project, BinaryWriter writer)
    {
        writer.Write(project.Name);
        writer.Write(project.DefaultScene.ToByteArray());

        AssetSerializationHelper.Serialize(project.AssetDescriptors, writer);
    }
}