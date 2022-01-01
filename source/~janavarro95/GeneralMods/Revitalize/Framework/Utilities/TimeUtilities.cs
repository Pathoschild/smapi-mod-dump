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

namespace Revitalize.Framework.Utilities
{
    public class TimeUtilities
    {
        /// <summary>
        /// Wraps SDV time to be able to display it better.
        /// </summary>
        public class StardewTime
        {
            public int days;
            public int hours;
            public int minutes;
            public int weeks;
            public int seasons;
            public int years;

            public StardewTime()
            {

            }

            public StardewTime(int Minutes)
            {
                this.parse(Minutes);
            }


            private void parse(int Minutes)
            {
                this.years = Minutes / 60 / 24 / 7 / 4 / 4;
                Minutes -= (this.years * 60 * 24 * 7 * 4 * 4);
                this.seasons = Minutes / 60 / 24 / 7 / 4;
                Minutes -= (this.seasons * 60 * 24 * 7 * 4);
                this.weeks = Minutes / 60 / 24 / 7;
                Minutes -= (this.weeks * 60 * 24 * 7);
                this.days = Minutes / 60 / 24;
                Minutes -= (this.days * 60 * 24);
                this.hours = Minutes / 60;
                Minutes -= (this.hours * 60);
                this.minutes = Minutes;
            }

            public string GetTimeString()
            {
                StringBuilder b = new StringBuilder();
                if (this.years > 0) b.Append("Y: " + this.years);
                if (this.seasons > 0) b.Append("S: " + this.seasons);
                if (this.weeks > 0) b.Append("W: " + this.weeks);
                if (this.days > 0) b.Append("D: " + this.days);
                if (this.hours > 0) b.Append("H: " + this.hours);
                if (this.minutes > 0) b.Append("M: " + this.minutes);
                else
                {
                    b.Append("M: " + 0);
                }

                return b.ToString();
            }

            public string GetVerboseString()
            {
                StringBuilder b = new StringBuilder();
                if (this.years > 0) b.Append("Years: " + this.years);
                if (this.seasons > 0) b.Append("Seasons: " + this.seasons);
                if (this.weeks > 0) b.Append("Weeks: " + this.weeks);
                if (this.days > 0) b.Append("Days: " + this.days);
                if (this.hours > 0) b.Append("Hours: " + this.hours);
                if (this.minutes > 0) b.Append("Minutes: " + this.minutes);
                else
                {
                    b.Append("Minutes: " + 0);
                }

                return b.ToString();
            }

        }


        /// <summary>
        /// Gets the minutes for the time passed in.
        /// </summary>
        /// <param name="Days"></param>
        /// <param name="Hours"></param>
        /// <param name="Minutes"></param>
        /// <returns></returns>
        public static int GetMinutesFromTime(int Days, int Hours, int Minutes)
        {
            int amount=0;
            amount += Days * 24 * 60;
            amount += Hours * 60;
            amount += Minutes;
            return amount;
        }

        /// <summary>
        /// Gets a shortened string representing the time.
        /// </summary>
        /// <param name="Minutes"></param>
        /// <returns></returns>
        public static string GetTimeString(int Minutes)
        {
            StardewTime s = new StardewTime(Minutes);
            return s.GetTimeString();
        }

        /// <summary>
        /// Gets a more detailed string representation of the time.
        /// </summary>
        /// <param name="Minutes"></param>
        /// <returns></returns>
        public static string GetVerboseTimeString(int Minutes)
        {
            StardewTime s = new StardewTime(Minutes);
            return s.GetVerboseString();
        }
    }
}
