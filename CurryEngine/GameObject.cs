using CurryEngine.Components;

namespace CurryEngine;

public class GameObject : IGameObjectParent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public bool DontDestroyOnLoad { get; set; }
    public bool Enabled { get; set; } = true;
    public List<GameObject> Children { get; set; } = new List<GameObject>();
    
    /*TODO: This is a very bad idea.
    Components should be stored in array that's ideally in continuous memory, 
    not scattered across different GameObjects. Which also makes querying harder. */
    public List<Component> Components { get; set; } = new List<Component>();

    public bool HasComponent<T>()
    {
        foreach (var component in Components)
        {
            if (component is T) return true;
        }

        return false;
    }
}