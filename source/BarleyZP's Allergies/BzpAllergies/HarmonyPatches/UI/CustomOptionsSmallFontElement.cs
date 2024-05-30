/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace BZP_Allergies.HarmonyPatches.UI
{
    internal class CustomOptionsSmallFontElement : OptionsElement
    {
        public CustomOptionsSmallFontElement(string label) : base(label)
        {
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            b.DrawString(Game1.dialogueFont, this.label, new Vector2(slotX + this.bounds.X + (int)this.labelOffset.X, slotY + this.bounds.Y + (int)this.labelOffset.Y + 12), Game1.textColor);
        }
    }
}
