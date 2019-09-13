using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace WhatAreYouMissingOriginal
{
    class ModConfig
    {
        public SButton button { get; set; }
        public bool showItemsFromCurrentSeasonButInLockedPlaces { get; set; }
        public bool showEveryItemFromCurrentSeason { get; set; }
        public bool showAllFish { get; set; }
        public bool showCommonCommunityCenterItems { get; set; }
        public bool showFruitTreesInPreviousSeason { get; set; }
        public bool checkGrowingCrops { get; set; }
        public bool onlyShowWhatCanBeGrownBeforeEndOfSeason { get; set; }
        public int amount { get; set; }

        public ModConfig()
        {
            this.button = SButton.F3;
            showItemsFromCurrentSeasonButInLockedPlaces = true;
            showEveryItemFromCurrentSeason = false;
            showAllFish = false;
            showCommonCommunityCenterItems = true;
            showFruitTreesInPreviousSeason = true;
            checkGrowingCrops = true;
            onlyShowWhatCanBeGrownBeforeEndOfSeason = true;
            amount = 1;

        }
    }
}
