using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using StardewValley;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using SObject = StardewValley.Object;


namespace WhatAreYouMissingOriginal
{
    class AllItems
    {
        private Dictionary<int, string> springData;
        private Dictionary<int, string> summerData;
        private Dictionary<int, string> fallData;
        private Dictionary<int, string> winterData;
        private Dictionary<int, string> ccData;
        private Dictionary<string, SObject> condensedItems;
        private TravellingMerchantStock stock;
        private IEnumerable<Item> playerItems;
        private IModHelper Helper;
        

        public AllItems(IEnumerable<Item> items, IModHelper helper)
        {
            playerItems = items;
            Helper = helper;
            //UpdateData();
        }
        public void UpdateTravellingMerchant()
        {
            //maybe I should pass in a brand new set of data with everything enabled just to add to avaialble inventory
            stock = new TravellingMerchantStock(Helper,
                new SpringSpecifics(true,true,true,true,false,false,false).content,
                 new SummerSpecifics(true, true, true, true, false, false, false).content,
                  new FallSpecifics(true, true, true, true, false, false, false).content,
                  new  WinterSpecifics(true, true, true, true, false, false, false).content,
                  new CommonCommunityCenterItems(true, true, true, true, false, false, false).content);
            //To follow the pattern of reading files, maybe should update this to write a file and then
            //read from it later - concern being it might write over the file data already there that this sorta uses
            //right now it should work because we read the data that is written only once when the game boots, (I was trying to avoid 
            //writing that data again but i don't think it'd affect performance)
            //could i read it in, call the functioin to add content and then write it again?
        }
        public void UpdateData()
        {
            //ModConfig modConfig = Helper.ReadConfig<ModConfig>();
            //bool showItemsFromCurrentSeasonButInLockedPlaces = modConfig.showItemsFromCurrentSeasonButInLockedPlaces;
            //bool showEveryItemFromCurrentSeason = modConfig.showEveryItemFromCurrentSeason;
            //bool showAllFish = modConfig.showAllFish;
            //bool showCommonCommunityCenterItems = modConfig.showCommonCommunityCenterItems;
            //bool showFruitTreesInPreviousSeason = modConfig.showFruitTreesInPreviousSeason;
            //bool checkGrowingCrops = modConfig.checkGrowingCrops;
            //bool onlyShowWhatCanBeGrownBeforeEndOfSeason = modConfig.onlyShowWhatCanBeGrownBeforeEndOfSeason;

            //SpringSpecifics springSpecifics = new SpringSpecifics(showItemsFromCurrentSeasonButInLockedPlaces, 
            //                                                        showEveryItemFromCurrentSeason, 
            //                                                        showAllFish, 
            //                                                        showCommonCommunityCenterItems, 
            //                                                        showFruitTreesInPreviousSeason, 
            //                                                        checkGrowingCrops,
            //                                                        onlyShowWhatCanBeGrownBeforeEndOfSeason);
            //springData = springSpecifics.content;
            //this.Helper.Data.WriteJsonFile(springSpecifics.fileName, springSpecifics);

            //SummerSpecifics summerSpecifics = new SummerSpecifics(showItemsFromCurrentSeasonButInLockedPlaces, 
            //                                                        showEveryItemFromCurrentSeason, 
            //                                                        showAllFish, 
            //                                                        showCommonCommunityCenterItems, 
            //                                                        showFruitTreesInPreviousSeason, 
            //                                                        checkGrowingCrops,
            //                                                        onlyShowWhatCanBeGrownBeforeEndOfSeason);
            //summerData = summerSpecifics.content;
            //this.Helper.Data.WriteJsonFile(summerSpecifics.fileName, summerSpecifics);

            //FallSpecifics fallSpecifics = new FallSpecifics(showItemsFromCurrentSeasonButInLockedPlaces, 
            //                                                    showEveryItemFromCurrentSeason, 
            //                                                    showAllFish,
            //                                                    showCommonCommunityCenterItems,
            //                                                    showFruitTreesInPreviousSeason, 
            //                                                    checkGrowingCrops,
            //                                                    onlyShowWhatCanBeGrownBeforeEndOfSeason);
            //fallData = fallSpecifics.content;
            //this.Helper.Data.WriteJsonFile(fallSpecifics.fileName, fallSpecifics);

            //WinterSpecifics winterSpecifics = new WinterSpecifics(showItemsFromCurrentSeasonButInLockedPlaces, 
            //                                                        showEveryItemFromCurrentSeason, 
            //                                                        showAllFish, 
            //                                                        showCommonCommunityCenterItems, 
            //                                                        showFruitTreesInPreviousSeason, 
            //                                                        checkGrowingCrops,
            //                                                        onlyShowWhatCanBeGrownBeforeEndOfSeason);
            //winterData = winterSpecifics.content;
            //this.Helper.Data.WriteJsonFile(winterSpecifics.fileName, winterSpecifics);

            //CommonCommunityCenterItems ccItems = new CommonCommunityCenterItems(showItemsFromCurrentSeasonButInLockedPlaces, 
            //                                                                        showEveryItemFromCurrentSeason, 
            //                                                                        showAllFish, 
            //                                                                        showCommonCommunityCenterItems, 
            //                                                                        showFruitTreesInPreviousSeason, 
            //                                                                        checkGrowingCrops,
            //                                                                        onlyShowWhatCanBeGrownBeforeEndOfSeason);
            //ccData = ccItems.content;
            //this.Helper.Data.WriteJsonFile(ccItems.fileName, ccItems);

            ////There's got to be a better way to do this but its 4am, probably change what class its in or something
            //springSpecifics.AddAvailableMerchantItems(springData, summerData, fallData, winterData);
            //springData = springSpecifics.content;
            ////changes what is passed in
            //summerSpecifics.AddAvailableMerchantItems(springData, summerData, fallData, winterData);
            //summerData = summerSpecifics.content;
            //fallSpecifics.AddAvailableMerchantItems(springData, summerData, fallData, winterData);
            //fallData = fallSpecifics.content;
            //winterSpecifics.AddAvailableMerchantItems(springData, summerData, fallData, winterData);
            //winterData = winterSpecifics.content;


            //just initilize the files i need by checking the season
            //if the performance is needed
            //switch (Game1.currentSeason)
            //{
            //    case "spring":
            //        springData = this.Helper.Data.ReadJsonFile<SpringSpecifics>("springSpecifics.json").content;
            //        break;
            //    case "summer":
            //        summerData = this.Helper.Data.ReadJsonFile<SummerSpecifics>("summerSpecifics.json").content;
            //        break;
            //    case "fall":
            //        fallData = this.Helper.Data.ReadJsonFile<FallSpecifics>("fallSpecifics.json").content;
            //        break;
            //    case "winter":
            //        winterData = this.Helper.Data.ReadJsonFile<WinterSpecifics>("winterSpecifics.json").content;
            //        break;
            //    default:
            //        //should never reach here
            //        break;
                    
            //}
            springData = this.Helper.Data.ReadJsonFile<SpringSpecifics>("springSpecifics.json").content;
            summerData = this.Helper.Data.ReadJsonFile<SummerSpecifics>("summerSpecifics.json").content;
            fallData = this.Helper.Data.ReadJsonFile<FallSpecifics>("fallSpecifics.json").content;
            winterData = this.Helper.Data.ReadJsonFile<WinterSpecifics>("winterSpecifics.json").content;
            ccData = this.Helper.Data.ReadJsonFile<CommonCommunityCenterItems>("commonCommunityCenterItems.json").content;
            condensedItems = CondensePlayerItems();
            UpdateTravellingMerchant();

        }

