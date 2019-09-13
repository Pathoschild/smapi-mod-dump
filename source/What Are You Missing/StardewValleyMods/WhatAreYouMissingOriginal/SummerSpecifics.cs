using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace WhatAreYouMissingOriginal
{
    class SummerSpecifics : SpecificItems
    {
        public SummerSpecifics(bool showItemsFromCurrentSeasonButInLockedPlaces,
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
            fileName = "summerSpecifics.json";
        }

        public override void UpdateData()
        {
            //The number is the index in the parent sheet, it will be used when comparing what the player has to what's available

            //Add Harvested Crops
            AddCrop(Constants.MELON, "Melon/250/45/Basic -79/Melon/A cool, sweet summer treat.");
            AddCrop(Constants.POPPY, "Poppy/140/18/Basic -80/Poppy/In addition to its colorful flower, the Poppy has culinary and medicinal uses.");
            AddCrop(Constants.RADISH, "Radish/90/18/Basic -75/Radish/A crisp and refreshing root vegetable with hints of pepper when eaten raw.");
            if(Game1.Date.Year > 1 || CheckMerchantForItemAndSeed(Constants.RED_CABBAGE) || showEveryItemFromCurrentSeason)
            {
                GeneralAdd(Constants.RED_CABBAGE, "Red Cabbage/260/30/Basic -75/Red Cabbage/Often used in salads and coleslaws. The color can range from purple to blue to green-yellow depending on soil conditions.");
            }
            //Starfruit can only be bought from the desert and gotten from the musem
            if(UnlockedDesert() || CheckMerchantForItemAndSeed(Constants.STARFRUIT) || showEveryItemFromCurrentSeason)
            {
                GeneralAdd(Constants.STARFRUIT, "Starfruit/750/50/Basic -79/Starfruit/An extremely juicy fruit that grows in hot, humid weather. Slightly sweet with a sour undertone.");
            }

            AddCrop(Constants.SUMMER_SPANGLE, "Summer Spangle/90/18/Basic -80/Summer Spangle/A tropical bloom that thrives in the humid summer air. Has a sweet, tangy aroma.");
            AddCrop(Constants.SUNFLOWER, "Sunflower/80/18/Basic -80/Sunflower/A common misconception is that the flower turns so it's always facing the sun.");
            AddCrop(Constants.WHEAT, "Wheat/25/-300/Basic -75/Wheat/One of the most widely cultivated grains. Makes a great flour for breads and cakes.");
            AddCrop(Constants.BLUEBERRY, "Blueberry/50/10/Basic -79/Blueberry/A popular berry reported to have many health benefits. The blue skin has the highest nutrient concentration.");
            //coffee beans can only come from travelling cart or mines
            AddCrop(Constants.COFFEE_BEAN, "Coffee Bean/15/-300/Seeds -74/Coffee Bean/Plant in spring or summer to grow a coffee plant. Place five beans in a keg to make coffee.");
            AddCrop(Constants.CORN, "Corn/50/10/Basic -75/Corn/One of the most popular grains. The sweet, fresh cobs are a summer favorite.");
            AddCrop(Constants.HOPS, "Hops/25/18/Basic -75/Hops/A bitter, tangy flower used to flavor beer.");
            AddCrop(Constants.HOT_PEPPER, "Hot Pepper/40/5/Basic -79/Hot Pepper/Fiery hot with a hint of sweetness.");
            AddCrop(Constants.TOMATO, "Tomato/60/8/Basic -75/Tomato/Rich and slightly tangy, the Tomato has a wide variety of culinary uses.");

            //Fruit Trees
            GeneralAdd(Constants.ORANGE, "Orange/100/15/Basic -79/Orange/Juicy, tangy, and bursting with sweet summer aroma.");
            GeneralAdd(Constants.PEACH, "Peach/140/15/Basic -79/Peach/It's almost fuzzy to the touch.");
            //if (showFruitTreesInPreviousSeason)
            //{
            //    GeneralAdd(Constants.APPLE, "Apple/100/15/Basic -79/Apple/A crisp fruit used for juice and cider.");
            //    GeneralAdd(Constants.POMEGRANATE, "Pomegranate/140/15/Basic -79/Pomegranate/Within the fruit are clusters of juicy seeds.");
            //}

            //Forage Items
            GeneralAdd(Constants.RAINBOW_SHELL, "Rainbow Shell/300/-300/Basic -23/Rainbow Shell/It's a very beautiful shell.");
            GeneralAdd(Constants.SPICE_BERRY, "Spice Berry/80/10/Basic -79/Spice Berry/It fills the air with a pungent aroma.");
            GeneralAdd(Constants.GRAPE, "Grape/80/15/Basic -79/Grape/A sweet cluster of fruit.");
            GeneralAdd(Constants.SWEET_PEA, "Sweet Pea/50/0/Basic -80/Sweet Pea/A fragrant summer flower.");
            //farm 4 is wilderness, 3 is hilltop, 2 is forest, 1 is river, 0 is normal
            if (Game1.whichFarm == 2)
            {
                GeneralAdd(Constants.COMMON_MUSHROOM, "Common Mushroom/40/15/Basic -81/Common Mushroom/Slightly nutty, with good texture.");
            }
            //Only if the secret woods is unlocked
            GeneralAdd(Constants.FIDDLEHEAD_FERN, "Fiddlehead Fern/90/10/Basic -75/Fiddlehead Fern/The young shoots are an edible specialty.");

            //Fish
            //public const int weather_sunny = 0;
            //public const int weather_rain = 1;
            //public const int weather_debris = 2;
            //public const int weather_lightning = 3;
            //public const int weather_festival = 4;
            //public const int weather_snow = 5;
            //public const int weather_wedding = 6;

            AddFish(Constants.PUFFERFISH, "Pufferfish/200/-40/Fish -4/Pufferfish/Inflates when threatened./Day^Summer");
            AddFish(Constants.TUNA, "Tuna/100/15/Fish -4/Tuna/A large fish that lives in the ocean./Day^Summer Winter");
            AddFish(Constants.RAINBOW_TROUT, "Rainbow Trout/65/10/Fish -4/Rainbow Trout/A freshwater trout with colorful markings./Day^Summer");
            AddFish(Constants.CATFISH, "Catfish/200/20/Fish -4/Catfish/An uncommon fish found in streams./Day^Spring Fall Winter");
            AddFish(Constants.PIKE, "Pike/100/15/Fish -4/Pike/A freshwater fish that's difficult to catch./Day Night^Summer Winter");
            AddFish(Constants.SUNFISH, "Sunfish/30/5/Fish -4/Sunfish/A common river fish./Day^Spring Summer");
            AddFish(Constants.RED_MULLET, "Red Mullet/75/10/Fish -4/Red Mullet/Long ago these were kept as pets./Day^Summer Winter");
            AddFish(Constants.OCTOPUS, "Octopus/150/-300/Fish -4/Octopus/A mysterious and intelligent creature./Day^Summer");
            AddFish(Constants.RED_SNAPPER, "Red Snapper/50/10/Fish -4/Red Snapper/A popular fish with a nice red color./Day^Summer Fall Winter");
            AddFish(Constants.SUPER_CUCUMBER, "Super Cucumber/250/50/Fish -4/Super Cucumber/A rare, purple variety of sea cucumber./Night^Summer Fall");
            AddFish(Constants.STURGEON, "Sturgeon/200/10/Fish -4/Sturgeon/An ancient bottom-feeder with a dwindling population. Females can live up to 150 years./Day^Spring Summer");
            AddFish(Constants.TILAPIA, "Tilapia/75/10/Fish -4/Tilapia/A primarily vegetarian fish that prefers warm water./Day^Spring Summer");
            AddFish(Constants.DORADO, "Dorado/100/10/Fish -4/Dorado/A fierce carnivore with brilliant orange scales./Day^Summer");
            AddFish(Constants.SHAD, "Shad/60/10/Fish -4/Shad/Lives in a school at sea, but returns to the rivers to spawn./Day^Spring Summer Fall");
            AddFish(Constants.HALIBUT, "Halibut/80/10/Fish -4/Halibut/A flat fish that lives on the ocean floor./Day^Spring Summer");

            //Needs some sort of special check to see if the player has already caught the crimson fish
            //other than that they have the high enough fishing level
            //hopefully that key thing works not sure if it uses that key
            if((Game1.player.getEffectiveSkillLevel(1) > 4 && !Game1.player.fishCaught.ContainsKey(Constants.CRIMSONFISH)) || showAllFish)
            {
                GeneralAdd(Constants.CRIMSONFISH, "Crimsonfish/1500/15/Fish -4/Crimsonfish/Lives deep in the ocean but likes to lay its eggs in the warm summer water./Day^Winter");
            }
            //Might need to throw in seeds here later if I want to check if they planted something so I don't need to remind them to plant it
            //Should check and only reccomend to plant things if there is enough time for it to grow before the season ends
            //may also want to throw in exclusive items like scarcrows or whatnot
        }
    }
}
