/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using System.Reflection;
using StardewValley;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Items.Crafting {
    internal class ModCraftingRecipe : CustomCraftingRecipe {
        private static readonly FieldInfo _descriptionField = typeof(CraftingRecipe).GetField("description", BindingFlags.Instance | BindingFlags.NonPublic);

        public override int ComponentWidth { get; }
        public override int ComponentHeight { get; }
        public override IRecipe Recipe { get; }

        public ModCraftingRecipe(string name, IRecipe recipe, bool isCookingRecipe) : base("Torch", isCookingRecipe) {
            this.Recipe = recipe;
            this.ComponentWidth = (int) Math.Ceiling(recipe.Sprite.Width / 16f);
            this.ComponentHeight = (int) Math.Ceiling(recipe.Sprite.Height / 16f);

            // Recipe details
            this.name = name;
            this.DisplayName = recipe.GetDisplayName();
            ModCraftingRecipe._descriptionField.SetValue(this, recipe.GetDescription());
            this.timesCrafted = Game1.player.craftingRecipes.ContainsKey(name) ? Game1.player.craftingRecipes[name] : 0;
            this.numberProducedPerCraft = recipe.Results.Count() == 1 ? recipe.Results.First().Quantity : 1;
        }
    }
}