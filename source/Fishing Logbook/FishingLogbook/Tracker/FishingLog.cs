using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.Tracker
{
    public class FishingLog
    {
        public FishingLog()
        {
            Logbook = new LinkedList<CaughtFish>();
            Conditions = new HashSet<AggregateCatchConditions>();
        }
        public HashSet<AggregateCatchConditions> Conditions
        {
            get;
            private set;
        }
        public LinkedList<CaughtFish> Logbook
        {
            get;
            private set;
        }
        public void RecordCatch(int fishID, int fishSize, int fishQuality)
        {
            Logbook.AddLast(new CaughtFish(fishID,fishQuality,Game1.timeOfDay,fishSize, new SDate(Game1.dayOfMonth, Game1.currentSeason, Game1.year),Game1.player.currentLocation.Name, Game1.isRaining));
            AggregateCatchConditions aggregate = Conditions.FirstOrDefault(c => c.ObjectID == fishID);
            if (aggregate != null)
                aggregate.Add(Logbook.Last().Conditions);
            else
                Conditions.Add(new AggregateCatchConditions(fishID, Logbook.Last().Conditions));
        }
        public string GetCatchConditionsAsString(Item item)
        {
            string str = "";
            if (item != null)
                if (item.Category == StardewValley.Object.FishCategory)
                {
                    AggregateCatchConditions conditions = Conditions.FirstOrDefault(c => c.ObjectID == item.ParentSheetIndex);
                    if (conditions != null)
                    {
                        str += "When: ";
                        if (conditions.Rain && !conditions.NoRain)
                            str += "\r\n" + "-Rain";
                        else if (conditions.Rain && conditions.NoRain)
                            str += "\r\n" + "-Any Weather";
                        else if (!conditions.Rain && conditions.NoRain)
                            str += "\r\n" + "-Sunshine";
                        if (conditions.Day && conditions.Night)
                            str += "\r\n" + "-Day or Night";
                        else if (conditions.Day && !conditions.Night)
                            str += "\r\n" + "-Daytime";
                        else
                            str += "\r\n" + "-Nighttime";
                        str += "\r\n" + "-" + conditions.Seasons.Select(c => c.Substring(0, 1).ToUpper() + c.Substring(1)).Aggregate((c, x) => c + "\n-" + x);
                        str += "\r\n" + "Where: ";
                        str += "\r\n" + "-" + conditions.Locations.Aggregate((c, x) => c + "\r\n" + "-" + x);
                    }
                    else
                        str += "Nothing Recorded in Fishing Logbook.";
                }
            return str;
        }
    }
}