        public List<List<SObject>> CompareBasedOnSeason()
        {
            //I might get rid of the global variabls later for the data and instead just read from the file when its needed

            //Overall I should consider what to do with fruit trees, do I show them in the 
            //previous season since they take a season to grow?
            List<List<SObject>> result = new List<List<SObject>>();
            UpdateData();
            switch (Game1.currentSeason)
            {
                case "spring":
                    result.Add(WhatIsMissingFromCommunityCenter(springData)[0]);
                    result.Add(WhatIsMissingFromCommunityCenter(springData)[1]);
                    result.Add(WhatIsMissing(springData));
                    return result;
                case "summer":
                    result.Add(WhatIsMissingFromCommunityCenter(summerData)[0]);
                    result.Add(WhatIsMissingFromCommunityCenter(summerData)[1]);
                    result.Add(WhatIsMissing(summerData));
                    return result;
                case "fall":
                    result.Add(WhatIsMissingFromCommunityCenter(fallData)[0]);
                    result.Add(WhatIsMissingFromCommunityCenter(fallData)[1]);
                    result.Add(WhatIsMissing(fallData));
                    return result;
                case "winter":
                    result.Add(WhatIsMissingFromCommunityCenter(winterData)[0]);
                    result.Add(WhatIsMissingFromCommunityCenter(winterData)[1]);
                    result.Add(WhatIsMissing(winterData));
                    return result;
                default:
                    //should never reach here
                    return null;
            }
        }
        private List<List<SObject>> WhatIsMissingFromCommunityCenter(Dictionary<int, String> data)
        {
            Dictionary<string, string> rawBundleData = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles");
            Dictionary<int, int[]> processedBundleData = new Dictionary<int, int[]>(); //the key will be the bundleId, the int[] is index from parent sheet and howMany and quality
            List<List<SObject>> result = new List<List<SObject>>();
            List<SObject> generalMissingCCItems = new List<SObject>();
            List<SObject> specificMissingCCItems = new List<SObject>();
            Dictionary<string, SObject> items = condensedItems;
            //in the bundles.json file it appears that the value is: name/reward/itemsNeeded
            //where each item needed is: index howMany quality 
            //where quality is an int from 0-3, 0 being normal, 2 being gold
            foreach (KeyValuePair<string, string> pair in rawBundleData)
            {
                int key = Convert.ToInt32(pair.Key.Split('/')[1]);
                int[] value = new int[pair.Value.Split('/')[2].Split(' ').Length];
                for(int i = 0; i < pair.Value.Split('/')[2].Split(' ').Length; ++i)
                {
                    value[i] = Convert.ToInt32(pair.Value.Split('/')[2].Split(' ')[i]);
                }
                processedBundleData.Add(key, value);
            }
            CommunityCenter communityCenter = new CommunityCenter();
            Dictionary<int, bool[]> completedItems = communityCenter.bundlesDict();

            foreach(KeyValuePair<int, int[]> pair in processedBundleData)
            {
                bool completedBundle = true;
                foreach(bool completed in completedItems[pair.Key])
                {
                    if (!completed)
                    {
                        completedBundle = false;
                    }
                }
                for(int i = 0; i < pair.Value.Length && !completedBundle; ++i)
                {
                    //The bundle is not complete
                    if(i % 3 == 0)
                    {
                        //i is currently on the parent sheet index to an item 

                        if (!completedItems[pair.Key][i / 3])
                        {
                            //We have not put this item into the community center
                            bool missingItemFlag = true;
                            string key = MakeCommonKey(pair.Key);
                            if (items.ContainsKey(key))
                            {
                                SObject dataItem = MakeSObject(pair.Value[i], pair.Value[i + 1], pair.Value[i + 2]);
                                if (EquivalentEnough(items[key], dataItem, true)) //doesn't check for stack size or quality yet
                                {
                                    //also not sure how it handles wine
                                    missingItemFlag = false;
                                }
                            }
                            //foreach (SObject ownedItem in items.Values)
                            //{
                            //    SObject dataItem = MakeSObject(pair.Value[i], pair.Value[i + 1], pair.Value[i + 2]);
                            //    if(EquivalentEnough(ownedItem, dataItem, true)) //doesn't check for stack size or quality yet
                            //    {
                            //        //also not sure how it handles wine
                            //        missingItemFlag = false;
                            //    }
                            //}
                            if (CheckGrowingCrops(pair.Value[i]))
                            {
                                //The player will have the item before the end of the season
                                missingItemFlag = false;
                            }
                            if (missingItemFlag)
                            {
                                //Let's make two lists, one general missing items from cc and one missing items from cc that are specific to season
                                if((data.ContainsKey(pair.Value[i]) || stock.availableStock.Contains(pair.Value[i])) && !ccData.ContainsKey(pair.Value[i]) )
                                {
                                    //this is specific to the season
                                    SObject missingItem = new SObject(pair.Value[i], pair.Value[i + 1], false, -1, pair.Value[i + 2]);
                                    specificMissingCCItems.Add(missingItem);
                                }
                                if (ccData.ContainsKey(pair.Value[i]) || stock.availableStock.Contains(pair.Value[i]))
                                {
                                    SObject missingItem = new SObject(pair.Value[i], pair.Value[i+1], false, -1, pair.Value[i+2]);
                                    generalMissingCCItems.Add(missingItem);
                                }

                            }
                        }                        
                    }
                }
                
            }
            result.Add(specificMissingCCItems);
            result.Add(generalMissingCCItems);
            return result;
        }

