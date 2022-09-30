/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace StardewRoguelike.Extensions
{
    internal static class ListShuffle
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Game1.random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
