/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewArchipelago.Extensions
{
    public static class ListExtensions
    {
        public static List<T> Shuffle<T>(this IList<T> list, Random random)
        {
            var elements = list.ToList();
            var newList = new List<T>();
            while (elements.Any())
            {
                var index = random.Next(0, elements.Count);
                var element = elements[index];
                newList.Add(element);
                elements.RemoveAt(index);
            }

            return newList;
        }
    }
}
