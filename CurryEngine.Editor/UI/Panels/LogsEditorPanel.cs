using System.Numerics;
using IconFonts;
using ImGuiNET;
using Serilog.Events;

namespace CurryEngine.Editor.UI.Panels;

public class LogsEditorPanel : EditorPanel
{
    private EditorRenderer _renderer;

    public LogsEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        ImGui.Begin($"{FontAwesome4.FileText} Logs");
        ImGui.Indent(15);
        foreach (var (level, message) in _renderer.Editor.VLogger.Logs)
        {
            ImGui.BeginGroup();
            var color = level switch
            {
                LogEventLevel.Warning => new Vector4(255 / 255, 150.0f / 255, 51.0f / 255, 1),
                LogEventLevel.Error => new Vector4(255 / 255, 66.0f / 255, 51.0f / 255, 1),
                _ => new Vector4(1, 1, 1, 1)
            };
            ImGui.TextColored(color, $"{level}: {message}");
            ImGui.EndGroup();
        }
        
        ImGui.End();
    }
}