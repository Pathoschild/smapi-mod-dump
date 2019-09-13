using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace WhatAreYouMissingOriginal
{
    class SpringSpecifics : SpecificItems
    {
        public SpringSpecifics(bool showItemsFromCurrentSeasonButInLockedPlaces,
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
            fileName = "springSpecifics.json";
        }
        public override void UpdateData()
        {
            //The number is the index in the parent sheet, it will be used when comparing what the player has to what's available

            //Add Harvested Crops, only use Add function for crops
            AddCrop(Constants.BLUE_JAZZ, "Blue Jazz/50/18/Basic -80/Blue Jazz/The flower grows in a sphere to invite as many butterflies as possible.");
            AddCrop(Constants.CAULIFLOWER, "Cauliflower/175/30/Basic -75/Cauliflower/Valuable, but slow-growing. Despite its pale color, the florets are packed with nutrients.");
            //Garlic is only available in year 2 or from travelling merchant
            if (Game1.Date.Year > 1 || CheckMerchantForItemAndSeed(Constants.GARLIC) || showEveryItemFromCurrentSeason)
            {
                GeneralAdd(Constants.GARLIC, "Garlic/60/8/Basic -75/Garlic/Adds a wonderful zestiness to dishes. High quality garlic can be pretty spicy."); //use direct add due to check in check merchant for item and seed
            }
            AddCrop(Constants.KALE, "Kale/110/20/Basic -75/Kale/The waxy leaves are great in soups and stir frys.");
            AddCrop(Constants.PARSNIP, "Parsnip/35/10/Basic -75/Parsnip/A spring tuber closely related to the carrot. It has an earthy taste and is full of nutrients.");
            AddCrop(Constants.POTATO, "Potato/80/10/Basic -75/Potato/A widely cultivated tuber.");
            //Rhubarb can only be purchased in the desert or from the travelling merchant
            if (UnlockedDesert() || CheckMerchantForItemAndSeed(Constants.RHUBARB) || showItemsFromCurrentSeasonButInLockedPlaces)
            {
                GeneralAdd(Constants.RHUBARB, "Rhubarb/220/-300/Basic -79/Rhubarb/The stalks are extremely tart, but make a great dessert when sweetened.");
            }
            AddCrop(Constants.TULIP, "Tulip/30/18/Basic -80/Tulip/The most popular spring flower. Has a very faint sweet smell.");
            //The coffee bean can be purchased from the travelling merchant or found in the mines so I'll leave it due to mines and the fact it can grow in summer
            AddCrop(Constants.COFFEE_BEAN, "Coffee Bean/15/-300/Seeds -74/Coffee Bean/Plant in spring or summer to grow a coffee plant. Place five beans in a keg to make coffee.");
            AddCrop(Constants.GREEN_BEAN, "Green Bean/40/10/Basic -75/Green Bean/A juicy little bean with a cool, crisp snap.");
            //Strawberry can only be purchased on the 13th of spring or from travelling merchant
            if ((Game1.Date.Year == 1 && Game1.Date.DayOfMonth >= 13) || DoesTravellingMerchantHaveIt(Constants.STRAWBERRY) || showEveryItemFromCurrentSeason)
            {
                GeneralAdd(Constants.STRAWBERRY, "Strawberry/120/20/Basic -79/Strawberry/A sweet, juicy favorite with an appealing red color.");
            }
            else if (Game1.Date.Year > 1) //It's there fault if they don't save any seeds or a strawberry to turn into seeds
            {
                AddCrop(Constants.STRAWBERRY, "Strawberry/120/20/Basic -79/Strawberry/A sweet, juicy favorite with an appealing red color.");
            }


            //Fruit Trees
            GeneralAdd(Constants.APRICOT, "Apricot/50/15/Basic -79/Apricot/A tender little fruit with a rock-hard pit.");
            GeneralAdd(Constants.CHEERY, "Cherry/80/15/Basic -79/Cherry/It's popular, and ripens sooner than most other fruits.");
            //if (showFruitTreesInPreviousSeason)
            //{
            //    GeneralAdd(Constants.ORANGE, "Orange/100/15/Basic -79/Orange/Juicy, tangy, and bursting with sweet summer aroma.");
            //    GeneralAdd(Constants.PEACH, "Peach/140/15/Basic -79/Peach/It's almost fuzzy to the touch.");
            //}


            //Add Forage
            GeneralAdd(Constants.WILD_HORSERADISH, "Wild Horseradish/50/5/Basic -81/Wild Horseradish/A spicy root found in the spring.");
            GeneralAdd(Constants.DAFFODIL, "Daffodil/30/0/Basic -81/Daffodil/A traditional spring flower that makes a nice gift.");
            GeneralAdd(Constants.LEEK, "Leek/60/16/Basic -81/Leek/A tasty relative of the onion.");
            GeneralAdd(Constants.DANDELION, "Dandelion/40/10/Basic -81/Dandelion/Not the prettiest flower, but the leaves make a good salad.");
            GeneralAdd(Constants.SPRING_ONION, "Spring Onion/8/5/Basic -81/Spring Onion/These grow wild during the spring.");
            //farm 4 is wilderness, 3 is hilltop, 2 is forest, 1 is river, 0 is normal
            if (UnlockedSecretWoods() || showItemsFromCurrentSeasonButInLockedPlaces || Game1.whichFarm == 2 || CheckMerchantForItemAndSeed(Constants.MOREL))
            {
                GeneralAdd(Constants.MOREL, "Morel/150/8/Basic -81/Morel/Sought after for its unique nutty flavor.");
            }

            if(UnlockedSecretWoods() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.COMMON_MUSHROOM))
            {
                GeneralAdd(Constants.COMMON_MUSHROOM, "Common Mushroom/40/15/Basic -81/Common Mushroom/Slightly nutty, with good texture.");
            }

            //You can only forage Salmonberries from the 15th-18th
            if ((Game1.Date.DayOfMonth > 14 && Game1.Date.DayOfMonth < 19) || DoesTravellingMerchantHaveIt(Constants.SALMONBERRY) || showEveryItemFromCurrentSeason)
            {
                GeneralAdd(Constants.SALMONBERRY, "Salmonberry/5/10/Basic -79/Salmonberry/A spring-time berry with the flavor of the forest.");
            }

            //Add Fish
            AddFish(Constants.ANCHOVY, "Anchovy/30/5/Fish -4/Anchovy/A small silver fish found in the ocean./Day Night^Spring Fall");
            AddFish(Constants.SMALLMOUTH_BASS, "Smallmouth Bass/50/10/Fish -4/Smallmouth Bass/A freshwater fish that is very sensitive to pollution./Day Night^Spring Fall");
            AddFish(Constants.CATFISH, "Catfish/200/20/Fish -4/Catfish/An uncommon fish found in streams./Day^Spring Fall Winter");
            AddFish(Constants.EEL, "Eel/85/12/Fish -4/Eel/A long, slippery little fish./Night^Spring Fall");
            AddFish(Constants.SHAD, "Shad/60/10/Fish -4/Shad/Lives in a school at sea, but returns to the rivers to spawn./Day^Spring Summer Fall");
            AddFish(Constants.SUNFISH, "Sunfish/30/5/Fish -4/Sunfish/A common river fish./Day^Spring Summer");
            AddFish(Constants.HERRING, "Herring/30/5/Fish -4/Herring/A common ocean fish./Day Night^Spring Winter");
            AddFish(Constants.SARDINE, "Sardine/40/5/Fish -4/Sardine/A common ocean fish./Day^Spring Summer Fall Winter");
            AddFish(Constants.HALIBUT, "Halibut/80/10/Fish -4/Halibut/A flat fish that lives on the ocean floor./Day^Spring Summer");
            //The Legend requires level 10 fishing
            if (Game1.player.getEffectiveSkillLevel(1) == 10 && Game1.isRaining || showAllFish)
            {
                GeneralAdd(Constants.LEGEND, "Legend/5000/200/Fish -4/Legend/The king of all fish! They said he'd never be caught./Day^Winter");
            }
            //Might need to throw in seeds here later if I want to check if they planted something so I don't need to remind them to plant it
            //Should check and only reccomend to plant things if there is enough time for it to grow before the season ends
            //may also want to throw in exclusive items like scarcrows or whatnot
            //may want to put specific checks in for missing community center bundles
        }
    }
}
