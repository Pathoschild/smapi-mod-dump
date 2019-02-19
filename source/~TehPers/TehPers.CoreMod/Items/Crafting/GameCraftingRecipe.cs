using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items.Crafting.Recipes;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Items.Crafting {
    internal class GameCraftingRecipe : CustomCraftingRecipe {
        public override int ComponentWidth => 1;
        public override int ComponentHeight => this.bigCraftable ? 2 : 1;
        public override IRecipe Recipe { get; }

        public GameCraftingRecipe(ICoreApi coreApi, string name, bool isCookingRecipe) : base(name, isCookingRecipe) {
            coreApi.Owner.Monitor.Log($"Creating {nameof(GameRecipe)} for {name}", LogLevel.Trace);
            this.Recipe = new GameRecipe(coreApi, name, isCookingRecipe);
        }
    }
}