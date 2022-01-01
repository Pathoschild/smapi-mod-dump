/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

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

        /// <summary>
        /// This returns a (uint) number from 1 to the passed number. Added function, not part of original code.
        /// </summary>
        /// <param name="x">The passed number</param>
        /// <returns>A number within [1, x].</returns>
        public static uint RollFrom1ToX(this Random r, uint x)
        {
            return (uint)(x * r.NextDouble() + 1);
        }

        public static double RollInRange(this Random r,double min, double max)
        {
            return r.NextDouble() * (max - min) + min;
        }

        public static int GetRandomItem(this int[] array, Random mt)
        {
            int l = array.Length;

            return array[mt.Next(l - 1)];
        }
    }
}
