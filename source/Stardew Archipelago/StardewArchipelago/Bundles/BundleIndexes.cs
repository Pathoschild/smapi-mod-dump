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
using System.Text;
using System.Threading.Tasks;

namespace StardewArchipelago.Bundles
{
    internal class BundleIndexes
    {
        // Ids above 100 are custom and not based in vanilla content
        public static readonly Dictionary<string, int> BundleSpriteIndexes = new()
        {
            { "Spring Crops", 0 },
            { "Summer Crops", 1 },
            { "Fall Crops", 2 },
            { "Quality Crops", 3 },
            { "Animal", 4 },
            { "Artisan", 5 },
            { "Spring Foraging", 13 },
            { "Summer Foraging", 14 },
            { "Fall Foraging", 15 },
            { "Winter Foraging", 16 },
            { "Construction", 17 },
            { "Exotic Foraging", 19 },
            { "River Fish", 6 },
            { "Lake Fish", 7 },
            { "Ocean Fish", 8 },
            { "Night Fishing", 9 },
            { "Specialty Fish", 10 },
            { "Crab Pot", 11 },
            { "Blacksmith's", 20 },
            { "Geologist's", 21 },
            { "Adventurer's", 22 },
            { "Chef's", 31 },
            { "Field Research", 32 },
            { "Enchanter's", 33 },
            { "Dye", 34 },
            { "Fodder", 35 },
            { "The Missing", 36 },

            {"Rare Crops", 103},
            {"Garden", 107},
            {"Brewer's", 102},
            {"Home Cook's", 106},
            {"Forager's", 105},
            {"Children's", 100},
            {"Wild Medicine", 109},
            {"Sticky", 110},
            {"Engineer's", 111},
            {"Treasure Hunter's", 101},
            {"Master Fisher's", 12},
            {"Quality Fish", 104},
            {"Fish Farmer's", 108},

            {"Beach Foraging", 240},
            {"Mines Foraging", 241},
            {"Desert Foraging", 242},
            {"Island Foraging", 243},
            {"Quality Foraging", 246},

            {"Orchard", 254},
            {"Island Crops", 255},
            {"Agronomist", 256},

            {"Tackle", 257},
            {"Recycling", 258},
            {"Spring Fishing", 27},
            {"Summer Fishing", 28},
            {"Fall Fishing", 29},
            {"Winter Fishing", 30},
            {"Rain Fishing", 263},
            {"Legendary Fish", 266},
            {"Island Fish", 267},
            {"Master Baiter", 12},

            {"Bartender's", 277},

            {"Demolition", 272},

            { "500g", 23 },
            { "1,500g", 23 },
            { "2,500g", 23 },
            { "3,500g", 23 },
            { "1,000g", 24 },
            { "3,000g", 24 },
            { "5,000g", 24 },
            { "7,000g", 24 },
            { "2,000g", 25 },
            { "6,000g", 25 },
            { "10,000g", 25 },
            { "14,000g", 25 },
            { "15,000g", 26 },
            { "25,000g", 26 },
            { "35,000g", 26 },

            {"Gambler's", 292},
            {"Carnival", 293},
            {"Walnut Hunter", 294},
            {"Qi's Helper", 295},
        };

        private const int BUNDLE_GREEN = 0;
        private const int BUNDLE_PURPLE = 1;
        private const int BUNDLE_ORANGE = 2;
        private const int BUNDLE_YELLOW = 3;
        private const int BUNDLE_RED = 4;
        private const int BUNDLE_DARK_BLUE = 5;
        private const int BUNDLE_LIGHT_BLUE = 6;

