using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
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
            AddForagables();
            AddFish();
        }

        private void AddCrops()
        {
            AddCrop(Constants.AMARANTH);
            AddCrop(Constants.BOK_CHOY);
            AddCrop(Constants.FAIRY_ROSE);
            AddCrop(Constants.PUMPKIN);
            AddCrop(Constants.SUNFLOWER);
            AddCrop(Constants.SWEET_GEM_BERRY);
            AddCrop(Constants.WHEAT);
            AddCrop(Constants.YAM);
            AddCrop(Constants.CORN);
            AddCrop(Constants.CRANBERRIES);
            AddCrop(Constants.EGGPLANT);
            AddCrop(Constants.GRAPE);

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.ARTICHOKE))
            {
                AddOneCommonObject(Constants.ARTICHOKE);
            }
            else if (Game1.year > 1 || Utilities.IsMerchantAvailiableAndHasItem(Constants.ARTICHOKE_SEEDS))
            {
                AddCrop(Constants.ARTICHOKE);
            }

            if (Utilities.IsMerchantAvailiableAndHasItem(Constants.BEET))
            {
                AddOneCommonObject(Constants.BEET);
            }
            if (Utilities.IsDesertUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.IsMerchantAvailiableAndHasItem(Constants.BEET_SEEDS) )
            {
                AddCrop(Constants.BEET);
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
            AddFish(Constants.ANCHOVY);
            AddFish(Constants.SARDINE);
            AddFish(Constants.SMALLMOUTH_BASS);
            AddFish(Constants.SALMON);
            AddFish(Constants.WALLEYE);
            AddFish(Constants.CATFISH);
            AddFish(Constants.EEL);
            AddFish(Constants.RED_SNAPPER);
            AddFish(Constants.SEA_CUCUMBER);
            AddFish(Constants.SUPER_CUCUMBER);
            AddFish(Constants.TIGER_TROUT);
            AddFish(Constants.TILAPIA);
            AddFish(Constants.ALBACORE);
            AddFish(Constants.SHAD);

            if (Config.ShowAllFishFromCurrentSeason || (Game1.player.getEffectiveSkillLevel(1) > 2 && !Game1.player.fishCaught.ContainsKey(Constants.ANGLER)))
            {
                AddFish(Constants.ANGLER);
            }
        }
    }
}
