/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley;

namespace PostBoxMod
{
    class PackingMenu : ItemGrabMenu
    {
        public Postbox originPostbox;

        public static ClickableTextureComponent lastDeliveryHolder;
        /*
         * Currently unused -- may continue to implement later to have a menu for posting gifts.
         */
        public PackingMenu(Postbox pb) :
            base (null, true, true, null, null, null, null, false, false, true, true,false, 0, null, -1, pb)
        {
            originPostbox = pb;
            base.behaviorOnItemGrab = onGrab;
            lastDeliveryHolder = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height / 2 - 80 - 64, 96, 96), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem"), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f)
            {
                myID = 12598,
                region = 12598
            };
        }

        public void onGrab(Item item, Farmer who)
        {
            Game1.showGlobalMessage("This is Grab");
        }
    }
}
