/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace ResourceStorage.BetterCrafting
{
    internal class Utilities
    {
        /// <summary>
        /// Creates a list of items whose Stacks add up to the given amount.
        /// </summary>
        /// <param name="itemId">The item id (qualified or unqualified) of the item to create.</param>
        /// <param name="amount">The total amount to create.</param>
        /// <returns></returns>
        public static ICollection<Item> CreateItemsWithTotalAmount(string itemId, long amount)
        {
            List<Item> items = new List<Item>();

            itemId = ItemRegistry.QualifyItemId(itemId);

            // If the id could not be qualified, return an empty list
            if(itemId is null)
                return items;

            while (amount > 0) 
            {
                // The ItemRegistry will automatically cap the stack size at the item's max stack size.
                Item item = ItemRegistry.Create(itemId, (int)amount);

                amount -= item.Stack;
                items.Add(item);
            }

            return items;
        }
    }
}
