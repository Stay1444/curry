using System.Numerics;
using ImGuiNET;

namespace CurryEngine.Editor.Rendering.ImGUI;

public static class ImImageRotated
{
    private static Vector2 ImRotate(Vector2 v, float cos_a, float sin_a) 
    { 
        return new Vector2(v.X * cos_a - v.Y * sin_a, v.X * sin_a + v.Y * cos_a);
    }
    
    public static void ImageRotated(nint tex_id, Vector2 center, Vector2 size, float angle)
    {
        var draw_list = ImGui.GetWindowDrawList();

        var cos_a = (float) Math.Cos(angle);
        var sin_a = (float) Math.Cos(angle);

        var pos = new[]
        {
            center + ImRotate(new(-size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
            center + ImRotate(new(+size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
            center + ImRotate(new(+size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a),
            center + ImRotate(new(-size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a)
        };
        
        var uvs = new Vector2[] 
        { 
            new(0.0f, 0.0f), 
            new(1.0f, 0.0f), 
            new(1.0f, 1.0f), 
            new(0.0f, 1.0f) 
        };

        draw_list.AddImageQuad(tex_id, pos[0], pos[1], pos[2], pos[3], uvs[0], uvs[1], uvs[2], uvs[3], 0xFFFFFFFF);
    }
    
    public static bool ImageButtonRotated(string id, nint tex_id, Vector2 center, Vector2 size, float angle)
    {
        var draw_list = ImGui.GetWindowDrawList();

        var cos_a = (float) Math.Cos(angle);
        var sin_a = (float) Math.Cos(angle);

        var pos = new[]
        {
            center + ImRotate(new(-size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
            center + ImRotate(new(+size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
            center + ImRotate(new(+size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a),
            center + ImRotate(new(-size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a)
        };
        
        var uvs = new Vector2[] 
        { 
            new(0.0f, 0.0f), 
            new(1.0f, 0.0f), 
            new(1.0f, 1.0f), 
            new(0.0f, 1.0f) 
        };
        var r = ImGui.InvisibleButton(id, size);
        draw_list.AddImageQuad(tex_id, pos[0], pos[1], pos[2], pos[3], uvs[0], uvs[1], uvs[2], uvs[3], 0xFFFFFFFF);
        return r;
    }
}