/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stokastic/PrismaticTools
**
*************************************************/

using StardewValley;

namespace PrismaticTools.Framework {

    public class PrismaticSprinklerItem { 
        public const int INDEX = 1113;
        public const int OLD_INDEX = 813;
        public const string NAME = "Prismatic Sprinkler";
        public const int PRICE = 2000;
        public const int EDIBILITY = -300;
        public const string TYPE = "Crafting";
        public const int CATEGORY = Object.CraftingCategory;
        public const string DESCRIPTION = "Waters the 48 adjacent tiles every morning.";
        public const int CRAFTING_LEVEL = 9;
    }

    public class PrismaticBarItem : Object {
        public const int INDEX = 1112;
        public const int OLD_INDEX = 812;
        public const string NAME = "Prismatic Bar";
        public const string DESCRIPTION = "A mystical ingot forged from legend itself.";
        public const int PRICE = 2500;
        public const string TYPE = "Basic";
        public const int CATEGORY = metalResources;
        public const int EDIBILITY = -300;
    }
}
