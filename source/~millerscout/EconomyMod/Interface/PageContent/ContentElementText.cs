/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace EconomyMod.Interface.PageContent
{
    public class ContentElementText : IContentElement
    {
        private Func<string> dynamicLabel;
        private Rectangle bounds;
        private Vector2 labelOffset;
        private string label;

        public ContentElementText(string label)
        {
            this.label = label;
        }
        public ContentElementText(Func<string> dynamicLabel) : this("ignored")
        {
            this.dynamicLabel = dynamicLabel;
            this.bounds = new Rectangle(32, 16, 36, 36);
            this.labelOffset = new Vector2(10, 0);
        }



        public void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu clickableMenu = null)
        {
            label = dynamicLabel == null ? label : dynamicLabel();
            Utility.drawTextWithShadow(b, label, Game1.dialogueFont, new Vector2(slotX + bounds.X + (int)labelOffset.X, slotY + bounds.Y + (int)labelOffset.Y + 12), Game1.textColor);
        }
    }
}
