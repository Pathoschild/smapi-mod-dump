/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/CustomEmotes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEmotes
{
    static class Extensions
    {
        public static IEnumerable<TSource> Deduplicate<TSource>(this IEnumerable<TSource> source, Func<TSource, object> keySelector)
        {
            var dict = new Dictionary<object, TSource>();

            foreach (var item in source)
            {
                dict[keySelector(item)] = item;
            }

            return dict.Values;
        }
    }
}
