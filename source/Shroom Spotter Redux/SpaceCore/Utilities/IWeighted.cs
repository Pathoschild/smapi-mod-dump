/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceCore.Utilities
{
    public class IWeighted
    {
        public int Weight { get; set; }

        public static IWeighted choose( List< IWeighted > choices, int totalWeight )
        {
            int num = new Random().Next(totalWeight);
            
            for (int i = 0; i < choices.Count; ++i)
            {
                IWeighted curr = choices[i];
                if (num <= curr.Weight)
                    return curr;
                else
                    num -= curr.Weight;
            }

            return choices.Last();
        }

        public static T choose< T >(List<T> choices, int totalWeight)
        where   T : IWeighted
        {
            int num = new Random().Next(totalWeight);

            for (int i = 0; i < choices.Count; ++i)
            {
                T curr = choices[i];
                if (num <= curr.Weight)
                    return curr;
                else
                    num -= curr.Weight;
            }

            return choices.Last();
        }
    }
}
