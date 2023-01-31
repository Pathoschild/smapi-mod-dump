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

using StardewValley.TerrainFeatures;

namespace GrowableBushes.Framework;

/// <summary>
/// Extension methods for BushSizes.
/// </summary>
internal static class BushSizesExtraExtensions
{
    /// <summary>
    /// Gets the <see cref="Bush.size"/> for the BushSizes.
    /// </summary>
    /// <param name="sizes">BushSize.</param>
    /// <returns>int size.</returns>
    internal static int ToStardewBush(this BushSizes sizes)
        => sizes switch
        {
            BushSizes.SmallAlt => Bush.smallBush,
            BushSizes.Town => Bush.mediumBush,
            BushSizes.TownLarge => Bush.largeBush,
            BushSizes.Harvested => Bush.walnutBush,
            _ when BushSizesExtensions.IsDefined(sizes) => (int)sizes,
            _ => Bush.smallBush,
        };

    /// <summary>
    /// Get the width (in tiles) of a specific BushSize.
    /// </summary>
    /// <param name="sizes">size.</param>
    /// <returns>width.</returns>
    internal static int GetWidth(this BushSizes sizes)
        => sizes switch
        {
            BushSizes.Small or BushSizes.SmallAlt => 1,
            BushSizes.Large or BushSizes.TownLarge => 3,
            _ => 2
        };

    /// <summary>
    /// Gets the BushSize this bush corresponds to.
    /// </summary>
    /// <param name="bush">Bush to check.</param>
    /// <returns>BushSize if found.</returns>
    /// <remarks>Does not look at the metadata.</remarks>
    internal static BushSizes ToBushSize(this Bush bush)
    {
        return bush.size.Value switch
        {
            Bush.smallBush when bush.tileSheetOffset.Value == 1 => BushSizes.SmallAlt,
            Bush.smallBush => BushSizes.Small,
            Bush.mediumBush when bush.townBush.Value => BushSizes.Town,
            Bush.mediumBush => BushSizes.Medium,
            Bush.largeBush when bush.townBush.Value => BushSizes.TownLarge,
            Bush.largeBush => BushSizes.Large,
            Bush.walnutBush when bush.tileSheetOffset.Value == 1 => BushSizes.Walnut,
            Bush.walnutBush => BushSizes.Harvested,
            _ => BushSizes.Invalid
        };
    }
}