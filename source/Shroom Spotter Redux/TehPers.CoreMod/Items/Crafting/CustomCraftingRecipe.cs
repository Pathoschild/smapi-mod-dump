/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

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