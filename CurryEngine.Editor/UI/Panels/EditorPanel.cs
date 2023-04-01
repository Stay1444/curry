namespace CurryEngine.Editor.UI.Panels;

public abstract class EditorPanel
{
    public abstract void Render();
    public bool ShouldBeRemoved { get; protected set; }
}