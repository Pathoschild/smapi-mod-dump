using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class CommonCCItems : Items, ISpecificItems
    {

        public CommonCCItems() : base() { }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        protected override void AddItems()
        {
            AddCommonCraftsRoom();
            AddCommonPantry();
            AddCommonFishTank();
            AddBoilerRoom();
            AddCommonBulletinBoard();
        }

        private void AddCommonCraftsRoom()
        {
            AddCommonSummerForaging();
            AddCommonFallForagaing();
            AddCommonWinterForagaing();
            AddConstruction();
            AddExoticForaging();
        }
        private void AddCommonPantry()
        {
            AddAnimal();
            AddCommonArtisan();
        }

        private void AddCommonFishTank()
        {
            AddCommonLakeFish();
            AddCommonNightFish();
            AddCrabPot();
            AddCommonSpecialtyFish();
        }

        private void AddBoilerRoom()
        {
            AddBlacksmiths();
            AddGeologists();
            AddAdventurers();
        }

        private void AddCommonBulletinBoard()
        {
            AddCommonChefs();
            AddCommonDye();
            AddCommonFieldResearch();
            AddCommonFoder();
            AddCommonEnchanters();
        }

        private void AddCommonSummerForaging()
        {
            if (Utilities.IsBatCaveUnlocked())
            {
                AddOneCommonObject(Constants.SPICE_BERRY);
            }
        }

        private void AddCommonFallForagaing()
        {
            if (Utilities.IsMushroomCaveUnlocked())
            {
                AddOneCommonObject(Constants.COMMON_MUSHROOM);
            }
            else if (Utilities.IsBatCaveUnlocked())
            {
                AddOneCommonObject(Constants.WILD_PLUM);
                AddOneCommonObject(Constants.BLACKBERRY);
            }
        }

        private void AddCommonWinterForagaing()
        {
            AddOneCommonObject(Constants.WINTER_ROOT);
            AddOneCommonObject(Constants.CRYSTAL_FRUIT);
        }

        private void AddConstruction()
        {
            AddCommonObject(Constants.WOOD, 99);
            AddCommonObject(Constants.STONE, 99);
            AddCommonObject(Constants.HARDWOOD, 10);
        }

        private void AddExoticForaging()
        {
            if(Utilities.IsDesertUnlocked() || Utilities.CheckMerchantForItemAndSeed(Constants.COCONUT) || Config.ShowItemsFromLockedPlaces)
            {
                AddOneCommonObject(Constants.COCONUT);
            }
            if(Utilities.IsDesertUnlocked() || Utilities.CheckMerchantForItemAndSeed(Constants.CACTUS_FRUIT) || Config.ShowItemsFromLockedPlaces)
            {
                AddOneCommonObject(Constants.CACTUS_FRUIT);
            }

            AddOneCommonObject(Constants.CAVE_CARROT);
            AddOneCommonObject(Constants.RED_MUSHROOM);
            AddOneCommonObject(Constants.PURPLE_MUSHROOM);
            AddOneCommonObject(Constants.MAPLE_SYRUP);
            AddOneCommonObject(Constants.OAK_RESIN);
            AddOneCommonObject(Constants.PINE_TAR);

            if (Utilities.IsMushroomCaveUnlocked())
            {
                AddOneCommonObject(Constants.MOREL);
            }
        }

        private void AddAnimal()
        {
            AddOneCommonObject(Constants.LARGE_MILK);
            AddOneCommonObject(Constants.LARGE_BROWN_EGG);
            AddOneCommonObject(Constants.LARGE_WHITE_EGG);
            AddOneCommonObject(Constants.LARGE_GOAT_MILK);
            AddOneCommonObject(Constants.WOOL);
            AddOneCommonObject(Constants.DUCK_EGG);
        }

       

        private void AddCommonArtisan()
        {
            AddOneCommonObject(Constants.TRUFFLE_OIL);
            AddOneCommonObject(Constants.CLOTH);
            AddOneCommonObject(Constants.GOAT_CHEESE);
            AddOneCommonObject(Constants.CHEESE);
            AddOneCommonObject(Constants.JELLY);
            if (Utilities.IsBatCaveUnlocked())
            {
                AddTreeFruits();
            }
        }

        private void AddTreeFruits()
        {
            AddOneCommonObject(Constants.APPLE);
            AddOneCommonObject(Constants.APRICOT);

            AddOneCommonObject(Constants.ORANGE);
            AddOneCommonObject(Constants.PEACH);

            AddOneCommonObject(Constants.POMEGRANATE);
            AddOneCommonObject(Constants.CHERRY);
        }

        private void AddCommonLakeFish()
        {
            AddOneCommonObject(Constants.LARGEMOUTH_BASS);
            AddOneCommonObject(Constants.CARP);
            AddOneCommonObject(Constants.BULLHEAD);
        }

        private void AddCommonNightFish()
        {
            AddOneCommonObject(Constants.BREAM);
        }

        private void AddCrabPot()
        {
            AddOneCommonObject(Constants.LOBSTER);
            AddOneCommonObject(Constants.CRAYFISH);
            AddOneCommonObject(Constants.CRAB);
            AddOneCommonObject(Constants.COCKLE);
            AddOneCommonObject(Constants.MUSSEL);
            AddOneCommonObject(Constants.SHRIMP);
            AddOneCommonObject(Constants.SNAIL);
            AddOneCommonObject(Constants.PERIWINKLE);
            AddOneCommonObject(Constants.OYSTER);
            AddOneCommonObject(Constants.CLAM);
        }

        private void AddCommonSpecialtyFish()
        {
            AddOneCommonObject(Constants.GHOSTFISH);
            
            if(Utilities.IsDesertUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.CheckMerchantForItemAndSeed(Constants.SANDFISH))
            {
                AddOneCommonObject(Constants.SANDFISH);
            }

            if(Utilities.IsSecretWoodsUnlocked() || Config.ShowItemsFromLockedPlaces || Utilities.CheckMerchantForItemAndSeed(Constants.WOODSKIP))
            {
                AddOneCommonObject(Constants.WOODSKIP);
            }
        }

        private void AddBlacksmiths()
        {
            AddOneCommonObject(Constants.COPPER_BAR);
            AddOneCommonObject(Constants.IRON_BAR);
            AddOneCommonObject(Constants.GOLD_BAR);
        }

        private void AddGeologists()
        {
            AddOneCommonObject(Constants.QUARTZ);
            AddOneCommonObject(Constants.EARTH_CRYSTAL);
            AddOneCommonObject(Constants.FROZEN_TEAR);
            AddOneCommonObject(Constants.FIRE_QUARTZ);
        }

        private void AddAdventurers()
        {
            AddCommonObject(Constants.SLIME, 99);
            AddCommonObject(Constants.BAT_WING, 10);
            AddOneCommonObject(Constants.SOLAR_ESSENCE);
            AddOneCommonObject(Constants.VOID_ESSENCE);
        }

        private void AddCommonChefs()
        {
            AddOneCommonObject(Constants.MAPLE_SYRUP);
            AddOneCommonObject(Constants.MAKI_ROLL);
            AddOneCommonObject(Constants.FRIED_EGG);
        }

        private void AddCommonDye()
        {
            AddOneCommonObject(Constants.RED_MUSHROOM);
            if (Utilities.IsSecondBeachUnlocked())
            {
                AddOneCommonObject(Constants.SEA_URCHIN);
            }

            AddOneCommonObject(Constants.DUCK_FEATHER);
            AddOneCommonObject(Constants.AQUAMARINE);
        }

        private void AddCommonFieldResearch()
        {
            AddOneCommonObject(Constants.PURPLE_MUSHROOM);
            AddOneCommonObject(Constants.CHUB);
            AddOneCommonObject(Constants.FROZEN_GEODE);
        }

        private void AddCommonFoder()
        {
            AddCommonObject(Constants.HAY, 10);

            if (Utilities.IsBatCaveUnlocked())
            {
                AddCommonObject(Constants.APPLE, 3);
            }
        }

        private void AddCommonEnchanters()
        {
            AddOneCommonObject(Constants.OAK_RESIN);
            AddOneCommonObject(Constants.WINE);
            AddOneCommonObject(Constants.RABBITS_FOOT);
            if (Utilities.IsBatCaveUnlocked())
            {
                AddOneCommonObject(Constants.POMEGRANATE);
            }
        }
    }
}
