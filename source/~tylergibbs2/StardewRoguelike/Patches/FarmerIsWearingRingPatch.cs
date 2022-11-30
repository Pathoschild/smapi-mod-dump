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

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Farmer), "isWearingRing")]
    internal class FarmerIsWearingRingPatch
    {
        public static void Postfix(ref bool __result, int ringIndex)
        {
            if (ringIndex == 525 && Perks.HasPerk(Perks.PerkType.Sturdy))
                __result = true;
        }
    }
}
