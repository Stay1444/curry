using System.Text.Json.Serialization;

namespace CurryEngine;

public class GameObject : IGameObjectParent
{
    public string Name { get; set; }
    public bool DontDestroyOnLoad { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<GameObject> Children { get; set; } = new List<GameObject>();
    public bool Enabled { get; set; } = true;
}