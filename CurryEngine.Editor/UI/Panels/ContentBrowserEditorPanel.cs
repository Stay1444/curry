using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class ContentBrowserEditorPanel : EditorPanel
{
    private EditorRenderer _renderer;

    public ContentBrowserEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        ImGui.Begin("Content Browser");

        ImGui.Columns(2);

        RenderTree();
        
        ImGui.NextColumn();

        ImGui.BeginChild("Browse");
        ImGui.Text("Child");
        ImGui.EndChild();
        
        ImGui.End();
    }

    private void RenderTree()
    {
        ImGui.BeginChild("Tree");
        
        ImGui.EndChild();
    }
}