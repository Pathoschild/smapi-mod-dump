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
using StardewValley;

namespace StardustCore.Events.Preconditions.TimeSpecific
{
    /// <summary>
    /// The event must occur on this year or after it.
    /// If year is 1 then the event must occur in the first year.
    /// If year is >1 then the year must be that year value atleast. Aka year 2 means the event can occur years 2+.
    /// </summary>
    public class YearPrecondition:EventPrecondition
    {

        public int year;
        public YearPrecondition()
        {

        }

        public YearPrecondition(int Year)
        {
            this.year = Year;
        }


        public override string ToString()
        {
            return this.precondition_occursThisYearOrBefore();
        }

        /// <summary>
        /// The event must occur on this year or after it.
        /// If year is 1 then the event must occur in the first year.
        /// If year is >1 then the year must be that year value atleast. Aka year 2 means the event can occur years 2+.
        /// </summary>
        /// <param name="Year">If <year> is 1, must be in the first year. Otherwise, year must be at least this value.</param>
        /// <returns></returns>
        public string precondition_occursThisYearOrBefore()
        {
            StringBuilder b = new StringBuilder();
            b.Append("y ");
            b.Append(this.year.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            if (this.year == 1)
            {
                if (Game1.year == 1) return true;
                else return false;
            }
            else
            {
                return this.year <= Game1.year;
            }
        }
    }
}
