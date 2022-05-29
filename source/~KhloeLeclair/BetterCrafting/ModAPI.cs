/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Crafting;

using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;

using Leclair.Stardew.BetterCrafting.Models;

namespace Leclair.Stardew.BetterCrafting;

public class ModAPI : IBetterCrafting {

	private readonly ModEntry Mod;

	public ModAPI(ModEntry mod) {
		Mod = mod;
	}

	#region GUI

	/// <inheritdoc />
	[Obsolete("Please use the other call with additional parameters.")]
	public bool OpenCraftingMenu(
		bool cooking,
		IList<Chest>? containers = null,
		GameLocation? location = null,
		Vector2? position = null,
		bool silent_open = false,
		IList<string>? listed_recipes = null
	) {
		var menu = Game1.activeClickableMenu;
		if (menu != null) {
			if (!menu.readyToClose())
				return false;

			CommonHelper.YeetMenu(menu);
			Game1.exitActiveMenu();
		}

		Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
			Mod,
			location: location,
			position: position,
			cooking: cooking,
			standalone_menu: true,
			material_containers: containers?.ToList<object>(),
			discover_containers: true,
			silent_open: silent_open,
			listed_recipes: listed_recipes
		);

		return true;
	}

	/// <inheritdoc />
	public bool OpenCraftingMenu(
		bool cooking,
		bool silent_open = false,
		GameLocation? location = null,
		Vector2? position = null,
		Rectangle? area = null,
		bool discover_containers = true,
		IList<Tuple<object, GameLocation>>? containers = null,
		IList<string>? listed_recipes = null
	) {
		if (listed_recipes == null && Mod.intCCStation != null)
			listed_recipes = cooking ?
				Mod.intCCStation.GetCookingRecipes() :
				Mod.intCCStation.GetCraftingRecipes();

		var menu = Game1.activeClickableMenu;
		if (menu != null) {
			if (!menu.readyToClose())
				return false;

			CommonHelper.YeetMenu(menu);
			Game1.exitActiveMenu();
		}

		Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
			Mod,
			location,
			position,
			area,
			cooking: cooking,
			standalone_menu: true,
			material_containers: containers?.Select(val => new LocatedInventory(val.Item1, val.Item2)).ToList(),
			silent_open: silent_open,
			discover_containers: discover_containers,
			listed_recipes: listed_recipes
		);

		return true;
	}

	/// <inheritdoc />
	public Type GetMenuType() {
		return typeof(Menus.BetterCraftingPage);
	}

	#endregion

	#region Recipes

	/// <inheritdoc />
	public void AddRecipeProvider(IRecipeProvider provider) {
		Mod.Recipes.AddProvider(provider);
	}

	/// <inheritdoc />
	public void RemoveRecipeProvider(IRecipeProvider provider) {
		Mod.Recipes.RemoveProvider(provider);
	}

	/// <inheritdoc />
	public void InvalidateRecipeCache() {
		Mod.Recipes.Invalidate();
	}

	/// <inheritdoc />
	public IReadOnlyCollection<IRecipe> GetRecipes(bool cooking) {
		return Mod.Recipes.GetRecipes(cooking).AsReadOnly();
	}

	/// <inheritdoc />
	public IRecipe CreateRecipeWithIngredients(CraftingRecipe recipe, IEnumerable<IIngredient> ingredients, Action<IPerformCraftEvent>? onPerformCraft = null) {
		return new RecipeWithIngredients(recipe, ingredients, onPerformCraft);
	}

	#endregion

	#region Ingredients

	/// <inheritdoc />
	public IIngredient CreateBaseIngredient(int item, int quantity) {
		return new BaseIngredient(item, quantity);
	}

	/// <inheritdoc />
	public IIngredient CreateCurrencyIngredient(CurrencyType type, int quantity) {
		return new CurrencyIngredient(type, quantity);
	}

	/// <inheritdoc />
	public IIngredient CreateMatcherIngredient(Func<Item, bool> matcher, int quantity, string displayName, Texture2D texture, Rectangle? source = null) {
		return new MatcherIngredient(matcher, quantity, displayName, texture, source);
	}

	/// <inheritdoc />
	public IIngredient CreateErrorIngredient() {
		return new ErrorIngredient();
	}

	/// <inheritdoc />
	public void ConsumeItems(IEnumerable<(Func<Item, bool>, int)> items, Farmer? who, IEnumerable<IInventory>? inventories, int maxQuality = int.MaxValue, bool lowQualityFirst = false) {
		InventoryHelper.ConsumeItems(items, who, inventories, maxQuality, lowQualityFirst);
	}

	#endregion

	#region Categories

	/// <inheritdoc />
	public void CreateDefaultCategory(bool cooking, string categoryId, Func<string> Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null) {
		Mod.Recipes.CreateDefaultCategory(cooking, categoryId, Name, recipeNames, iconRecipe);
	}

	/// <inheritdoc />
	public void CreateDefaultCategory(bool cooking, string categoryId, string Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null) {
		Mod.Recipes.CreateDefaultCategory(cooking, categoryId, Name, recipeNames, iconRecipe);
	}

	/// <inheritdoc />
	public void AddRecipesToDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames) {
		Mod.Recipes.AddRecipesToDefaultCategory(cooking, categoryId, recipeNames);
	}

	/// <inheritdoc />
	public void RemoveRecipesFromDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames) {
		Mod.Recipes.RemoveRecipesFromDefaultCategory(cooking, categoryId, recipeNames);
	}

	#endregion

	#region Inventories

	/// <inheritdoc />
	public void RegisterInventoryProvider(Type type, IInventoryProvider provider) {
		Mod.RegisterInventoryProvider(type, provider);
	}

	/// <inheritdoc />
	public void UnregisterInventoryProvider(Type type) {
		Mod.UnregisterInventoryProvider(type);
	}

	/// <inheritdoc />
	public void RegisterInventoryProvider(
		Type type,
		Func<object, GameLocation?, Farmer?, bool>? isValid,
		Func<object, GameLocation?, Farmer?, bool>? canExtractItems,
		Func<object, GameLocation?, Farmer?, bool>? canInsertItems,
		Func<object, GameLocation?, Farmer?, NetMutex?>? getMutex,
		Func<object, GameLocation?, Farmer?, bool>? isMutexRequired,
		Func<object, GameLocation?, Farmer?, int>? getActualCapacity,
		Func<object, GameLocation?, Farmer?, IList<Item?>?>? getItems,
		Func<object, GameLocation?, Farmer?, Item, bool>? isItemValid,
		Action<object, GameLocation?, Farmer?>? cleanInventory,
		Func<object, GameLocation?, Farmer?, Rectangle?>? getMultiTileRegion,
		Func<object, GameLocation?, Farmer?, Vector2?>? getTilePosition
	) {
		
		var provider = new ModInventoryProvider(
			canExtractItems: canExtractItems,
			canInsertItems: canInsertItems,
			cleanInventory: cleanInventory,
			getActualCapacity: getActualCapacity,
			getItems: getItems,
			isItemValid: isItemValid,
			getMultiTileRegion: getMultiTileRegion,
			getTilePosition: getTilePosition,
			getMutex: getMutex,
			isMutexRequired: isMutexRequired,
			isValid: isValid
		);

		Mod.RegisterInventoryProvider(type, provider);
	}

	#endregion
}
