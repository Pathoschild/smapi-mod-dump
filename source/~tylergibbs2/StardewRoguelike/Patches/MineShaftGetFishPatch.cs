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
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), "getFish")]
    internal class MineShaftGetFishPatch
    {
        public static bool Prefix(MineShaft __instance, ref SObject? __result, Farmer who)
        {
            __result = Roguelike.GetFish(__instance, who);
            return false;
        }
    }
}
