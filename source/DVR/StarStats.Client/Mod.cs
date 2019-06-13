using System;
using System.IO;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using StarStats.Common;
using ProtoBuf;

namespace StarStats.Client
{

    public class ModEntry : Mod
    {

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.TimeChanged += (o, e) => TimeChanged();
            helper.Events.GameLoop.DayStarted += (o, e) => DayStart();
            helper.Events.GameLoop.DayEnding += (o, e) => DayEnding();
        }

        
        uint lastTimeChange = 0;
        string fileName;
        public Database db;

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            fileName = Path.Combine(Constants.CurrentSavePath, "stats.bin");

            if (!File.Exists(fileName))
            {
                db = new Database();
            }
            else
            {
                using (var file = File.OpenRead(fileName))
                {
                    db = Serializer.Deserialize<Database>(file);
                }
            }

            var ts = TimeStamp();
            lastTimeChange = 0;
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            using (var file = File.Create(fileName))
            {
                Serializer.Serialize(file, db);
            }
        }

        private void DayStart()
        {
            TimeChanged();
        }

        private void DayEnding()
        {
            if (SDate.Now().DaysSinceStart == 0) return;
            db.AddRaw(TimeStamp(), Game1.timeOfDay, "bedtime");
            // move end of day stats up one tick. If you go to bed too fast, you won't get the final actions performed between last timechanged and getting to bed.
            // mostly stepstaken, but could be more.
            var ts = TimeStamp();
            if (Game1.timeOfDay < 2600)
            {
                ts++;
            }
            Collect(ts);
        }

        private void TimeChanged()
        {
            var ts = TimeStamp();
            Monitor.Log($"Time Changed to {SDate.Now()} {Game1.timeOfDay} {TimeStamp()}");
            if (ts == lastTimeChange && lastTimeChange > 0)
            {
                return;
            }
            lastTimeChange = ts;

            Collect(ts);
        }

        private void Collect(uint ts)
        {
            var p = Game1.player;

            db.Add(ts, p.stats.stepsTaken, "stepstaken");
            db.Add(ts, p.Money, "money");
            db.Add(ts, p.totalMoneyEarned, "earnings");
            db.Add(ts, p.stamina, "stamina");
            db.Add(ts, p.health, "health");
            db.Add(ts, (float)Game1.dailyLuck, "luck");
            db.Add(ts, p.experiencePoints[0], "skill", $"skill=farming");
            db.Add(ts, p.experiencePoints[1], "skill", $"skill=fishing");
            db.Add(ts, p.experiencePoints[2], "skill", $"skill=foraging");
            db.Add(ts, p.experiencePoints[3], "skill", $"skill=mining");
            db.Add(ts, p.experiencePoints[4], "skill", $"skill=combat");
            db.Add(ts, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, "time");
            db.Add(ts, p.deepestMineLevel, "minelevel");
            db.Add(ts, Game1.weatherIcon, "weather");
            db.Add(ts, Game1.stats.timesFished, "timesfished");
            
            foreach(var kvp in p.fishCaught)
            {
                var obj = new StardewValley.Object(kvp.Key, 1);
                db.Add(ts, kvp.Value[0], "fishCaught", $"type={obj.Name}");
            }
            foreach (var kvp in p.friendshipData.Pairs)
            {
                db.Add(ts, kvp.Value.Points, "friendship", $"npc={kvp.Key}");
                db.Add(ts, kvp.Value.GiftsThisWeek, "giftsweek", $"npc={kvp.Key}");
                db.Add(ts, kvp.Value.GiftsToday, "giftstoday", $"npc={kvp.Key}");
            }
            foreach (var kvp in p.basicShipped)
            {
                var obj = new StardewValley.Object(kvp.Key, 1);
                db.Add(ts, kvp.Value, "shipped", $"item={obj.Name}");
            }

            foreach (var loc in Game1.locations)
            {
                locationStats(loc, ts);
            }
        }

        // Converts time into a continuous counter from 0 to n.
        // Each increment represents 10 minutes, and each day has 120 possible
        // values. 600 is 0, 610 is 1, 700 is 6, and 2600 is 120. 
        // 121 possible values for each day.
        // 600 day 2 is 121.
        public static uint TimeStamp()
        {
            return TimeStamp(SDate.Now(), Game1.timeOfDay);
        }

        public static uint TimeStamp(SDate date, int tod)
        {
            var datePart = 121 * (date.DaysSinceStart - 1);
            // 10 minute increments starting from 600. So Subtract 600 and divide by 10.
            var timePart = (tod - 600) / 10;
            // Now its 0 1 2 3 4 5 10 11 12 13 14 15 20 21 ....
            var tens = (int)Math.Floor((double)timePart / 10);
            timePart -= 4 * tens;
            return (uint)(datePart + timePart);
        }


        private void locationStats(GameLocation loc, uint ts)
        {
            var watered = 0;
            var grass = 0;
            var stumps = 0;
            var trees = 0;
            var saplings = 0;
            var hoedirt = 0;
            var crops = 0;
            var dead = 0;
            var weeds = 0;
            var stone = 0;
            var artifacts = 0;
            var twigs = 0;
            var forage = 0;

            foreach (var tf in loc.terrainFeatures.Values)
            {
                if (tf is Grass)
                {
                    grass++;
                    continue;
                }
                if (tf is Tree)
                {
                    var tree = tf as Tree;
                    if (tree.stump.Value)
                    {
                        stumps++;
                        continue;
                    }
                    if (tree.growthStage.Value >= 5)
                    {
                        trees++;
                        continue;
                    }
                    saplings++;
                    continue;
                }
                if (tf is HoeDirt)
                {
                    var hd = tf as HoeDirt;
                    if (hd.crop == null)
                    {
                        hoedirt++;
                        continue;
                    }
                    if (hd.crop.dead.Value)
                    {
                        dead++;
                        continue;
                    }
                    if (hd.state.Value == 1)
                    {
                        watered++;
                    }
                    crops++;
                    continue;
                }
            }
            var stones = loc.Objects.Values.Where(x => x.Name == "Stone").GroupBy(x => x.ParentSheetIndex).ToDictionary(x => x.Key, x => x.Count());
            foreach (var obj in loc.Objects.Values)
            {
                if (obj.Name == "Weeds") {
                    weeds++;
                    continue;
                }
                if (obj.Name == "Stone")
                {
                    stone++;
                    continue;
                }
                if (obj.Name == "Twig")
                {
                    twigs++;
                    continue;
                }
                if (obj.Name == "Artifact Spot")
                {
                    artifacts++;
                    continue;
                }
                else if (obj.isForage(loc))
                {
                    forage++;
                    continue;
                }
                else
                {

                }
            }
            db.AddSkipZero(ts, watered, "objects", $"loc={loc.Name},type=watered");
            // todo: big crops, big stumps, boulders, meteors
            db.AddSkipZero(ts, grass, "objects", $"loc={loc.Name},type=grass");
            db.AddSkipZero(ts,stumps, "objects", $"loc={loc.Name},type=stumps");
            db.AddSkipZero(ts,trees, "objects", $"loc={loc.Name},type=trees");
            db.AddSkipZero(ts,saplings, "objects", $"loc={loc.Name},type=saplings");
            db.AddSkipZero(ts,hoedirt, "objects",$"loc={loc.Name},type=hoedirt");
            db.AddSkipZero(ts,crops, "objects", $"loc={loc.Name},type=crops");
            db.AddSkipZero(ts,dead, "objects", $"loc={loc.Name},type=deadcrops");
            db.AddSkipZero(ts,weeds, "objects", $"loc={loc.Name},type=weeds");
            db.AddSkipZero(ts,stone, "objects", $"loc={loc.Name},type=stone");
            db.AddSkipZero(ts,artifacts, "objects", $"loc={loc.Name},type=artifacts");
            db.AddSkipZero(ts,twigs, "objects", $"loc={loc.Name},type=twigs");
            db.AddSkipZero(ts,forage, "objects", $"loc={loc.Name},type=forage");
            db.AddSkipZero(ts, loc.debris.Count, "objects", $"loc={loc.Name},type=debris");
        }
    }
}
