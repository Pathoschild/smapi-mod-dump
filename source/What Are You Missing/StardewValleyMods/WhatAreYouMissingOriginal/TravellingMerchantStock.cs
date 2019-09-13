using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace WhatAreYouMissingOriginal
{
    class TravellingMerchantStock : SpecificItems
    {
        public List<int> data { get; set; }
        public List<int> availableStock { get; set; }
        private List<int> merchantStock;
        private IModHelper Helper;
        public TravellingMerchantStock(IModHelper helper,
            Dictionary<int, string> data1, 
            Dictionary<int, string> data2, 
            Dictionary<int, string> data3, 
            Dictionary<int, string> data4,
            Dictionary<int, string> data5)
        {
            Helper = helper;
         
            availableStock = new List<int>();
            AddAvailableMerchantItems(data1, data2, data3, data4, data5);
        
            //data = new List<int>();
            //UpdateData();
            
        }
        public TravellingMerchantStock()
        {
            data = new List<int>();
            availableStock = new List<int>();
            UpdateData();

        }
        


        public void AddAvailableMerchantItems(Dictionary<int, string> data1, Dictionary<int, string> data2, Dictionary<int, string> data3, Dictionary<int, string> data4, Dictionary<int, string> data5)
        {
            merchantStock = Helper.Data.ReadJsonFile<TravellingMerchantStock>("TravellingMerchantStock.json").data;
            foreach (int item in merchantStock)
            {
                if (CheckMerchantForItemAndSeed(item))
                {
                    if (data1.ContainsKey(item) || data2.ContainsKey(item) || data3.ContainsKey(item) || data4.ContainsKey(item) || data5.ContainsKey(item))
                    {
                        if (!availableStock.Contains(item))
                        {
                            availableStock.Add(item);
                        }

                    }
                }
            }
        }

        public override void UpdateData()
        {
            data.Add(Constants.RARE_SEED);
            //add snow rarecrow
            data.Add(Constants.COFFEE_BEAN);
            //add wedding ring for multiplayer
            //add fertilizers
            data.Add(Constants.SPRING_SEEDS);
            data.Add(Constants.GARLIC_SEEDS);
            data.Add(Constants.POTATO_SEEDS);
            data.Add(Constants.CAULIFLOWER_SEEDS);
            data.Add(Constants.RHUBARB_SEEDS);
            data.Add(Constants.PARSNIP_SEEDS);
            data.Add(Constants.KALE_SEEDS);
            data.Add(Constants.JAZZ_SEEDS);
            data.Add(Constants.TULIP_BULB);
            data.Add(Constants.SUMMER_SEEDS);
            data.Add(Constants.CORN_SEEDS);
            data.Add(Constants.WHEAT_SEEDS);
            data.Add(Constants.SPANGLE_SEEDS);
            data.Add(Constants.POPPY_SEEDS);
            data.Add(Constants.TOMATO_SEEDS);
            data.Add(Constants.MELON_SEEDS);
            data.Add(Constants.RADISH_SEEDS);
            data.Add(Constants.RED_CABBAGE_SEEDS);
            data.Add(Constants.BLUEBERRY_SEEDS);
            data.Add(Constants.STARFRUIT_SEEDS);
            data.Add(Constants.PEPPER_SEEDS);
            data.Add(Constants.FAIRY_SEEDS);
            data.Add(Constants.AMARANTH_SEEDS);
            data.Add(Constants.ARTICHOKE_SEEDS);
            data.Add(Constants.CRANBERRY_SEEDS);
            data.Add(Constants.EGGPLANT_SEEDS);
            data.Add(Constants.BEET_SEEDS);
            data.Add(Constants.BOK_CHOY_SEEDS);
            data.Add(Constants.YAM_SEEDS);
            data.Add(Constants.PUMPKIN_SEEDS);
            data.Add(Constants.SUNFLOWER_SEEDS);
            data.Add(Constants.FALL_SEEDS);
            data.Add(Constants.WINTER_SEEDS);
            data.Add(Constants.BEAN_STARTER);
            data.Add(Constants.HOPS_STARTER);
            data.Add(Constants.GRAPE_STARTER);
            //Add saplings

            data.Add(Constants.GARLIC);
            data.Add(Constants.POTATO);
            data.Add(Constants.CAULIFLOWER);
            data.Add(Constants.RHUBARB);
            data.Add(Constants.PARSNIP);
            data.Add(Constants.SWEET_GEM_BERRY);
            data.Add(Constants.KALE);
            data.Add(Constants.BLUE_JAZZ);
            data.Add(Constants.CORN);
            data.Add(Constants.HOPS);
            data.Add(Constants.WHEAT);
            data.Add(Constants.SUMMER_SPANGLE);
            data.Add(Constants.POPPY);
            data.Add(Constants.TOMATO);
            data.Add(Constants.MELON);
            data.Add(Constants.RADISH);
            data.Add(Constants.RED_CABBAGE);
            data.Add(Constants.BLUEBERRY);
            data.Add(Constants.STARFRUIT);
            data.Add(Constants.HOT_PEPPER);
            data.Add(Constants.FAIRY_ROSE);
            data.Add(Constants.SPICE_BERRY);
            data.Add(Constants.AMARANTH);
            data.Add(Constants.ARTICHOKE);
            data.Add(Constants.CRANBERRIES);
            data.Add(Constants.EGGPLANT);
            data.Add(Constants.BEET);
            data.Add(Constants.BOK_CHOY);
            data.Add(Constants.YAM);
            data.Add(Constants.PUMPKIN);
            data.Add(Constants.SUNFLOWER);
            data.Add(Constants.GREEN_BEAN);
            data.Add(Constants.GRAPE);
            data.Add(Constants.APRICOT);
            data.Add(Constants.CHEERY);
            data.Add(Constants.POMEGRANATE);
            data.Add(Constants.APPLE);
            data.Add(Constants.ORANGE);
            data.Add(Constants.PEACH);
            data.Add(Constants.TULIP);
            data.Add(Constants.DANDELION);
            data.Add(Constants.DAFFODIL);
            data.Add(Constants.SWEET_PEA);
            data.Add(Constants.CROCUS);
            data.Add(Constants.LEEK);
            data.Add(Constants.STRAWBERRY);
            data.Add(Constants.COCONUT);
            data.Add(Constants.CACTUS_FRUIT);
            data.Add(Constants.WILD_HORSERADISH);
            data.Add(Constants.SPRING_ONION);
            data.Add(Constants.SALMONBERRY);
            data.Add(Constants.BLACKBERRY);
            data.Add(Constants.HAZELNUT);
            data.Add(Constants.WILD_PLUM);
            data.Add(Constants.CRYSTAL_FRUIT);
            data.Add(Constants.SNOW_YAM);
            data.Add(Constants.FIDDLEHEAD_FERN);
            data.Add(Constants.COMMON_MUSHROOM);
            data.Add(Constants.CHANTERELLE);
            data.Add(Constants.RED_MUSHROOM);
            data.Add(Constants.PURPLE_MUSHROOM);
            data.Add(Constants.MOREL);
            data.Add(Constants.CAVE_CARROT);
            data.Add(Constants.HOLLY);
            data.Add(Constants.WINTER_ROOT);

            //Add the rest of the cooked dishes
            data.Add(Constants.FRIED_EGG);
            data.Add(Constants.MAKI_ROLL);

            //Add the rest of Artisian
            data.Add(Constants.JELLY);
            data.Add(Constants.HONEY);
            data.Add(Constants.CHEESE);
            data.Add(Constants.GOAT_CHEESE);
            data.Add(Constants.LARGE_WHITE_EGG);
            data.Add(Constants.LARGE_BROWN_EGG);
            data.Add(Constants.DUCK_EGG);
            data.Add(Constants.LARGE_MILK);
            data.Add(Constants.LARGE_GOAT_MILK);
            data.Add(Constants.WINE);
            data.Add(Constants.TRUFFLE);
            data.Add(Constants.TRUFFLE_OIL);
            data.Add(Constants.CLOTH);
            data.Add(Constants.WOOL);
            data.Add(Constants.DUCK_FEATHER);
            data.Add(Constants.RABBITS_FOOT);

            //fish
            data.Add(Constants.SALMON);
            data.Add(Constants.PERCH);
            data.Add(Constants.SUNFISH);
            data.Add(Constants.CARP);
            data.Add(Constants.HALIBUT);
            data.Add(Constants.SARDINE);
            data.Add(Constants.BREAM);
            data.Add(Constants.TUNA);
            data.Add(Constants.RED_SNAPPER);
            data.Add(Constants.SMALLMOUTH_BASS);
            data.Add(Constants.PUFFERFISH);
            data.Add(Constants.LINGCOD);
            data.Add(Constants.TIGER_TROUT);
            data.Add(Constants.RAINBOW_TROUT);
            data.Add(Constants.LARGEMOUTH_BASS);
            data.Add(Constants.TILAPIA);
            data.Add(Constants.DORADO);
            data.Add(Constants.EEL);
            data.Add(Constants.PIKE);
            data.Add(Constants.SHAD);
            data.Add(Constants.ALBACORE);
            data.Add(Constants.ANCHOVY);
            data.Add(Constants.BULLHEAD);
            data.Add(Constants.WALLEYE);
            data.Add(Constants.WOODSKIP);
            data.Add(Constants.STURGEON);
            data.Add(Constants.CATFISH);
            //scorpian fish
            data.Add(Constants.SANDFISH);
            data.Add(Constants.GHOSTFISH);
            data.Add(Constants.SEA_CUCUMBER);
            data.Add(Constants.SUPER_CUCUMBER);
            data.Add(Constants.OCTOPUS);
            data.Add(Constants.SQUID);
            data.Add(Constants.LOBSTER);
            data.Add(Constants.CRAYFISH);
            data.Add(Constants.CRAB);
            data.Add(Constants.SHRIMP);
            data.Add(Constants.SNAIL);
            data.Add(Constants.OYSTER);
            data.Add(Constants.MUSSEL);
            data.Add(Constants.CLAM);
            data.Add(Constants.COCKLE);
            data.Add(Constants.RAINBOW_SHELL);
            data.Add(Constants.NAUTILUS_SHELL);
            //coral
            data.Add(Constants.SEA_URCHIN);
            data.Add(Constants.RED_MULLET);
            data.Add(Constants.HERRING);
            data.Add(Constants.CHUB);
            data.Add(Constants.PERIWINKLE);

            //bait and tackle

            //crafting, monster loot and misc
            data.Add(Constants.PINE_TAR);
            data.Add(Constants.OAK_RESIN);
            data.Add(Constants.MAPLE_SYRUP);
            data.Add(Constants.SLIME);
            data.Add(Constants.BAT_WING);
            data.Add(Constants.VOID_ESSENCE);
            data.Add(Constants.SOLAR_ESSENCE);
            data.Add(Constants.COPPER_BAR);
            data.Add(Constants.IRON_BAR);
            data.Add(Constants.GOLD_BAR);
            data.Add(Constants.WOOD);
            data.Add(Constants.STONE);
            data.Add(Constants.HARDWOOD);

            //furniture
        }


    }
}
