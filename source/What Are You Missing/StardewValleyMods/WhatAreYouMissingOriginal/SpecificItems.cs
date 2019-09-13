using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using SObject = StardewValley.Object;
using StardewValley.Locations;

namespace WhatAreYouMissingOriginal
{
    abstract class SpecificItems
    {
        public String fileName;
        public Dictionary<int, string> content { get; set; }
        public bool showItemsFromCurrentSeasonButInLockedPlaces;
        public bool showEveryItemFromCurrentSeason;
        public bool showAllFish;
        public bool showCommonCommunityCenterItems;
        public bool showFruitTreesInPreviousSeason;
        public bool checkGrowingCrops;
        public bool onlyShowWhatCanBeGrownBeforeEndOfSeason;

        public SpecificItems()
        {

        }
        public SpecificItems(bool showItemsFromCurrentSeasonButInLockedPlaces,
            bool showEveryItemFromCurrentSeason,
            bool showAllFish,
            bool showCommonCommunityCenterItems,
            bool showFruitTreesInPreviousSeason,
            bool checkGrowingCrops,
            bool onlyShowWhatCanBeGrownBeforeEndOfSeason)
        {
            content = new Dictionary<int, string>();
            this.showItemsFromCurrentSeasonButInLockedPlaces = showItemsFromCurrentSeasonButInLockedPlaces;
            this.showEveryItemFromCurrentSeason = showEveryItemFromCurrentSeason;
            this.showAllFish = showAllFish;
            this.showCommonCommunityCenterItems = showCommonCommunityCenterItems;
            this.showFruitTreesInPreviousSeason = showFruitTreesInPreviousSeason;
            this.checkGrowingCrops = checkGrowingCrops;
            this.onlyShowWhatCanBeGrownBeforeEndOfSeason = onlyShowWhatCanBeGrownBeforeEndOfSeason;
            
            if(this.showEveryItemFromCurrentSeason == true)
            {
                this.showItemsFromCurrentSeasonButInLockedPlaces = true;
                this.showAllFish = true;
            }
            UpdateData(); //update before checking to remove items that won't be specific anymore
            //RemoveTreeFruits();
            //RemoveMushrooms(); //I'll leave it out for now 
        }

        abstract public void UpdateData();

        public bool DoesTravellingMerchantHaveIt(int parentSheetIndex)
        {
            DayOfWeek currentDay = Game1.Date.DayOfWeek;
            if (currentDay == DayOfWeek.Friday || currentDay == DayOfWeek.Sunday)
            {
                //this is how the source code generates the seed
                int seed = (int)((long)Game1.uniqueIDForThisGame + (long)Game1.stats.DaysPlayed);

                //initialize the stock
                Dictionary<Item, int[]> stock = Utility.getTravelingMerchantStock(seed);
                foreach(Item item in stock.Keys)
                {
                    if (item.ParentSheetIndex == parentSheetIndex)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }

        public bool UnlockedDesert()
        {
            return Game1.player.mailReceived.Contains("ccVault");
        }

        public bool UnlockedGreenHouse()
        {
            return Game1.player.mailReceived.Contains("ccPantry");
        }

        public bool UnlockedSecretWoods()
        {
            //Not 100% confident in this check
            return Game1.player.mailReceived.Contains("beenToWoods");
        }

        public bool UnlockedSecondBeach()
        {
            return new Beach().bridgeFixed.Value;
        }

        public bool UnlockedMushroomCave()
        {
            return Game1.player.caveChoice.Value == 2 ;
        }

        public bool UnlockedBatCave()
        {
            return Game1.player.caveChoice.Value == 1;
        }

        public bool EnoughTimeToGrowSeeds(int seed)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            if (data.ContainsKey(seed))
            {
                int[] daysInEachStage = new int[data[seed].Split('/')[0].Split(' ').Length];
                int totalDaysNeeded = 0;

                for (int i = 0; i < data[seed].Split('/')[0].Split(' ').Length; ++i)
                {
                    daysInEachStage[i] = Convert.ToInt32(data[seed].Split('/')[0].Split(' ')[i]);
                }

                for (int i = 0; i < daysInEachStage.Length; ++i)
                {
                    totalDaysNeeded += daysInEachStage[i];
                }

                if ((Game1.Date.DayOfMonth + totalDaysNeeded < 29) || UnlockedGreenHouse())
                {
                    //it'll finish growing before the end of the month
                    return true;
                }
            }
            return false;
        }

        public bool CheckMerchantForItemAndSeed(int item)
        {
            CropConversion cropConverter = new CropConversion();
            if(DoesTravellingMerchantHaveIt(item) || (DoesTravellingMerchantHaveIt(cropConverter.CropToSeedIndex(item)) && (EnoughTimeToGrowSeeds(cropConverter.CropToSeedIndex(item)) || UnlockedGreenHouse())))
            {
                return true;
            }
            return false;
        }

        public void AddCrop(int key, string description)
        {
            if (onlyShowWhatCanBeGrownBeforeEndOfSeason)
            {
                CropConversion cropConverter = new CropConversion();
                if (EnoughTimeToGrowSeeds(cropConverter.CropToSeedIndex(key)))
                {
                    GeneralAdd(key, description);
                }
            }
            else
            {
                GeneralAdd(key, description);
            }
            
        }

        public void Remove(int key)
        {
            if (content.ContainsKey(key))
            {
                content.Remove(key);
            }
        }

        public void AddFish(int key, string description)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            switch (data[key].Split('/')[7])
            {
                case "sunny":
                    if (!Game1.isRaining || showAllFish)
                    {
                        GeneralAdd(key, description);
                    }
                    break;
                case "rainy":
                    if (Game1.isRaining || showAllFish)
                    {
                        GeneralAdd(key, description);
                    }
                    break;
                case "both":
                    GeneralAdd(key, description);
                    break;
                default:
                    break;
            }
        }

