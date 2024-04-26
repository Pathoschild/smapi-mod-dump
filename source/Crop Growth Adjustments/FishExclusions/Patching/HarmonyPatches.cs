/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace FishExclusions.Patching;

public class HarmonyPatches
{
    public static void GetFish(GameLocation __instance, float millisecondsAfterNibble, string bait, int waterDepth,
        Farmer who, double baitPotency, Vector2 bobberTile, ref Item __result, string locationName = null)
    {
        try
        {
            HarmonyPatchExecutors.GetFish(__instance, millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, ref __result, locationName);
        }
        catch (Exception e)
        {
            ModEntry.ModMonitor.Log($"Failed in { nameof(GetFish) }:\n{ e }", LogLevel.Error);
        }
    }
}