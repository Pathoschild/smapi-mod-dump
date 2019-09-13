using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatAreYouMissingOriginal
{
    class CommonCommunityCenterItems : SpecificItems
    {
        public CommonCommunityCenterItems(bool showItemsFromCurrentSeasonButInLockedPlaces,
            bool showEveryItemFromCurrentSeason,
            bool showAllFish,
            bool showCommonCommunityCenterItems,
            bool showFruitTreesInPreviousSeason,
            bool checkGrowingCrops,
            bool onlyShowWhatCanBeGrownBeforeEndOfSeason)
            : base(showItemsFromCurrentSeasonButInLockedPlaces,
                showEveryItemFromCurrentSeason,
                showAllFish,
                showCommonCommunityCenterItems,
                showFruitTreesInPreviousSeason,
                checkGrowingCrops,
                onlyShowWhatCanBeGrownBeforeEndOfSeason)
        {
            fileName = "commonCommunityCenterItems.json";
        }

        public override void RemoveMushrooms()
        {
            //Don't remove mushrooms
        }

        public override void RemoveTreeFruits()
        {
            //Dont' remove fruits
        }

        public override void UpdateData()
        {
            if (showCommonCommunityCenterItems)
            {
                //Adding Community Center Items that aren't in the derived classes
                GeneralAdd(Constants.WOOD, "Wood/2/-300/Basic -16/Wood/A sturdy, yet flexible plant material with a wide variety of uses.");
                GeneralAdd(Constants.STONE, "Stone/2/-300/Basic -16/Stone/A common material with many uses in crafting and building.");
                GeneralAdd(Constants.HARDWOOD, "Hardwood/15/-300/Basic -16/Hardwood/A special kind of wood with superior strength and beauty.");
                //SHOULD I ADD SOME SORT OF CHECK TO ENSURE DESERT IS UNLOCKED
                if(UnlockedDesert() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.COCONUT))
                {
                    GeneralAdd(Constants.COCONUT, "Coconut/100/-300/Basic -79/Coconut/A seed of the coconut palm. It has many culinary uses.");
                }
                if (UnlockedDesert() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.CACTUS_FRUIT))
                {
                    GeneralAdd(Constants.CACTUS_FRUIT, "Cactus Fruit/75/30/Basic -79/Cactus Fruit/The sweet fruit of the prickly pear cactus.");
                }
                GeneralAdd(Constants.CAVE_CARROT, "Cave Carrot/25/12/Basic -81/Cave Carrot/A starchy snack found in caves. It helps miners work longer.");
                GeneralAdd(Constants.RED_MUSHROOM, "Red Mushroom/75/-20/Basic -81/Red Mushroom/A spotted mushroom sometimes found in caves.");
                GeneralAdd(Constants.PURPLE_MUSHROOM, "Purple Mushroom/250/50/Basic -81/Purple Mushroom/A rare mushroom found deep in caves.");
                GeneralAdd(Constants.MAPLE_SYRUP, "Maple Syrup/200/20/Basic -27/Maple Syrup/A sweet syrup with a unique flavor./drink/0 0 0 0 0 0 0 0 0 0 0/0");
                GeneralAdd(Constants.OAK_RESIN, "Oak Resin/150/-300/Basic -27/Oak Resin/A sticky, fragrant substance derived from oak sap.");
                GeneralAdd(Constants.PINE_TAR, "Pine Tar/100/-300/Basic -27/Pine Tar/A pungent substance derived from pine sap.");

                GeneralAdd(Constants.LARGE_MILK, "Large Milk/190/20/Basic -6/Large Milk/A large jug of cow's milk./drink/0 0 0 0 0 0 0 0 0 0 0/0");
                GeneralAdd(Constants.LARGE_BROWN_EGG, "Large Egg/95/15/Basic -5/Large Egg/It's an uncommonly large brown egg!");
                GeneralAdd(Constants.LARGE_WHITE_EGG, "Large Egg/95/15/Basic -5/Large Egg/It's an uncommonly large white egg!");
                GeneralAdd(Constants.LARGE_GOAT_MILK, "L. Goat Milk/345/35/Basic -6/L. Goat Milk/A gallon of creamy goat's milk./drink/0 0 0 0 0 0 0 0 0 0 0/0");
                GeneralAdd(Constants.WOOL, "Wool/340/-300/Basic -18/Wool/Soft, fluffy wool.");
                GeneralAdd(Constants.DUCK_EGG, "Duck Egg/95/15/Basic -5/Duck Egg/It's still warm.");

                GeneralAdd(Constants.TRUFFLE_OIL, "Truffle Oil/1065/15/Basic -26/Truffle Oil/A gourmet cooking ingredient./drink/0 0 0 0 0 0 0 0 0 0 0/0");
                GeneralAdd(Constants.CLOTH, "Cloth/470/-300/Basic -26/Cloth/A bolt of fine wool cloth.");
                GeneralAdd(Constants.GOAT_CHEESE, "Goat Cheese/375/50/Basic -26/Goat Cheese/Soft cheese made from goat's milk.");
                GeneralAdd(Constants.CHEESE, "Cheese/200/50/Basic -26/Cheese/It's your basic cheese.");
                GeneralAdd(Constants.HONEY, "Honey/100/-300/Basic -26/Honey/It's a sweet syrup produced by bees.");
                GeneralAdd(Constants.JELLY, "Jelly/160/-300/Basic -26/Jelly/Gooey.");
                //if (UnlockedBatCave())
                //{
                //    AddTreeFruits();
                //}

                AddFish(Constants.LARGEMOUTH_BASS, "Largemouth Bass/100/15/Fish -4/Largemouth Bass/A popular fish that lives in lakes./Day^Spring Summer Fall Winter");
                AddFish(Constants.CARP, "Carp/30/5/Fish -4/Carp/A common pond fish./Day Night^Spring Summer Fall");
                AddFish(Constants.BULLHEAD, "Bullhead/75/10/Fish -4/Bullhead/A relative of the catfish that eats a variety of foods off the lake bottom./Day^Spring Summer");

                AddFish(Constants.BREAM, "Bream/45/5/Fish -4/Bream/A fairly common river fish that becomes active at night./Night^Spring Summer Fall Winter");

                GeneralAdd(Constants.LOBSTER, "Lobster/120/-300/Fish -4/Lobster/A large ocean-dwelling crustacean with a strong tail./Day^Spring Summer");
                GeneralAdd(Constants.CRAYFISH, "Crayfish/75/-300/Fish -4/Crayfish/A small freshwater relative of the lobster./Day^Spring Summer");
                GeneralAdd(Constants.CRAB, "Crab/100/-300/Fish -4/Crab/A marine crustacean with two powerful pincers./Day^Spring Summer");
                GeneralAdd(Constants.COCKLE, "Cockle/50/-300/Fish -4/Cockle/A common saltwater clam./Day^Spring Summer");
                GeneralAdd(Constants.MUSSEL, "Mussel/30/-300/Fish -4/Mussel/A common bivalve that often lives in clusters./Day^Spring Summer");
                GeneralAdd(Constants.SHRIMP, "Shrimp/60/-300/Fish -4/Shrimp/A scavenger that feeds off the ocean floor. Widely prized for its meat./Day^Spring Summer");
                GeneralAdd(Constants.SNAIL, "Snail/65/-300/Fish -4/Snail/A wide-ranging mollusc that lives in a spiral shell./Day^Spring Summer");
                GeneralAdd(Constants.PERIWINKLE, "Periwinkle/20/-300/Fish -4/Periwinkle/A tiny freshwater snail that lives in a blue shell./Day^Spring Summer");
                GeneralAdd(Constants.OYSTER, "Oyster/40/-300/Fish -4/Oyster/Constantly filters water to find food. In the process, it removes dangerous toxins from the environment./Day^Spring Summer");
                GeneralAdd(Constants.CLAM, "Clam/50/-300/Basic -23/Clam/Someone lived here once.");

                AddFish(Constants.GHOSTFISH, "Ghostfish/45/15/Fish -4/Ghostfish/A pale, blind fish found in underground lakes./Day Night^Spring Summer Fall Winter");

                if(UnlockedDesert() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.SANDFISH))
                {
                    AddFish(Constants.SANDFISH, "Sandfish/75/5/Fish -4/Sandfish/It tries to hide using camouflage./Day Night^Spring Summer Fall Winter");
                }
                
                if(UnlockedSecretWoods() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.WOODSKIP))
                {
                    AddFish(Constants.WOODSKIP, "Woodskip/75/10/Fish -4/Woodskip/A very sensitive fish that can only live in pools deep in the forest./Day^Spring Summer");
                }
                

                GeneralAdd(Constants.COPPER_BAR, "Copper Bar/60/-300/Basic -15/Copper Bar/A bar of pure copper.");
                GeneralAdd(Constants.IRON_BAR, "Iron Bar/120/-300/Basic -15/Iron Bar/A bar of pure iron.");
                GeneralAdd(Constants.GOLD_BAR, "Gold Bar/250/-300/Basic -15/Gold Bar/A bar of pure gold.");

                GeneralAdd(Constants.QUARTZ, "Quartz/25/-300/Minerals -2/Quartz/A clear crystal commonly found in caves and mines.");
                GeneralAdd(Constants.EARTH_CRYSTAL, "Earth Crystal/50/-300/Minerals -2/Earth Crystal/A resinous substance found near the surface.");
                GeneralAdd(Constants.FROZEN_TEAR, "Frozen Tear/75/-300/Minerals -2/Frozen Tear/A crystal fabled to be the frozen tears of a yeti.");
                GeneralAdd(Constants.FIRE_QUARTZ, "Fire Quartz/100/-300/Minerals -2/Fire Quartz/A glowing red crystal commonly found near hot lava.");

                GeneralAdd(Constants.SLIME, "Slime/5/-300/Basic -28/Slime/A shimmering, gelatinous glob with no smell.");
                GeneralAdd(Constants.BAT_WING, "Bat Wing/15/-300/Basic -28/Bat Wing/The material is surprisingly delicate.");
                GeneralAdd(Constants.SOLAR_ESSENCE, "Solar Essence/40/-300/Basic -28/Solar Essence/The glowing face is warm to the touch.");
                GeneralAdd(Constants.VOID_ESSENCE, "Void Essence/50/-300/Basic -28/Void Essence/It's quivering with dark energy.");

                GeneralAdd(Constants.TRUFFLE, "Truffle/625/5/Basic -17/Truffle/A gourmet type of mushroom with a unique taste.");
                GeneralAdd(Constants.MAKI_ROLL, "Maki Roll/220/40/Cooking -7/Maki Roll/Fish and rice wrapped in seaweed./food/0 0 0 0 0 0 0 0 0 0 0/0");
                GeneralAdd(Constants.FRIED_EGG, "Fried Egg/35/20/Cooking -7/Fried Egg/Sunny-side up./food/0 0 0 0 0 0 0 0 0 0 0/0");

                //SHOULD I ADD SOME CHECK TO SEE IF OTHER BEACH UNLOCKED?
                if(UnlockedSecondBeach() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.SEA_URCHIN))
                {
                    GeneralAdd(Constants.SEA_URCHIN, "Sea Urchin/160/-300/Basic -23/Sea Urchin/A slow-moving, spiny creature that some consider a delicacy.");
                }
                
                GeneralAdd(Constants.DUCK_FEATHER, "Duck Feather/125/-300/Basic -18/Duck Feather/It's so colorful.");
                GeneralAdd(Constants.AQUAMARINE, "Aquamarine/180/-300/Minerals -2/Aquamarine/A shimmery blue-green gem .");

                AddFish(Constants.CHUB, "Chub/50/10/Fish -4/Chub/A common freshwater fish known for its voracious appetite./Day^Spring Summer");
                GeneralAdd(Constants.FROZEN_GEODE, "Frozen Geode/100/-300/Basic/Frozen Geode/A blacksmith can break this open for you./541 544 545 546 550 551 559 560 561 564 567 572 573 577 123");

                GeneralAdd(Constants.HAY, "Hay/0/-300/Basic/Hay/Dried grass used as animal food.");

                GeneralAdd(Constants.WINE, "Wine/400/20/Basic -26/Wine/Drink in moderation./drink/0 0 0 0 0 0 0 0 0 0 0/0");
                GeneralAdd(Constants.RABBITS_FOOT, "Rabbit's Foot/565/-300/Basic -18/Rabbit's Foot/Some say it's lucky.");
            }
        }
    }
}
