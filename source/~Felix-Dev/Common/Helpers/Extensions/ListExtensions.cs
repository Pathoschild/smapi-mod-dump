/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.Common.Helpers.Extensions
{
    internal static class ListExtensions
    {
        public static int Replace<T>(this List<T> list, Predicate<T> oldItemSelector, T newItem)
        {
            var oldItemIndex = list.FindIndex(oldItemSelector);
            if (oldItemIndex == -1)
            {
                return -1;
            }

            list[oldItemIndex] = newItem;
            return oldItemIndex;
        }
    }
}
