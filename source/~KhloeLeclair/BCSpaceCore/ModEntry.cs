/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Leclair.Stardew.Common.Events;

#if IS_BETTER_CRAFTING
using Leclair.Stardew.Common.Crafting;
#endif
using Leclair.Stardew.BetterCrafting;

using SpaceCore;

using StardewModdingAPI.Events;
using StardewValley;

namespace Leclair.Stardew.BCSpaceCore;

public class ModEntry : ModSubscriber, IRecipeProvider {

	internal IBetterCrafting? API;

	[Subscriber]
	private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
		API = Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
		if (API != null)
			API.AddRecipeProvider(this);
	}

	#region IRecipeProvider

	public int RecipePriority => 5;

	public IRecipe? GetRecipe(CraftingRecipe recipe) {
		Dictionary<string, CustomCraftingRecipe> container;
		if (recipe.isCookingRecipe)
			container = CustomCraftingRecipe.CookingRecipes;
		else
			container = CustomCraftingRecipe.CraftingRecipes;

		if (!container.TryGetValue(recipe.name, out var ccr) || ccr == null)
			return null;

		return new SpaceCoreRecipe(recipe.name, ccr, recipe.isCookingRecipe, this);
	}

	public bool CacheAdditionalRecipes => true;

	public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) {
		return null;
	}

	#endregion

}
