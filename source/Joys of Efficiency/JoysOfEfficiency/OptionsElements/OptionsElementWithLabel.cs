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
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    public abstract class OptionsElementWithLabel : OptionsElement
    {
        protected OptionsElementWithLabel(string label, int x, int y, int width, int height, int whichOption = -1)
            : base(label, x, y, width, height, whichOption) { }

        private int GetOffsetLabel()
        {
            return Util.IsAndroid() ? bounds.Width + 8 : 0;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            base.draw(b, slotX + GetOffsetLabel(), slotY, context);
        }
    }
}
