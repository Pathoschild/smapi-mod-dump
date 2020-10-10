/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace WhatAreYouMissing
{
    public class ModConfig
    {
        public SButton button { get; set; }
        public bool ShowItemsFromLockedPlaces { get; set; }
        public bool ShowAllFishFromCurrentSeason { get; set; }
        public bool AlwaysShowAllFish { get; set; }
        public bool ShowAllRecipes { get; set; }
        public bool AlwaysShowAllRecipes { get; set; }
        public int CommonAmount { get; set; }
        public int HighestQualityAmount { get; set; }
        public int FishHighestQuality { get; set; }
        public bool DoNotShowCaughtFish { get; set; }

        public bool IgnoreQuality { get; set; }
        public bool LogWarnings { get; set; }

        public ModConfig()
        {
            button = SButton.F2;
            ShowItemsFromLockedPlaces = true;
            ShowAllFishFromCurrentSeason = false;
            AlwaysShowAllFish = false;
            ShowAllRecipes = false;
            AlwaysShowAllRecipes = false;
            CommonAmount = 5;
            HighestQualityAmount = 5;
            FishHighestQuality = 4;
            DoNotShowCaughtFish = false;
            IgnoreQuality = true;
            LogWarnings = true;
        }
    }
}