        public void GeneralAdd(int key, string description)
        {
            if (!content.ContainsKey(key))
            {
                content.Add(key, description);
            }
        }

        public virtual void RemoveMushrooms() //since they won't be specific to a season anymore
        {
            if (UnlockedMushroomCave())
            {
                //GeneralAdd(Constants.MOREL, "Morel/150/8/Basic -81/Morel/Sought after for its unique nutty flavor.");
                //GeneralAdd(Constants.COMMON_MUSHROOM, "Common Mushroom/40/15/Basic -81/Common Mushroom/Slightly nutty, with good texture.");
                //GeneralAdd(Constants.RED_MUSHROOM, "Red Mushroom/75/-20/Basic -81/Red Mushroom/A spotted mushroom sometimes found in caves.");
                //GeneralAdd(Constants.CHANTERELLE, "Chanterelle/160/30/Basic -81/Chanterelle/A tasty mushroom with a fruity smell and slightly peppery flavor.");
                //GeneralAdd(Constants.PURPLE_MUSHROOM, "Purple Mushroom/250/50/Basic -81/Purple Mushroom/A rare mushroom found deep in caves.");

                Remove(Constants.MOREL);
                Remove(Constants.COMMON_MUSHROOM);
                Remove(Constants.RED_MUSHROOM);
                Remove(Constants.CHANTERELLE);
                Remove(Constants.PURPLE_MUSHROOM);
            }
        }

        public virtual void RemoveTreeFruits() //since they won't be specific to a season anymore
        {
            if (UnlockedBatCave())
            {
                //GeneralAdd(Constants.APPLE, "Apple/100/15/Basic -79/Apple/A crisp fruit used for juice and cider.");
                //GeneralAdd(Constants.POMEGRANATE, "Pomegranate/140/15/Basic -79/Pomegranate/Within the fruit are clusters of juicy seeds.");

                //GeneralAdd(Constants.APRICOT, "Apricot/50/15/Basic -79/Apricot/A tender little fruit with a rock-hard pit.");
                //GeneralAdd(Constants.CHEERY, "Cherry/80/15/Basic -79/Cherry/It's popular, and ripens sooner than most other fruits.");

                //GeneralAdd(Constants.ORANGE, "Orange/100/15/Basic -79/Orange/Juicy, tangy, and bursting with sweet summer aroma.");
                //GeneralAdd(Constants.PEACH, "Peach/140/15/Basic -79/Peach/It's almost fuzzy to the touch.");

                Remove(Constants.APPLE);
                Remove(Constants.POMEGRANATE);

                Remove(Constants.APRICOT);
                Remove(Constants.CHEERY);

                Remove(Constants.ORANGE);
                Remove(Constants.PEACH);
            }
        }

        public void AddTreeFruits()
        {
            if (UnlockedBatCave())
            {
                GeneralAdd(Constants.APPLE, "Apple/100/15/Basic -79/Apple/A crisp fruit used for juice and cider.");
                GeneralAdd(Constants.POMEGRANATE, "Pomegranate/140/15/Basic -79/Pomegranate/Within the fruit are clusters of juicy seeds.");

                GeneralAdd(Constants.APRICOT, "Apricot/50/15/Basic -79/Apricot/A tender little fruit with a rock-hard pit.");
                GeneralAdd(Constants.CHEERY, "Cherry/80/15/Basic -79/Cherry/It's popular, and ripens sooner than most other fruits.");

                GeneralAdd(Constants.ORANGE, "Orange/100/15/Basic -79/Orange/Juicy, tangy, and bursting with sweet summer aroma.");
                GeneralAdd(Constants.PEACH, "Peach/140/15/Basic -79/Peach/It's almost fuzzy to the touch.");
            }
        }

        
    }
}
