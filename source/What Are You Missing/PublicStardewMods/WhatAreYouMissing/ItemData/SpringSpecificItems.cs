using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
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
            AddFruitTrees();
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
            AddFish(Constants.ANCHOVY);
            AddFish(Constants.SMALLMOUTH_BASS);
            AddFish(Constants.CATFISH);
            AddFish(Constants.EEL);
            AddFish(Constants.SHAD);
            AddFish(Constants.SUNFISH);
            AddFish(Constants.HERRING);
            AddFish(Constants.SARDINE);
            AddFish(Constants.HALIBUT);
            //The Legend requires level 10 fishing
            if (Config.ShowAllFishFromCurrentSeason || Game1.player.getEffectiveSkillLevel(1) == 10)
            {
                AddFish(Constants.LEGEND);
            }
        }

        private void AddFruitTrees()
        {
            AddOneCommonObject(Constants.CHEERY);
            AddOneCommonObject(Constants.APRICOT);
        }

        private void AddCrops()
        {
            AddCrop(Constants.BLUE_JAZZ);
            AddCrop(Constants.CAULIFLOWER);
            AddCrop(Constants.KALE);
            AddCrop(Constants.PARSNIP);
            AddCrop(Constants.POTATO);
            AddCrop(Constants.TULIP);
            AddCrop(Constants.GREEN_BEAN);
            //The coffee bean can be purchased from the travelling merchant or found 
            //in the mines so I'll leave it due to mines and the fact it can grow in summer
            AddCrop(Constants.COFFEE_BEAN);

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.GARLIC))
            {
                AddOneCommonObject(Constants.GARLIC);
            }
            else if (Game1.Date.Year > 1 || Utilities.IsMerchantAvailiableAndHasItem(Constants.GARLIC_SEEDS))
            {
                AddCrop(Constants.GARLIC);
            }

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.RHUBARB))
            {
                AddOneCommonObject(Constants.RHUBARB);
            }
            else if (Utilities.IsDesertUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.IsMerchantAvailiableAndHasItem(Constants.RHUBARB_SEEDS))
            {
                AddCrop(Constants.RHUBARB);
            }

            if ((Game1.Date.Year == 1 && Game1.Date.DayOfMonth >= 13) || Utilities.IsMerchantAvailiableAndHasItem(Constants.STRAWBERRY))
            {
                AddOneCommonObject(Constants.STRAWBERRY);
            }
            else if (Game1.Date.Year > 1)
            {
                AddCrop(Constants.STRAWBERRY);
            }
        }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }
    }
}
