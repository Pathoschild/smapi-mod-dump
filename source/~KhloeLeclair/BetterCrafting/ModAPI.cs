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
using Leclair.Stardew.BetterCrafting.Menus;
using StardewValley.Menus;
using Leclair.Stardew.BetterCrafting.DynamicRules;
using StardewModdingAPI;

namespace Leclair.Stardew.BetterCrafting;


public class PopulateContainersEventArgs : IPopulateContainersEvent {
	public IBetterCraftingMenu Menu { get; }

	public IList<Tuple<object, GameLocation?>> Containers { get; }

	public PopulateContainersEventArgs(IBetterCraftingMenu menu, IList<Tuple<object, GameLocation?>> containers) {
		Menu = menu;
		Containers = containers;
	}
}


public class ModAPI : IBetterCrafting {

	private readonly ModEntry Mod;

	private readonly IManifest Other;


	public ModAPI(ModEntry mod, IManifest other) {
		Mod = mod;
		Other = other;
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
		IList<Tuple<object, GameLocation?>>? containers = null,
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

	/// <inheritdoc />
	public IBetterCraftingMenu? GetActiveMenu() {
		var menu = Game1.activeClickableMenu;

		while (menu is not null) {
			if (menu is IBetterCraftingMenu bcm)
				return bcm;

			if (menu is GameMenu gm) {
				for (int i = 0; i < gm.pages.Count; i++) {
					if (gm.pages[i] is IBetterCraftingMenu bcm2)
						return bcm2;
				}
			}

			menu = menu.GetChildMenu();
		}

		return null;
	}

	/// <inheritdoc />
	public event Action<IPopulateContainersEvent>? MenuPopulateContainers;

	internal void EmitMenuPopulate(BetterCraftingPage menu, ref IList<LocatedInventory>? containers) {
		if (MenuPopulateContainers is not null) {
			List<Tuple<object, GameLocation?>> values = containers == null ? new() :
				containers.Select(x => new Tuple<object, GameLocation?>(x.Source, x.Location)).ToList();

			MenuPopulateContainers.Invoke(new PopulateContainersEventArgs(menu, values));

			if (values.Count == 0)
				containers = null;
			else
				containers = values.Select(x => new LocatedInventory(x.Item1, x.Item2)).ToList();
		}
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

	/// <inheritdoc />
	public IRecipeBuilder RecipeBuilder(CraftingRecipe recipe) {
		return new RecipeBuilder(recipe);
	}

	/// <inheritdoc />
	public IRecipeBuilder RecipeBuilder(string name) {
		return new RecipeBuilder(name);
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
	public IIngredient CreateMatcherIngredient(Func<Item, bool> matcher, int quantity, Func<string> displayName, Func<Texture2D> texture, Rectangle? source = null) {
		return new MatcherIngredient(matcher, quantity, displayName, texture, source);
	}

	/// <inheritdoc />
	[Obsolete("Use the method that takes functions instead.")]
	public IIngredient CreateMatcherIngredient(Func<Item, bool> matcher, int quantity, string displayName, Texture2D texture, Rectangle? source = null) {
		return new MatcherIngredient(matcher, quantity, () => displayName, () => texture, source);
	}

	/// <inheritdoc />
	public IIngredient CreateErrorIngredient() {
		return new ErrorIngredient();
	}

	/// <inheritdoc />
	public void ConsumeItems(IEnumerable<(Func<Item, bool>, int)> items, Farmer? who, IEnumerable<IInventory>? inventories, int maxQuality = int.MaxValue, bool lowQualityFirst = false) {
		InventoryHelper.ConsumeItems(items, who, inventories, maxQuality, lowQualityFirst);
	}

	/// <inheritdoc />
	public int CountItem(Func<Item, bool> predicate, Farmer? who, IEnumerable<Item?>? items, int maxQuality = int.MaxValue) {
		return InventoryHelper.CountItem(predicate, who, items, out bool _, max_quality: maxQuality);
	}

	#endregion

	#region Categories

	/// <inheritdoc />
	public void CreateDefaultCategory(bool cooking, string categoryId, Func<string> Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null, bool useRules = false, IEnumerable<IDynamicRuleData>? rules = null) {
		Mod.Recipes.CreateDefaultCategory(cooking, categoryId, Name, recipeNames, iconRecipe, useRules, rules);
	}

	[Obsolete("For compatibility after changing the API to use a function for the display name")]
	public void CreateDefaultCategory(bool cooking, string categoryId, string Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null, bool useRules = false, IEnumerable<IDynamicRuleData>? rules = null) {
		Mod.Recipes.CreateDefaultCategory(cooking, categoryId, Name, recipeNames, iconRecipe, useRules, rules);
	}

	[Obsolete("For compatibility after adding API parameters")]
	public void CreateDefaultCategory(bool cooking, string categoryId, Func<string> Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null) {
		Mod.Recipes.CreateDefaultCategory(cooking, categoryId, Name, recipeNames, iconRecipe);
	}

	[Obsolete("For compatibility after adding API parameters")]
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

	#region Dynamic Rules

	/// <inheritdoc />
	public string GetAbsoluteRuleId(string id) {
		return $"{Other.UniqueID}/{id}";
	}

	/// <inheritdoc />
	public bool RegisterRuleHandler(string id, IDynamicRuleHandler handler) {
		string fullId = $"{Other.UniqueID}/{id}";
		return Mod.Recipes.RegisterRuleHandler(fullId, handler);
	}

	/// <inheritdoc />
	public bool RegisterRuleHandler(string id, ISimpleInputRuleHandler handler) {
		string fullId = $"{Other.UniqueID}/{id}";
		return Mod.Recipes.RegisterRuleHandler(fullId, handler);
	}

	/// <inheritdoc />
	public bool UnregisterRuleHandler(string id) {
		string fullId = $"{Other.UniqueID}/{id}";
		return Mod.Recipes.UnregisterRuleHandler(fullId);
	}

	[Obsolete("Use the version that doesn't require a manifest.")]
	public bool RegisterRuleHandler(IManifest manifest, string id, IDynamicRuleHandler handler) {
		string fullId = $"{manifest.UniqueID}/{id}";
		return Mod.Recipes.RegisterRuleHandler(fullId, handler);
	}

	[Obsolete("Use the version that doesn't require a manifest.")]
	public bool RegisterRuleHandler(IManifest manifest, string id, ISimpleInputRuleHandler handler) {
		string fullId = $"{manifest.UniqueID}/{id}";
		return Mod.Recipes.RegisterRuleHandler(fullId, handler);
	}

	[Obsolete("Use the version that doesn't require a manifest.")]
	public bool UnregisterRuleHandler(IManifest manifest, string id) {
		string fullId = $"{manifest.UniqueID}/{id}";
		return Mod.Recipes.UnregisterRuleHandler(fullId);
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

	[Obsolete("Included to avoid breaking API compatibility with older mods.")]
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
