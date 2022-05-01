/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common.Crafting;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models;

internal class RecipeWithIngredients : BaseRecipe {

	private readonly Action<IPerformCraftEvent>? OnPerformCraft;

	public RecipeWithIngredients(CraftingRecipe recipe, IEnumerable<IIngredient> ingredients, Action<IPerformCraftEvent>? onPerformCraft) : base(recipe) {
		if (ingredients is null)
			Ingredients = new IIngredient[] {
				new ErrorIngredient()
			};
		else
			Ingredients = ingredients.ToArray();

		OnPerformCraft = onPerformCraft;
	}

	public override void PerformCraft(IPerformCraftEvent evt) {
		if (OnPerformCraft is not null)
			OnPerformCraft(evt);
		else
			base.PerformCraft(evt);
	}

}
