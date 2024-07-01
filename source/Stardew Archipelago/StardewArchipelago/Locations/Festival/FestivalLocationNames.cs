/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace StardewArchipelago.Locations.Festival
{
    public class FestivalLocationNames
    {
        public static readonly string EGG_FESTIVAL = FestivalIdentifier(Season.Spring, 13);
        public static readonly string FLOWER_DANCE = FestivalIdentifier(Season.Spring, 24);
        public static readonly string LUAU = FestivalIdentifier(Season.Summer, 11);
        public static readonly string MOONLIGHT_JELLIES = FestivalIdentifier(Season.Summer, 28);
        public static readonly string FAIR = FestivalIdentifier(Season.Fall, 16);
        public static readonly string SPIRIT_EVE = FestivalIdentifier(Season.Fall, 27);
        public static readonly string FESTIVAL_OF_ICE = FestivalIdentifier(Season.Winter, 8);
        public static readonly string FEAST_OF_THE_WINTER_STAR = FestivalIdentifier(Season.Winter, 25);
        public static readonly string NIGHT_MARKET_15 = FestivalIdentifier(Season.Winter, 15);
        public static readonly string NIGHT_MARKET_16 = FestivalIdentifier(Season.Winter, 16);
        public static readonly string NIGHT_MARKET_17 = FestivalIdentifier(Season.Winter, 17);
        public static readonly string DESERT_FESTIVAL_15 = FestivalIdentifier(Season.Spring, 15);
        public static readonly string DESERT_FESTIVAL_16 = FestivalIdentifier(Season.Spring, 16);
        public static readonly string DESERT_FESTIVAL_17 = FestivalIdentifier(Season.Spring, 17);
        public static readonly string TROUT_DERBY_20 = FestivalIdentifier(Season.Summer, 20);
        public static readonly string TROUT_DERBY_21 = FestivalIdentifier(Season.Summer, 21);
        public static readonly string SQUIDFEST_12 = FestivalIdentifier(Season.Winter, 12);
        public static readonly string SQUIDFEST_13 = FestivalIdentifier(Season.Winter, 13);


        public const string EGG_HUNT = "Egg Hunt Victory";
        public const string STRAWBERRY_SEEDS = "Egg Festival: Strawberry Seeds";

        public const string CALICO_RACE = "Calico Race";
        public const string MUMMY_MASK = "Mummy Mask";
        public const string CALICO_STATUE = "Calico Statue";
        public const string EMILYS_OUTFIT_SERVICES = "Emily's Outfit Services";
        public const string EARTHY_MOUSSE = "Earthy Mousse";
        public const string SWEET_BEAN_CAKE = "Sweet Bean Cake";
        public const string SKULL_CAVERN_CASSEROLE = "Skull Cavern Casserole";
        public const string SPICY_TACOS = "Spicy Tacos";
        public const string MOUNTAIN_CHILI = "Mountain Chili";
        public const string CRYSTAL_CAKE = "Crystal Cake";
        public const string CAVE_KEBAB = "Cave Kebab";
        public const string HOT_LOG = "Hot Log";
        public const string SOUR_SALAD = "Sour Salad";
        public const string SUPERFOOD_CAKE = "Superfood Cake";
        public const string WARRIOR_SMOOTHIE = "Warrior Smoothie";
        public const string RUMPLED_FRUIT_SKIN = "Rumpled Fruit Skin";
        public const string CALICO_PIZZA = "Calico Pizza";
        public const string STUFFED_MUSHROOMS = "Stuffed Mushrooms";
        public const string ELF_QUESADILLA = "Elf Quesadilla";
        public const string NACHOS_OF_THE_DESERT = "Nachos Of The Desert";
        public const string CLOPPINO = "Cloppino";
        public const string RAINFOREST_SHRIMP = "Rainforest Shrimp";
        public const string SHRIMP_DONUT = "Shrimp Donut";
        public const string SMELL_OF_THE_SEA = "Smell Of The Sea";
        public const string DESERT_GUMBO = "Desert Gumbo";
        public const string FREE_CACTIS = "Free Cactis";
        public const string MONSTER_HUNT = "Monster Hunt";
        public const string DEEP_DIVE = "Deep Dive";
        public const string TREASURE_HUNT = "Treasure Hunt";
        public const string TOUCH_A_CALICO_STATUE = "Touch A Calico Statue";
        public const string REAL_CALICO_EGG_HUNTER = "Real Calico Egg Hunter";
        public const string WILLYS_CHALLENGE = "Willy's Challenge";
        public const string DESERT_SCHOLAR = "Desert Scholar";

        public const string DANCE_WITH_SOMEONE = "Dance with someone";
        public const string RARECROW_5 = "Rarecrow #5 (Woman)";
        public const string TUB_O_FLOWERS_RECIPE = "Tub o' Flowers Recipe";

        public const string LUAU_SOUP = "Luau Soup";

        public const string TROUT_DERBY_REWARD_PATTERN = "Trout Derby Reward {0}";
        public static readonly string[] TROUT_DERBY_REWARDS = new[]
        {
            string.Format(TROUT_DERBY_REWARD_PATTERN, 1), string.Format(TROUT_DERBY_REWARD_PATTERN, 2), string.Format(TROUT_DERBY_REWARD_PATTERN, 3),
            string.Format(TROUT_DERBY_REWARD_PATTERN, 4), string.Format(TROUT_DERBY_REWARD_PATTERN, 5), string.Format(TROUT_DERBY_REWARD_PATTERN, 6),
            string.Format(TROUT_DERBY_REWARD_PATTERN, 7), string.Format(TROUT_DERBY_REWARD_PATTERN, 8), string.Format(TROUT_DERBY_REWARD_PATTERN, 9),
            string.Format(TROUT_DERBY_REWARD_PATTERN, 10),
        };

        public const string WATCH_MOONLIGHT_JELLIES = "Dance of the Moonlight Jellies";
        public const string MOONLIGHT_JELLIES_BANNER = "Moonlight Jellies Banner";
        public const string STARPORT_DECAL = "Starport Decal";

        public const string STRENGTH_GAME = "Smashing Stone";
        public const string GRANGE_DISPLAY = "Grange Display";
        public const string RARECROW_1 = "Rarecrow #1 (Turnip Head)";
        public const string FAIR_STARDROP = "Fair Stardrop";

        public const string GOLDEN_PUMPKIN = "Spirit's Eve Maze";
        public const string RARECROW_2 = "Rarecrow #2 (Witch)";
        public const string JACK_O_LANTERN_RECIPE = "Jack-O-Lantern Recipe";

        public const string FISHING_COMPETITION = "Win Fishing Competition";
        public const string RARECROW_4 = "Rarecrow #4 (Snowman)";

        public const string SQUIDFEST_DAY_1_COPPER = "SquidFest Day 1 Copper";
        public const string SQUIDFEST_DAY_1_IRON = "SquidFest Day 1 Iron";
        public const string SQUIDFEST_DAY_1_GOLD = "SquidFest Day 1 Gold";
        public const string SQUIDFEST_DAY_1_IRIDIUM = "SquidFest Day 1 Iridium";
        public const string SQUIDFEST_DAY_2_COPPER = "SquidFest Day 2 Copper";
        public const string SQUIDFEST_DAY_2_IRON = "SquidFest Day 2 Iron";
        public const string SQUIDFEST_DAY_2_GOLD = "SquidFest Day 2 Gold";
        public const string SQUIDFEST_DAY_2_IRIDIUM = "SquidFest Day 2 Iridium";
        public static readonly string[,] SQUIDFEST_REWARDS = {
            { SQUIDFEST_DAY_1_COPPER, SQUIDFEST_DAY_1_IRON, SQUIDFEST_DAY_1_GOLD, SQUIDFEST_DAY_1_IRIDIUM },
            { SQUIDFEST_DAY_2_COPPER, SQUIDFEST_DAY_2_IRON, SQUIDFEST_DAY_2_GOLD, SQUIDFEST_DAY_2_IRIDIUM },
        };

        public const string MERMAID_PEARL = "Mermaid Pearl";
        public const string CONE_HAT = "Cone Hat";
        public const string IRIDIUM_FIREPLACE = "Iridium Fireplace";
        public const string RARECROW_7 = "Rarecrow #7 (Tanuki)";
        public const string RARECROW_8 = "Rarecrow #8 (Tribal Mask)";
        public const string RARECROW_3 = "Rarecrow #3 (Alien)";
        public const string RARECROW_6 = "Rarecrow #6 (Dwarf)";
        public const string LUPINI_YEAR_1_PAINTING_1 = "Lupini: Red Eagle";
        public const string LUPINI_YEAR_1_PAINTING_2 = "Lupini: Portrait Of A Mermaid";
        public const string LUPINI_YEAR_1_PAINTING_3 = "Lupini: Solar Kingdom";
        public const string LUPINI_YEAR_2_PAINTING_1 = "Lupini: Clouds";
        public const string LUPINI_YEAR_2_PAINTING_2 = "Lupini: 1000 Years From Now";
        public const string LUPINI_YEAR_2_PAINTING_3 = "Lupini: Three Trees";
        public const string LUPINI_YEAR_3_PAINTING_1 = "Lupini: The Serpent";
        public const string LUPINI_YEAR_3_PAINTING_2 = "Lupini: 'Tropical Fish #173'";
        public const string LUPINI_YEAR_3_PAINTING_3 = "Lupini: Land Of Clay";

        public const string LEGEND_OF_THE_WINTER_STAR = "The Legend of the Winter Star";
        public const string SECRET_SANTA = "Secret Santa";

        private static readonly string[] DESERT_FESTIVAL_ALL = new[]
        {
            CALICO_RACE, MUMMY_MASK, EMILYS_OUTFIT_SERVICES, EARTHY_MOUSSE, SWEET_BEAN_CAKE, SKULL_CAVERN_CASSEROLE, SPICY_TACOS,
            MOUNTAIN_CHILI, CRYSTAL_CAKE, CAVE_KEBAB, HOT_LOG, SOUR_SALAD, SUPERFOOD_CAKE, WARRIOR_SMOOTHIE, RUMPLED_FRUIT_SKIN, CALICO_PIZZA,
            STUFFED_MUSHROOMS, ELF_QUESADILLA, NACHOS_OF_THE_DESERT, CLOPPINO, RAINFOREST_SHRIMP, SHRIMP_DONUT, SMELL_OF_THE_SEA, DESERT_GUMBO,
            FREE_CACTIS, MONSTER_HUNT, DEEP_DIVE, TREASURE_HUNT, TOUCH_A_CALICO_STATUE, REAL_CALICO_EGG_HUNTER, WILLYS_CHALLENGE, DESERT_SCHOLAR,
        };
        private static readonly string[] TROUT_DERBY_ALL = TROUT_DERBY_REWARDS;
        private static readonly string[] SQUIDFEST_ALL = Array.Empty<string>();
        private static readonly string[] NIGHT_MARKET_ALL = new[] { MERMAID_PEARL, CONE_HAT, IRIDIUM_FIREPLACE, RARECROW_7, RARECROW_8, };

        public static readonly Dictionary<string, string[]> LocationsByFestival = new()
        {
            { EGG_FESTIVAL, new[] { EGG_HUNT, STRAWBERRY_SEEDS } },
            { DESERT_FESTIVAL_15, DESERT_FESTIVAL_ALL.Union(new[] { CALICO_STATUE, }).ToArray() },
            { DESERT_FESTIVAL_16, DESERT_FESTIVAL_ALL },
            { DESERT_FESTIVAL_17, DESERT_FESTIVAL_ALL.Union(new[] { CALICO_STATUE, }).ToArray() },
            { FLOWER_DANCE, new[] { DANCE_WITH_SOMEONE, RARECROW_5, TUB_O_FLOWERS_RECIPE } },
            { LUAU, new[] { LUAU_SOUP } },
            { TROUT_DERBY_20, TROUT_DERBY_ALL },
            { TROUT_DERBY_21, TROUT_DERBY_ALL },
            { MOONLIGHT_JELLIES, new[] { WATCH_MOONLIGHT_JELLIES, MOONLIGHT_JELLIES_BANNER, STARPORT_DECAL } },
            { FAIR, new[] { STRENGTH_GAME, RARECROW_1, FAIR_STARDROP, GRANGE_DISPLAY } },
            { SPIRIT_EVE, new[] { GOLDEN_PUMPKIN, RARECROW_2, JACK_O_LANTERN_RECIPE } },
            { FESTIVAL_OF_ICE, new[] { FISHING_COMPETITION, RARECROW_4 } },
            { SQUIDFEST_12, SQUIDFEST_ALL.Union(new[] { SQUIDFEST_DAY_1_COPPER, SQUIDFEST_DAY_1_IRON, SQUIDFEST_DAY_1_GOLD, SQUIDFEST_DAY_1_IRIDIUM }).ToArray()  },
            { SQUIDFEST_13, SQUIDFEST_ALL.Union(new[] { SQUIDFEST_DAY_2_COPPER, SQUIDFEST_DAY_2_IRON, SQUIDFEST_DAY_2_GOLD, SQUIDFEST_DAY_2_IRIDIUM }).ToArray()  },
            { FEAST_OF_THE_WINTER_STAR, new[] { LEGEND_OF_THE_WINTER_STAR, SECRET_SANTA } },
            { NIGHT_MARKET_15, NIGHT_MARKET_ALL.Union(new[] { LUPINI_YEAR_1_PAINTING_1, LUPINI_YEAR_2_PAINTING_1, LUPINI_YEAR_3_PAINTING_1 }).ToArray() },
            { NIGHT_MARKET_16, NIGHT_MARKET_ALL.Union(new[] { LUPINI_YEAR_1_PAINTING_2, LUPINI_YEAR_2_PAINTING_2, LUPINI_YEAR_3_PAINTING_2 }).ToArray() },
            { NIGHT_MARKET_17, NIGHT_MARKET_ALL.Union(new[] { LUPINI_YEAR_1_PAINTING_3, LUPINI_YEAR_2_PAINTING_3, LUPINI_YEAR_3_PAINTING_3 }).ToArray() },
        };

        public static string FestivalIdentifier(Season season, int day)
        {
            return $"{season} {day}";
        }
    }
}
