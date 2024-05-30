/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/foxwhite25/Stardew-Ultimate-Fertilizer
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;

namespace UltimateFertilizer;

public interface IUltimateFertilizerApi {
    /// <summary>Apply fertilizer to a HoeDirt instance without side-effects.</summary>
    /// <param name="dirt">The HoeDirt instance.</param>
    /// <param name="itemId">The fertilizer you want to apply.</param>
    /// <param name="who">Player instance to check if they have perks that boost speed.</param>
    /// <returns>Whether the fertilizer was applied.</returns>
    /// <remarks>This method does not check for valid fertilizer. You can check that manually via <see cref="HoeDirt.CheckApplyFertilizerRules"/>.</remarks>
    bool ApplyFertilizerOnDirt(HoeDirt dirt, string itemId, Farmer who);

    /// <summary>Check if fertilizer is applied.</summary>
    /// <param name="dirt">The HoeDirt instance.</param>
    /// <param name="itemId">The fertilizer you want to check.</param>
    bool IsFertilizerApplied(HoeDirt dirt, string itemId);

    /// <summary>Register fertilizer types.</summary>
    /// <param name="itemIds">A collection of fertilizer itemIds that are considered same type.</param>
    /// <remarks>The fertilizer should be tiered from low to high.</remarks>
    void RegisterFertilizerType(IEnumerable<string> itemIds);
}