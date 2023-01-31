/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Models.WeightedRandom;

using AtraShared.ConstantsAndEnums;

using BetterIntegratedModItems.DataModels;

namespace BetterIntegratedModItems.Framework;
internal static class SeasonalFishHandler
{
    private static WeightedManager<int>? manager = new();
    private static StardewSeasons lastLoadedSeason = StardewSeasons.None;

    internal static void Initialize(ModEntry mod)
    {
        mod.OnLocationSeen += Reset;
    }

    private static bool Load()
    {
        if (!StardewSeasonsExtensions.TryParse(Game1.currentSeason, value: out StardewSeasons currentSeason, ignoreCase: true))
        {
            ModEntry.ModMonitor.Log($"Failed to parse {Game1.currentSeason} as a season, skipping.", LogLevel.Warn);
            return false;
        }

        if (currentSeason == lastLoadedSeason)
        {
            return false;
        }
        lastLoadedSeason = currentSeason;

        return true;
    }

    private static void Reset(object? sender, LocationSeenEventArgs e) => manager = null;
}
