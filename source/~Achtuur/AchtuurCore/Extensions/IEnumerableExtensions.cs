/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Extensions;
public static class IEnumerableExtensions
{
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        return source.Select(item =>
        {
            action(item);
            return item;
        });
    }
}
