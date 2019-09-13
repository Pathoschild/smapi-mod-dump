using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatAreYouMissing
{
    public class Constants
    {
        //Spring First
        public const int BLUE_JAZZ = 597;
        public const int CAULIFLOWER = 190;
        public const int GARLIC = 248;
        public const int KALE = 250;
        public const int PARSNIP = 24;
        public const int POTATO = 192;
        public const int RHUBARB = 252;
        public const int TULIP = 591;
        public const int COFFEE_BEAN = 433;
        public const int GREEN_BEAN = 188;
        public const int STRAWBERRY = 400;

        public const int WILD_HORSERADISH = 16;
        public const int DAFFODIL = 18;
        public const int LEEK = 20;
        public const int DANDELION = 22;
        public const int SPRING_ONION = 399;
        public const int MOREL = 257;
        public const int COMMON_MUSHROOM = 404;
        public const int SALMONBERRY = 296;

        public const int ANCHOVY = 129;
        public const int SMALLMOUTH_BASS = 137;
        public const int CATFISH = 143;
        public const int SUNFISH = 145;
        public const int HERRING = 147;
        public const int EEL = 148;
        public const int SARDINE = 131;
        public const int SHAD = 706;
        public const int HALIBUT = 708;
        public const int LEGEND = 163;

        //Summer minus common crops from spring
        public const int MELON = 254;
        public const int POPPY = 376;
        public const int RADISH = 264;
        public const int RED_CABBAGE = 266;
        public const int STARFRUIT = 268;
        public const int SUMMER_SPANGLE = 593;
        public const int SUNFLOWER = 421;
        public const int WHEAT = 262;
        public const int BLUEBERRY = 258;
        public const int CORN = 270;
        public const int HOPS = 304;
        public const int HOT_PEPPER = 260;
        public const int TOMATO = 256;

        public const int RAINBOW_SHELL = 394;
        public const int SPICE_BERRY = 396;
        public const int GRAPE = 398;
        public const int SWEET_PEA = 402;
        public const int RED_MUSHROOM = 420;
        public const int FIDDLEHEAD_FERN = 259;

        public const int PUFFERFISH = 128;
        public const int TUNA = 130;
        public const int RAINBOW_TROUT = 138;
        public const int PIKE = 144;
        public const int RED_MULLET = 146;
        public const int OCTOPUS = 149;
        public const int RED_SNAPPER = 150;
        public const int SUPER_CUCUMBER = 155;
        public const int STURGEON = 698;
        public const int TILAPIA = 701;
        public const int DORADO = 704;
        public const int CRIMSONFISH = 159;

        //Fall 
        public const int AMARANTH = 300;
        public const int ARTICHOKE = 274;
        public const int BEET = 284;
        public const int BOK_CHOY = 278;
        public const int FAIRY_ROSE = 595;
        public const int PUMPKIN = 276;
        public const int SWEET_GEM_BERRY = 417;
        public const int YAM = 280;
        public const int CRANBERRIES = 282;
        public const int EGGPLANT = 272;

        public const int WILD_PLUM = 406;
        public const int HAZELNUT = 408;
        public const int BLACKBERRY = 410;
        public const int CHANTERELLE = 281;

        public const int SALMON = 139;
        public const int WALLEYE = 140;
        public const int SEA_CUCUMBER = 154;
        public const int TIGER_TROUT = 699;
        public const int ALBACORE = 705;
        public const int ANGLER = 160;

        //Winter
        public const int NAUTILUS_SHELL = 392;
        public const int WINTER_ROOT = 412;
        public const int CRYSTAL_FRUIT = 414;
        public const int SNOW_YAM = 416;
        public const int CROCUS = 418;
        public const int HOLLY = 283;

        public const int PERCH = 141;
        public const int SQUID = 151;
        public const int LINGCOD = 707;
        public const int GLACIERFISH = 775;
        public const int MIDNIGHT_SQUID = 798;
        public const int SPOOK_FISH = 799;
        public const int BLOBFISH = 800;

        public const int ANCIENT_FRUIT = 454;


        //Seeds
        public const int AMARANTH_SEEDS = 299;
        public const int GRAPE_STARTER = 301;
        public const int HOPS_STARTER = 302;
        public const int RARE_SEED = 347;
        public const int FAIRY_SEEDS = 425;
        public const int TULIP_BULB = 427;
        public const int JAZZ_SEEDS = 429;
        public const int SUNFLOWER_SEEDS = 431;
        public const int POPPY_SEEDS = 453;
        public const int SPANGLE_SEEDS = 455;
        public const int PARSNIP_SEEDS = 472;
        public const int BEAN_STARTER = 473;
        public const int CAULIFLOWER_SEEDS = 474;
        public const int POTATO_SEEDS = 475;
        public const int GARLIC_SEEDS = 476;
        public const int KALE_SEEDS = 477;
        public const int RHUBARB_SEEDS = 478;
        public const int MELON_SEEDS = 479;
        public const int TOMATO_SEEDS = 480;
        public const int BLUEBERRY_SEEDS = 481;
        public const int PEPPER_SEEDS = 482;
        public const int WHEAT_SEEDS = 483;
        public const int RADISH_SEEDS = 484;
        public const int RED_CABBAGE_SEEDS = 485;
        public const int STARFRUIT_SEEDS = 486;
        public const int CORN_SEEDS = 487;
        public const int EGGPLANT_SEEDS = 488;
        public const int ARTICHOKE_SEEDS = 489;
        public const int PUMPKIN_SEEDS = 490;
        public const int BOK_CHOY_SEEDS = 491;
        public const int YAM_SEEDS = 492;
        public const int CRANBERRY_SEEDS = 493;
        public const int BEET_SEEDS = 494;
        public const int SPRING_SEEDS = 495;
        public const int SUMMER_SEEDS = 496;
        public const int FALL_SEEDS = 497;
        public const int WINTER_SEEDS = 498;
        public const int ANCIENT_SEEDS = 499;
        public const int STRAWBERRY_SEEDS = 745;
        public const int CACTUS_SEEDS = 802;

        //Other Items from the community center bundles
        public const int WOOD = 388;
        public const int STONE = 390;
        public const int HARDWOOD = 709;
        public const int COCONUT = 88;
        public const int CACTUS_FRUIT = 90;
        public const int CAVE_CARROT = 78;
        public const int PURPLE_MUSHROOM = 422;
        public const int PINE_TAR = 726;
        public const int LARGE_MILK = 186;
        public const int LARGE_WHITE_EGG = 174;
        public const int LARGE_BROWN_EGG = 182;
        public const int LARGE_GOAT_MILK = 438;
        public const int WOOL = 440;
        public const int DUCK_EGG = 442;
        public const int TRUFFLE_OIL = 432;
        public const int CLOTH = 428;
        public const int GOAT_CHEESE = 426;
        public const int CHEESE = 424;
        public const int HONEY = 340;
        public const int JELLY = 344;
        public const int APPLE = 613;
        public const int APRICOT = 634;
        public const int ORANGE = 635;
        public const int PEACH = 636;
        public const int POMEGRANATE = 637;
        public const int CHEERY = 638;
        public const int LARGEMOUTH_BASS = 136;
        public const int CARP = 142;
        public const int BULLHEAD = 700;
        public const int BREAM = 132;
        public const int LOBSTER = 715;
        public const int CRAYFISH = 716;
        public const int CRAB = 717;
        public const int COCKLE = 718;
        public const int MUSSEL = 719;
        public const int SHRIMP = 720;
        public const int SNAIL = 721;
        public const int PERIWINKLE = 722;
        public const int OYSTER = 723;
        public const int CLAM = 372;
        public const int GHOSTFISH = 156;
        public const int SANDFISH = 164;
        public const int WOODSKIP = 734;
        public const int COPPER_BAR = 334;
        public const int IRON_BAR = 335;
        public const int GOLD_BAR = 336;
        public const int QUARTZ = 80;
        public const int EARTH_CRYSTAL = 86;
        public const int FROZEN_TEAR = 84;
        public const int FIRE_QUARTZ = 82;
        public const int SLIME = 766;
        public const int BAT_WING = 767;
        public const int SOLAR_ESSENCE = 768;
        public const int VOID_ESSENCE = 769;
        public const int MAPLE_SYRUP = 724;
        public const int TRUFFLE = 430;
        public const int MAKI_ROLL = 228;
        public const int FRIED_EGG = 194;
        public const int SEA_URCHIN = 397;
        public const int DUCK_FEATHER = 444;
        public const int AQUAMARINE = 62;
        public const int CHUB = 702;
        public const int FROZEN_GEODE = 536;
        public const int HAY = 178;
        public const int OAK_RESIN = 725;
        public const int WINE = 348; //basic wine
        public const int RABBITS_FOOT = 446;
        //public const int GOLD_2500 = ; //Not sure about this 

        public const int SPICY_EEL = 226;

        public const int COMMON_QUALITY = 0;
        public const int SILVER_QUALITY = 1;
        public const int GOLD_QUALITY = 2;
        public const int IRIDIUM_QUALITY = 4;

        public const int CINDERSAP_POND_AREA_CODE = 1;
        public const int CINDERSAP_RIVER_AREA_CODE = 0;
        public const int DEFAULT_AREA_CODE = -1;

        public const int SPRITE_SIZE = 64;
        public const int MENU_WIDTH = 880;

        public const string SEASON_SPECIFIC_HEADER = "Season Items";
        public const string SEASON_SPECIFIC_CC_HEADER = "Season Community Center Items";
        public const string COMMON_CC_HEADER = "Common Community Center Items";
        public const string MERCHANT_HEADER = "Travelling Cart Items";
        public const string COOKED_ITEMS_HEADER = "Cooked Items";

        public const string SEASON_SPECIFIC_DESCRIPTION = "Missing items specific to this season";
        public const string SEASON_SPECIFIC_CC_DESCRIPTION = "Missing Community Center items specific to this season";
        public const string COMMON_CC_DESCRIPTION = "Missing Community Center items common to all seasons";
        public const string MERCHANT_DESCRIPTION = "Missing uncommon items that are available from the Travelling Cart";
        public const string COOKED_ITEMS_DESCRIPTION = "Missing cooked items that you know how to make";

        public const string EMPTY_LIST_MESSAGE = "There is nothing you are missing that is available";

        public const string GAME_NAME_MINES = "UndergroundMine";
        public const string GAME_NAME_DESERT = "Desert";
        public const string GAME_NAME_CINDERSAP_FOREST = "Forest";
        public const string GAME_NAME_TOWN_RIVER = "Town";
        public const string GAME_NAME_MOUNTAIN_LAKE = "Mountain"; //same thing?
        public const string GAME_NAME_MOUNTAIN_LAKE_2 = "Backwoods"; //same thing?
        public const string GAME_NAME_BEACH = "Beach";
        public const string GAME_NAME_SECRET_WOODS = "Woods";
        public const string GAME_NAME_SEWER = "Sewer";
        public const string GAME_NAME_MUTANT_BUG_LAIR = "BugLand";
        public const string GAME_NAME_WITCHS_SWAMP = "WitchSwamp";

        public const string MINES_DISPLAY_NAME = "Mines";
        public const string DESERT_DISPLAY_NAME = "Desert";
        public const string CINDERSAP_RIVER_DISPLAY_NAME = "Cindersap River";
        public const string CINDERSAP_POND_DISPLAY_NAME = "Cindersap Pond";
        public const string FOREST_FARM_POND_DIAPLAY_NAME = "Forest Farm Pond";
        public const string TOWN_RIVER_DISPLAY_NAME = "Town River";
        public const string MOUNTAIN_LAKE_DISPLAY_NAME = "Mountain Lake";
        public const string BEACH_DISPLAY_NAME = "Beach";
        public const string SECRET_WOODS_DISPLAY_NAME = "Secret Woods";
        public const string SEWER_DISPLAY_NAME = "Sewer";
        public const string MUTANT_BUG_LAIR_DISPLAY_NAME = "Mutant Bug Lair";
        public const string WITCHS_SWAMP_DISPLAY_NAME = "Witch's Swamp";

        public const string NIGHT_MARKET = "Night Market Sub";

        public const string OBTAINED_FROM_CRAB_POT_IN_OCEAN = "Obtained from a crab pot in the ocean";
        public const string OBTAINED_FROM_CRAB_POT_IN_FRESHWATER = "Obtained from a crab pot in freshwater";

        public const string TEXT_TO_HOVER_OVER_FOR_INFO = "Info";

        public const string ANY_WEATHER = "Any Weather";
        public const string ANY_EGG = "Egg (Any)";
        public const string ANY_MILK = "Milk (Any)";
        public const string ANY_FISH = "Fish (Any)";
        public const string ANYTIME = "Anytime";
        public const string INGREDIENTS = "Ingredients";

        public readonly ReadOnlyDictionary<int, string> OBTAIN_LEGENDARY_FISH = new ReadOnlyDictionary<int, string>(
                                                new Dictionary<int, string>()
                                                {
                                                    [LEGEND] = "Fish close to the giant log in the Mountain Lake when its raining",
                                                    [GLACIERFISH] = "At the bottom of the giant log in the Mountain Lake when its raining",
                                                    [CRIMSONFISH] = "Second Beach Pier",
                                                    [ANGLER] = "Fish upwards on the small bridge near JoJo Mart"
                                                });

        public readonly ReadOnlyCollection<int> LEGENDARY_FISH = new ReadOnlyCollection<int>(
                                                new List<int> { LEGEND, GLACIERFISH, CRIMSONFISH, ANGLER });

        public readonly ReadOnlyCollection<int> ITEMS_PLAYER_CAN_ONLY_HAVE_ONE_OF = new ReadOnlyCollection<int>(
                                                new List<int> { GLACIERFISH, CRIMSONFISH, ANGLER });

        public readonly ReadOnlyCollection<int> NIGHT_MARKET_FISH = new ReadOnlyCollection<int>(
                                                new List<int> { MIDNIGHT_SQUID, SPOOK_FISH, BLOBFISH });

        public readonly ReadOnlyCollection<int> SPECIAL_COOKING_IDS = new ReadOnlyCollection<int>(
                                                new List<int> { EGG_CATEGORY, MILK_CATEGORY, FISH_CATEGORY });
        //categories
        public const int EGG_CATEGORY = -5;
        public const int MILK_CATEGORY = -6;
        public const int FISH_CATEGORY = -4;
    }
}
