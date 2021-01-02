/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class ColorBox : OptionsElement
    {
        //Color of the inner box.
        private Color _color;

        public ColorBox(string name, int which, Color color, int width = 128, int height = 128)
            : base(name, -1, -1, width, height, which)
        {
            _color = color;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            slotX += 32;

            Util.DrawWindow(slotX, slotY, bounds.Width, bounds.Height); //Draw outer frame.
            Util.DrawColoredBox(
                b,
                slotX + 12, slotY + 12,
                bounds.Width - 24, bounds.Height - 24,
                _color); //Draw inner box.
        }

        /// <summary>
        /// Sets color of the inner box.
        /// </summary>
        /// <param name="c">The color you want to change.</param>
        public void SetColor(Color c)
        {
            SetColor(c.R, c.G, c.B);
        }

        /// <summary>
        /// Sets color of the inner box.
        /// </summary>
        /// <param name="r">The red value you want to change.</param>
        /// <param name="g">The green value you want to change.</param>
        /// <param name="b">The blue value you want to change.</param>
        public void SetColor(int r, int g, int b)
        {
            _color.R = (byte) r;
            _color.G = (byte) g;
            _color.B = (byte) b;
        }
    }
}
