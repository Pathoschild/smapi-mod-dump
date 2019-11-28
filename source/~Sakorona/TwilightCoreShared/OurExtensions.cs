using StardewModdingAPI;
using System;

namespace TwilightShards.Common
{
    public static class TwilightExtensions
    {
        public static string GetRandomItem(this string[] array, Random r)
        {
            int l = array.Length;

            return array[r.Next(l)];
        }

        public static bool Contains<T>(this T[] array, T val)
        {
            foreach (T i in array)
            {
                if (val.Equals(i))
                    return true;
            }

            return false;
        }
    }
}
