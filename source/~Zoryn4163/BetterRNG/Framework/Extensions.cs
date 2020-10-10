/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zoryn4163/SMAPI-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterRNG.Framework
{
    internal static class Extensions
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default : list[ModEntry.Twister.Next(0, list.Count)];
        }

        public static T Choose<T>(this T[] list) where T : IWeighted
        {
            return Weighted.Choose(list);
        }
    }
}
