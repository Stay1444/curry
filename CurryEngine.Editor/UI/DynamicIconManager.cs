using CurryEngine.Editor.Rendering.ImGUI;
using Microsoft.Xna.Framework.Graphics;

namespace CurryEngine.Editor.UI;

public class DynamicIconManager
{
    private static DynamicIconManager? _instance;

    public static void Initialize(ImGuiRenderer renderer)
    {
        if (_instance is not null) return;
        _instance = new DynamicIconManager(renderer);
    }

    private ImGuiRenderer _renderer;
    private Dictionary<string, nint> _registered = new Dictionary<string, nint>();

    public static DynamicIconManager Instance => _instance;
    
    private DynamicIconManager(ImGuiRenderer renderer)
    {
        _renderer = renderer;
    }

    public nint GetOrLoad(string path, int size = 512)
    {
        if (_registered.ContainsKey(path)) return _registered[path];

        using var fs = File.OpenRead(path);
        var texture = Texture2D.FromStream(_renderer.GraphicsDevice, fs, size, size, false);
        var id = _renderer.BindTexture(texture);
        _registered.Add(path, id);
        return id;
    }

    public void Unload(string path)
    {
        if (!_registered.ContainsKey(path)) return;
        
        _renderer.UnbindTexture(_registered[path]);
    }
}