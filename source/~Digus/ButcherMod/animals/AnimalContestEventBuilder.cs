using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnimalHusbandryMod.common;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestEventBuilder
    {
        public static readonly IList<string> Seasons = new ReadOnlyCollection<string>(new List<string>() {"spring", "summer", "fall", "winter"});

        public static CustomEvent createEvent(Character participant)
        {
            string key = generateKey();
            string script = "";

            return new CustomEvent(key,script);
        }

        private static string generateKey()
        {
            string key = "{0}/z {1}/u {2}/t {3}";
            var date = SDate.Now();
            string id = $"657277{date.Year:00}{Utility.getSeasonNumber(date.Season)}{date.Day:00}";

            string seasons = String.Join(" ",Seasons.Where(s => s != date.Season).ToArray());
            string day = date.Day.ToString();
            string time = "600 900";
            return String.Format(key, id,seasons,day,time);
        }
    }
}
