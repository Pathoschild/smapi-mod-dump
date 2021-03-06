/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System;

namespace ImJustMatt.Common.Extensions
{
    internal static class CommonExtensions
    {
        public static int RoundUp(this int i, int d = 1)
        {
            return (int) (d * Math.Ceiling((float) i / d));
        }
    }
}