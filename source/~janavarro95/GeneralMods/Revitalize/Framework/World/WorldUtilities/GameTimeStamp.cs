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

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    /// <summary>
    /// Wraps SDV time to be able to display it better.
    /// </summary>
    public class GameTimeStamp
    {

        public const int SeasonsPerYear = 4;
        public const int DaysPerSeason = 28;
        public const int HoursPerDay = 24;
        public const int MinutesPerHour = 60;

        public const int MinutesPerDay = MinutesPerHour * HoursPerDay;
        public const int MinutesPerSeason = MinutesPerDay * DaysPerSeason;
        public const int MinutesPerYear = MinutesPerSeason * SeasonsPerYear;

        public int years;
        public int seasons;
        public int days;
        public int hours;
        public int minutes;


        public GameTimeStamp()
        {

        }

        public GameTimeStamp(int Minutes)
        {
            this.parse(Minutes);
        }

        private void parse(int Minutes)
        {
            this.years = Minutes / MinutesPerYear;
            Minutes -= this.years * MinutesPerYear;
            this.seasons = Minutes / MinutesPerSeason;
            Minutes -= this.seasons * MinutesPerSeason;
            this.days = Minutes / MinutesPerDay;
            Minutes -= this.days *MinutesPerDay;
            this.hours = Minutes / MinutesPerHour;
            Minutes -= this.hours * MinutesPerHour;
            this.minutes = Minutes;
        }

        public GameTimeStamp(int Years, int Seasons, int Days, int Hours, int Minutes)
        {
            this.years= Years;
            this.seasons= Seasons;
            this.days= Days;
            this.hours= Hours;
            this.minutes= Minutes;
        }

        public virtual int toInGameMinutes()
        {
            int output = this.years * MinutesPerYear;

            output += this.seasons * MinutesPerSeason;
            output += this.days * MinutesPerDay;
            output += this.hours * MinutesPerHour;
            output += this.minutes;
            return output;
        }

    }
}
