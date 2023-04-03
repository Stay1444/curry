namespace CurryEngine.Assets;

public abstract class Asset
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public Asset(Guid id)
    {
        this.Id = id;
    }
}