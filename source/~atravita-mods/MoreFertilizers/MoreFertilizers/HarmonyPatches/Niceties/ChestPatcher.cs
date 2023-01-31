/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using HarmonyLib;
using StardewValley.Objects;

namespace MoreFertilizers.HarmonyPatches.Niceties;

/// <summary>
/// Holds patches against Chests.
/// </summary>
[HarmonyPatch(typeof(Chest))]
internal static class ChestPatcher
{
    private const string HasGottenLucky = "atravita.MoreFertilizers/HasGottenLuckyFertilizer";

    [HarmonyPatch(nameof(Chest.dumpContents))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void Postfix(Chest __instance, GameLocation location)
    {
        if (__instance.giftbox.Value && ModEntry.LuckyFertilizerID != -1 && Game1.player.modData?.GetBool(HasGottenLucky) != true)
        {
            Game1.createMultipleObjectDebris(
                index: ModEntry.LuckyFertilizerID,
                xTile: (int)__instance.TileLocation.X,
                yTile: (int)__instance.TileLocation.Y,
                number: 5,
                location: location);
            Game1.player.modData?.SetBool(HasGottenLucky, true);
        }
    }
}