using System.Drawing.Imaging;
using System.Reflection;
using CurryEngine.Editor.Rendering.ImGUI;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Svg;

namespace CurryEngine.Editor.UI;

public class Icon
{
    public string Name { get; }
    public string AssemblyPath { get; }

    public int W { get; }
    public int H { get; }
    
    private nint _imGuiId;
    private bool _imGuiInitialized = false;

    internal Icon(string path, int w, int h)
    {
        this.W = w;
        this.H = h;
        Name = Path.GetFileName(path);
        AssemblyPath = path;
    }

    public nint GetImGuiId(ImGuiRenderer renderer)
    {
        if (_imGuiInitialized) return _imGuiId;
        Initialize(renderer);
        _imGuiInitialized = true;
        return _imGuiId;
    }

    private void Initialize(ImGuiRenderer renderer)
    {
        var stream = Assembly.GetAssembly(typeof(Icon))!.GetManifestResourceStream(AssemblyPath);
        Texture2D texture;
        if (Path.GetExtension(AssemblyPath).Contains("svg"))
        {
            Log.Debug("Rasterizing {path}", AssemblyPath);

            var svgDoc = SvgDocument.Open<SvgDocument>(stream);
            var bitmap = svgDoc.Draw(W, H);
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            texture = Texture2D.FromStream(renderer.GraphicsDevice, ms, W, H, false);
        }
        else
        {
            texture = Texture2D.FromStream(renderer.GraphicsDevice, stream, W, H, false);
        }
        _imGuiId = renderer.BindTexture(texture);
        Log.Debug("Bound icon {path} to ImGui ({id})", AssemblyPath, _imGuiId);
    }
}