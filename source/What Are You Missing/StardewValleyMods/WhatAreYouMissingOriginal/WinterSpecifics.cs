using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace WhatAreYouMissingOriginal
{
    class WinterSpecifics : SpecificItems
    {
        public WinterSpecifics(bool showItemsFromCurrentSeasonButInLockedPlaces,
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
            fileName = "winterSpecifics.json";
        }

        public override void UpdateData()
        {
            //The number is the index in the parent sheet, it will be used when comparing what the player has to what's available

            //Add Harvested Crops

            //Fruit Trees
            //if (showFruitTreesInPreviousSeason)
            //{
            //    GeneralAdd(Constants.APRICOT, "Apricot/50/15/Basic -79/Apricot/A tender little fruit with a rock-hard pit.");
            //    GeneralAdd(Constants.CHEERY, "Cherry/80/15/Basic -79/Cherry/It's popular, and ripens sooner than most other fruits.");
            //}

            //Forage Items
            GeneralAdd(Constants.NAUTILUS_SHELL, "Nautilus Shell/120/-300/Basic -23/Nautilus Shell/An ancient shell.");
            GeneralAdd(Constants.WINTER_ROOT, "Winter Root/70/10/Basic -81/Winter Root/A starchy tuber.");
            GeneralAdd(Constants.CRYSTAL_FRUIT, "Crystal Fruit/150/25/Basic -79/Crystal Fruit/A delicate fruit that pops up from the snow.");
            GeneralAdd(Constants.SNOW_YAM, "Snow Yam/100/12/Basic -81/Snow Yam/This little yam was hiding beneath the snow.");
            GeneralAdd(Constants.CROCUS, "Crocus/60/0/Basic -80/Crocus/A flower that can bloom in the winter.");
            GeneralAdd(Constants.HOLLY, "Holly/80/-15/Basic -81/Holly/The leaves and bright red berries make a popular winter decoration.");



            //Fish
            AddFish(Constants.TUNA, "Tuna/100/15/Fish -4/Tuna/A large fish that lives in the ocean./Day^Summer Winter");
            AddFish(Constants.SARDINE, "Sardine/40/5/Fish -4/Sardine/A common ocean fish./Day^Spring Summer Fall Winter");
            AddFish(Constants.PERCH, "Perch/55/10/Fish -4/Perch/A freshwater fish of the winter./Day Night^Winter");
            AddFish(Constants.PIKE, "Pike/100/15/Fish -4/Pike/A freshwater fish that's difficult to catch./Day Night^Summer Winter");
            AddFish(Constants.RED_MULLET, "Red Mullet/75/10/Fish -4/Red Mullet/Long ago these were kept as pets./Day^Summer Winter");
            AddFish(Constants.HERRING, "Herring/30/5/Fish -4/Herring/A common ocean fish./Day Night^Spring Winter");
            AddFish(Constants.SQUID, "Squid/80/10/Fish -4/Squid/A deep sea creature that can grow to enormous size./Day^Winter");
            AddFish(Constants.SEA_CUCUMBER, "Sea Cucumber/75/-10/Fish -4/Sea Cucumber/A slippery, slimy creature found on the ocean floor./Day^Fall Winter");
            AddFish(Constants.STURGEON, "Sturgeon/200/10/Fish -4/Sturgeon/An ancient bottom-feeder with a dwindling population. Females can live up to 150 years./Day^Spring Summer");
            AddFish(Constants.TIGER_TROUT, "Tiger Trout/150/10/Fish -4/Tiger Trout/A rare hybrid trout that cannot bear offspring of its own./Day^Spring Summer");
            AddFish(Constants.ALBACORE, "Albacore/75/10/Fish -4/Albacore/Prefers temperature \"edges\" where cool and warm water meet./Day^Spring Fall");
            AddFish(Constants.LINGCOD, "Lingcod/120/10/Fish -4/Lingcod/A fearsome predator that will eat almost anything it can cram into its mouth./Day^Fall");
            AddFish(Constants.RED_SNAPPER, "Red Snapper/50/10/Fish -4/Red Snapper/A popular fish with a nice red color./Day^Summer Fall Winter");
            AddFish(Constants.HALIBUT, "Halibut/80/10/Fish -4/Halibut/A flat fish that lives on the ocean floor./Day^Spring Summer");
            //Needs some sort of special check to see if the player has already caught the crimson fish
            //other than that they have the high enough fishing level
            //hopefully that key thing works not sure if it uses that key
            if (Game1.player.getEffectiveSkillLevel(1) > 6 && !Game1.player.fishCaught.ContainsKey(Constants.GLACIERFISH) || showAllFish)
            {
                GeneralAdd(Constants.GLACIERFISH, "Glacierfish/1000/10/Fish -4/Glacierfish/Builds a nest on the underside of glaciers./Day^Winter");
            }

            if((Game1.Date.DayOfMonth > 14 && Game1.Date.DayOfMonth < 18) || showAllFish)
            {
                GeneralAdd(798, "Midnight Squid/100/15/Fish -4/Midnight Squid/A strange and mysterious denizen of the ocean's twilight depths./Day^Spring Summer");
                GeneralAdd(799, "Spook Fish/220/15/Fish -4/Spook Fish/The huge eyes can detect the faint silhouettes of prey./Day^Spring Summer");
                GeneralAdd(800, "Blobfish/500/15/Fish -4/Blobfish/This odd creature floats above the ocean floor, consuming any edible material in its path./Day^Spring Summer");
            }
            //Might need to throw in seeds here later if I want to check if they planted something so I don't need to remind them to plant it
            //Should check and only reccomend to plant things if there is enough time for it to grow before the season ends
            //may also want to throw in exclusive items like scarcrows or whatnot
        }
    }
}