        private List<SObject> WhatIsMissing(Dictionary<int, string> data)
        {
            ModConfig modConfig = Helper.ReadConfig<ModConfig>();
            int amount = modConfig.amount;
            Dictionary<string, SObject> items = condensedItems;
            //Currently will not check for fruit tree items, I'll probably add that later
            List<SObject> result = new List<SObject>();
            foreach (KeyValuePair<int, string> pair in data)
            {
                bool missingItemFlag = true;
                string key = CheckForAnyQuality(pair.Key);
                if (!key.Equals("NoKeys"))
                {
                    SObject dataItem = MakeSObject(pair.Key, amount);
                    if (EquivalentEnough(items[key], dataItem, false))
                    {
                        //The player has the item
                        missingItemFlag = false;
                    }
                }
                //foreach (SObject item in items.Values)
                //{
                //    //Due to how the data is stored right now itd be really hard to check for x amount of an item for all its different qualities so
                //    //leaving it for now
                //    SObject dataItem = MakeSObject(pair, amount);
                //    if (EquivalentEnough(item, dataItem, false))
                //    {
                //        //The player has the item
                //        missingItemFlag = false;
                //    }
                //}
                if (CheckGrowingCrops(pair.Key))
                {
                    //The player will have the item before the end of the season
                    missingItemFlag = false;
                }
                if (missingItemFlag)
                {
                    //add it to the list of missing items
                    if(!key.Equals("NoKeys"))
                    {
                        SObject missingItem = new SObject(pair.Key, amount - totalAmountOf(pair.Key));
                        result.Add(missingItem);
                    }
                    else
                    {
                        SObject missingItem = new SObject(pair.Key, amount);
                        result.Add(missingItem);
                    }
                    
                   
                }
            }


            foreach (int key in stock.availableStock)
            {
                bool missingItemFlag = true;
                string playerKey = CheckForAnyQuality(key);
                if (!playerKey.Equals("NoKeys"))
                {
                    SObject dataItem = MakeSObject(key, amount);
                    if (EquivalentEnough(items[playerKey], dataItem, false))
                    {
                        //The player has the item
                        missingItemFlag = false;
                    }
                }
                //foreach (SObject item in items.Values)
                //{
                //    //Due to how the data is stored right now itd be really hard to check for x amount of an item for all its different qualities so
                //    //leaving it for now
                //    SObject dataItem = MakeSObject(key, amount);
                //    if (EquivalentEnough(item, dataItem, false))
                //    {
                //        //The player has the item
                //        missingItemFlag = false;
                //    }
                //}
                if (CheckGrowingCrops(key))
                {
                    //The player will have the item before the end of the season
                    missingItemFlag = false;
                }
                if (missingItemFlag)
                {
                    //add it to the list of missing items
                    if (!playerKey.Equals("NoKeys"))
                    {
                        SObject missingItem = new SObject(key, amount - totalAmountOf(key));
                        result.Add(missingItem);
                    }
                    else
                    {
                        SObject missingItem = new SObject(key, amount);
                        result.Add(missingItem);
                    }


                }
            }

            return result;
        }

