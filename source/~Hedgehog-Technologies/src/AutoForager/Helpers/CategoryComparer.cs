/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace AutoForager.Helpers
{
    internal class CategoryComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x is null || y is null)
            {
                throw new NullReferenceException();
            }

            if (x.Equals(y)) return 0;
            if (x.Equals("Lumisteria - Mt. Vapius")) return 1;
            if (y.Equals("Lumisteria - Mt. Vapius")) return -1;
            if (x.Equals("Lumisteria - Serene Meadow")) return 1;
            if (y.Equals("Lumisteria - Serene Meadow")) return -1;
            if (x.Equals("Stardew Valley Expanded")) return 1;
            if (y.Equals("Stardew Valley Expanded")) return -1;
            if (x.Equals("Other")) return 1;
            if (y.Equals("Other")) return -1;
            if (x.Equals("Special")) return 1;
            if (y.Equals("Special")) return -1;

            return string.Compare(x, y);
        }
    }
}
