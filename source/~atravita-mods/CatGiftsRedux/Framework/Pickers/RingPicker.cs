/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.ItemManagement;

using AtraShared.ConstantsAndEnums;

using StardewValley.Objects;

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Picks a ring.
/// </summary>
internal static class RingPicker
{
    /// <summary>
    /// Picks a ring.
    /// </summary>
    /// <param name="random">Random instance to use.</param>
    /// <returns>A random ring.</returns>
    internal static Ring? Pick(Random random)
    {
        List<int>? possibilities = DataToItemMap.GetAll(ItemTypeEnum.Ring).ToList();

        int id = possibilities[random.Next(possibilities.Count)];

        return new Ring(id);
    }
}
