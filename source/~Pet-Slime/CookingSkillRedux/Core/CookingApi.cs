/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using StardewValley;
using StardewModdingAPI;

namespace CookingSkill.Core
{
    public interface ICookingApi
    {
        /// <summary>
        /// Modify a cooked item based on the player's cooking skill.
        /// Always returns true.
        /// </summary>
        /// <param name="recipe">The crafting recipe.</param>
        /// <param name="item">The crafted item from the recipe. Nothing is changed if the recipe isn't cooking.</param>
        /// <returns>Returns the held item.</returns>
        Item PreCook(CraftingRecipe recipe, Item item, bool betterCrafting = false);

        /// <summary>
        /// Grants the player EXP and increases the held Item by the value of the crafting recipe.
        /// </summary>
        /// <param name="recipe">The crafting recipe.</param>
        /// <param name="heldItem">The held item, to increase the stack size of and to get the edibility of.</param>
        /// <param name="who"> the player who did the cooking, to grant exp to.</param>
        /// <returns>Returns the held item.</returns>
        Item PostCook(CraftingRecipe recipe, Item heldItem, Dictionary<Item, int> consumed_items, Farmer who, bool betterCrafting = false);
    }

    public class CookingAPI : ICookingApi
    {
        public Item PreCook(CraftingRecipe recipe, Item item, bool betterCrafting = false)
        {
            return CookingSkill.Core.Events.PreCook(recipe, item, betterCrafting);
        }

        public Item PostCook(CraftingRecipe recipe, Item heldItem, Dictionary<Item,int> consumedItems, Farmer who, bool betterCrafting = false)
        {
            return CookingSkill.Core.Events.PostCook(recipe, heldItem, consumedItems, who, betterCrafting);
        }
    }
}
