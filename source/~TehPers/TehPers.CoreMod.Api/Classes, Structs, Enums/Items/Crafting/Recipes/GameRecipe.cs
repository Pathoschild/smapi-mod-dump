using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Crafting.Recipes.Parts;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes {
    public class GameRecipe : SimpleRecipe {
        private readonly string _recipeName;
        private readonly bool _cooking;

        public override IEnumerable<IRecipePart> Ingredients { get; }
        public override IEnumerable<IRecipePart> Results { get; }
        public override ISprite Sprite { get; }

        public GameRecipe(ICoreApi coreApi, string recipeName, bool cooking) : base(cooking) {
            this._recipeName = recipeName;
            this._cooking = cooking;
            if (!cooking || !CraftingRecipe.cookingRecipes.TryGetValue(recipeName, out string data)) {
                if (!CraftingRecipe.craftingRecipes.TryGetValue(recipeName, out data)) {
                    recipeName = "Torch";
                    data = CraftingRecipe.craftingRecipes[recipeName];
                }
            }

            // Create ingredients
            List<IRecipePart> ingredients = new List<IRecipePart>();
            string[] splitData = data.Split('/');
            string[] ingredientData = splitData[0].Split(' ');
            for (int i = 0; i < ingredientData.Length; i += 2) {
                ingredients.Add(new SObjectRecipePart(coreApi, Convert.ToInt32(ingredientData[i]), Convert.ToInt32(ingredientData[i + 1])));
            }
            this.Ingredients = ingredients;

            // Create results
            List<IRecipePart> results = new List<IRecipePart>();
            bool bigCraftable = !cooking && Convert.ToBoolean(splitData[3]);
            string[] resultData = splitData[2].Split(' ');
            for (int i = 0; i < resultData.Length; i += 2) {
                if (bigCraftable) {
                    results.Add(new BigCraftableRecipePart(coreApi, Convert.ToInt32(resultData[i]), i + 1 < resultData.Length ? Convert.ToInt32(resultData[i + 1]) : 1));
                } else {
                    results.Add(new SObjectRecipePart(coreApi, Convert.ToInt32(resultData[i]), i + 1 < resultData.Length ? Convert.ToInt32(resultData[i + 1]) : 1));
                }
            }
            this.Results = results;

            // Get sprite
            if (results.Any()) {
                ISpriteSheet spriteSheet = bigCraftable ? coreApi.Drawing.CraftableSpriteSheet : coreApi.Drawing.ObjectSpriteSheet;
                this.Sprite = spriteSheet.TryGetSprite(Convert.ToInt32(resultData[0]), out ISprite sprite) ? sprite : throw new InvalidOperationException($"Failed to create a sprite for {(cooking ? "cooking" : "crafting")} recipe \"{recipeName}\"");
            } else {
                throw new InvalidOperationException($"Unable to create a sprite for {(cooking ? "cooking" : "crafting")} recipe \"{recipeName}\" because it has no results");
            }
        }

        public override string GetDisplayName() {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en) {
                return this._recipeName;
            }

            string rawData = this._cooking ? CraftingRecipe.cookingRecipes[this._recipeName] : CraftingRecipe.craftingRecipes[this._recipeName];
            return rawData.Split('/').Last();
        }
    }
}