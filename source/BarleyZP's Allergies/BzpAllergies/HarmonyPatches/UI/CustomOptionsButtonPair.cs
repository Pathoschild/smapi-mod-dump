/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewModdingAPI;

namespace BZP_Allergies.HarmonyPatches.UI
{
    internal class CustomOptionsButtonPair : OptionsElement
    {
        private OptionsButton btn1;
        private OptionsButton btn2;
        public CustomOptionsButtonPair(string label1, string label2, Action action1, Action action2) : base("")
        {
            btn1 = new(label1, action1);
            btn2 = new(label2, action2);

            int btn2Offset = btn1.bounds.Width + 32;
            btn2.bounds = new(btn2.bounds.X + btn2Offset, btn2.bounds.Y, btn2.bounds.Width, btn2.bounds.Height);

            int newWidth = btn2Offset + btn2.bounds.Width;
            this.bounds = new(bounds.X, bounds.Y, newWidth, bounds.Height);
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu? context = null)
        {
            btn1.draw(b, slotX, slotY, context);
            btn2.draw(b, slotX, slotY, context);
        }

        public override void receiveLeftClick(int x, int y)
        {
            btn1.receiveLeftClick(x, y);
            btn2.receiveLeftClick(x, y);
        }
    }
}
