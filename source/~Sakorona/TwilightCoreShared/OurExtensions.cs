/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
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
    }
}
