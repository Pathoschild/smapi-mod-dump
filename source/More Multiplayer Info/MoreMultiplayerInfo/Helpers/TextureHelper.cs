using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MoreMultiplayerInfo
{
    public class TextureHelper
    {
        static TextureHelper()
        {
            TextureHelper.WhitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            TextureHelper.WhitePixel.SetData(new[] { Color.White });

        }

        public static Texture2D WhitePixel;
    }
}