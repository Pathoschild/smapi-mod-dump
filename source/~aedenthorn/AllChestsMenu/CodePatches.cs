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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Linq;

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
    }
}