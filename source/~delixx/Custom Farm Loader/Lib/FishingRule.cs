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
using StardewValley.Tools;

namespace Custom_Farm_Loader.Lib
{
    public class FishingRule
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public Filter Filter = new Filter();
        public Area Area = new Area();

        public List<Fish> Fish = new List<Fish>();
        public Fish ForAll = null;
        public bool CatchOceanCrabPotFish = false;
        public GameLocation Location = null;
        public string LocationName = "Farm";

        public bool ChangedCatchOceanCrabPotFish = false;


        public static List<FishingRule> parseFishingRuleJsonArray(JProperty fishingRuleArray, IManifest manifest)
        {
            List<FishingRule> ret = new List<FishingRule>();
            int i = 0;

            foreach (JObject obj in fishingRuleArray.First())
                ret.Add(parseFishingRuleJObject(obj, i++, manifest));

            return ret;
        }

        private static FishingRule parseFishingRuleJObject(JObject obj, int i, IManifest manifest)
        {
            FishingRule fishingRule = new FishingRule();
            fishingRule.Filter.Manifest = manifest;
            string name = "";

            try {
                foreach (JProperty property in obj.Properties()) {
                    if (property.Value.Type == JTokenType.Null)
                        continue;

                    name = property.Name;
                    string value = property.Value.ToString();

                    switch (name.ToLower()) {
                        case "fish":
                            fishingRule.Fish = Lib.Fish.parseFishJsonArray(property);
                            break;
                        case "catchoceancrabpotfish":
                            fishingRule.ChangedCatchOceanCrabPotFish = true;
                            fishingRule.CatchOceanCrabPotFish = Boolean.Parse(value);
                            break;
                        case "forall":
                            fishingRule.ForAll = Lib.Fish.parseFishJObject((JObject)property.First());
                            break;
                        default:
                            if (fishingRule.Filter.parseAttribute(property))
                                break;
                            if (fishingRule.Area.parseAttribute(property))
                                break;
                            Monitor.Log("Unknown FishingRule Attribute", LogLevel.Error);
                            throw new ArgumentException($"Unknown FishingRule Attribute", name);
                    }

                }

                if (fishingRule.ForAll != null) {
                    name = "ForAll (Parse)";
                    fishingRule.parseForAll();
                }
            } catch (Exception ex) {
                Monitor.Log($"At FishingRules[{i}] -> '{name}'", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Trace);
                throw;
            }

            return fishingRule;
        }
        public StardewValley.Object getFish(bool isUsingMagicBait, float millisecondsAfterNibble, string bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        {
            var validFish = Fish.FindAll(el => el.Filter.isValid(excludeSeason: isUsingMagicBait, excludeTime: isUsingMagicBait, excludeWeather: isUsingMagicBait, who: who));
            validFish = UtilityMisc.PickSomeInRandomOrder(validFish, validFish.Count).ToList();
            string whichFish = "";
            FishType fishType = FishType.Item;

            foreach (Fish fish in validFish) {
                double chance = fish.Chance;
                chance -= Math.Max(0, fish.OptimalDepth - waterDepth) * (fish.DepthDropOff * chance);
                chance += who.FishingLevel * fish.ChancePerLevel;
                if (fish.ChanceModifiedByLuck)
                    chance += who.DailyLuck;

                if (chance > 0.9)
                    chance = 0.9;

                bool beginnersRod = who != null && who.CurrentTool != null && who.CurrentTool is FishingRod && who.CurrentTool.UpgradeLevel == 1;
                if (beginnersRod) {
                    chance *= 1.1;

                    string value = ItemObject.GetItemData(fish.Id, 1);
                    if (value != "" && int.Parse(value) >= 50)
                        continue;
                }

                if (Game1.random.NextDouble() > chance)
                    continue;

                whichFish = fish.Id;
                fishType = fish.Type;
                break;
            }

            if (whichFish == "") {
                whichFish = Game1.random.Next(167, 173).ToString();

                if (who.currentLocation.HasUnlockedAreaSecretNotes(who) && Game1.random.NextDouble() < 0.08) {
                    StardewValley.Object o = Game1.getFarm().tryToCreateUnseenSecretNote(who);
                    if (o != null)
                        return o;
                }
            }

            if (!Game1.isFestival() && Game1.random.NextDouble() <= 0.15 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                whichFish = "890";

            if (fishType == FishType.Item)
                return new StardewValley.Object(whichFish, 1);

            if (fishType == FishType.Furniture)
                return new StardewValley.Objects.Furniture(whichFish, Vector2.Zero);


            Monitor.LogOnce($"Item not found: {whichFish}", LogLevel.Warn);
            return new StardewValley.Object(Game1.random.Next(167, 173).ToString(), 1);
        }

        //Yes, this is very hard coded. Don't judge
        private void parseForAll()
        {
            var fishProperties = new Dictionary<string, string>() {
                { nameof(ForAll.ChangedType), nameof(ForAll.Type) },
                { nameof(ForAll.ChangedChance), nameof(ForAll.Chance) },
                { nameof(ForAll.ChangedChancePerLevel), nameof(ForAll.ChancePerLevel) },
                { nameof(ForAll.ChangedOptimalDepth), nameof(ForAll.OptimalDepth) },
                { nameof(ForAll.ChangedDepthDropOff), nameof(ForAll.DepthDropOff) },
                { nameof(ForAll.ChangedChanceModifiedByLuck), nameof(ForAll.ChanceModifiedByLuck) },
            };

            var filterProperties = new Dictionary<string, string>() {
                { nameof(ForAll.Filter.ChangedSeasons), nameof(ForAll.Filter.Seasons) },
                { nameof(ForAll.Filter.ChangedWeather), nameof(ForAll.Filter.Weather) },
                { nameof(ForAll.Filter.ChangedAfterDay), nameof(ForAll.Filter.AfterDay) },
                { nameof(ForAll.Filter.ChangedBeforeDay), nameof(ForAll.Filter.BeforeDay) },
                { nameof(ForAll.Filter.ChangedStartTime), nameof(ForAll.Filter.StartTime) },
                { nameof(ForAll.Filter.ChangedEndTime), nameof(ForAll.Filter.EndTime) },
                { nameof(ForAll.Filter.ChangedFishingLevel), nameof(ForAll.Filter.FishingLevel) },
                { nameof(ForAll.Filter.ChangedForagingLevel), nameof(ForAll.Filter.ForagingLevel) },
                { nameof(ForAll.Filter.ChangedMiningLevel), nameof(ForAll.Filter.MiningLevel) },
                { nameof(ForAll.Filter.ChangedCombatLevel), nameof(ForAll.Filter.CombatLevel) },
                { nameof(ForAll.Filter.ChangedFarmingLevel), nameof(ForAll.Filter.FarmingLevel) },
                { nameof(ForAll.Filter.ChangedCPConditions), nameof(ForAll.Filter.CPConditions) },
            };

            foreach (var fish in Fish) {
                foreach (var prop in fishProperties)
                    compareChangedProperties(prop, ForAll, fish);

                foreach (var prop in filterProperties)
                    compareChangedProperties(prop, ForAll.Filter, fish.Filter);
            }

        }

        private void compareChangedProperties(KeyValuePair<string, string> kvp, System.Object source, System.Object target)
        {
            var type = source.GetType();
            var cProperty = type.GetField(kvp.Key);
            var vProperty = type.GetField(kvp.Value);

            if (!(bool)cProperty.GetValue(target) && (bool)cProperty.GetValue(source)) {
                cProperty.SetValue(target, true);
                vProperty.SetValue(target, vProperty.GetValue(source));
            }
        }
    }
}
