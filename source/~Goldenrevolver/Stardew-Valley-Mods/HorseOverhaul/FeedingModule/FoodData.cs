/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewValley;
    using System;
    using StardewObject = StardewValley.Object;

    internal class FoodData
    {
        protected const int BugMeatID = 684;

        public static int CalculateGenericFriendshipGain(Item item, int currentFriendship)
        {
            return (int)Math.Floor((5 + (item.healthRecoveredOnConsumption() / 10)) * Math.Pow(1.2, -currentFriendship / 200));
        }

        public static bool IsGenericEdible(Item item)
        {
            return item.Category == StardewObject.CookingCategory || item.healthRecoveredOnConsumption() > 0;
        }

        public static bool IsDairyProduct(Item item)
        {
            // TODO check if this also applies for Items or only objects in all languages, if so, cast
            if (item.Name.ToLower().Contains("mayonnaise") || item.Name.ToLower().Contains("cheese"))
            {
                return true;
            }
            else
            {
                return item.Category == StardewObject.MilkCategory || item.Category == StardewObject.EggCategory;
            }
        }

        // bug meat or modded meat
        public static bool IsEdibleMeat(Item item)
        {
            // TODO maybe exclude a few items (maybe differences between cat and dog?)
            // TODO maybe include a few cooked meats? then you could do the same for cooked fish too though
            // TODO no healthRecoveredOnConsumption > 0 in case raw chicken is classified as toxic?
            return item.ParentSheetIndex == BugMeatID || item.Category == StardewObject.meatCategory;
        }

        public static bool IsChocolate(Item item)
        {
            // Chocolate Cake, currently the only chocolate item
            return item?.ParentSheetIndex == 220;
        }
    }
}