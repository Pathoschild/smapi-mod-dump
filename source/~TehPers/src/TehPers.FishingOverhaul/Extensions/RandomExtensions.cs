/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.FishingOverhaul.Extensions
{
    internal static class RandomExtensions
    {
        public static double NextDouble(this Random rand, double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}