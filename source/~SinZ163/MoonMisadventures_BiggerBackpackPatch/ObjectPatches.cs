/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonMisadventures_BiggerBackpackPatch
{
    internal class ObjectPatches
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void InventoryPage_Constructor__Postfix(InventoryPage __instance)
        {
            var equipment = __instance.equipmentIcons.Find(item => item.myID == 123450101);
            if (equipment == null)
            {
                Monitor.Log("Unable to find the necklace slot for Moon Misadentures", LogLevel.Warn);
            }
            equipment.bounds.Y =
                (__instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 256 - 12) // original
                + (64); // Extra
        }

        internal static bool DrawHoverPatch_Prefix(InventoryPage __0)
        {
            var equipment = __0.equipmentIcons.Find(item => item.myID == 123450101);
            if (equipment == null)
            {
                return false;
            }
            return true;
        }
    }
}
