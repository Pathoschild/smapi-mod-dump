/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), "GetAdditionalDifficulty")]
    internal class GetAdditionalDifficultyPatch
    {
        public static bool Prefix(MineShaft __instance, ref int __result)
        {
            int level = Roguelike.GetLevelFromMineshaft(__instance);
            __result = level >= Roguelike.DangerousThreshold ? 1 : 0;
            return false;
        }
    }
}
