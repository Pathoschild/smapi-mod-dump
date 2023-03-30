/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick))]
    internal class InventoryPageReceiveLeftClickPatch
    {
        public static bool Prefix(out Hat? __state)
        {
            __state = Game1.player.hat.Value;
            return true;
        }

        public static void Postfix(Hat? __state)
        {
            if (!ModEntry.ShouldPatch())
                return;

            if (__state != Game1.player.hat.Value && Game1.player.hat.Value is not null)
                ModEntry.Instance.TaskManager?.OnHatEquipped(Game1.player.hat.Value);
        }
    }
}