        private bool CheckGrowingCrops(int key)
        {
            ModConfig modConfig = Helper.ReadConfig<ModConfig>();
            bool checkGrowingCrops = modConfig.checkGrowingCrops;
            if (checkGrowingCrops)
            {
                IEnumerable<Crop> crops = GetCrops();
                foreach (Crop crop in crops)
                {
                    //there is another seedindex i think if its a foragable item - check it later
                    int index = crop.indexOfHarvest.Value;
                    if (index == key)
                    {
                        int totalDaysNeeded = 0; //not sure about the crop.currentPhase.Value - 1
                                                                                                                                         //not sure how crop.currentPhase is treated (starts at 1 or 0) need a test game with different crops 
                        for (int i = crop.currentPhase.Value; i < crop.phaseDays.Count - 1; ++i) //crop.phaseDays has a garbage number at the end (99999)
                        {
                            if(i == crop.currentPhase.Value)
                            {
                                totalDaysNeeded = crop.phaseDays.ElementAt<int>(crop.currentPhase.Value) - crop.dayOfCurrentPhase.Value;
                            }
                            else
                            {
                                //I'll assume that the player waters their crops because well everyone does
                                totalDaysNeeded += crop.phaseDays.ElementAt<int>(i);
                            }
                           
                        }

                        if (Game1.Date.DayOfMonth + totalDaysNeeded < 29)
                        {
                            //it'll finish growing before the end of the month
                            return true;
                        }
                        //else
                        //{
                        //What should I do if it won't finish in time? Keep it on the list or take it off?
                        //}
                    }
                }
            }
            return false;
        }
        private IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations
                .Concat(
                from location in Game1.locations.OfType<BuildableGameLocation>()
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
                );
        }

        private IEnumerable<Crop> GetCrops()
        {
            //Don't think this includes all growing spots, have to check later for something like the greenhouse
            List<Crop> result = new List<Crop>();

            foreach(GameLocation location in GetAllLocations())
            {
                var terrain = location.terrainFeatures.FieldDict;
                foreach (KeyValuePair<Microsoft.Xna.Framework.Vector2, Netcode.NetRef<TerrainFeature>> pair in terrain)
                {
                    if (pair.Value.Value is HoeDirt hoeDirt)
                    {
                        if (hoeDirt.crop != null)
                        {
                            result.Add(hoeDirt.crop);
                        }
                    }
                }
            }

            
            return result;
        }

