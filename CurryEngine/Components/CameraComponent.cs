using System.ComponentModel;
using System.Numerics;

namespace CurryEngine.Components;

public class CameraComponent : Component
{
    public bool IsPrimary;
    public Vector2 Scale;

    public CameraComponent(Guid id) : base(id, ComponentType.Camera)
    {
        
    }
    
    public static CameraComponent Deserialize(BinaryReader reader)
    {
        return new CameraComponent(new Guid(reader.ReadBytes(16)))
        {
            IsPrimary = reader.ReadBoolean(),
            Scale = new Vector2(reader.ReadSingle(), reader.ReadSingle())
        };
    }

    public static void Serialize(CameraComponent component, BinaryWriter writer)
    {
        writer.Write(component.Id.ToByteArray());
        writer.Write(component.IsPrimary);
        writer.Write(component.Scale.X);
        writer.Write(component.Scale.Y);
    }
}