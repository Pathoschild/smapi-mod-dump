using Newtonsoft.Json;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.Tracker
{
    public class CatchConditions
    {
        public CatchConditions(string season, bool raining, bool day, string location)
        {
            Season = season;
            Raining = raining;
            Day = day;
            Location = location;
        }

        [JsonProperty("season", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Season
        {
            get;
            private set;
        }

        [JsonProperty("location", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Location
        {
            get;
            private set;
        }

        [JsonProperty("rain", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Raining
        {
            get;
            private set;
        }

        [JsonProperty("day", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Day
        {
            get;
            private set;
        }
    }
}
