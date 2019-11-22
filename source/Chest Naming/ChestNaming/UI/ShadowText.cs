using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ChestNaming.UI
{
    /// <summary>
    /// Text with a dropshadow
    /// </summary>
    public class ShadowText : IFrameDrawable
    {
        public ShadowText(string text, SpriteFont font, Color color)
        {
            this.text = text;
            this.font = font;
            this.color = color;
        }
        public SpriteFont font;
        public Color color;
        public string text;
        public int SizeX
        {
            get
            {
                if (font != null && text != "")
                {
                    return (int)(font.MeasureString(text).X);
                }
                return 0;
            }
        }

        public int SizeY
        {
            get
            {
                if (font != null && text != "")
                {
                    return (int)(font.MeasureString(text).Y);
                }
                return 0;
            }
        }

        public void Draw(SpriteBatch b, int x, int y, Frame parentFrame)
        {
            Utility.drawTextWithShadow(b, text, font, new Vector2(x, y), this.color);
        }
    }
}
