/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkbenchAnywhere.Utils
{
    public static class ItemListExtension
    {
        /// <summary>
        /// Consume itemId x count from given list of items, and returns a number of items that maybe remaining
        /// </summary>
        /// <param name="list"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int ConsumeItem(this IList<Item> list, int itemId, int count)
        {
            var remainingCount = count;

            for (var i = 0; i < list.Count && remainingCount > 0; i++)
            {
                var item = list.ElementAt(i);
                if (item == null || item.parentSheetIndex.Value != itemId)
                    continue;

                var numberToRemove = Math.Min(item.Stack, remainingCount);
                item.Stack -= numberToRemove;
                remainingCount -= numberToRemove;
                if (item.Stack <= 0)
                    list[i] = null;
            }

            return remainingCount;
        }
    }
}
