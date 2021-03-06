/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

using StardewValley;

namespace RadioactiveTools.Framework {
    public class RadioactiveSprinklerItem {
        public const int INDEX = 1113;
        public const int PRICE = 2000;
        public const int EDIBILITY = -300;
        public const string TYPE = "Crafting";
        public const int CATEGORY = Object.CraftingCategory;
        public const int CRAFTING_LEVEL = 10;
    }
    public class RadioactiveFertilizerItem {
        public const int INDEX = 1114;
        public const int PRICE = 90;
        public const int EDIBILITY = -300;
        public const string TYPE = "Fertilizer";
        public const int CATEGORY = Object.fertilizerCategory;
        public const int CRAFTING_LEVEL = 10;
    }
    public class RadioactiveRodItem {
        public const int id = 1115;
        public static string name = ModEntry.ModHelper.Translation.Get("radioactiveRod.name");
        public static string baseName = "Radioactive Rod";
        public static string description = ModEntry.ModHelper.Translation.Get("radioactiveRod.description");
    }

}
