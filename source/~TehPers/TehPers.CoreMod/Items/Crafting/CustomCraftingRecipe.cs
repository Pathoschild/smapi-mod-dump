using StardewValley;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Items.Crafting {
    internal abstract class CustomCraftingRecipe : CraftingRecipe {
        public abstract int ComponentWidth { get; }
        public abstract int ComponentHeight { get; }
        public abstract IRecipe Recipe { get; }

        protected CustomCraftingRecipe(string name, bool isCookingRecipe) : base(name, isCookingRecipe) { }
    }
}