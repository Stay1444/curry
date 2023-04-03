using Microsoft.Xna.Framework.Graphics;

namespace CurryEngine.Assets.Types;

public class Sprite : Asset
{
    public Texture2D Frame { get; }
    
    public Sprite(Guid id, Texture2D frame) : base(id)
    {
        Frame = frame;
    }
}