namespace CurryEngine;

public interface IGameObjectParent
{
    public string Name { get; set; }
    public  Guid Id { get; set; }
    public List<GameObject> Children { get; }
}