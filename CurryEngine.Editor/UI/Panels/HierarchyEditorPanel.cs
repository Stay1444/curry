using System.Numerics;
using System.Text;
using CurryEngine.Components;
using IconFonts;
using ImGuiNET;

namespace CurryEngine.Editor.UI.Panels;

public class HierarchyEditorPanel : EditorPanel
{
    private readonly EditorRenderer _renderer;
    private KeyValuePair<IGameObjectParent, IGameObjectParent>? _dragItem;

    public HierarchyEditorPanel(EditorRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        ImGui.Begin($"{FontAwesome4.List} Hierarchy###hierarchy-panel", _renderer.Editor.SceneHasUnsavedChanges ? ImGuiWindowFlags.UnsavedDocument : ImGuiWindowFlags.None);

        if (_renderer.Editor.Game?.ActiveScene is not null)
        {
            if (ImGui.BeginPopupContextWindow("###hierarchy-menu"))
            {
                if (ImGui.MenuItem("New"))
                {
                    _renderer.Editor.Game.ActiveScene.Children.Add(new GameObject
                    {
                        Name = "New GameObject",
                        Id = Guid.NewGuid(),
                        Components = new List<Component>()
                        {
                            new TransformComponent(Guid.NewGuid())
                        }
                    });
                    _renderer.Editor.SceneHasUnsavedChanges = true;
                }

                ImGui.EndPopup();
            }

            if (ImGui.CollapsingHeader(FontAwesome4.Film + " " + _renderer.Editor.Game.ActiveScene.Name + (_renderer.Editor.SceneHasUnsavedChanges ? " *" : "" + $"###{_renderer.Editor.Game.ActiveScene.Id}"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                for (var index = 0; index < _renderer.Editor.Game.ActiveScene.Children.Count; index++)
                {
                    var child = _renderer.Editor.Game.ActiveScene.Children[index];
                    RenderGO(child, _renderer.Editor.Game.ActiveScene);
                }    
            }
        }

        ImGui.End();
    }

    private void RenderGO(IGameObjectParent item, IGameObjectParent parent, bool disabled = false)
    {
        var flags = item.Children.Count > 0 ? ImGuiTreeNodeFlags.OpenOnArrow : ImGuiTreeNodeFlags.Leaf;

        flags |= ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanAvailWidth |
                 ImGuiTreeNodeFlags.OpenOnDoubleClick;
            
        if (_renderer.Editor.SelectedEntity == item) flags |= ImGuiTreeNodeFlags.Selected;

        void Interact()
        {
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                _renderer.Editor.SelectedEntity = item as GameObject;

            if (ImGui.BeginDragDropSource())
            {
                ImGui.Text(item.Name);

                var text = "hello";
                var bytes = Encoding.Unicode.GetBytes(text).AsSpan();
                ImGui.SetDragDropPayload("test", (nint)bytes[0], (uint)bytes.Length);

                _dragItem = new KeyValuePair<IGameObjectParent, IGameObjectParent>(parent, item);
                ImGui.EndDragDropSource();
            }

            if (ImGui.BeginDragDropTarget())
                unsafe
                {
                    var payload = ImGui.AcceptDragDropPayload("GameObject");

                    if (payload.NativePtr != null)
                    {
                        if (_dragItem.HasValue)
                        {
                            var parent = _dragItem.Value.Key;
                            var target = _dragItem.Value.Value;

                            bool IsChild(IGameObjectParent item, IGameObjectParent target)
                            {
                                if (item.Children.Contains(target)) return true;
                                foreach (var targetChild in target.Children)
                                {
                                    if (IsChild(item, targetChild)) return true;
                                }

                                return false;
                            }

                            if (IsChild(target, parent) || item == target || item == parent || target.Children.Contains(item))
                            {
                                
                            }
                            else
                            {
                                _dragItem.Value.Key.Children.Remove((_dragItem.Value.Value as GameObject)!);
                                item.Children.Add(_dragItem.Value.Value as GameObject ??
                                                  throw new InvalidOperationException());
                                _renderer.Editor.SceneHasUnsavedChanges = true;
                            }
                            
                            _dragItem = null;
                        }
                    }

                    ImGui.EndDragDropTarget();
                }

            if (ImGui.BeginPopupContextItem($"###{item.Id}"))
            {
                if (ImGui.MenuItem("New Child"))
                {
                    item.Children.Add(new GameObject()
                    {
                        Id = Guid.NewGuid(),
                        Name = item.Name + " Child"
                    });
                    _renderer.Editor.SceneHasUnsavedChanges = true;
                }
                
                ImGui.Separator();
                
                if (ImGui.MenuItem("Delete"))
                {
                    parent.Children.Remove((item as GameObject)!);
                    _renderer.Editor.SceneHasUnsavedChanges = true;

                    void RemoveChildren(IGameObjectParent p)
                    {
                        if (_renderer.Editor.SelectedEntity == p) _renderer.Editor.SelectedEntity = null;
                        foreach (var pc in p.Children)
                        {
                            RemoveChildren(pc);
                        }
                    }
                    RemoveChildren(item);
                }
                
                ImGui.EndPopup();
            }
        }

        if (item is GameObject {Enabled: false} || disabled) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 0.5f));

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        if (ImGui.TreeNodeEx($"{FontAwesome4.Cube} " + item.Name + "###" + item.Id, flags))
        {
            if (item is GameObject {Enabled: false} || disabled) ImGui.PopStyleColor();

            Interact();

            for (var index = 0; index < item.Children.Count; index++)
            {
                var child = item.Children[index];
                RenderGO(child, item, disabled || item is GameObject {Enabled: false});
            }

            ImGui.TreePop();
            ImGui.PopStyleVar();
        }
        else
        {
            ImGui.PopStyleVar();
            Interact();
            if (item is GameObject {Enabled: false} || disabled) ImGui.PopStyleColor();
        }

    }
}