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
