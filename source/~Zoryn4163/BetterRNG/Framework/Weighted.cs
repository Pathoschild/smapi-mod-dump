/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zoryn4163/SMAPI-Mods
**
*************************************************/

using System.Linq;

namespace BetterRNG.Framework
{
    internal static class Weighted
    {
        public static T Choose<T>(T[] list) where T : IWeighted
        {
            if (!list.Any())
                return default;

            int totalweight = list.Sum(c => c.Weight);
            int choice = ModEntry.Twister.Next(totalweight);
            int sum = 0;

            foreach (var obj in list)
            {
                for (float i = sum; i < obj.Weight + sum; i++)
                {
                    if (i >= choice)
                        return obj;
                }
                sum += obj.Weight;
            }

            return list.First();
        }
    }
}