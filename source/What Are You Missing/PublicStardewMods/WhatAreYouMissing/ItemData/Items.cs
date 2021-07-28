/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public abstract class Items
    {
        protected Dictionary<int, SObject> items;
        protected ModConfig Config;
        private Dictionary<int, IList<string>> JsonAssetsObjects;
        public enum FarmTypes
        {
            Normal = 0,
            River = 1,
            Forest = 2,
            Hilltop = 3,
            Wilderness = 4
        };

        abstract protected void AddItems();

        public Items()
        {
            Config = ModEntry.modConfig;
            items = new Dictionary<int, SObject>();
            JsonAssetsObjects = new Dictionary<int, IList<string>>();
            GetJsonAssetsInfo();
            AddItems();
        }

        protected void AddFish(int parentSheetIndex)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            switch (data[parentSheetIndex].Split('/')[7])
            {
                case "sunny":
                    if (!Game1.isRaining || Config.ShowAllFishFromCurrentSeason)
                    {
                        AddOneCommonObject(parentSheetIndex);
                    }
                    break;
                case "rainy":
                    if (Game1.isRaining || Config.ShowAllFishFromCurrentSeason)
                    {
                        AddOneCommonObject(parentSheetIndex);
                    }
                    break;
                case "both":
                    AddOneCommonObject(parentSheetIndex);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds a crop if there is sufficient time to grow it before the 
        /// season ends.
        /// </summary>
        /// <param name="key"></param>
        protected void ManuallyAddCrop(int key)
        {
            CropConversion cropConverter = new CropConversion();
            if (Utilities.IsThereEnoughTimeToGrowSeeds(cropConverter.CropToSeedIndex(key)))
            {
                AddOneCommonObject(key);
            }
        }

        protected void AddOneCommonObject(int parentSheetIndex)
        {
            AddCommonObject(parentSheetIndex, 1);
        }

        protected void AddCommonObject(int parentSheetIndex, int stackSize)
        {
            if (!items.ContainsKey(parentSheetIndex))
            {
                items.Add(parentSheetIndex, new SObject(parentSheetIndex, stackSize));
            }
        }
        private int SeasonNameToIndex(string season)
        {
            switch (season)
            {
                case "spring":
                    return (int)SeasonIndex.Spring;
                case "summer":
                    return (int)SeasonIndex.Summer;
                case "fall":
                    return (int)SeasonIndex.Fall;
                case "winter":
                    return (int)SeasonIndex.Winter;
                default:
                    return -1;
            }
        }

        protected void AddNormalSeasonalFish(string season)
        {
            Dictionary<string, string> LocationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            int seasonIndex = SeasonNameToIndex(season);

            foreach (KeyValuePair<string, string> data in LocationData)
            {
                if (!Utilities.IsTempOrFishingGameOrBackwoodsLocation(data.Key))
                {
                    string[] seasonalFish = data.Value.Split('/')[seasonIndex].Split(' ');
                    for (int i = 0; i < seasonalFish.Length; ++i)
                    {
                        if (i % 2 == 0)
                        {
                            //Its a parent sheet index
                            bool successful = int.TryParse(seasonalFish[i], out int parentSheetIndex);
                            if (!successful)
                            {
                                ModEntry.Logger.LogFishIndexError(data.Value.Split('/')[seasonIndex], seasonalFish[i], i);
                                continue;
                            }
                            //I want to add them manually, -1 means no fish at this location
                            if (IsNormalFish(parentSheetIndex) && NotInAllSeasons(parentSheetIndex))
                            {
                                if (!Config.DoNotShowCaughtFish)
                                {
                                    //Add the fish regardless of if its been caught or not
                                    AddFish(parentSheetIndex);
                                }
                                else if(!IsFishAlreadyCaught(parentSheetIndex))
                                {
                                    //only add it if it hasn't been caught yet
                                    AddFish(parentSheetIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected bool IsFishAlreadyCaught(int id)
        {
            return Game1.player.fishCaught.ContainsKey(id);
        }

        private bool IsNormalFish(int parentSheetIndex)
        {
            bool isAFish = IsAFish(parentSheetIndex);
            Constants constants = new Constants();
            return !constants.LEGENDARY_FISH.Contains(parentSheetIndex) && isAFish;
        }

        protected bool IsAFish(int parentSheetIndex)
        {
            //Sometimes a mod can put the info into location data but not edit fish data
            //or object info so the mod it is meant to support doesn't exist
            //on this machine. Just double check
            if (parentSheetIndex == -1 || !Game1.objectInformation.ContainsKey(parentSheetIndex))
            {
                return false;
            }

            int category = -1;
            bool isAFish = true;

            string[] typeAndCategory = Game1.objectInformation[parentSheetIndex].Split('/')[3].Split(' ');
            if (typeAndCategory.Length > 1)
            {
                category = int.Parse(typeAndCategory[1]);
            }
            else
            {
                //Things like Algae don't have the category -4 (fish category)
                //they only have the word Fish
                //i.e Fish vs Fish -4
                isAFish = false;
            }

            return isAFish && category != SObject.junkCategory;
        }

        private bool NotInAllSeasons(int parentSheetIndex)
        {
            List<FishInfo> fishInfo = new FishDisplayInfo(parentSheetIndex).GetFishInfoList();

            foreach(FishInfo info in fishInfo)
            {
                if(info.GetSeasons().Count != 4)
                {
                    return true; 
                }
            }

            return false;
        }

        protected void AddCrops(string season)
        {
            Dictionary<int, string> cropData = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            foreach(KeyValuePair<int, string> data in cropData)
            {
                if (IsFromJsonAssets(data.Key))
                {
                    AddJsonAssetCrop(season, data);
                }
                else
                {
                    AddCrop(season, data);
                }
                
            }
        }

        private void AddJsonAssetCrop(string season, KeyValuePair<int, string> data)
        {
            if (IsPurchasable(data.Key))
            {
                AddCrop(season, data);
            }
        }

        private bool IsPurchasable(int id)
        {
            IList<string> requirements = JsonAssetsObjects[id];
            foreach(string requirement in requirements)
            {
                //Only want to check year requirements, not season
                //Assuming you can't buy JA stuff from travelling cart
                string[] info = requirement.Split(' ');
                if(info[0] == "y")
                {
                    return Game1.year >= int.Parse(info[1]);
                }
            }
            return true;
        }

        private void AddCrop(string season, KeyValuePair<int, string> data)
        {
            Constants constants = new Constants();
            string[] crop = data.Value.Split('/');
            string[] seasons = crop[1].Split(' ');
            //Don't add it if its common to all seasons
            if (seasons.Length != 4 && seasons.Contains(season) && !constants.SPECIAL_SEEDS.Contains(data.Key))
            {
                if (Game1.currentSeason == season)
                {
                    if (Utilities.IsThereEnoughTimeToGrowSeeds(data.Key))
                    {
                        AddOneCommonObject(int.Parse(crop[3]));
                    }
                }
                else
                {
                    AddOneCommonObject(int.Parse(crop[3]));
                }
            }
        }

        private bool IsFromJsonAssets(int id)
        {
            return JsonAssetsObjects.ContainsKey(id);
        }

        protected void AddFruitTrees(string season)
        {
            Dictionary<int, string> fruitTreesData = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
            foreach (KeyValuePair<int, string> data in fruitTreesData)
            {
                if (IsFromJsonAssets(data.Key))
                {
                    AddJsonAssetFruitTree(season, data);
                }
                else
                {
                    AddFruitTree(season, data);
                }
            }
        }

        private void AddJsonAssetFruitTree(string season, KeyValuePair<int, string> data)
        {
            if (IsPurchasable(data.Key))
            {
                AddFruitTree(season, data);
            }
        }

        private void AddFruitTree(string season, KeyValuePair<int, string> data)
        {
            string[] fruitTree = data.Value.Split('/');
            if (fruitTree[1] == season)
            {
                AddOneCommonObject(int.Parse(fruitTree[2]));
            }
        }

        private void GetJsonAssetsInfo()
        {
            IModHelper helper = ModEntry.HelperInstance;
            // get Json Assets
            object mod = this.GetJsonAssets();
            if (mod == null)
                return;

            // get objects
            IEnumerable objects = helper.Reflection.GetField<IEnumerable>(mod, "Objects").GetValue();
            foreach (object obj in objects)
            {
                int id = helper.Reflection.GetProperty<int>(obj, "Id").GetValue();
                IList<string> requirements = helper.Reflection.GetProperty<IList<string>>(obj, "PurchaseRequirements").GetValue();
                JsonAssetsObjects.Add(id, requirements != null ? requirements : new List<string>());
            }
        }

        private object GetJsonAssets()
        {
            IModHelper helper = ModEntry.HelperInstance;

            // get mod info
            IModInfo modInfo = helper.ModRegistry.Get("spacechase0.jsonAssets");
            if (modInfo == null)
                return null; // mod isn't installed

            // get mod instance
            var property = modInfo.GetType().GetProperty("Mod", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property == null)
                throw new InvalidOperationException($"Can't access 'Mod' field on type '{modInfo.GetType().FullName}'.");
            return property.GetValue(modInfo);
        }
    }
}
