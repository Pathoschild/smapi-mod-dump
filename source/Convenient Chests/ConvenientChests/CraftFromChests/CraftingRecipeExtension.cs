using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ConvenientChests.CraftFromChests {
    public static class CraftingRecipeExtension {
        public static Dictionary<int, int> GetIngredients(this CraftingRecipe recipe)
            => ModEntry.StaticHelper.Reflection.GetField<Dictionary<int, int>>(recipe, "recipeList").GetValue();

        public static bool ConsumeIngredients(this CraftingRecipe recipe, IList<IList<Item>> extraInventories) {
            foreach (var i in recipe.GetIngredients()) {
                var itemKey = i.Key;
                var count   = i.Value;

                // start with player inventory
                count = ConsumeFromInventory(Game1.player.Items, itemKey, count);
                if (count <= 0) goto NEXT_ITEM;

                // check extra inventories
                foreach (var extraInventory in extraInventories) {
                    count = ConsumeFromInventory(extraInventory, itemKey, count);
                    if (count <= 0) goto NEXT_ITEM;
                }

                if (count > 0)
                    ModEntry.Log($"\tOnly found {i.Value - count} / {count} of {i.Key}", LogLevel.Warn);

                NEXT_ITEM:;
            }

            return true;
        }

        private static int ConsumeFromInventory(IList<Item> items, int itemKey, int count) {
            // start from the end
            for (var index = items.Count - 1; index >= 0; --index) {
                var item = items[index];
                if (!MatchesItemKey(item, itemKey))
                    continue;

                var actualValue = item.Stack;

                if (item.Stack > count)
                    item.Stack -= count;

                else {
                    item.Stack   = 0;
                    items[index] = null;
                }

                count -= actualValue;

                if (count <= 0)
                    return 0;
            }

            return count;
        }

        private static bool MatchesItemKey(Item item, int itemKey) =>
            item != null          &&
            item is Object o      &&
            !o.bigCraftable.Value &&
            (o.ParentSheetIndex == itemKey || o.Category == itemKey);
    }
}