/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    class AvailableMerchantStock : Items, ISpecificItems
    {
        private Dictionary<int, SObject> SpringItems;
        private Dictionary<int, SObject> SummerItems;
        private Dictionary<int, SObject> FallItems;
        private Dictionary<int, SObject> WinterItems;
        private Dictionary<int, SObject> CommonCCItems;
        private Dictionary<int, SObject> AllPossibleMerchantItems;
        public AvailableMerchantStock(Dictionary<int, SObject> spring, Dictionary<int, SObject> summer, Dictionary<int, SObject> fall, Dictionary<int, SObject> winter, Dictionary<int, SObject> cc) : base()
        {
            SpringItems = spring;
            SummerItems = summer;
            FallItems = fall;
            WinterItems = winter;
            CommonCCItems = cc;
            AllPossibleMerchantItems = new Dictionary<int, SObject>();
            AddPossibleItems();
            AddMissingItems();
        }
        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        protected override void AddItems()
        {
        }

        private void AddMissingItems()
        {
            foreach (KeyValuePair<int, SObject> pair in AllPossibleMerchantItems)
            {
                if (Utilities.CheckMerchantForItemAndSeed(pair.Key))
                {
                    if (IsSeedOrSapling(pair.Value))
                    {
                        int harvestID = ConvertSeedIdToHarvestId(pair.Key);
                        AddIfSeasonal(harvestID);
                    }
                    else
                    {
                        AddIfSeasonal(pair.Key);
                    }
                }
            }
        }

        private void AddIfSeasonal(int id)
        {
            if (IsSeasonSpecific(id))
            {
                AddOneCommonObject(id);
            }
        }

        private bool IsSeedOrSapling(SObject item)
        {
            return SObject.SeedsCategory == item.Category;
        }

        private int ConvertSeedIdToHarvestId(int id)
        {
            Dictionary<int, string> cropData = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            Dictionary<int, string> fruitTreeData = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");

            if (cropData.ContainsKey(id))
            {
                return int.Parse(cropData[id].Split('/')[3]);
            }
            else
            {
                return int.Parse(fruitTreeData[id].Split('/')[2]);
            }
        }

        private bool IsSeasonSpecific(int parentSheetIndex)
        {
            return SpringItems.ContainsKey(parentSheetIndex) || SummerItems.ContainsKey(parentSheetIndex) 
                || FallItems.ContainsKey(parentSheetIndex) || WinterItems.ContainsKey(parentSheetIndex);
        }

        private void AddPossibleItems()
        {
            AddPossibleItem(Constants.RARE_SEED);
            AddPossibleItem(Constants.ANCIENT_SEEDS);
            //add snow rarecrow
            AddPossibleItem(Constants.COFFEE_BEAN);
            //add wedding ring for multiplayer
            //add fertilizers
            AddPossibleItem(Constants.SPRING_SEEDS);
            AddPossibleItem(Constants.GARLIC_SEEDS);
            AddPossibleItem(Constants.POTATO_SEEDS);
            AddPossibleItem(Constants.CAULIFLOWER_SEEDS);
            AddPossibleItem(Constants.RHUBARB_SEEDS);
            AddPossibleItem(Constants.PARSNIP_SEEDS);
            AddPossibleItem(Constants.KALE_SEEDS);
            AddPossibleItem(Constants.JAZZ_SEEDS);
            AddPossibleItem(Constants.TULIP_BULB);
            AddPossibleItem(Constants.SUMMER_SEEDS);
            AddPossibleItem(Constants.CORN_SEEDS);
            AddPossibleItem(Constants.WHEAT_SEEDS);
            AddPossibleItem(Constants.SPANGLE_SEEDS);
            AddPossibleItem(Constants.POPPY_SEEDS);
            AddPossibleItem(Constants.TOMATO_SEEDS);
            AddPossibleItem(Constants.MELON_SEEDS);
            AddPossibleItem(Constants.RADISH_SEEDS);
            AddPossibleItem(Constants.RED_CABBAGE_SEEDS);
            AddPossibleItem(Constants.BLUEBERRY_SEEDS);
            AddPossibleItem(Constants.STARFRUIT_SEEDS);
            AddPossibleItem(Constants.PEPPER_SEEDS);
            AddPossibleItem(Constants.FAIRY_SEEDS);
            AddPossibleItem(Constants.AMARANTH_SEEDS);
            AddPossibleItem(Constants.ARTICHOKE_SEEDS);
            AddPossibleItem(Constants.CRANBERRY_SEEDS);
            AddPossibleItem(Constants.EGGPLANT_SEEDS);
            AddPossibleItem(Constants.BEET_SEEDS);
            AddPossibleItem(Constants.BOK_CHOY_SEEDS);
            AddPossibleItem(Constants.YAM_SEEDS);
            AddPossibleItem(Constants.PUMPKIN_SEEDS);
            AddPossibleItem(Constants.SUNFLOWER_SEEDS);
            AddPossibleItem(Constants.FALL_SEEDS);
            AddPossibleItem(Constants.WINTER_SEEDS);
            AddPossibleItem(Constants.BEAN_STARTER);
            AddPossibleItem(Constants.HOPS_STARTER);
            AddPossibleItem(Constants.GRAPE_STARTER);

            //Add saplings
            AddPossibleItem(Constants.APRICOT_SAPLING);
            AddPossibleItem(Constants.CHERRY_SAPLING);
            AddPossibleItem(Constants.POMEGRANATE_SAPLING);
            AddPossibleItem(Constants.APPLE_SAPLING);
            AddPossibleItem(Constants.ORANGE_SAPLING);
            AddPossibleItem(Constants.PEACH_SAPLING);

            AddPossibleItem(Constants.GARLIC);
            AddPossibleItem(Constants.POTATO);
            AddPossibleItem(Constants.CAULIFLOWER);
            AddPossibleItem(Constants.RHUBARB);
            AddPossibleItem(Constants.PARSNIP);
            AddPossibleItem(Constants.KALE);
            AddPossibleItem(Constants.BLUE_JAZZ);
            AddPossibleItem(Constants.CORN);
            AddPossibleItem(Constants.HOPS);
            AddPossibleItem(Constants.WHEAT);
            AddPossibleItem(Constants.SUMMER_SPANGLE);
            AddPossibleItem(Constants.POPPY);
            AddPossibleItem(Constants.TOMATO);
            AddPossibleItem(Constants.MELON);
            AddPossibleItem(Constants.RADISH);
            AddPossibleItem(Constants.RED_CABBAGE);
            AddPossibleItem(Constants.BLUEBERRY);
            AddPossibleItem(Constants.STARFRUIT);
            AddPossibleItem(Constants.HOT_PEPPER);
            AddPossibleItem(Constants.FAIRY_ROSE);
            AddPossibleItem(Constants.SPICE_BERRY);
            AddPossibleItem(Constants.AMARANTH);
            AddPossibleItem(Constants.ARTICHOKE);
            AddPossibleItem(Constants.CRANBERRIES);
            AddPossibleItem(Constants.EGGPLANT);
            AddPossibleItem(Constants.BEET);
            AddPossibleItem(Constants.BOK_CHOY);
            AddPossibleItem(Constants.YAM);
            AddPossibleItem(Constants.PUMPKIN);
            AddPossibleItem(Constants.SUNFLOWER);
            AddPossibleItem(Constants.GREEN_BEAN);
            AddPossibleItem(Constants.GRAPE);
            AddPossibleItem(Constants.APRICOT);
            AddPossibleItem(Constants.CHERRY);
            AddPossibleItem(Constants.POMEGRANATE);
            AddPossibleItem(Constants.APPLE);
            AddPossibleItem(Constants.ORANGE);
            AddPossibleItem(Constants.PEACH);
            AddPossibleItem(Constants.TULIP);
            AddPossibleItem(Constants.DANDELION);
            AddPossibleItem(Constants.DAFFODIL);
            AddPossibleItem(Constants.SWEET_PEA);
            AddPossibleItem(Constants.CROCUS);
            AddPossibleItem(Constants.LEEK);
            AddPossibleItem(Constants.STRAWBERRY);
            AddPossibleItem(Constants.COCONUT);
            AddPossibleItem(Constants.CACTUS_FRUIT);
            AddPossibleItem(Constants.WILD_HORSERADISH);
            AddPossibleItem(Constants.SPRING_ONION);
            AddPossibleItem(Constants.SALMONBERRY);
            AddPossibleItem(Constants.BLACKBERRY);
            AddPossibleItem(Constants.HAZELNUT);
            AddPossibleItem(Constants.WILD_PLUM);
            AddPossibleItem(Constants.CRYSTAL_FRUIT);
            AddPossibleItem(Constants.SNOW_YAM);
            AddPossibleItem(Constants.FIDDLEHEAD_FERN);
            AddPossibleItem(Constants.COMMON_MUSHROOM);
            AddPossibleItem(Constants.CHANTERELLE);
            AddPossibleItem(Constants.RED_MUSHROOM);
            AddPossibleItem(Constants.PURPLE_MUSHROOM);
            AddPossibleItem(Constants.MOREL);
            AddPossibleItem(Constants.CAVE_CARROT);
            AddPossibleItem(Constants.HOLLY);
            AddPossibleItem(Constants.WINTER_ROOT);

            //Add the rest of the cooked dishes
            AddPossibleItem(Constants.FRIED_EGG);
            AddPossibleItem(Constants.MAKI_ROLL);

            //Add the rest of Artisian
            AddPossibleItem(Constants.JELLY);
            AddPossibleItem(Constants.HONEY);
            AddPossibleItem(Constants.CHEESE);
            AddPossibleItem(Constants.GOAT_CHEESE);
            AddPossibleItem(Constants.LARGE_WHITE_EGG);
            AddPossibleItem(Constants.LARGE_BROWN_EGG);
            AddPossibleItem(Constants.DUCK_EGG);
            AddPossibleItem(Constants.LARGE_MILK);
            AddPossibleItem(Constants.LARGE_GOAT_MILK);
            AddPossibleItem(Constants.WINE);
            AddPossibleItem(Constants.TRUFFLE);
            AddPossibleItem(Constants.TRUFFLE_OIL);
            AddPossibleItem(Constants.CLOTH);
            AddPossibleItem(Constants.WOOL);
            AddPossibleItem(Constants.DUCK_FEATHER);
            AddPossibleItem(Constants.RABBITS_FOOT);

            //fish
            AddPossibleItem(Constants.SALMON);
            AddPossibleItem(Constants.PERCH);
            AddPossibleItem(Constants.SUNFISH);
            AddPossibleItem(Constants.CARP);
            AddPossibleItem(Constants.HALIBUT);
            AddPossibleItem(Constants.SARDINE);
            AddPossibleItem(Constants.BREAM);
            AddPossibleItem(Constants.TUNA);
            AddPossibleItem(Constants.RED_SNAPPER);
            AddPossibleItem(Constants.SMALLMOUTH_BASS);
            AddPossibleItem(Constants.PUFFERFISH);
            AddPossibleItem(Constants.LINGCOD);
            AddPossibleItem(Constants.TIGER_TROUT);
            AddPossibleItem(Constants.RAINBOW_TROUT);
            AddPossibleItem(Constants.LARGEMOUTH_BASS);
            AddPossibleItem(Constants.TILAPIA);
            AddPossibleItem(Constants.DORADO);
            AddPossibleItem(Constants.EEL);
            AddPossibleItem(Constants.PIKE);
            AddPossibleItem(Constants.SHAD);
            AddPossibleItem(Constants.ALBACORE);
            AddPossibleItem(Constants.ANCHOVY);
            AddPossibleItem(Constants.BULLHEAD);
            AddPossibleItem(Constants.WALLEYE);
            AddPossibleItem(Constants.WOODSKIP);
            AddPossibleItem(Constants.STURGEON);
            AddPossibleItem(Constants.CATFISH);
            //scorpian fish
            AddPossibleItem(Constants.SANDFISH);
            AddPossibleItem(Constants.GHOSTFISH);
            AddPossibleItem(Constants.SEA_CUCUMBER);
            AddPossibleItem(Constants.SUPER_CUCUMBER);
            AddPossibleItem(Constants.OCTOPUS);
            AddPossibleItem(Constants.SQUID);
            AddPossibleItem(Constants.LOBSTER);
            AddPossibleItem(Constants.CRAYFISH);
            AddPossibleItem(Constants.CRAB);
            AddPossibleItem(Constants.SHRIMP);
            AddPossibleItem(Constants.SNAIL);
            AddPossibleItem(Constants.OYSTER);
            AddPossibleItem(Constants.MUSSEL);
            AddPossibleItem(Constants.CLAM);
            AddPossibleItem(Constants.COCKLE);
            AddPossibleItem(Constants.RAINBOW_SHELL);
            AddPossibleItem(Constants.NAUTILUS_SHELL);
            //coral
            AddPossibleItem(Constants.SEA_URCHIN);
            AddPossibleItem(Constants.RED_MULLET);
            AddPossibleItem(Constants.HERRING);
            AddPossibleItem(Constants.CHUB);
            AddPossibleItem(Constants.PERIWINKLE);

            //bait and tackle

            //crafting, monster loot and misc
            AddPossibleItem(Constants.PINE_TAR);
            AddPossibleItem(Constants.OAK_RESIN);
            AddPossibleItem(Constants.MAPLE_SYRUP);
            AddPossibleItem(Constants.SLIME);
            AddPossibleItem(Constants.BAT_WING);
            AddPossibleItem(Constants.VOID_ESSENCE);
            AddPossibleItem(Constants.SOLAR_ESSENCE);
            AddPossibleItem(Constants.COPPER_BAR);
            AddPossibleItem(Constants.IRON_BAR);
            AddPossibleItem(Constants.GOLD_BAR);
            AddPossibleItem(Constants.WOOD);
            AddPossibleItem(Constants.STONE);
            AddPossibleItem(Constants.HARDWOOD);

            //furniture
        }

        private void AddPossibleItem(int parentSheetIndex)
        {
            if (!AllPossibleMerchantItems.ContainsKey(parentSheetIndex))
            {
                AllPossibleMerchantItems.Add(parentSheetIndex, new SObject(parentSheetIndex, 1));
            }
        }
    }
}
