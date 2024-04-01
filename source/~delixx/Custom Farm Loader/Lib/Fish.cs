/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
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
using StardewValley.GameData.Locations;

namespace Custom_Farm_Loader.Lib
{
    public class Fish
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private string Name = ""; //Makes debugging easier

        public string Id = "";
        public float Chance = 0;
        public float ChancePerLevel = 0.02f;
        public int OptimalDepth = 0;
        public float DepthDropOff = 0;
        public bool ChanceModifiedByLuck = false;
        public Filter Filter = new Filter();
        public FishType Type = FishType.Item;

        //Whether each field was changed during parse
        public bool ChangedType = false;
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
        }

        public static List<Fish> parseFishJsonArray(JProperty jArray)
        {
            List<Fish> ret = new List<Fish>();

            foreach (JObject obj in jArray.First())
                ret.Add(parseFishJObject(obj));

            return ret;
        }

        public static Fish parseFishJObject(JObject obj)
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
                        fish.ChangedType = true;
                        fish.Type = UtilityMisc.parseEnum<FishType>(value); break;
                    case "chanceperlevel" or "chanceperlvl":
                        fish.ChangedChancePerLevel = true;
                        fish.ChancePerLevel = float.Parse(value); break;
                    case "optimaldepth":
                        fish.ChangedOptimalDepth = true;
                        fish.OptimalDepth = int.Parse(value); break;
                    case "depthdropoff":
                        fish.ChangedDepthDropOff = true;
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

            return fish;
        }

        public void updateType()
        {
            if (Id == "" || Id.StartsWith("("))
                return;

            if (Type == FishType.Item) {
                var itemIdString = ItemObject.MapNameToItemId(Id);

                Id = itemIdString;
                Type = FishType.Item;
                return;
            }

            if (Type == FishType.Furniture) {
                var furnitureIdString = Furniture.MapNameToParentsheetindex(Id);

                Id = furnitureIdString;
                Type = FishType.Furniture;
                return;
            }

            if (Type == FishType.Location) {
                if (Id.ToLower() == "farm")
                    Id = "Farm_Standard";

                return;
            }

            throw new Exception($"Fish ID not found: {Id}");
        }

        public void applyDefaultIfNotChanged()
        {
            if (Type != FishType.Item)
                return;

            var fishAsset = Game1.content.Load<Dictionary<string, string>>("Data\\Fish");

            if (!fishAsset.ContainsKey(Id))
                return;

            Type = FishType.Item;
            var fishData = fishAsset[Id];
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
}
