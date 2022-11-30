/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Extensions
{
    internal static class ListShuffle
    {
        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            random ??= Game1.random;

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
