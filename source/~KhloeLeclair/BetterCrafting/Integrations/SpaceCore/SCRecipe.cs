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
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCRecipe : IRecipe {

	public readonly ICustomCraftingRecipe Recipe;
	public readonly Item? ExampleItem;
	public readonly bool Cooking;

	public SCRecipe(string name, ICustomCraftingRecipe recipe, bool cooking, IEnumerable<IIngredient> ingredients) {
		Name = name;
		Recipe = recipe;
		Cooking = cooking;
		Ingredients = ingredients.ToArray();

		Item? example = CreateItem();
		SortValue = $"{example?.ParentSheetIndex ?? 0}";
		QuantityPerCraft = example?.Stack ?? 1;
		Stackable = (example?.maximumStackSize() ?? 1) > 1;

		if (recipe.Name != null)
			DisplayName = recipe.Name;
		else if (example is not null && ItemRegistry.GetData(example.QualifiedItemId) is ParsedItemData data)
			DisplayName = data.DisplayName;
		else
			DisplayName = Name;

		// Ensure we can access things.
		string? test = recipe.Description;
		Texture2D testtwo = recipe.IconTexture;
		Rectangle? testthree = recipe.IconSubrect;
	}

	#region Identity

	public string SortValue { get; }
	public string Name { get; }

	public virtual bool HasRecipe(Farmer who) {
		if (Cooking)
			return who.cookingRecipes.ContainsKey(Name);
		else
			return who.craftingRecipes.ContainsKey(Name);
	}

	public virtual int GetTimesCrafted(Farmer who) {
		if (Cooking) {
			// TODO: This
			return 0;

		} else if (who.craftingRecipes.ContainsKey(Name))
			return who.craftingRecipes[Name];

		return 0;
	}

	public CraftingRecipe? CraftingRecipe => null;

	#endregion

	#region Display

	public bool AllowRecycling { get; } = true;

	public string DisplayName { get; }
	public string Description => Recipe.Description ?? string.Empty;

	public Texture2D Texture => Recipe.IconTexture;

	public Rectangle SourceRectangle => Recipe.IconSubrect ?? Texture.Bounds;

	public int GridHeight {
		get {
			Rectangle rect = SourceRectangle;
			if (rect.Height > rect.Width)
				return 2;
			return 1;
		}
	}
	public int GridWidth {
		get {
			Rectangle rect = SourceRectangle;
			if (rect.Width > rect.Height)
				return 2;
			return 1;
		}
	}

	#endregion

	#region Cost

	public int QuantityPerCraft { get; }
	public IIngredient[] Ingredients { get; }

	#endregion

	#region Creation

	public bool Stackable { get; }

	public bool CanCraft(Farmer who) {
		return true;
	}

	public string? GetTooltipExtra(Farmer who) {
		return null;
	}

	public Item? CreateItem() {
		return Recipe.CreateResult();
	}

	public void PerformCraft(IPerformCraftEvent evt) {
		if (evt.Item is null)
			evt.Cancel();
		else
			evt.Complete();
	}

	#endregion

}
