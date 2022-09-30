/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation
{
    /// <summary>
    /// Static class to keep track of item categories.
    /// </summary>
    public class ItemCategories
    {
        public static readonly ItemCategory Crafting = new ItemCategory(CategoryNames.Crafting, CategoryColors.Crafting);
        public static readonly ItemCategory Farming = new ItemCategory(CategoryNames.Farming, CategoryColors.Farming);
        public static readonly ItemCategory Machine = new ItemCategory(CategoryNames.Machine, CategoryColors.Machines);
        public static readonly ItemCategory Misc = new ItemCategory(CategoryNames.Misc, CategoryColors.Misc);
        public static readonly ItemCategory Ore = new ItemCategory(CategoryNames.Ore, CategoryColors.Ore);
        public static readonly ItemCategory Resource = new ItemCategory(CategoryNames.Resource, CategoryColors.Resource);


        /// <summary>
        /// A mapping of <see cref="ItemCategoryIds"/> to an actual <see cref="ItemCategory"/> which contains name and <see cref="Color"/> information.
        /// </summary>
        public static Dictionary<string, ItemCategory> CategoriesById = new Dictionary<string, ItemCategory>()
        {
            {ItemCategoryIds.StardewValley_Crafting, Crafting},
            {ItemCategoryIds.Revitalize_Farming, Farming},
            {ItemCategoryIds.Revitalize_Machine, Machine},
            {ItemCategoryIds.StardewValley_Misc, Misc},
            {ItemCategoryIds.StardewValley_Ore, Ore},
            {ItemCategoryIds.StardewValley_Resource, Resource},
        };

        public static ItemCategory GetItemCategory(string CategoryId)
        {
            if (CategoriesById.ContainsKey(CategoryId))
            {
                return CategoriesById[CategoryId];
            }
            throw new Exception(string.Format("Revitalize category exception! Category with the given id has not been registered! {0}", CategoryId));
        }


    }
}
