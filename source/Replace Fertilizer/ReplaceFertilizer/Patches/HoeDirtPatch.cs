/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jingshenSN2/ReplaceFertilizer
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace JingshenSN2.ReplaceFertilizer.Patches
{
    internal class HoeDirtPatch : Patch
    {
        internal HoeDirtPatch(IMonitor monitor) : base(monitor)
        {
        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.CheckApplyFertilizerRules)),
                postfix: new HarmonyMethod(typeof(HoeDirtPatch), nameof(CheckApplyFertilizerRules_Postfix))
            );
        }

        internal static void CheckApplyFertilizerRules_Postfix(HoeDirt __instance, string fertilizerId, ref HoeDirtFertilizerApplyStatus __result)
        {
            if (__result == HoeDirtFertilizerApplyStatus.HasAnotherFertilizer)
            {
                // When blocked by another fertilizer, we can still apply the new one.
                __result = HoeDirtFertilizerApplyStatus.Okay;
            }
        }
    }
}
