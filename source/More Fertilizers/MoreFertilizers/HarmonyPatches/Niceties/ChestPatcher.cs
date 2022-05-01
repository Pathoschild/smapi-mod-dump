/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using HarmonyLib;
using StardewValley.Objects;

namespace MoreFertilizers.HarmonyPatches.Niceties;

/// <summary>
/// Holds patches against Chests.
/// </summary>
[HarmonyPatch(typeof(Chest))]
internal static class ChestPatcher
{
    [HarmonyPatch(nameof(Chest.dumpContents))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void Postfix(Chest __instance, GameLocation location)
    {
        if (__instance.giftbox.Value && ModEntry.LuckyFertilizerID != -1)
        {
            Game1.createMultipleObjectDebris(ModEntry.LuckyFertilizerID, (int)__instance.TileLocation.X, (int)__instance.TileLocation.Y, 5, location);
        }
    }
}