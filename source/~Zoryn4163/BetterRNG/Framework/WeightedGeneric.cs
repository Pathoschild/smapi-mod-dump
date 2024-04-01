/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zoryn4163/SMAPI-Mods
**
*************************************************/

namespace BetterRNG.Framework
{
    internal class WeightedGeneric<T> : IWeighted
    {
        public object Value { get; set; }
        public int Weight { get; set; }

        public T TValue => (T)this.Value;

        public WeightedGeneric(int weight, T value)
        {
            this.Weight = weight;
            this.Value = value;
        }

        public static WeightedGeneric<T> Create(int weight, T value)
        {
            return new WeightedGeneric<T>(weight, value);
        }
    }
}
