/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Lib
{
    public class Fish
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static Dictionary<int, string> CachedFishData;

        private string Name = ""; //Makes debugging easier

        public string Id = "";
        public float Chance = 0;
        public float ChancePerLevel = 0.02f;
        public int OptimalDepth = 0;
        public float DepthDropOff = 0;
        public bool ChanceModifiedByLuck = false;
        public Filter Filter = new Filter();
        public FishType Type = FishType.Any;

        //Whether each field was changed during parse
        public bool ChangedChance = false;
        public bool ChangedChancePerLevel = false;
        public bool ChangedOptimalDepth = false;
        public bool ChangedDepthDropOff = false;
        public bool ChangedChanceModifiedByLuck = false;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            CachedFishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
        }

        public static List<Fish> parseFishJsonArray(JProperty jArray)
        {
            List<Fish> ret = new List<Fish>();

            foreach (JObject obj in jArray.First())
                ret.AddRange(checkGroups(obj));

            return ret;
        }

        private static Fish parseFishJObject(JObject obj)
        {
            Fish fish = new Fish();

            foreach (JProperty property in obj.Properties()) {
                string name = property.Name;
                string value = property.Value.ToString();

                switch (name.ToLower()) {
                    case "id":
                        fish.Name = value;
                        fish.Id = value; break;
                    case "type":
                        fish.Type = UtilityMisc.parseEnum<FishType>(value); break;
                    case "chanceperlevel" or "chanceperlvl":
                        fish.ChancePerLevel = float.Parse(value); break;
                    case "optimaldepth":
                        fish.OptimalDepth = int.Parse(value); break;
                    case "depthdropoff":
                        fish.DepthDropOff = float.Parse(value); break;
                    case "chance":
                        fish.ChangedChance = true;
                        fish.Chance = float.Parse(value); break;
                    case "chancemodifiedbyluck":
                        fish.ChangedChanceModifiedByLuck = true;
                        fish.ChanceModifiedByLuck = bool.Parse(value); break;
                    default:
                        if (fish.Filter.parseAttribute(property))
                            break;
                        Monitor.Log($"Unknown Fish Attribute '{fish.Id}' -> '{name}'", LogLevel.Error);
                        throw new ArgumentException($"Unknown Fish Attribute '{fish.Id}' -> '{name}'", name);
                }
            }
            fish.updateType();
            fish.applyDefaultIfNotChanged();

            return fish;
        }

        private void updateType()
        {
            if (Id == "")
                return;

            if (int.TryParse(Id, out int id))
                return;

            if (Type == FishType.Any || Type == FishType.Item) {
                var itemIdString = ItemObject.MapNameToParentsheetindex(Id);

                if (int.TryParse(itemIdString, out int itemId)) {
                    Id = itemIdString;
                    Type = FishType.Item;
                    return;
                }
            }

            if (Type == FishType.Any || Type == FishType.Furniture) {
                var furnitureIdString = Furniture.MapNameToParentsheetindex(Id);

                if (int.TryParse(furnitureIdString, out int furnitureId)) {
                    Id = furnitureIdString;
                    Type = FishType.Furniture;
                    return;
                }
            }

            if(Type == FishType.Any || Type == FishType.Location) {
                Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
                var fishLocationData = locationData.FirstOrDefault(el => el.Key.ToLower() == Id.ToLower());

                if (fishLocationData.Key != null)
                    return;
            }

            Monitor.Log($"Item/Fish/Furniture/Location not found: {Id}", LogLevel.Error);
            throw new Exception($"Item/Fish/Furniture/Location not found: {Id}");
        }

        private static List<Fish> checkGroups(JObject obj)
        {
            var baseFish = parseFishJObject(obj);

            if (baseFish.Type == FishType.Any || baseFish.Type == FishType.Location) {
                Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
                var fishLocationData = locationData.FirstOrDefault(el => el.Key.ToLower() == baseFish.Id.ToLower());

                if (fishLocationData.Key != null)
                    return parseFishLocationData(fishLocationData.Value, obj);
            }

            return new List<Fish>() { parseFishJObject(obj) };
        }

        private static List<Fish> parseFishLocationData(string fishLocationData, JObject obj)
        {
            List<Fish> ret = new List<Fish>();
            var splitData = fishLocationData.Split('/');
            for (int k = 0; k < 4; k++) {
                //Locations data shows fish per season in index 4 (spring) to 7 (winter)
                //Every second entry isn't a fish, but instead a weird location restriction paired with the previous entry
                //-1 = this fish can be caught everywhere, 0 and 1 only in certain areas decided by the GameLocations' getFishingLocation.
                //I am just going to ignore that location restriction for simplicity sake
                //If someone wants those they can just always list the fish separately instead of using a group
                var seasonFish = splitData[4 + k].Split(' ').Where((_, i) => i % 2 == 0).ToList();

                foreach (string fishId in seasonFish) {
                    var newFish = parseFishJObject(obj);
                    newFish.Id = fishId;
                    newFish.Type = FishType.Item;
                    newFish.applyDefaultIfNotChanged();

                    if (newFish.ChangedChance)
                        newFish.Chance *= newFish.getDefaultChanceOr1();

                    if (!newFish.Filter.ChangedSeasons) {
                        newFish.Filter.Seasons = new List<string>() { UtilityMisc.getSeasonString(k) };
                        ret.Add(newFish);

                    } else {
                        if (!ret.Exists(el => el.Id == newFish.Id))
                            ret.Add(newFish);
                    }
                }
            }

            return ret;
        }

        public void applyDefaultIfNotChanged()
        {
            if (Type != FishType.Any && Type != FishType.Item)
                return;

            if (int.TryParse(Id, out int fishId)) {
                if (!CachedFishData.ContainsKey(fishId))
                    return;

                Type = FishType.Item;
                var fishData = CachedFishData[fishId];
                var split = fishData.Split('/');

                Name = split[0];

                if (split[1] == "trap")
                    return;

                if (!Filter.ChangedStartTime)
                    Filter.StartTime = int.Parse(split[5].Split(' ')[0]);

                if (!Filter.ChangedEndTime)
                    Filter.EndTime = int.Parse(split[5].Split(' ')[1]);

                if (!Filter.ChangedSeasons)
                    Filter.Seasons = split[6].Split(' ').ToList();

                if (!Filter.ChangedWeather)
                    Filter.parseNativeWeather(split[7]);

                if (!ChangedOptimalDepth)
                    OptimalDepth = Convert.ToInt32(split[9]);

                if (!ChangedChance)
                    Chance = (float)Convert.ToDouble(split[10]);

                if (!ChangedDepthDropOff)
                    DepthDropOff = (float)Convert.ToDouble(split[11]);
            }
        }

        private float getDefaultChanceOr1()
        {
            if (int.TryParse(Id, out int fishId)) {
                if (!CachedFishData.ContainsKey(fishId))
                    return 1f;

                var fishData = CachedFishData[fishId];
                var split = fishData.Split('/');

                if (split[1] == "trap")
                    return 1f;

                return (float)Convert.ToDouble(split[10]);
            }

            return 1f;
        }
    }
}
