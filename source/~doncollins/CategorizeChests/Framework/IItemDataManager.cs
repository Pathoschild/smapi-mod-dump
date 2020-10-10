/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// A repository of item data that maps item keys to representative items
    /// and vice versa.
    /// </summary>
    interface IItemDataManager
    {
        IDictionary<string, IEnumerable<ItemKey>> Categories { get; }

        ItemKey GetKey(Item item);
        Item    GetItem(ItemKey itemKey);
        bool    HasItem(ItemKey itemKey);
    }
}