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

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Picks a random unlocked hat.
/// </summary>
internal static class HatPicker
{
    /// <summary>
    /// Picks a random unlocked hat.
    /// </summary>
    /// <param name="random">Random instance.</param>
    /// <returns>Hat.</returns>
    internal static SObject? Pick(Random random)
    {
        ModEntry.ModMonitor.DebugOnlyLog("Picked hats");

        Dictionary<ISalable, int[]>? stock = Utility.getHatStock();
        return stock.Count != 0 ? stock.ElementAt(random.Next(stock.Count)).Key as SObject : null;
    }
}
