/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Api.Items.Recipes {
    public interface IRecipe {
        /// <summary>The ingredients required to create the result.</summary>
        IEnumerable<IRecipePart> Ingredients { get; }

        /// <summary>The result of crafting this recipe.</summary>
        IEnumerable<IRecipePart> Results { get; }

        /// <summary>The sprite that will be displayed when this recipe is drawn in the crafting page.</summary>
        ISprite Sprite { get; }

        /// <summary>Whether the recipe is a cooking recipe.</summary>
        bool IsCooking { get; }

        /// <summary>Gets the name displayed by this recipe when it appears in the crafting page.</summary>
        /// <returns>The display name of this recipe.</returns>
        string GetDisplayName();

        /// <summary>Gets the description displayed by this recipe when it appears in the crafting page.</summary>
        /// <returns>The description of this recipe.</returns>
        string GetDescription();

        /// <summary>Tries to craft this recipe.</summary>
        /// <param name="inventory">The inventory to pull items from.</param>
        /// <param name="results">The results from crafting this recipe.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryCraft(IInventory inventory, out IEnumerable<Item> results);

        /// <summary>Checks whether this recipe can be crafted.</summary>
        /// <param name="inventory">The inventory to search ingredients from.</param>
        /// <returns>True if this recipe can be crafted from the given inventory, false if otherwise.</returns>
        bool CanCraft(IInventory inventory);
    }
}