/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.Utilities
{
    public class TimeUtilities
    {
        /// <summary>
        /// Gets the minutes for the time passed in.
        /// </summary>
        /// <param name="Days"></param>
        /// <param name="Hours"></param>
        /// <param name="Minutes"></param>
        /// <returns></returns>
        public static int GetMinutesFromTime(int Days, int Hours, int Minutes)
        {
            int amount = 0;
            amount += Days * 24 * 60;
            amount += Hours * 60;
            amount += Minutes;
            return amount;
        }
    }
}
