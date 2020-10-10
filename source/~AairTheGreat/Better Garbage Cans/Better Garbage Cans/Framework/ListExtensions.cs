/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterGarbageCans.Framework
{
    public static class ListExtensions
    {
        public static T ChooseItem<T>(this IList<T> source, Random rand) where T : IWeighted
        {
            if (!source.Any())
                throw new ArgumentException("Source must contain entries", nameof(source));

            double totalWeight = source.SumWeights();
            if (Math.Abs(totalWeight) < double.Epsilon * 10)
                throw new ArgumentException("Source must have a non-zero total weight", nameof(source));

            double n = rand.NextDouble();
            foreach (T entry in source)
            {
                double chance = entry.GetWeight() / totalWeight;
                if (n < chance)
                    return entry;
                n -= chance;
            }

            throw new ArgumentException("Source should contain positively weighted entries", nameof(source));
        }

        public static double SumWeights<T>(this IEnumerable<T> source) where T : IWeighted => source.Where(e => e.GetEnabled() == true && e.IsValid() == true).Sum(e => e.GetWeight());
    }
}
