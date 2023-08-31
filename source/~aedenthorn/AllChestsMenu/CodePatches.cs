/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Objects;

namespace AllChestsMenu
{
    public partial class ModEntry
    {

        [HarmonyPatch(typeof(Chest), nameof(Chest.GetActualCapacity))]
        public class Chest_GetActualCapacity_Patch
        {
            public static void Postfix(Chest __instance, ref int __result)
            {
                __result = 12 * Config.ChestRows;
            }
        }

        [HarmonyPatch(typeof(ShipObjective), nameof(ShipObjective.OnItemShipped))]
        public class ShipObjective_OnItemShipped_Patch
        {
            public static bool Prefix(Item item)
            {
                return item is not null;
            }
        }
    }
}