namespace CurryEngine.IO;

public static class GameObjectSerializationHelper
{
    public static GameObject Deserialize(BinaryReader reader)
    {
        var gameObject = new GameObject
        {
            Id = new Guid(reader.ReadBytes(16)),
            Name = reader.ReadString(),
            DontDestroyOnLoad = reader.ReadBoolean(),
            Enabled = reader.ReadBoolean()
        };

        var childrenCount = reader.ReadInt16();

        for (var _ = 0; _ < childrenCount; _++)
        {
            gameObject.Children.Add(Deserialize(reader));
        }

        var componentCount = reader.ReadInt16();

        for (var _ = 0; _ < componentCount; _++)
        {
            gameObject.Components.Add(ComponentSerializationHelper.Deserialize(reader));
        }

        return gameObject;
    }

    public static void Serialize(this GameObject gameObject, BinaryWriter writer)
    {
        writer.Write(gameObject.Id.ToByteArray());
        writer.Write(gameObject.Name);
        writer.Write(gameObject.DontDestroyOnLoad);
        writer.Write(gameObject.Enabled);
        
        writer.Write((short)gameObject.Children.Count);

        foreach (var child in gameObject.Children)
        {
            Serialize(child, writer);
        }

        writer.Write((short)gameObject.Components.Count);

        foreach (var component in gameObject.Components)
        {
            component.Serialize(writer);
        }
    }
}