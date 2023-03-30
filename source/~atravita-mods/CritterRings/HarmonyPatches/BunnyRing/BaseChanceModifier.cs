/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

namespace CritterRings.HarmonyPatches.BunnyRing;

/// <summary>
/// Changes the base chance of bunnies spawning if the bunny ring is equipped.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class BaseChanceModifier
{
    [HarmonyPatch(nameof(GameLocation.addBunnies))]
    private static void Prefix(ref double chance)
    {
        if (ModEntry.BunnyRing > 0 && Game1.player.isWearingRing(ModEntry.BunnyRing))
        {
            chance = 1.1;
        }
    }
}