        // Ids above 100 are custom and not based in vanilla content
        public static readonly Dictionary<string, int> BundleColorIndexes = new()
        {
            { "Spring Crops", BUNDLE_GREEN },
            { "Summer Crops", BUNDLE_YELLOW },
            { "Fall Crops", BUNDLE_ORANGE },
            { "Quality Crops", BUNDLE_LIGHT_BLUE },
            { "Animal", BUNDLE_RED },
            { "Artisan", BUNDLE_PURPLE },
            { "Spring Foraging", BUNDLE_GREEN },
            { "Summer Foraging", BUNDLE_YELLOW },
            { "Fall Foraging", BUNDLE_ORANGE },
            { "Winter Foraging", BUNDLE_LIGHT_BLUE},
            { "Construction", BUNDLE_RED},
            { "Exotic Foraging", BUNDLE_PURPLE},
            { "River Fish", BUNDLE_LIGHT_BLUE},
            { "Lake Fish", BUNDLE_GREEN},
            { "Ocean Fish", BUNDLE_DARK_BLUE},
            { "Night Fishing", BUNDLE_PURPLE},
            { "Specialty Fish", BUNDLE_RED},
            { "Crab Pot", BUNDLE_PURPLE},
            { "Blacksmith's", BUNDLE_ORANGE},
            { "Geologist's", BUNDLE_PURPLE},
            { "Adventurer's", BUNDLE_PURPLE},
            { "Chef's", BUNDLE_RED},
            { "Field Research", BUNDLE_DARK_BLUE},
            { "Enchanter's", BUNDLE_PURPLE},
            { "Dye", BUNDLE_LIGHT_BLUE},
            { "Fodder", BUNDLE_YELLOW},
            { "The Missing", BUNDLE_PURPLE},

            {"Rare Crops", BUNDLE_LIGHT_BLUE},
            {"Garden", BUNDLE_RED},
            {"Brewer's", BUNDLE_ORANGE},
            {"Home Cook's", BUNDLE_YELLOW},
            {"Forager's", BUNDLE_ORANGE},
            {"Children's", BUNDLE_GREEN},
            {"Wild Medicine", BUNDLE_GREEN},
            {"Sticky", BUNDLE_YELLOW},
            {"Engineer's", BUNDLE_PURPLE},
            {"Treasure Hunter's", BUNDLE_YELLOW},
            {"Master Fisher's", BUNDLE_RED},
            {"Quality Fish", BUNDLE_RED},
            {"Fish Farmer's", BUNDLE_DARK_BLUE},

            {"Beach Foraging", BUNDLE_LIGHT_BLUE},
            {"Mines Foraging", BUNDLE_PURPLE},
            {"Desert Foraging", BUNDLE_ORANGE},
            {"Island Foraging", BUNDLE_YELLOW},
            {"Quality Foraging", BUNDLE_LIGHT_BLUE},

            {"Orchard", BUNDLE_RED},
            {"Island Crops", BUNDLE_YELLOW},
            {"Agronomist", 256},

            {"Tackle", BUNDLE_YELLOW},
            {"Recycling", BUNDLE_GREEN},
            {"Spring Fishing", BUNDLE_GREEN},
            {"Summer Fishing", BUNDLE_YELLOW},
            {"Fall Fishing", BUNDLE_ORANGE},
            {"Winter Fishing", BUNDLE_LIGHT_BLUE},
            {"Rain Fishing", BUNDLE_DARK_BLUE},
            {"Legendary Fish", BUNDLE_GREEN},
            {"Island Fish", BUNDLE_YELLOW},
            {"Master Baiter", BUNDLE_RED},

            {"Bartender's", BUNDLE_ORANGE},

            {"Demolition", BUNDLE_RED},

            { "500g", BUNDLE_YELLOW},
            { "1,500g", BUNDLE_YELLOW},
            { "2,500g",  BUNDLE_YELLOW},
            { "3,500g", BUNDLE_YELLOW },
            { "1,000g", BUNDLE_YELLOW },
            { "3,000g", BUNDLE_YELLOW },
            { "5,000g",  BUNDLE_YELLOW},
            { "7,000g", BUNDLE_YELLOW },
            { "2,000g", BUNDLE_YELLOW },
            { "6,000g", BUNDLE_YELLOW },
            { "10,000g", BUNDLE_YELLOW },
            { "14,000g", BUNDLE_YELLOW },
            { "15,000g", BUNDLE_YELLOW },
            { "25,000g", BUNDLE_YELLOW },
            { "35,000g", BUNDLE_YELLOW },

            {"Gambler's", BUNDLE_PURPLE},
            {"Carnival", BUNDLE_ORANGE},
            {"Walnut Hunter", BUNDLE_YELLOW},
            {"Qi's Helper", BUNDLE_PURPLE},
        };
    }
}
