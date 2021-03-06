/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSprinklerOneScarecrow.Framework
{
    internal class Items
    {

    }

    internal class HaxorSprinkler
    {
        public const string Name = "Haxor Sprinkler";
        public static string TranslatedName = "";
        public static string TranslatedDescription = "";
        public static int ParentSheetIndex = 622;//1120;
        public const int Price = 2000;
        public const int Edibility = -300;
        public const int Category = StardewValley.Object.CraftingCategory;
        public const string Type = "Crafting";

        public static Dictionary<int, int> Ingredients = new Dictionary<int, int>()
        {
            {390, 100}, //Easy Mode = 100 Stone
            {386, 10} // Hard Mode = 10 Iridium Ore
        };

        //Easy Mode


    }

    internal class HaxorScarecrow
    {
        public const string Name = "Haxor Scarecrow";
        public static string TranslatedName = "";
        public static string TranslatedDescription = "";
        public static int ParentSheetIndex = 93;//PetterSeen273;//300;
        public const int Price = 2000;
        public const int Edibility = -300;
        public const int Category = StardewValley.Object.BigCraftableCategory;
        public const string Type = "Crafting";

        public static Dictionary<int, int> Ingredients = new Dictionary<int, int>()
        {
            {388, 100}, //Easy Mode = 100 Wood
            {337, 10} // Hard Mode = 10 Iridium Bars
        };
    }
}
