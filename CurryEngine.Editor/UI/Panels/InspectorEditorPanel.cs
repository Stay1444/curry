using System.Numerics;
using IconFonts;
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
        
        if (ImGui.Begin($"{FontAwesome4.Sliders} Inspector###inspector"))
        {
            if (_renderer.Editor.SelectedEntity is not null)
            {
                var name = _renderer.Editor.SelectedEntity.Name;
                var enabled = _renderer.Editor.SelectedEntity.Enabled;
                
                ImGui.BeginGroup();

                if (ImGui.Checkbox("###entity_enabled", ref enabled))
                {
                    _renderer.Editor.SelectedEntity.Enabled = enabled;
                }

                ImGui.SameLine();
                
                var width = ImGui.GetContentRegionAvail().X;
                ImGui.SetNextItemWidth(width);
                
                if (ImGui.InputText("###entity_name", ref name, 128))
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = "_";
                    }
                    _renderer.Editor.SelectedEntity.Name = name;
                }
                
                ImGui.EndGroup();
                
                ImGui.Separator();
            }
        }
        ImGui.End();
    }
}