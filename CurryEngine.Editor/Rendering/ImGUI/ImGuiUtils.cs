using System.Numerics;
using ImGuiNET;

namespace CurryEngine.Editor.Rendering.ImGUI;

public class ImGuiUtils
{
    public static void SetupImGuiStyle()
    {
        var style = ImGui.GetStyle();

        style.Alpha = 1f;
        style.GrabRounding = style.FrameRounding = 2.3f;
        
        style.Colors[(int)ImGuiCol.Text]                  = new (1.00f, 1.00f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.TextDisabled]          = new (0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int)ImGuiCol.WindowBg]              = new (0.13f, 0.14f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.ChildBg]               = new (0.13f, 0.14f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.PopupBg]               = new (0.13f, 0.14f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.Border]                = new (0.43f, 0.43f, 0.50f, 0.50f);
        style.Colors[(int)ImGuiCol.BorderShadow]          = new (0.00f, 0.00f, 0.00f, 0.00f);
        style.Colors[(int)ImGuiCol.FrameBg]               = new (0.25f, 0.25f, 0.25f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBgHovered]        = new (0.38f, 0.38f, 0.38f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBgActive]         = new (0.67f, 0.67f, 0.67f, 0.39f);
        style.Colors[(int)ImGuiCol.TitleBg]               = new (0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgActive]         = new (0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed]      = new (0.00f, 0.00f, 0.00f, 0.51f);
        style.Colors[(int)ImGuiCol.MenuBarBg]             = new (0.14f, 0.14f, 0.14f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarBg]           = new (0.02f, 0.02f, 0.02f, 0.53f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab]         = new (0.31f, 0.31f, 0.31f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]  = new (0.41f, 0.41f, 0.41f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive]   = new (0.51f, 0.51f, 0.51f, 1.00f);
        style.Colors[(int)ImGuiCol.CheckMark]             = new (0.11f, 0.64f, 0.92f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrab]            = new (0.11f, 0.64f, 0.92f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrabActive]      = new (0.08f, 0.50f, 0.72f, 1.00f);
        style.Colors[(int)ImGuiCol.Button]                = new (0.25f, 0.25f, 0.25f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonHovered]         = new (0.38f, 0.38f, 0.38f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonActive]          = new (0.67f, 0.67f, 0.67f, 0.39f);
        style.Colors[(int)ImGuiCol.Header]                = new (0.22f, 0.22f, 0.22f, 1.00f);
        style.Colors[(int)ImGuiCol.HeaderHovered]         = new (0.25f, 0.25f, 0.25f, 1.00f);
        style.Colors[(int)ImGuiCol.HeaderActive]          = new (0.67f, 0.67f, 0.67f, 0.39f);
        style.Colors[(int)ImGuiCol.Separator]             = style.Colors[(int)ImGuiCol.Border];
        style.Colors[(int)ImGuiCol.SeparatorHovered]      = new (0.41f, 0.42f, 0.44f, 1.00f);
        style.Colors[(int)ImGuiCol.SeparatorActive]       = new (0.26f, 0.59f, 0.98f, 0.95f);
        style.Colors[(int)ImGuiCol.ResizeGrip]            = new (0.00f, 0.00f, 0.00f, 0.00f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered]     = new (0.29f, 0.30f, 0.31f, 0.67f);
        style.Colors[(int)ImGuiCol.ResizeGripActive]      = new (0.26f, 0.59f, 0.98f, 0.95f);
        style.Colors[(int)ImGuiCol.Tab]                   = new (0.08f, 0.08f, 0.09f, 0.83f);
        style.Colors[(int)ImGuiCol.TabHovered]            = new (0.33f, 0.34f, 0.36f, 0.83f);
        style.Colors[(int)ImGuiCol.TabActive]             = new (0.23f, 0.23f, 0.24f, 1.00f);
        style.Colors[(int)ImGuiCol.TabUnfocused]          = new (0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TabUnfocusedActive]    = new (0.13f, 0.14f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.DockingPreview]        = new (0.26f, 0.59f, 0.98f, 0.70f);
        style.Colors[(int)ImGuiCol.DockingEmptyBg]        = new (0.20f, 0.20f, 0.20f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLines]             = new (0.61f, 0.61f, 0.61f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered]      = new (1.00f, 0.43f, 0.35f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogram]         = new (0.90f, 0.70f, 0.00f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered]  = new (1.00f, 0.60f, 0.00f, 1.00f);
        style.Colors[(int)ImGuiCol.TextSelectedBg]        = new (0.26f, 0.59f, 0.98f, 0.35f);
        style.Colors[(int)ImGuiCol.DragDropTarget]        = new (0.11f, 0.64f, 0.92f, 1.00f);
        style.Colors[(int)ImGuiCol.NavHighlight]          = new (0.26f, 0.59f, 0.98f, 1.00f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new (1.00f, 1.00f, 1.00f, 0.70f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg]     = new (0.80f, 0.80f, 0.80f, 0.20f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg]      = new (0.80f, 0.80f, 0.80f, 0.0f);
    }
}