using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace WhatAreYouMissingOriginal
{
    class FallSpecifics : SpecificItems
    {
        public FallSpecifics(bool showItemsFromCurrentSeasonButInLockedPlaces,
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
            fileName = "fallSpecifics.json";
        }

        public override void UpdateData()
        {
            //The number is the index in the parent sheet, it will be used when comparing what the player has to what's available

            //Add Harvested Crops
            AddCrop(Constants.AMARANTH, "Amaranth/150/20/Basic -75/Amaranth/A purple grain cultivated by an ancient civilization.");
            if(Game1.year > 1 || CheckMerchantForItemAndSeed(Constants.ARTICHOKE) || showEveryItemFromCurrentSeason)
            {
                AddCrop(Constants.ARTICHOKE, "Artichoke/160/12/Basic -75/Artichoke/The bud of a thistle plant. The spiny outer leaves conceal a fleshy, filling interior.");
            }
            //beets can only be purchased from the desert
            if(UnlockedDesert() || CheckMerchantForItemAndSeed(Constants.BEET) || showItemsFromCurrentSeasonButInLockedPlaces)
            {
                AddCrop(Constants.BEET, "Beet/100/12/Basic -75/Beet/A sweet and earthy root vegatable. As a bonus, the leaves make a great salad.");
            }
            AddCrop(Constants.BOK_CHOY, "Bok Choy/80/10/Basic -75/Bok Choy/The leafy greens and fibrous stalks are healthy and delicious.");
            AddCrop(Constants.FAIRY_ROSE, "Fairy Rose/290/18/Basic -80/Fairy Rose/An old folk legend suggests that the sweet smell of this flower attracts fairies.");
            AddCrop(Constants.PUMPKIN, "Pumpkin/320/-300/Basic -75/Pumpkin/A fall favorite, grown for its crunchy seeds and delicately flavored flesh. As a bonus, the hollow shell can be carved into a festive decoration.");
            AddCrop(Constants.SUNFLOWER, "Sunflower/80/18/Basic -80/Sunflower/A common misconception is that the flower turns so it's always facing the sun.");
            //The sweet gem berry can only come from the travelling cart or the seed maker
            AddCrop(Constants.SWEET_GEM_BERRY, "Sweet Gem Berry/3000/-300/Basic -17/Sweet Gem Berry/It's by far the sweetest thing you've ever smelled.");
            AddCrop(Constants.WHEAT, "Wheat/25/-300/Basic -75/Wheat/One of the most widely cultivated grains. Makes a great flour for breads and cakes.");
            AddCrop(Constants.YAM, "Yam/160/18/Basic -75/Yam/A starchy tuber with a lot of culinary versatility.");
            AddCrop(Constants.CORN, "Corn/50/10/Basic -75/Corn/One of the most popular grains. The sweet, fresh cobs are a summer favorite.");
            AddCrop(Constants.CRANBERRIES, "Cranberries/75/15/Basic -79/Cranberries/These tart red berries are a traditional winter food.");
            AddCrop(Constants.EGGPLANT, "Eggplant/60/8/Basic -75/Eggplant/A rich and wholesome relative of the tomato. Delicious fried or stewed.");
            AddCrop(Constants.GRAPE, "Grape/80/15/Basic -79/Grape/A sweet cluster of fruit.");

            //Fruit Trees
            GeneralAdd(Constants.APPLE, "Apple/100/15/Basic -79/Apple/A crisp fruit used for juice and cider.");
            GeneralAdd(Constants.POMEGRANATE, "Pomegranate/140/15/Basic -79/Pomegranate/Within the fruit are clusters of juicy seeds.");


            //Forage Items
            GeneralAdd(Constants.WILD_PLUM, "Wild Plum/80/10/Basic -79/Wild Plum/Tart and juicy with a pungent aroma.");
            GeneralAdd(Constants.HAZELNUT, "Hazelnut/90/12/Basic -81/Hazelnut/That's one big hazelnut!");
            GeneralAdd(Constants.BLACKBERRY, "Blackberry/20/10/Basic -79/Blackberry/An early-fall treat.");
            if(UnlockedSecretWoods() || showItemsFromCurrentSeasonButInLockedPlaces || CheckMerchantForItemAndSeed(Constants.CHANTERELLE))
            {
                GeneralAdd(Constants.CHANTERELLE, "Chanterelle/160/30/Basic -81/Chanterelle/A tasty mushroom with a fruity smell and slightly peppery flavor.");
            }
            GeneralAdd(Constants.COMMON_MUSHROOM, "Common Mushroom/40/15/Basic -81/Common Mushroom/Slightly nutty, with good texture.");

            //Fish
            AddFish(Constants.ANCHOVY, "Anchovy/30/5/Fish -4/Anchovy/A small silver fish found in the ocean./Day Night^Spring Fall");
            AddFish(Constants.SARDINE, "Sardine/40/5/Fish -4/Sardine/A common ocean fish./Day^Spring Summer Fall Winter");
            AddFish(Constants.SMALLMOUTH_BASS, "Smallmouth Bass/50/10/Fish -4/Smallmouth Bass/A freshwater fish that is very sensitive to pollution./Day Night^Spring Fall");
            AddFish(Constants.SALMON, "Salmon/75/15/Fish -4/Salmon/Swims upstream to lay its eggs./Day^Fall");
            AddFish(Constants.WALLEYE, "Walleye/105/12/Fish -4/Walleye/A freshwater fish caught at night./Night^Fall Winter");
            AddFish(Constants.CATFISH, "Catfish/200/20/Fish -4/Catfish/An uncommon fish found in streams./Day^Spring Fall Winter");
            AddFish(Constants.EEL, "Eel/85/12/Fish -4/Eel/A long, slippery little fish./Night^Spring Fall");
            AddFish(Constants.RED_SNAPPER, "Red Snapper/50/10/Fish -4/Red Snapper/A popular fish with a nice red color./Day^Summer Fall Winter");
            AddFish(Constants.SEA_CUCUMBER, "Sea Cucumber/75/-10/Fish -4/Sea Cucumber/A slippery, slimy creature found on the ocean floor./Day^Fall Winter");
            AddFish(Constants.SUPER_CUCUMBER, "Super Cucumber/250/50/Fish -4/Super Cucumber/A rare, purple variety of sea cucumber./Night^Summer Fall");
            AddFish(Constants.TIGER_TROUT, "Tiger Trout/150/10/Fish -4/Tiger Trout/A rare hybrid trout that cannot bear offspring of its own./Day^Spring Summer");
            AddFish(Constants.TILAPIA, "Tilapia/75/10/Fish -4/Tilapia/A primarily vegetarian fish that prefers warm water./Day^Spring Summer");
            AddFish(Constants.ALBACORE, "Albacore/75/10/Fish -4/Albacore/Prefers temperature \"edges\" where cool and warm water meet./Day^Spring Fall");
            AddFish(Constants.SHAD, "Shad/60/10/Fish -4/Shad/Lives in a school at sea, but returns to the rivers to spawn./Day^Spring Summer Fall");
            //Needs some sort of special check to see if the player has already caught the crimson fish
            //other than that they have the high enough fishing level
            //hopefully that key thing works not sure if it uses that key
            if (Game1.player.getEffectiveSkillLevel(1) > 2 && !Game1.player.fishCaught.ContainsKey(Constants.ANGLER))
            {
                GeneralAdd(Constants.ANGLER, "Angler/900/10/Fish -4/Angler/Uses a bioluminescent dangler to attract prey./Day Night^Spring Summer Fall Winter");
            }
            //Might need to throw in seeds here later if I want to check if they planted something so I don't need to remind them to plant it
            //Should check and only reccomend to plant things if there is enough time for it to grow before the season ends
            //may also want to throw in exclusive items like scarcrows or whatnot
        }
    }
}
