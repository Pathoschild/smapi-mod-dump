using Newtonsoft.Json;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.Tracker
{
    public class CaughtFish
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int ObjectID
        {
            get;
            private set;
        }
        [JsonProperty("quality", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int FishQuality
        {
            get;
            private set;
        }
        [JsonProperty("time", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int TimeOfDay
        {
            get;
            private set;
        }
        [JsonIgnore]
        public string TimeOfDayFormatted
        {
            get
            {
                return Game1.getTimeOfDayString(TimeOfDay);
            }
        }
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Size
        {
            get;
            private set;
        }
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SDate DateCaught
        {
            get;
            private set;
        }
        [JsonProperty("catchConditions", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public CatchConditions Conditions
        {
            get;
            private set;
        }
        [JsonIgnore]
        public string FishName
        {
            get
            {
                if (Game1.objectInformation.ContainsKey(ObjectID))
                {
                    return Game1.objectInformation[ObjectID].Split('/')[4];
                }
                return "null";
            }
        }
        private static int GetNightTime(string currentSeason)
        {
            if (currentSeason == "spring" || currentSeason == "summer")
                return 1800 + 200;
            if (currentSeason == "fall")
                return 1700 + 200;
            return currentSeason == "winter" ? 1600 + 200 : 1800 + 200;
        }
        private static bool WasDayTime(int time, string currentSeason)
        {
            return time < GetNightTime(currentSeason);
        }
        public CaughtFish(int objectID, int fishQuality, int timeOfDay, int size, SDate dateCaught, string location, bool raining)
        {
            ObjectID = objectID;
            FishQuality = fishQuality;
            TimeOfDay = timeOfDay;
            Size = size;
            DateCaught = dateCaught;
            Conditions = new CatchConditions(DateCaught.Season, raining, WasDayTime(TimeOfDay, DateCaught.Season), location);
        }
    }
}