        private int ConvertCropToSeedIndex(int cropKey)
        {
            CropConversion convert = new CropConversion();
            return convert.CropToSeedIndex(cropKey);
        }

        private bool EquivalentEnough(SObject item, SObject obj, bool checkCCItem)
        {
            if (checkCCItem)
            {
               return PerformActualEquivalentCheck(item, obj, checkCCItem);
            }
            //Otherwise count the total amount we have of the item (all qualities)
            
            return PerformActualEquivalentCheck(new SObject(item.ParentSheetIndex, totalAmountOf(item.ParentSheetIndex)), obj, false);
            
        }

        private int totalAmountOf(int key)
        {
            string commonKey = MakeCommonKey(key);
            string silverKey = MakeSilverKey(key);
            string goldKey = MakeGoldKey(key);
            string iridiumKey = MakeIridiumKey(key);
            string[] keys = new string[4] { commonKey, silverKey, goldKey, iridiumKey };

            int amount = 0;

            for (int i = 0; i < 4; ++i)
            {
                if (condensedItems.ContainsKey(keys[i]))
                {
                    amount += condensedItems[keys[i]].Stack;
                }
            }
            return amount;
        }
        private bool PerformActualEquivalentCheck(SObject item, SObject obj, bool checkCCItem)
        {
            if (checkCCItem) //does this work for something like an iron bar?
            {
                if (item.ParentSheetIndex == obj.ParentSheetIndex && item.Stack >= obj.Stack && item.Quality == obj.Quality)
                {
                    return true;
                }
            }
            else
            {
                if (item.ParentSheetIndex == obj.ParentSheetIndex && item.Stack >= obj.Stack)
                {
                    return true;
                }
            }
            return false;
        }
        private Dictionary<string, SObject> CondensePlayerItems()
        {
            //the key is int[2] where int[0]=parentSheetIndex int[1]=quality
            Dictionary<string, SObject> result = new Dictionary<string, SObject>();
            
            foreach(Item item in playerItems)
            {
                if (item is SObject)
                {
                    SObject obj = (SObject)item;

                    string key = String.Concat(Convert.ToString(obj.ParentSheetIndex), " ", Convert.ToString(obj.Quality));
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, obj);
                    }
                    else
                    {
                        SObject currentObj = result[key];

                        result.Remove(key);

                        result.Add(key, MakeSObject(Convert.ToInt32(key.Split(' ')[0]), obj.Stack + currentObj.Stack));
                    }
                }
                else
                {
                    //don't add it (things like pickaxe, I don't care about that sort of stuff anyways for this mod
                }
            }
            return result;
        }

        private SObject MakeSObject(KeyValuePair<int, int[]> pair, int startIndex) //for the community center items
        {
            return new SObject(pair.Value[startIndex], pair.Value[startIndex + 1], false, -1, pair.Value[startIndex + 2]);
        }

        private SObject MakeSObject(KeyValuePair<int, string> pair, int amount)
        {
            return MakeSObject(pair.Key, amount);
        }

        private SObject MakeSObject(int parentSheetIndex, int howMany, int quality)
        {
            return new SObject(parentSheetIndex, howMany, false, -1, quality);
        }

        private SObject MakeSObject(int parentSheetIndex, int howMany)
        {
            return new SObject(parentSheetIndex, howMany, false, -1, 0);
        }

        private string MakeCommonKey(int key)
        {
            return String.Concat(Convert.ToString(key), " ", "0");
        }

        private string MakeSilverKey(int key)
        {
            return String.Concat(Convert.ToString(key), " ", "1");
        }

        private string MakeGoldKey(int key)
        {
            return String.Concat(Convert.ToString(key), " ", "2");
        }

        private string MakeIridiumKey(int key)
        {
            return String.Concat(Convert.ToString(key), " ", "3");
        }

        public string CheckForAnyQuality(int key)
        {
            string commonKey = MakeCommonKey(key);
            string silverKey = MakeSilverKey(key);
            string goldKey = MakeGoldKey(key);
            string iridiumKey = MakeIridiumKey(key);
            string[] keys = new string[4] { commonKey, silverKey, goldKey, iridiumKey };
            string aValidKey = "NoKeys";

            for(int i = 0; i < 4; ++i)
            {
                if (condensedItems.ContainsKey(keys[i]))
                {
                    aValidKey = keys[i];
                }
            }

            return aValidKey;
        }
    }
}
