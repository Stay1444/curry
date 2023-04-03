using System.Numerics;

namespace CurryEngine.Components;

public class TransformComponent : Component
{
    public ComponentType ComponentType => ComponentType.Transform;

    public Vector3 Position;

    public static TransformComponent Deserialize(BinaryReader reader)
    {
        return new TransformComponent(new Guid(reader.ReadBytes(16)))
        {
            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
        };
    }

    public static void Serialize(TransformComponent component, BinaryWriter writer)
    {
        writer.Write(component.Id.ToByteArray());
        writer.Write(component.Position.X);   
        writer.Write(component.Position.Y);   
        writer.Write(component.Position.Z);   
    }

    public TransformComponent(Guid id) : base(id, ComponentType.Transform)
    {
        
    }
}