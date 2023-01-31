/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Picks the dish of the day.
/// </summary>
internal static class DailyDishPicker
{
    /// <summary>
    /// Picks the dish of the day.
    /// </summary>
    /// <param name="random">Ignored.</param>
    /// <returns>Dish of the day.</returns>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Match signature of other pickers.")]
    internal static Item? Pick(Random random)
        => Game1.dishOfTheDay.getOne();
}
