/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    /// <summary>
    /// Taken from https://stardewcommunitywiki.com/Modding:Object_data#Categories
    /// These are all the categories that actually have a translation
    /// </summary>
    public enum ItemCategories
    {
        Gems = -2,
        Fish = -4,
        Eggs = -5,
        Milk = -6,
        Cooking = -7,
        Crafting = -8,
        Minterals = -12,
        Metals = -15,
        Resources = -16,
        Trash = -20,
        Bait = -21,
        Tackles = -22,
        Decor = -24,
        ArtisanGoods = -26,
        TreeProducts = -27,
        MonsterLoot = -28,
        Seeds = -74,
        Vegetables = -75,
        Fruit = -79,
        Flowers = -80,
        Foragables = -81
    }

    /// <summary>
    /// Extension methods for categories
    /// </summary>
    public static class CategoryExtentions
    {
        /// <summary>
        /// Gets all the int values for the categories
        /// </summary>
        /// <returns>The int values of the enum</returns>
        public static List<int> GetIntValues()
        {
            return Enum.GetValues(typeof(ItemCategories))
                .Cast<int>()
                .ToList();
        }

        /// <summary>
        /// Gets the translation for the category
        /// </summary>
        /// <param name="category">The category</param>
        /// <returns>The display name for the category</returns>
        public static string GetTranslation(this ItemCategories category)
        {
            return SVObject.GetCategoryDisplayName((int)category);
        }

        /// <summary>
        /// Gets a list of random categories
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="numberToGet">The number of categories to get</param>
        /// <returns>The categories chosen</returns>
        public static List<ItemCategories> GetRandomCategories(RNG rng, int numberToGet)
        {
            var categoryIds = Enum.GetValues(typeof(ItemCategories))
                .Cast<ItemCategories>()
                .ToList();

            return rng.GetRandomValuesFromList(categoryIds, numberToGet);
        }
    }
}
