/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroCore.Generics
{
    public class WeightedArray<T>
    {
        private readonly T[] items;
        private readonly int[] sums;

        public WeightedArray(IEnumerable<T> Items, IEnumerable<int> Weights)
        {
            items = Items.ToArray();
            sums = new int[items.Length];
            int i = 0, sum = 0;
            foreach(var weight in Weights)
            {
                if (i >= items.Length)
                    break;
                sum += weight;
                sums[i] = sum;
                i++;
            }
        }
        public WeightedArray(T[] Items, int[] Weights)
        {
            items = Items[..]; //copy
            sums = new int[items.Length];
            int sum = 0;
            for(int i = 0; i < Weights.Length; i++)
            {
                if (i >= items.Length)
                    break;
                sum += Weights[i];
                sums[i] = sum;
            }
        }
        public T Choose(Random random = null)
        {
            random ??= Game1.random;
            int rand = random.Next(sums[^1] + 1);
            int ind = Array.BinarySearch(sums, rand);
            if (ind < 0)
                ind = ~ind;
            if (ind > 0 && ind < items.Length)
                return items[ind];
            return default;
        }
    }
}
