namespace CurryEngine.Components;

public class SpriteComponent : Component
{
    public Guid AssetId { get; set; }
    
    
    public SpriteComponent(Guid id) : base(id, ComponentType.Sprite)
    {
        
    }

    public static SpriteComponent Deserialize(BinaryReader reader)
    {
        return new SpriteComponent(new Guid(reader.ReadBytes(16)))
        {
            AssetId = new Guid(reader.ReadBytes(16))
        };
    }

    public static void Serialize(SpriteComponent component, BinaryWriter writer)
    {
        writer.Write(component.Id.ToByteArray());
        writer.Write(component.AssetId.ToByteArray());
    }
}