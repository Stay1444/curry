namespace CurryEngine.IO;

public static class SceneSerializationHelper
{
    public static Scene Deserialize(BinaryReader reader)
    {
        var id = new Guid(reader.ReadBytes(16));
        
        var scene = new Scene(id)
        {
            Name = reader.ReadString()
        };

        var count = reader.ReadInt16();

        for (var _ = 0; _ < count; _++)
        {
            scene.Children.Add(GameObjectSerializationHelper.Deserialize(reader));
        }

        return scene;
    }

    public static void Serialize(Scene scene, BinaryWriter writer)
    {
        writer.Write(scene.Id.ToByteArray());
        writer.Write(scene.Name);
        writer.Write((short)scene.Children.Count);

        foreach (var gameObject in scene.Children)
        {
            gameObject.Serialize(writer);
        }
    }
}