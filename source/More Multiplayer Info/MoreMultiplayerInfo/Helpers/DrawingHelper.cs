using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoreMultiplayerInfo
{
    public class DrawingHelper
    {
        public static void DrawBorder(SpriteBatch b, Rectangle dims, int borderWidth, Color color)
        {
            b.Draw(TextureHelper.WhitePixel, new Rectangle(dims.Left, dims.Top, borderWidth, dims.Height), color); /* Left */
            b.Draw(TextureHelper.WhitePixel, new Rectangle(dims.Left, dims.Top, dims.Width, borderWidth), color); /* Top */
            b.Draw(TextureHelper.WhitePixel, new Rectangle(dims.Right, dims.Top, borderWidth, dims.Height), color); /* Right  */
            b.Draw(TextureHelper.WhitePixel, new Rectangle(dims.Left, dims.Bottom, dims.Width, borderWidth), color); /* Bottom */
        }
    }
}