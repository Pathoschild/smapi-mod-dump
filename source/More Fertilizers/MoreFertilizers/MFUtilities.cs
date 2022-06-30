/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace MoreFertilizers;

/// <summary>
/// Generalized utilities for this mod.
/// </summary>
internal static class MFUtilities
{
    /// <summary>
    /// Gets a random fertilizer taking into account the player's level.
    /// </summary>
    /// <param name="level">Int skill level.</param>
    /// <returns>Fertilizer ID (-1 if not found).</returns>
    internal static int GetRandomFertilizerFromLevel(this int level)
        => Game1.random.Next(Math.Clamp(level + 1, 0, 11)) switch
            {
                0 => ModEntry.LuckyFertilizerID,
                1 => ModEntry.JojaFertilizerID,
                2 => ModEntry.PaddyCropFertilizerID,
                3 => ModEntry.OrganicFertilizerID,
                4 => ModEntry.FruitTreeFertilizerID,
                5 => ModEntry.FishFoodID,
                6 => ModEntry.DeluxeFishFoodID,
                7 => ModEntry.DomesticatedFishFoodID,
                8 => ModEntry.DeluxeJojaFertilizerID,
                9 => ModEntry.DeluxeFruitTreeFertilizerID,
                _ => ModEntry.BountifulFertilizerID,
            };

    /// <summary>
    /// Whether hoedirt contains a crop should be considered a Joja crop for the Joja and Organic fertilizers.
    /// </summary>
    /// <param name="dirt">Hoedirt.</param>
    /// <returns>True if the hoedirt has a joja crop.</returns>
    internal static bool HasJojaCrop(this HoeDirt dirt)
        => dirt.crop is not null && dirt.crop.IsJojaCrop();

    /// <summary>
    /// Whether the crop should be considered a Joja crop for the Joja and Organic fertilizers.
    /// </summary>
    /// <param name="crop">crop.</param>
    /// <returns>True if crop is a joja crop.</returns>
    internal static bool IsJojaCrop(this Crop crop)
    {
        string data = Game1.objectInformation[crop.indexOfHarvest.Value];
        int index = data.IndexOf('/');
        if (index >= 0)
        {
            ReadOnlySpan<char> span = data.AsSpan(0, index);
            return span.Contains("Joja", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }
}