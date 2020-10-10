/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

namespace TwilightShards.Common
{
    public static class MTwisterExtensions
    {
        public static string GetRandomItem(this string[] array, MersenneTwister mt)
        {
            int l = array.Length;

            return array[mt.Next(l - 1)];
        }

        public static int GetRandomItem(this int[] array, MersenneTwister mt)
        {
            int l = array.Length;

            return array[mt.Next(l - 1)];
        }

    }
}
