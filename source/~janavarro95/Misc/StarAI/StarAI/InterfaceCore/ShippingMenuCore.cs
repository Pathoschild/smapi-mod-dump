/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.InterfaceCore
{
    class ShippingMenuCore
    {
        public static void closeMenu()
        {
            (Game1.activeClickableMenu as StardewValley.Menus.ShippingMenu).okButton.snapMouseCursorToCenter();
            if (Game1.activeClickableMenu is StardewValley.Menus.ShippingMenu)
            {
                ModCore.CoreMonitor.Log("PRESS ALREADY!");
                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.ESCAPE);
            }
        }
    }
}
