using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CurryEngine.Assets.Types;

public class AnimatedSprite : Asset
{
    public Texture2D SpriteSheet { get; }
    public int FrameCount { get; }
    public int FrameWidth { get; }
    public int FrameHeight { get; }
    public int Columns { get; }
    public int Rows { get; }
    
    public AnimatedSprite(Guid id, Texture2D frame, int frameCount, int fWidth, int fHeight, int cols, int rows) : base(id)
    {
        this.SpriteSheet = frame;
        this.FrameCount = frameCount;
        this.FrameWidth = fWidth;
        this.FrameHeight = fHeight;
        this.Columns = cols;
        this.Rows = rows;
    }
    
    public Rectangle CalculateRenderRectangle(int frame)
    {
        var x = (frame % Columns) * FrameWidth;
        var y = (frame / Columns) * FrameHeight;
    
        return new Rectangle(x, y, FrameWidth, FrameHeight);
    }
}