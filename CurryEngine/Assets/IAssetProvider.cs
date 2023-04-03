namespace CurryEngine.Assets;

public interface IAssetProvider
{
    public T? Get<T>(Guid id) where T : Asset;
}