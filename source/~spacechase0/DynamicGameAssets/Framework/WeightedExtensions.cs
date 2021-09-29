/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicGameAssets.Framework
{
    internal static class WeightedExtensions
    {
        public static T Choose<T>(this Weighted<T>[] choices, Random r = null) where T : ICloneable
        {
            if (choices.Length == 0)
                return default(T);
            if (choices.Length == 1)
                return choices[0].Value;

            if (r == null)
                r = new Random();

            double sum = choices.Sum(choice => choice.Weight);

            double chosenWeight = r.NextDouble() * sum;
            foreach (var choice in choices)
            {
                if (chosenWeight < choice.Weight) // might need change to <=
                    return choice.Value;
                chosenWeight -= choice.Weight;
            }

            throw new Exception("This should never happen");
        }
        public static T Choose<T>(this List<Weighted<T>> choices, Random r = null) where T : ICloneable
        {
            return choices.ToArray().Choose(r);
        }
    }
}
