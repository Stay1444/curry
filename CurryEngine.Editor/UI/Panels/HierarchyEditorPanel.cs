using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class HierarchyEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;

    public HierarchyEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        ImGui.Begin("Hierarchy");
        ImGui.End();
    }
}