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
    /// Event occurs on this day of the month. Range 1-28
    /// </summary>
    /// <returns></returns>
    public class DayOfTheMonth:EventPrecondition
    {

        public int day;
        public DayOfTheMonth()
        {

        }

        public DayOfTheMonth(int Day)
        {
            if (Day <= 0) throw new Exception("Day must be in range of 1-28!");
            if (Day > 28) throw new Exception("Day must be in range of 1-28!");
            this.day = Day;
        }

        public override string ToString()
        {
            return this.precondition_occursOnThisDayOfTheMonth();
        }

        /// <summary>
        /// Event occurs on this day of the month. Range 1-28
        /// </summary>
        /// <param name="Day">The day this can occur on. Range 1-28</param>
        /// <returns></returns>
        public string precondition_occursOnThisDayOfTheMonth()
        {
            StringBuilder b = new StringBuilder();
            b.Append("u ");
            b.Append(this.day.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.dayOfMonth == this.day;
        }
    }
}
