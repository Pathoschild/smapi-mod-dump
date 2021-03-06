/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Inventory;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes {
    public abstract class SimpleRecipe : IRecipe {
        public abstract IEnumerable<IRecipePart> Ingredients { get; }
        public abstract IEnumerable<IRecipePart> Results { get; }
        public abstract ISprite Sprite { get; }
        public bool IsCooking { get; }

        protected SimpleRecipe(bool isCooking) {
            this.IsCooking = isCooking;
        }

        public virtual string GetDisplayName() {
            return this.Results.FirstOrDefault() is IRecipePart firstResult && firstResult.TryCreateOne(out Item item) ? item.DisplayName : "Invalid Recipe";
        }

        public virtual string GetDescription() {
            return this.Results.FirstOrDefault() is IRecipePart firstResult && firstResult.TryCreateOne(out Item item) ? item.getDescription() : "Invalid recipe";
        }

        public bool TryCraft(IInventory inventory, out IEnumerable<Item> results) {
            // Create results to make sure it's possible
            List<Item> resultItems = new List<Item>();
            foreach (IRecipePart result in this.Results) {
                for (int n = 0; n < result.Quantity; n++) {
                    if (!result.TryCreateOne(out Item item)) {
                        results = default;
                        return false;
                    }

                    resultItems.Add(item);
                }
            }

            // Try to remove the ingredients from the inventory
            if (inventory.TryRemoveAll(this.Ingredients, out _)) {
                // The items were crafted successfully
                results = resultItems;
                return true;
            }

            results = default;
            return false;
        }

        public bool CanCraft(IInventory inventory) {
            return inventory.Contains(this.Ingredients);
        }
    }
}