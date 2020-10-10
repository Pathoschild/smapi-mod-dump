/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace FishingAdjustMod
{
    [HarmonyPatch(typeof(Beach))]
    [HarmonyPatch("getFish")]
    [HarmonyPatch(new[] { typeof(float), typeof(int), typeof(int), typeof(global::StardewValley.Farmer), typeof(double) })]
    internal static class PatchBeachGetFish
    {
        internal static bool Prefix(ref global::StardewValley.Object __result, int waterDepth, global::StardewValley.Farmer who)
        {
            if (Global.Config.OverrideGetSummerFishKing
                && Game1.currentSeason.Equals("summer")
                && who.getTileX() >= 82
                && who.FishingLevel >= 5
                && waterDepth >= 3
                && Game1.random.NextDouble() < Global.Config.SummerFishKingThreshold)
            {
                __result = new global::StardewValley.Object(159, 1, false, -1, 0);
                return false;
            }

            return true;
        }
    }
}
