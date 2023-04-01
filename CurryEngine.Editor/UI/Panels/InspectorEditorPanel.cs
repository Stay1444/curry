using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class InspectorEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;

    public InspectorEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        ImGui.Begin("Inspector");
        ImGui.Text("Inspector");
        ImGui.End();
    }
}