using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class FallSpecificItems : Items, ISpecificItems
    {
        public FallSpecificItems() : base() { }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        protected override void AddItems()
        {
            AddCrops();
            AddFruitTrees("fall");
            AddForagables();
            AddFish();
        }

        private void AddCrops()
        {
            AddCrops("fall");

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.ARTICHOKE))
            {
                AddOneCommonObject(Constants.ARTICHOKE);
            }
            else if (Game1.year > 1 || Utilities.IsMerchantAvailiableAndHasItem(Constants.ARTICHOKE_SEEDS))
            {
                ManuallyAddCrop(Constants.ARTICHOKE);
            }

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.BEET))
            {
                AddOneCommonObject(Constants.BEET);
            }
            if (Utilities.IsDesertUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.IsMerchantAvailiableAndHasItem(Constants.BEET_SEEDS) )
            {
                ManuallyAddCrop(Constants.BEET);
            }
        }

        private void AddForagables()
        {
            AddOneCommonObject(Constants.WILD_PLUM);
            AddOneCommonObject(Constants.HAZELNUT);
            AddOneCommonObject(Constants.BLACKBERRY);
            if (Utilities.IsSecretWoodsUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.IsMerchantAvailiableAndHasItem(Constants.CHANTERELLE))
            {
                AddOneCommonObject(Constants.CHANTERELLE);
            }
            AddOneCommonObject(Constants.COMMON_MUSHROOM);
            AddOneCommonObject(Constants.TRUFFLE);
        }

        private void AddFish()
        {
            AddNormalSeasonalFish("fall");
            if (Config.ShowAllFishFromCurrentSeason || (Game1.player.getEffectiveSkillLevel(1) > 2 && !Game1.player.fishCaught.ContainsKey(Constants.ANGLER)))
            {
                AddFish(Constants.ANGLER);
            }
        }
    }
}
