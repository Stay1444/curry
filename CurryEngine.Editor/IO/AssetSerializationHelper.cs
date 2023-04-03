using CurryEngine.Editor.Assets;

namespace CurryEngine.Editor.IO;

public static class AssetSerializationHelper
{
    public static void Deserialize(Dictionary<Guid, AssetDescriptor> dictionary, BinaryReader reader)
    {
        var count = reader.ReadInt32();

        for (var _ = 0; _ < count; _++)
        {
            var id = new Guid(reader.ReadBytes(16));
            dictionary.Add(id, new AssetDescriptor()
            {
                Id = id,
                Path = reader.ReadString(),
                Type = (AssetType)reader.ReadInt16()
            });
        }
    }

    public static void Serialize(Dictionary<Guid, AssetDescriptor> dictionary, BinaryWriter writer)
    {
        writer.Write(dictionary.Count);

        foreach (var assetDescriptor in dictionary)
        {
            writer.Write(assetDescriptor.Key.ToByteArray());
            writer.Write(assetDescriptor.Value.Path);
            writer.Write((short)assetDescriptor.Value.Type);
        }
    }
}