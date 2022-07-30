/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Collections;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace FarmCaveSpawn;

/// <summary>
/// Handles the fake assets for this mod.
/// </summary>
internal static class AssetManager
{
    /// <summary>
    /// Gets fake asset location for the denylist.
    /// </summary>
    internal static string DENYLIST_LOCATION { get; } = PathUtilities.NormalizeAssetName("Mods/atravita_FarmCaveSpawn_denylist");

    /// <summary>
    /// Gets fake asset location for more locations that can spawn in fruit.
    /// </summary>
    internal static string ADDITIONAL_LOCATIONS_LOCATION { get; } = PathUtilities.NormalizeAssetName("Mods/atravita_FarmCaveSpawn_additionalLocations");

    /// <summary>
    /// Loads assets for this mod.
    /// </summary>
    /// <param name="e">Event args.</param>
    internal static void Load(AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(DENYLIST_LOCATION))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Low);
        }
        else if (e.Name.IsEquivalentTo(ADDITIONAL_LOCATIONS_LOCATION))
        {
            e.LoadFrom(GetInitialAdditionalLocations, AssetLoadPriority.High);
        }
    }

    private static Dictionary<string, string> GetInitialAdditionalLocations()
        => new()
        {
            ["FlashShifter.SVECode"] = "Custom_MinecartCave, Custom_DeepCave",
#if DEBUG // Regex's test!
            ["atravita.FarmCaveSpawn"] = "Town:[(4;5);(34;40)]",
#endif
        };
}