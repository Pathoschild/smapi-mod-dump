/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System.Linq;
using Circuit.Events;
using HarmonyLib;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.getOne))]
    internal class SObjectGetOnePatch
    {
        public static bool Prefix(SObject __instance)
        {
            if (!ModEntry.ShouldPatch(EventType.BountifulHarvest))
                return true;

            if (BountifulHarvest.HarvestIndices.Contains(__instance.ParentSheetIndex))
            {
                __instance.Quality = __instance.Quality switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 4,
                    4 => 4,
                    _ => 0
                };
            }

            return true;
        }
    }
}
