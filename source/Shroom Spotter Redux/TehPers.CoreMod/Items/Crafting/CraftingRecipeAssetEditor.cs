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
using StardewModdingAPI;

namespace TehPers.CoreMod.Items.Crafting {
    internal class CraftingRecipeAssetEditor : IAssetEditor {
        private readonly CraftingManager _craftingManager;

        public CraftingRecipeAssetEditor(CraftingManager craftingManager) {
            this._craftingManager = craftingManager;
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals("Data/CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset) {
            if (asset.AssetNameEquals("Data/CraftingRecipes")) {
                IDictionary<string, string> recipes = asset.AsDictionary<string, string>().Data;

                foreach (string addedRecipe in this._craftingManager.GetAddedRecipes()) {
                    recipes.Add(addedRecipe, recipes["Torch"]);
                }
            }
        }
    }
}