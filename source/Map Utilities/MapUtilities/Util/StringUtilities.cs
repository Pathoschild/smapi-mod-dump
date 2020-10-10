/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/Map-Utilities
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Util
{
    internal static class StringUtilities
    {
        internal static string getSeasonalName(string input)
        {
            string[] splitName = input.Split('_');
            if (splitName.Length < 2 || !splitName[0].Equals("spring"))
                return input;
            string outString = Game1.currentSeason;
            if (splitName.Length > 1)
            {
                outString += "_";
                for (int i = 1; i < splitName.Length; i++)
                {
                    outString += splitName[i];
                    if (i != splitName.Length - 1)
                        outString += "_";
                }
            }
            return outString;
        }
    }
}
