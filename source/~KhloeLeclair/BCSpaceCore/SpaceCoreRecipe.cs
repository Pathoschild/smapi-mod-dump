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
using System.Linq;

#if IS_BETTER_CRAFTING
using Leclair.Stardew.Common.Crafting;
#else
using Leclair.Stardew.BetterCrafting;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SpaceCore;

using StardewValley;

namespace Leclair.Stardew.BCSpaceCore;

public class SpaceCoreRecipe : IRecipe {

	public readonly CustomCraftingRecipe Recipe;
	public readonly bool Cooking;

	public SpaceCoreRecipe(string name, CustomCraftingRecipe recipe, bool cooking, ModEntry mod) {
		Recipe = recipe;
		Cooking = cooking;
		Name = name;

		Ingredients = recipe.Ingredients.Select<CustomCraftingRecipe.IngredientMatcher, IIngredient>(val => {
			Type type = val.GetType();
			string cls = type.FullName ?? type.Name;

			// Check for known classes that implement ingredient matchers that
			// we can use our own logic for.
			if (cls.Equals("SpaceCore.CustomCraftingRecipe+ObjectIngredientMatcher")) {
				int? index = mod.Helper.Reflection.GetField<int>(val, "objectIndex", false)?.GetValue();
				if (index.HasValue)
					return mod.API!.CreateBaseIngredient(index.Value, val.Quantity);

			} else if (cls.Equals("DynamicGameAssets.DGACustomCraftingRecipe+DGAIngredientMatcher")) {
				var matcher = mod.Helper.Reflection.GetMethod(val, "ItemMatches", false);
				if (matcher != null) {
					bool Match(Item item) {
						try {
							return matcher.Invoke<bool>(item);
						} catch {
							return false;
						}
					}

					return mod.API!.CreateMatcherIngredient(
						Match,
						val.Quantity,
						val.DispayName,
						val.IconTexture,
						val.IconSubrect ?? val.IconTexture.Bounds
					);
				}
			}

			// If it's not a specific, known class then we can't guarantee our
			// logic is correct, so fall back to just wrapping SpaceCore's
			// ingredients.
			return new SpaceCoreIngredient(val);
		}).ToArray();

		Item? example = CreateItem();
		SortValue = example?.ParentSheetIndex ?? 0;
		QuantityPerCraft = example?.Stack ?? 1;
		Stackable = (example?.maximumStackSize() ?? 1) > 1;
	}


	// Identity

	public int SortValue { get; }

	public string Name { get; }
	public string DisplayName => Recipe.Name;
	public string Description => Recipe.Description;

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

	// Display

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


	// Cost

	public int QuantityPerCraft { get; }

	public IIngredient[] Ingredients { get; }

	// Creation

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
}
