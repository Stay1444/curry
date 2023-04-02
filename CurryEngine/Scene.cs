namespace CurryEngine;

public class Scene : IGameObjectParent
{
    public string Name { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<GameObject> Children { get; }= new List<GameObject>();
}