/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace WhatAreYouMissing
{
    class CropConversion
    {
        //Might rename the class later and make this like a data class to hold different dictionaries of information
        private Dictionary<int, int> cropToSeeds { get; set; }

        public CropConversion()
        {
            cropToSeeds = new Dictionary<int, int>();

            //SPRING
            cropToSeeds.Add(Constants.BLUE_JAZZ, Constants.JAZZ_SEEDS);
            cropToSeeds.Add(Constants.CAULIFLOWER , Constants.CAULIFLOWER_SEEDS);
            cropToSeeds.Add(Constants.GARLIC , Constants.GARLIC_SEEDS);
            cropToSeeds.Add(Constants.KALE , Constants.KALE_SEEDS);
            cropToSeeds.Add(Constants.PARSNIP , Constants.PARSNIP_SEEDS);
            cropToSeeds.Add(Constants.POTATO , Constants.POTATO_SEEDS);
            cropToSeeds.Add(Constants.RHUBARB , Constants.RHUBARB_SEEDS);
            cropToSeeds.Add(Constants.TULIP , Constants.TULIP_BULB);
            cropToSeeds.Add(Constants.COFFEE_BEAN , Constants.COFFEE_BEAN);
            cropToSeeds.Add(Constants.GREEN_BEAN , Constants.BEAN_STARTER);
            cropToSeeds.Add(Constants.STRAWBERRY , Constants.STRAWBERRY_SEEDS);

            cropToSeeds.Add(Constants.WILD_HORSERADISH , Constants.SPRING_SEEDS);
            cropToSeeds.Add(Constants.DAFFODIL , Constants.SPRING_SEEDS);
            cropToSeeds.Add(Constants.LEEK , Constants.SPRING_SEEDS);
            cropToSeeds.Add(Constants.DANDELION , Constants.SPRING_SEEDS);

            //SUMMER
            cropToSeeds.Add(Constants.MELON , Constants.MELON_SEEDS);
            cropToSeeds.Add(Constants.POPPY , Constants.POPPY_SEEDS);
            cropToSeeds.Add(Constants.RADISH , Constants.RADISH_SEEDS);
            cropToSeeds.Add(Constants.RED_CABBAGE , Constants.RED_CABBAGE_SEEDS);
            cropToSeeds.Add(Constants.STARFRUIT , Constants.STARFRUIT_SEEDS);
            cropToSeeds.Add(Constants.SUMMER_SPANGLE , Constants.SPANGLE_SEEDS);
            cropToSeeds.Add(Constants.SUNFLOWER , Constants.SUNFLOWER_SEEDS);
            cropToSeeds.Add(Constants.WHEAT , Constants.WHEAT_SEEDS);
            cropToSeeds.Add(Constants.BLUEBERRY , Constants.BLUEBERRY_SEEDS);
            cropToSeeds.Add(Constants.CORN , Constants.CORN_SEEDS);
            cropToSeeds.Add(Constants.HOPS , Constants.HOPS_STARTER);
            cropToSeeds.Add(Constants.HOT_PEPPER , Constants.PEPPER_SEEDS);
            cropToSeeds.Add(Constants.TOMATO , Constants.TOMATO_SEEDS);

            cropToSeeds.Add(Constants.SPICE_BERRY , Constants.SUMMER_SEEDS);
            //cropToSeeds.Add(Constants.GRAPE , Constants.SUMMER_SEEDS); //Can't have duplicate keys...don't want to switch to a list because 
            //then I'd have to iterate so the other value is more important, I was on the fence including these anyway.
            cropToSeeds.Add(Constants.SWEET_PEA , Constants.SUMMER_SEEDS);

            //FALL
            cropToSeeds.Add(Constants.AMARANTH , Constants.AMARANTH_SEEDS);
            cropToSeeds.Add(Constants.ARTICHOKE , Constants.ARTICHOKE_SEEDS);
            cropToSeeds.Add(Constants.BEET , Constants.BEET_SEEDS);
            cropToSeeds.Add(Constants.BOK_CHOY , Constants.BOK_CHOY_SEEDS);
            cropToSeeds.Add(Constants.FAIRY_ROSE , Constants.FAIRY_SEEDS);
            cropToSeeds.Add(Constants.PUMPKIN , Constants.PUMPKIN_SEEDS);
            cropToSeeds.Add(Constants.SWEET_GEM_BERRY , Constants.RARE_SEED);
            cropToSeeds.Add(Constants.YAM , Constants.YAM_SEEDS);
            cropToSeeds.Add(Constants.CRANBERRIES , Constants.CRANBERRY_SEEDS);
            cropToSeeds.Add(Constants.EGGPLANT , Constants.EGGPLANT_SEEDS);
            cropToSeeds.Add(Constants.GRAPE , Constants.GRAPE_STARTER);

            cropToSeeds.Add(Constants.WILD_PLUM , Constants.FALL_SEEDS);
            cropToSeeds.Add(Constants.HAZELNUT , Constants.FALL_SEEDS);
            cropToSeeds.Add(Constants.BLACKBERRY , Constants.FALL_SEEDS);
            cropToSeeds.Add(Constants.COMMON_MUSHROOM , Constants.FALL_SEEDS);

            //WINTER
            cropToSeeds.Add(Constants.WINTER_ROOT , Constants.WINTER_SEEDS);
            cropToSeeds.Add(Constants.CRYSTAL_FRUIT , Constants.WINTER_SEEDS);
            cropToSeeds.Add(Constants.SNOW_YAM , Constants.WINTER_SEEDS);
            cropToSeeds.Add(Constants.CROCUS , Constants.WINTER_SEEDS);
        }

        public int CropToSeedIndex(int crop)
        {
            if (cropToSeeds.ContainsKey(crop))
            {
                return cropToSeeds[crop];
            }

            return -1;
        }
    }
}
