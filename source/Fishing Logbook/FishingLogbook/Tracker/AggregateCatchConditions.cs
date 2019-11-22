using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.Tracker
{
    public class AggregateCatchConditions
    {
        public AggregateCatchConditions(int objectID, CatchConditions conditions)
        {
            ObjectID = objectID;
            Seasons = new HashSet<string>();
            Locations = new HashSet<string>();
            Add(conditions);
        }

        [JsonProperty("id")]
        public int ObjectID
        {
            get;
            private set;
        }
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public HashSet<string> Locations
        {
            get;
            private set;
        }
        [JsonProperty("seasons", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public HashSet<string> Seasons
        {
            get;
            private set;
        }

        [JsonProperty("rain", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Rain;
        [JsonProperty("noRain", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool NoRain;
        [JsonProperty("day", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Day;
        [JsonProperty("night", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Night;

        public AggregateCatchConditions(int objectID, HashSet<string> locations, HashSet<string> seasons, bool rain, bool noRain, bool day, bool night)
        {
            ObjectID = objectID;
            Locations = locations;
            Seasons = seasons;
            Rain = rain;
            NoRain = noRain;
            Day = day;
            Night = night;
        }

        public void Add(CatchConditions c)
        {
            Rain = Rain || c.Raining;
            NoRain = NoRain || !c.Raining;
            Day = Day || c.Day;
            Night = Night || !c.Day;
            Seasons.Add(c.Season);
            Locations.Add(c.Location);
        }

        public override int GetHashCode()
        {
            return ObjectID;
        }
    }
}
