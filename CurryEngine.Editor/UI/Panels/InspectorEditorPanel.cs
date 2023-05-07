using System.Numerics;
using System.Text;
using CurryEngine.Components;
using IconFonts;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Serilog.Sinks.SystemConsole.Themes;

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
                    _renderer.Editor.SceneHasUnsavedChanges = true;
                }

                ImGui.SameLine();
                
                var width = ImGui.GetContentRegionAvail().X;
                ImGui.SetNextItemWidth(width);
                
                if (ImGui.InputText("###entity_name", ref name, 128))
                {
                    _renderer.Editor.SceneHasUnsavedChanges = true;

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = "_";
                    }
                    _renderer.Editor.SelectedEntity.Name = name;
                }
                
                ImGui.EndGroup();
                
                ImGui.Separator();

                if (ImGui.BeginPopupContextWindow("###inspector-cmenu"))
                {
                    if (ImGui.BeginMenu("Add Component"))
                    {
                        if (ImGui.MenuItem("Transform",
                                !_renderer.Editor.SelectedEntity.HasComponent<TransformComponent>()))
                        {
                            _renderer.Editor.SelectedEntity.Components.Add(new TransformComponent(Guid.NewGuid()));
                            _renderer.Editor.SceneHasUnsavedChanges = true;
                        }
                        
                        if (ImGui.MenuItem("Camera", !_renderer.Editor.SelectedEntity.HasComponent<CameraComponent>()))
                        {
                            _renderer.Editor.SelectedEntity.Components.Add(new CameraComponent(Guid.NewGuid()));
                            _renderer.Editor.SceneHasUnsavedChanges = true;
                        }

                        if (ImGui.MenuItem("Sprite"))
                        {
                            _renderer.Editor.SelectedEntity.Components.Add(new SpriteComponent(Guid.NewGuid()));
                            _renderer.Editor.SceneHasUnsavedChanges = true;
                        }
                        
                        ImGui.EndMenu();
                    }
                    ImGui.EndPopup();
                }

                if (_renderer.Editor.Project is not null && _renderer.Editor.Game is not null)
                {
                    for (var index = 0; index < _renderer.Editor.SelectedEntity.Components.Count; index++)
                    {
                        var component = _renderer.Editor.SelectedEntity.Components[index];
                        RenderComponent(_renderer.Editor.SelectedEntity, component);
                        ImGui.Separator();
                    }
                }
            }
        }
        ImGui.End();
    }

    private void BeginRenderComponent(Icon icon, string name)
    {
        ImGui.BeginGroup();
        ImGui.Image(icon.GetImGuiId(_renderer.ImGuiRenderer), new Vector2(64, 64));
        ImGui.SameLine();

        ImGui.PushFont(_renderer.Font18);
        
        var pos = ImGui.GetCursorPos();
        var textSize = ImGui.CalcTextSize(name);
        ImGui.SetCursorPos(new Vector2(pos.X + 15, pos.Y + (64 / 2 - textSize.Y / 2)));
        
        ImGui.Text(name);
        
        ImGui.PopFont();
    }

    private void EndRenderComponent()
    {
        ImGui.EndGroup();
    }

    private void RenderComponent(GameObject gameObject, Component component)
    {
        if (component is TransformComponent transformComponent)
        {
            BeginRenderComponent(IconManager.I_Gizmo_Icon, "Transform");
            
            ImGui.Text("Position");
            ImGui.SameLine();

            var speed = 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                speed = 0.005f;
            }

            if (ImGui.DragFloat3("", ref transformComponent.Position, speed))
            {
                _renderer.Editor.SceneHasUnsavedChanges = true;
            }
            
            EndRenderComponent();
        }else if (component is CameraComponent cameraComponent)
        {
            BeginRenderComponent(IconManager.I_Camera_Icon, "Camera");
            ImGui.Text("Primary");
            ImGui.SameLine();
            if (ImGui.Checkbox("", ref cameraComponent.IsPrimary))
            {
                _renderer.Editor.SceneHasUnsavedChanges = true;
            }

            ImGui.Text("Scale");
            ImGui.SameLine();
            if (ImGui.DragFloat2("", ref cameraComponent.Scale, 0.0001f))
            {
                _renderer.Editor.SceneHasUnsavedChanges = true;
            }
            
            EndRenderComponent();
        }else if (component is SpriteComponent spriteComponent)
        {
            BeginRenderComponent(IconManager.I_Image_Icon, "Sprite");

            if (_renderer.Editor.Project!.AssetDescriptors.ContainsKey(spriteComponent.AssetId))
            {
                var descriptor = _renderer.Editor.Project!.AssetDescriptors[spriteComponent.AssetId];
                
                ImGui.Text(Path.GetFileName(descriptor.Path));
                
                ImGui.Image(DynamicIconManager.Instance.GetOrLoad(descriptor.Path), new Vector2(64, 64));
            }
            else
            {
                ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX() + 16, ImGui.GetCursorPosY() + 32));
                ImGui.Text("< Sprite > ");

                ImGui.SameLine();

                var imageSize = new Vector2(64, 64);
                var spaceAvail = ImGui.GetContentRegionAvail();
                var cursorPosition = ImGui.GetCursorPos();
                var targetPos = new Vector2(spaceAvail.X, cursorPosition.Y - imageSize.Y / 2 + 16); // Why are we dividing by 4?
            
                ImGui.SetCursorPos(targetPos);
                ImGui.Image(IconManager.I_FailedImage_Icon.GetImGuiId(_renderer.ImGuiRenderer), new Vector2(64,64));

                if (ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload("test");
                    unsafe
                    {
                        if (payload.NativePtr != null)
                        {
                            var textBegin = (byte*) payload.NativePtr->Data;
                            var length = payload.NativePtr->DataSize;

                            var sb = new StringBuilder();
                            for (var i = 0; i < length; i += 2) // iterate over each UTF-16 character (which takes two bytes)
                            {
                                var character = Encoding.Unicode.GetString(textBegin + i, 2)[0];
                                sb.Append(character);
                            }
                            Console.WriteLine(sb.ToString());
                        }
                        ImGui.EndDragDropTarget();
                    }
                }
            }
            
            EndRenderComponent();
        }
        else
        {
            ImGui.Text($"Component {component} rendering is not implemented.");
        }

        if (ImGui.BeginPopupContextItem(component.Id.ToString()))
        {
            if (ImGui.MenuItem("Remove"))
            {
                gameObject.Components.Remove(component);
                _renderer.Editor.SceneHasUnsavedChanges = true;
            }
            
            ImGui.EndPopup();
        }
    }
}