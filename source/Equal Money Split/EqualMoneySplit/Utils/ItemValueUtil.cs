/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EqualMoneySplit.Utils
{
    /// <summary>
    /// Utility used to calculate value of core Stardew Item objects
    /// </summary>
    public static class ItemValueUtil
    {
        /// <summary>
        /// Calculates the value of Items in a given collection
        /// </summary>
        /// <typeparam name="T">The generic type of Item to be calculated</typeparam>
        /// <param name="genericItems">The collection that contains a list of Items</param>
        /// <param name="isStackSizeChangeType">Whether or not the T type is a StackSizeChange Item type</param>
        /// <returns>The total value of all the items in the collection</returns>
        public static int CalculateItemCollectionValue<T>(IEnumerable<T> genericItems, bool isStackSizeChangeType = false)
        {
            int totalValue = 0;

            foreach (object genericItem in genericItems)
            {
                Item item = !isStackSizeChangeType ? (Item)genericItem : ((ItemStackSizeChange)genericItem).Item;
                int quantity = !isStackSizeChangeType ? item.Stack : ((ItemStackSizeChange)genericItem).OldSize - ((ItemStackSizeChange)genericItem).NewSize;
                int itemValue = GetAdjustedItemPrice(item);

                totalValue += itemValue * quantity;
            }

            return totalValue;
        }

        /// <summary>
        /// Calculates the value of a single core Stardew Valley Item
        /// </summary>
        /// <param name="item">The item to retrieve the value of</param>
        /// <returns>The value of the item</returns>
        private static int GetAdjustedItemPrice(Item item)
        {
            // If the item is actually a StardewValley object, then we use the already calculated price
            //    Otherwise we need to calculate this on our own
            if (item is StardewValley.Object)
                return Convert.ToInt32((item as StardewValley.Object).sellToStorePrice());
            else
                return Convert.ToInt32(Math.Ceiling(item.salePrice() / 2.0) * Game1.player.difficultyModifier);
        }
    }
}
