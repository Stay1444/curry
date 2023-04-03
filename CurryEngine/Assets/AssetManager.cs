using System.Text.Json;
using CurryEngine.Assets.Types;
using Microsoft.Xna.Framework.Graphics;

namespace CurryEngine.Assets;
/*
public class AssetManager
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly IAssetDeserializer _assetDeserializer;
    private readonly IAssetSerializer? _assetSerializer;
    private readonly Dictionary<Guid, Asset> _assets = new Dictionary<Guid, Asset>();

    internal AssetManager(GraphicsDevice graphicsDevice, IAssetDeserializer assetDeserializer, IAssetSerializer? serializer)
    {
        _graphicsDevice = graphicsDevice;
        _assetDeserializer = assetDeserializer;
        this._assetSerializer = serializer;
    }
    
    public T? Get<T>(Guid id) where T : Asset
    {
        if (!_assets.ContainsKey(id)) return null;
        return _assets[id] as T;
    }
    
    public bool TryGet<T>(Guid id, out T asset) where T : Asset
    {
        asset = null!;
        
        if (!_assets.ContainsKey(id)) return false;

        if (_assets[id] is not T) return false;

        asset = _assets[id] as T ?? null!;
        return true;
    }
    
    public Guid LoadSprite(string path)
    {
        using var stream = _assetDeserializer.GetStream(path);
        var texture = Texture2D.FromStream(_graphicsDevice, stream);

        var id = Guid.NewGuid();
        
        _assets.Add(id, new Sprite(id, path, texture));
        return id;
    }

    public Guid LoadSpriteSheet(string path, int frameCount, int columns, int rows, int frameWidth, int frameHeight)
    {
        using var stream = _assetDeserializer.GetStream(path);
        var texture = Texture2D.FromStream(_graphicsDevice, stream);
        
        var id = Guid.NewGuid();
        
        _assets.Add(id, new AnimatedSprite(id, path, texture, frameCount, frameWidth, frameHeight, columns, rows));
        return id;
    }

    public Guid LoadScene(string path)
    {
        using var stream = _assetDeserializer.GetStream(path);
        var scene = JsonSerializer.Deserialize<Scene>(stream);
        var id = Guid.NewGuid();
        _assets.Add(id, scene);
        return id;
    }

    public void SerializeScene(Scene scene)
    {
        if (_assetSerializer is null) throw new NotImplementedException();
        
        var ms = new MemoryStream();

        JsonSerializer.Serialize(ms, scene);
        
        _assetSerializer.WriteStream(ms, scene.Location);
    }

    public void SerializeSceneTo(Scene scene)
    {
        if (_assetSerializer is null) throw new NotImplementedException();
        
        var ms = new MemoryStream();

        JsonSerializer.Serialize(ms, scene);
        
        _assetSerializer.WriteStream(ms, scene.Location);
    }

    public void Unload(Guid guid)
    {
        _assets.Remove(guid);
    }
}*/