using CurryEngine.Assets;

namespace CurryEngine;

public class Scene : IGameObjectParent
{
    public string Name { get; set; }
    public Guid Id { get; }
    public List<GameObject> Children { get; }= new List<GameObject>();

    public Scene(Guid id)
    {
        this.Id = id;
    }
}