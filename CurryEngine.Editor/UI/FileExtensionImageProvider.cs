using System.Drawing.Imaging;
using System.Reflection;
using CurryEngine.Editor.Rendering.ImGUI;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Svg;

namespace CurryEngine.Editor.UI;

public class FileExtensionImageProvider
{
    private static Dictionary<string, nint>? _map = new Dictionary<string, nint>();

    public static void Register(ImGuiRenderer renderer)
    {
        _map = new Dictionary<string, nint>();

        foreach (var fileResourceName in Assembly.GetCallingAssembly().GetManifestResourceNames())
        {
            if (!fileResourceName.StartsWith("CurryEngine.Editor.Resources.Icons.fs.ext"))
            {
                continue;
            }

            var key = fileResourceName.Split(".")[fileResourceName.Split(".").Length - 2];
            _map.Add(key, Load(fileResourceName, renderer));
        }
    }

    public static nint GetFileIcon(string extension)
    {
        return _map != null && _map.TryGetValue(extension, out nint value) ? value : _map["unknown"];
    }

    private static nint Load(string path, ImGuiRenderer renderer)
    {
        var stream = Assembly.GetAssembly(typeof(Icon))!.GetManifestResourceStream(path);
        Texture2D texture;
        if (Path.GetExtension(path).Contains("svg"))
        {
            Log.Debug("Rasterizing {path}", path);
            var svgDoc = SvgDocument.Open<SvgDocument>(stream);
            var bitmap = svgDoc.Draw(128, 128);
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            texture = Texture2D.FromStream(renderer.GraphicsDevice, ms, 128, 128, false);
        }
        else
        {
            texture = Texture2D.FromStream(renderer.GraphicsDevice, stream, 128, 128, false);
        }
        var id = renderer.BindTexture(texture);
        Log.Debug("Bound mimetype icon {path} to ImGui ({id})", path, id);
        return id;
    }
}