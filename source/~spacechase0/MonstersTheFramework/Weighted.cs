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

namespace MonstersTheFramework
{
    public class Weighted<T>// : ICloneable where T : ICloneable
    {
        public double Weight { get; set; }
        public T Value { get; set; }

        public Weighted(double weight, T value)
        {
            this.Weight = weight;
            this.Value = value;
        }

        /*
        public object Clone()
        {
            return new Weighted<T>(this.Weight, (T)this.Value?.Clone());
        }
        */
    }
}
