using System.Numerics;
using IconFonts;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace CurryEngine.Editor.UI.Panels;

public class GameEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private Vector2 _previousSize;
    
    private bool _textureRegistered = false;
    private nint _textureId;
    
    public GameEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }
    
    public override void Render()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        if (ImGui.Begin($"{FontAwesome4.Gamepad} Game###game") && _renderer.Editor.Game is not null && _renderer.Editor.Game.ActiveScene is not null)
        {
            var size = ImGui.GetContentRegionAvail();
            
            if (size != _previousSize)
            {
                // Viewport size changed
                _previousSize = ImGui.GetContentRegionAvail();

                if (_textureRegistered)
                {
                    _renderer.ImGuiRenderer.UnbindTexture(_textureId);
                    Log.Debug("Game Texture {id} unbound", _textureId);
                }

                if (size is {X: > 1, Y: > 1})
                {
                    _renderer.Editor.GameOutputTexture = new RenderTarget2D(_renderer.Editor.GraphicsDevice, (int) size.X, (int) size.Y);
                    _textureId = _renderer.ImGuiRenderer.BindTexture(_renderer.Editor.GameOutputTexture);
                    _textureRegistered = true;
                }

            } else if (_textureRegistered)
            {
                ImGui.Image(_textureId, size);
            }
        }
        else
        {
            _previousSize = Vector2.Zero;
            if (_textureRegistered)
            {
                _textureRegistered = false;
                _renderer.ImGuiRenderer.UnbindTexture(_textureId);
                Log.Debug("Game Texture {id} unbound", _textureId);
            }

            _renderer.Editor.GameOutputTexture = null;
        }
        ImGui.End();
        ImGui.PopStyleVar();
    }
}