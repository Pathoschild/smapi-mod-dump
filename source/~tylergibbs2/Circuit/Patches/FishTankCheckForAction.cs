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
using StardewValley.Objects;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(FishTankFurniture), nameof(FishTankFurniture.checkForAction))]
    internal class FishTankCheckForAction
    {
        public static void Postfix(FishTankFurniture __instance, bool __result)
        {
            if (!ModEntry.ShouldPatch())
                return;

            if (__result)
                ModEntry.Instance.TaskManager?.OnFishTankCheckForAction(__instance);
        }
    }
}
