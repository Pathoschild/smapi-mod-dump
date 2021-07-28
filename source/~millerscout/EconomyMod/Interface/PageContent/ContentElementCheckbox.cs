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
using EconomyMod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Interface.PageContent
{
    public class ContentElementCheckbox : OptionsElement, IContentElement
    {
        public const int pixelsWide = 9;
        public int Slot { get; set; } = -1;

        public bool isChecked;

        public static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);

        public static Rectangle sourceRectChecked = new Rectangle(236, 425, 9, 9);

        public Rectangle SlotBounds { get; set; }

        private Action<bool> updateAction;

        public ContentElementCheckbox(string label, bool currentValue, Action<bool> update, int x = -1, int y = -1)
            : base(label, x, y, 36, 36, 0)
        {
            updateAction = update;
            isChecked = currentValue;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (InterfaceHelper.ClickOnTriggerArea(x, y, SlotBounds))
            {
                if (!greyedOut)
                {
                    Game1.playSound("drumkit6");
                    base.receiveLeftClick(x, y);
                    isChecked = !isChecked;
                    updateAction(isChecked);
                }
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {

            b.Draw(Game1.mouseCursors, new Vector2(slotX + bounds.X + 2, slotY + bounds.Y + 1), isChecked ? sourceRectChecked : sourceRectUnchecked, Color.White * (greyedOut ? 0.33f : 1f), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
            InterfaceHelper.Draw(this.SlotBounds, InterfaceHelper.InterfaceHelperType.Red);
            base.draw(b, slotX, slotY, context);
        }
    }
}
