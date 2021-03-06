/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using StardewValley.Menus;
using System.Linq;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;

namespace HandyHeadphones.Patches
{
    [HarmonyPatch]
    class InventoryPageRightClickPatch
    {
        private static IMonitor monitor = ModEntry.monitor;
        private static IModHelper helper = ModEntry.modHelper;
        private static ModConfig config = ModEntry.config;


        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.Menus.InventoryPage), nameof(StardewValley.Menus.InventoryPage.receiveRightClick));
        }

        internal static bool Prefix(InventoryPage __instance, int x, int y, bool playSound = true)
        {
            ClickableComponent hatComponent = __instance.equipmentIcons.First(s => s.name == "Hat");
            if (hatComponent.containsPoint(x, y))
            {
                Hat wornHat = Game1.player.hat;
                if (wornHat is null)
                {
                    return true;
                }

                if (wornHat.Name == "Headphones" || wornHat.Name == "Earbuds")
                {
                    ModEntry.ShowMusicMenu();
                    return false;
                }
                if (wornHat.Name == "Studio Headphones")
                {
                    ModEntry.ShowSoundMenu();
                    return false;
                }
            }

            return true;
        }
    }
}
