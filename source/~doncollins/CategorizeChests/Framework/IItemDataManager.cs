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