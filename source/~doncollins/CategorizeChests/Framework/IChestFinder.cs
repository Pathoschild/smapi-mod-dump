/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewValley.Objects;
using StardewValleyMods.CategorizeChests.Framework.Persistence;

namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// A helper for finding the chest object corresponding to a given chest address.
    /// </summary>
    interface IChestFinder
    {
        Chest GetChestByAddress(ChestAddress address);
    }
}