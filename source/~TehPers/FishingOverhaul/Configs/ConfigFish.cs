using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.FishingOverhaul.Configs {
    public class ConfigFish {

        public Dictionary<string, Dictionary<int, FishData>> PossibleFish { get; set; }

        public void PopulateData() {
            ModFishing.INSTANCE.Monitor.Log("Automatically populating fish.json with data from Fish.xnb and Locations.xnb", LogLevel.Info);
            ModFishing.INSTANCE.Monitor.Log("NOTE: If either of these files are modded, the config will reflect the changes! However, legendary fish and fish in the UndergroundMine are not being pulled from those files due to technical reasons.", LogLevel.Info);

            Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            Dictionary<string, string> locations = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

            this.PossibleFish = this.PossibleFish ?? new Dictionary<string, Dictionary<int, FishData>>();

            foreach (KeyValuePair<string, string> locationKv in locations) {
                string location = locationKv.Key;
                string[] locData = locationKv.Value.Split('/');
                const int offset = 4;

                Dictionary<int, FishData> possibleFish = this.PossibleFish.ContainsKey(location) ? this.PossibleFish[location] : new Dictionary<int, FishData>();
                this.PossibleFish[location] = possibleFish;

                for (int i = 0; i <= 3; i++) {
                    Season s = Season.SPRINGSUMMERFALLWINTER;
                    switch (i) {
                        case 0:
                            s = Season.SPRING;
                            break;
                        case 1:
                            s = Season.SUMMER;
                            break;
                        case 2:
                            s = Season.FALL;
                            break;
                        case 3:
                            s = Season.WINTER;
                            break;
                    }

                    string[] seasonData = locData[offset + i].Split(' ');
                    for (int j = 0; j < seasonData.Length; j += 2) {
                        if (seasonData.Length <= j + 1)
                            break;

                        int id = Convert.ToInt32(seasonData[j]);

                        // From location data
                        WaterType water = Helpers.ConvertWaterType(Convert.ToInt32(seasonData[j + 1])) ?? WaterType.BOTH;

                        // From fish data
                        if (possibleFish.TryGetValue(id, out FishData f)) {
                            f.WaterType |= water;
                            f.Season |= s;
                        } else if (fish.ContainsKey(id)) {
                            string[] fishInfo = fish[id].Split('/');
                            if (fishInfo[1] == "5") // Junk item
                                continue;

                            string[] times = fishInfo[5].Split(' ');
                            string weather = fishInfo[7].ToLower();
                            int minDepth = Convert.ToInt32(fishInfo[9]);
                            int minLevel = Convert.ToInt32(fishInfo[12]);
                            double chance = Convert.ToDouble(fishInfo[10]);

                            Weather w = Weather.BOTH;
                            switch (weather) {
                                case "sunny":
                                    w = Weather.SUNNY;
                                    break;
                                case "rainy":
                                    w = Weather.RAINY;
                                    break;
                            }

                            // Add initial data
                            f = new FishData(chance, water, s, Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), minDepth, minLevel, w);

                            // Add extra time ranges to the data
                            for (int startTime = 2; startTime + 1 < times.Length; startTime += 2)
                                f.Times.Add((Convert.ToInt32(times[startTime]), Convert.ToInt32(times[startTime + 1])));

                            possibleFish[id] = f;
                        } else {
                            ModFishing.INSTANCE.Monitor.Log("A fish listed in Locations.xnb cannot be found in Fish.xnb! Make sure those files aren't corrupt. ID: " + id, LogLevel.Warn);
                        }
                    }
                }
            }

            // NOW THEN, for the special cases >_>

            // Glacierfish
            this.PossibleFish["Forest"][775] = new FishData(.02, WaterType.RIVER, Season.WINTER, maxTime: 2000, minDepth: 5, minLevel: 6);

            // Crimsonfish
            this.PossibleFish["Beach"][159] = new FishData(.02, WaterType.BOTH, Season.SUMMER, maxTime: 2000, minDepth: 4, minLevel: 5);

            // Legend
            this.PossibleFish["Mountain"][163] = new FishData(.02, WaterType.LAKE, Season.SPRING, maxTime: 2300, minDepth: 5, minLevel: 10, weather: Weather.RAINY);

            // Angler
            this.PossibleFish["Town"][160] = new FishData(.02, WaterType.BOTH, Season.FALL, minDepth: 4, minLevel: 3);

            // Mutant Carp
            this.PossibleFish["Sewer"][682] = new FishData(.02, WaterType.BOTH, Season.SPRINGSUMMERFALLWINTER, minDepth: 5);

            // UndergroundMine
            double mineBaseChance = 0.3;
            if (this.PossibleFish["UndergroundMine"].TryGetValue(156, out FishData ghostFish))
                mineBaseChance = ghostFish.Chance;
            this.PossibleFish["UndergroundMine"][158] = new FishData(mineBaseChance / 3d, WaterType.BOTH, Season.SPRINGSUMMERFALLWINTER, mineLevel: 0);
            this.PossibleFish["UndergroundMine"][158] = new FishData(mineBaseChance / 2d, WaterType.BOTH, Season.SPRINGSUMMERFALLWINTER, mineLevel: 20);
            this.PossibleFish["UndergroundMine"][161] = new FishData(mineBaseChance / 3d, WaterType.BOTH, Season.SPRINGSUMMERFALLWINTER, mineLevel: 60);
            this.PossibleFish["UndergroundMine"][162] = new FishData(mineBaseChance / 3d, WaterType.BOTH, Season.SPRINGSUMMERFALLWINTER, mineLevel: 100);
        }

        public class FishData {
            //public string Name { get; set; }
            public double Chance { get; set; }
            public int MinCastDistance { get; set; }
            public WaterType WaterType { get; set; }
            public List<(int start, int finish)> Times { get; } = new List<(int start, int finish)>();
            [Obsolete]
            public int MinTime {
                get => this.Times.FirstOrDefault().start;
                set {
                    (int start, int finish) cur = this.Times.FirstOrDefault();
                    cur.start = value;
                    if (this.Times.Any())
                        this.Times[0] = cur;
                    else
                        this.Times.Add(cur);
                }
            }
            [Obsolete]
            public int MaxTime {
                get => this.Times.FirstOrDefault().finish;
                set {
                    (int start, int finish) cur = this.Times.FirstOrDefault();
                    cur.finish = value;
                    if (this.Times.Any())
                        this.Times[0] = cur;
                    else
                        this.Times.Add(cur);
                }
            }
            public Season Season { get; set; }
            public int MinLevel { get; set; }
            public Weather Weather { get; set; }
            public int MineLevel { get; set; }

            public FishData(double chance, WaterType waterType, Season season, int minTime = 600, int maxTime = 2600, int minDepth = 0, int minLevel = 0, Weather weather = Weather.BOTH, int mineLevel = -1) {
                this.Chance = chance;
                this.WaterType = waterType;
                this.Season = season;
                this.Times.Add((minTime, maxTime));
                this.MinCastDistance = minDepth;
                this.MinLevel = minLevel;
                this.Weather = weather;
                this.MineLevel = mineLevel;
            }

            public bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int depth, int level) {
                return (this.WaterType & waterType) > 0 && (this.Season & season) > 0 && (this.Weather & weather) > 0 && depth >= this.MinCastDistance && level >= this.MinLevel && this.Times.Any(range => time >= range.start && time <= range.finish);
            }

            public bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int depth, int level, int mineLevel) {
                return this.MeetsCriteria(waterType, season, weather, time, depth, level) && (this.MineLevel == -1 || mineLevel == this.MineLevel);
            }

            public virtual float GetWeightedChance(int depth, int level) {
                if (this.MinCastDistance >= 5) return (float) this.Chance + level / 50f;
                return (float) (5 - depth) / (5 - this.MinCastDistance) * (float) this.Chance + level / 50f;
            }

            public override string ToString() => $"Chance: {this.Chance}, Weather: {this.Weather}, Season: {this.Season}";
        }
    }
}
