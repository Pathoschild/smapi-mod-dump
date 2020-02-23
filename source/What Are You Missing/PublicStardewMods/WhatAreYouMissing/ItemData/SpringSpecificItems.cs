using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{

    public interface ISpecificItems
    {
        Dictionary<int, SObject> GetItems();
    }
    public class SpringSpecificItems : Items, ISpecificItems
    {
        public SpringSpecificItems() : base(){ }

        protected override void AddItems()
        {
            AddCrops();
            AddForagables();
            AddFruitTrees("spring");
            AddFish();
            //Might need to throw in seeds here later if I want to check if they planted something so I don't need to remind them to plant it
            //Should check and only reccomend to plant things if there is enough time for it to grow before the season ends
            //may also want to throw in exclusive items like scarcrows or whatnot
        }

        private void AddForagables()
        {
            AddOneCommonObject(Constants.WILD_HORSERADISH);
            AddOneCommonObject(Constants.DAFFODIL);
            AddOneCommonObject(Constants.LEEK);
            AddOneCommonObject(Constants.DANDELION);
            AddOneCommonObject(Constants.SPRING_ONION);
            AddOneCommonObject(Constants.TRUFFLE);

            if (Utilities.IsSecretWoodsUnlocked() || Config.ShowItemsFromLockedPlaces || Game1.whichFarm == (int)FarmTypes.Forest || Utilities.CheckMerchantForItemAndSeed(Constants.MOREL))
            {
                AddOneCommonObject(Constants.MOREL);
            }

            if(Utilities.IsSecretWoodsUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.CheckMerchantForItemAndSeed(Constants.COMMON_MUSHROOM))
            {
                AddOneCommonObject(Constants.COMMON_MUSHROOM);
            }

            if((Game1.Date.DayOfMonth > 14 &&  Game1.Date.DayOfMonth < 19) || Utilities.IsMerchantAvailiableAndHasItem(Constants.SALMONBERRY))
            {
                AddOneCommonObject(Constants.SALMONBERRY);
            }
        }

        private void AddFish()
        {
            //The Legend requires level 10 fishing
            AddNormalSeasonalFish("spring");
            if (Config.ShowAllFishFromCurrentSeason || Game1.player.getEffectiveSkillLevel(1) == 10)
            {
                AddFish(Constants.LEGEND);
            }
        }

        private void AddCrops()
        {
            AddCrops("spring");

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.GARLIC))
            {
                AddOneCommonObject(Constants.GARLIC);
            }
            else if (Game1.Date.Year > 1 || Utilities.IsMerchantAvailiableAndHasItem(Constants.GARLIC_SEEDS))
            {
                ManuallyAddCrop(Constants.GARLIC);
            }

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.RHUBARB))
            {
                AddOneCommonObject(Constants.RHUBARB);
            }
            else if (Utilities.IsDesertUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.IsMerchantAvailiableAndHasItem(Constants.RHUBARB_SEEDS))
            {
                ManuallyAddCrop(Constants.RHUBARB);
            }

            if ((Game1.Date.Year == 1 && Game1.Date.DayOfMonth >= 13) || Utilities.IsMerchantAvailiableAndHasItem(Constants.STRAWBERRY))
            {
                AddOneCommonObject(Constants.STRAWBERRY);
            }
            else if (Game1.Date.Year > 1)
            {
                ManuallyAddCrop(Constants.STRAWBERRY);
            }
        }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }
    }
}
