using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.Stardew.SCCL.Content {
    public class OffsetTexture2D {

        public Texture2D Texture { get; }
        public Point Offset { get; }

        public OffsetTexture2D(Texture2D texture, int xOff = 0, int yOff = 0) {
            this.Texture = texture;
            this.Offset = new Point(xOff, yOff);
        }
    }
}
