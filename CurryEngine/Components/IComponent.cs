namespace CurryEngine.Components;

public abstract class Component
{
    public ComponentType ComponentType { get; }
    public Guid Id { get; }

    public Component(Guid id, ComponentType componentType)
    {
        this.Id = id;
        this.ComponentType = componentType;
    }
}