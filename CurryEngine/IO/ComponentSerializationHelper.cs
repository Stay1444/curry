using CurryEngine.Components;

namespace CurryEngine.IO;

public static class ComponentSerializationHelper
{
    public static Component Deserialize(BinaryReader reader)
    {
        var type = (ComponentType) reader.ReadByte();

        return type switch
        {
            ComponentType.Transform => TransformComponent.Deserialize(reader),
            ComponentType.Camera => CameraComponent.Deserialize(reader),
            ComponentType.Sprite => SpriteComponent.Deserialize(reader),
            _ => throw new NotImplementedException($"Deserialization for {type} not implemented!")
        };
    }
    
    public static void Serialize(this Component component, BinaryWriter writer)
    {
        writer.Write((byte)component.ComponentType);

        switch (component.ComponentType)
        {
            case ComponentType.Transform:
                TransformComponent.Serialize((TransformComponent)component, writer);
                break;
            case ComponentType.Camera:
                CameraComponent.Serialize((CameraComponent)component, writer);
                break;
            case ComponentType.Sprite:
                SpriteComponent.Serialize((SpriteComponent)component, writer);
                break;
            default:
                throw new NotImplementedException($"Serialization for {component.ComponentType} not implemented!");
        }
    }
    
    
}