/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.ReflectionManager;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace GiantCropFertilizer.HarmonyPatches;

/// <summary>
/// Fixes the issue where giant crops are not properly handled in the save on maps that are not Farm or IslandWest.
/// </summary>
internal static class FixSaveThing
{
    /// <summary>
    /// Applies this patch (for versions of the game before 1.6).
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(GameLocation).GetCachedMethod(nameof(GameLocation.TransferDataFromSavedLocation), ReflectionCache.FlagTypes.InstanceFlags),
            postfix: new HarmonyMethod(typeof(FixSaveThing), nameof(Postfix)));
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(GameLocation __instance, GameLocation l)
    {
        // game handles these two.
        if (__instance is IslandWest || __instance.Name.Equals("Farm", StringComparison.OrdinalIgnoreCase)
            || __instance.resourceClumps.Count >= l.resourceClumps.Count)
        {
            return;
        }

        // We need to avoid accidentally adding duplicates.
        // Keep track of occupied tiles here.
        HashSet<Vector2> prev = new(l.resourceClumps.Count);

        foreach (var clump in __instance.resourceClumps)
        {
            prev.Add(clump.tile.Value);
        }

        // restore previous giant crops.
        int count = 0;
        foreach (var clump in l.resourceClumps)
        {
            if (clump is GiantCrop crop && prev.Add(crop.tile.Value))
            {
                count++;
                __instance.resourceClumps.Add(crop);
            }
        }

        ModEntry.ModMonitor.Log($"Restored {count} giant crops at {__instance.NameOrUniqueName}");
    }
}
