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
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.BetterCrafting.DynamicRules;
using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Delegates;

namespace Leclair.Stardew.BetterCrafting.Managers;

public class DataRecipeManager : BaseManager, IRecipeProvider {

	public readonly static string RECIPE_PATH = @"Mods/leclair.bettercrafting/Recipes";
	public readonly static string RULES_PATH = @"Mods/leclair.bettercrafting/Rules";

	public Dictionary<string, DataRecipe>? DataRecipesById;

	private bool HasLoadedRules;
	public readonly Dictionary<string, DataRuleHandler> RulesById = [];

	public DataRecipeManager(ModEntry mod) : base(mod) {

		Mod.Recipes.AddProvider(this);

	}

	#region Loading

	public void Invalidate() {
		DataRecipesById = null;
		HasLoadedRules = false;
		LoadRules();
	}

	[Subscriber]
	private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
		LoadRules();
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(RECIPE_PATH))
			e.LoadFrom(() => new Dictionary<string, JsonRecipeData>(), AssetLoadPriority.Exclusive);
		if (e.Name.IsEquivalentTo(RULES_PATH))
			e.LoadFrom(() => new Dictionary<string, JsonDynamicRule>(), AssetLoadPriority.Exclusive);
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.Names) {
			if (name.IsEquivalentTo(RECIPE_PATH))
				DataRecipesById = null;
			if (name.IsEquivalentTo(RULES_PATH)) {
				HasLoadedRules = false;
				LoadRules();
			}
		}
	}

	[MemberNotNull(nameof(DataRecipesById))]
	public void LoadRecipes() {
		if (DataRecipesById != null)
			return;

		var loaded = Mod.Helper.GameContent.Load<Dictionary<string, JsonRecipeData>>(RECIPE_PATH);
		DataRecipesById = new();

		// Time to hydrate our recipes.

		foreach (var pair in loaded) {
			var recipe = pair.Value;
			recipe.Id = pair.Key;

			recipe.Ingredients ??= Array.Empty<JsonIngredientData>();

			if (recipe.Output == null || recipe.Output.Length < 1) {
				Log($"Skipping recipe '{recipe.Id}' with no output.", StardewModdingAPI.LogLevel.Warn);
				continue;
			}

			recipe.Icon ??= new CategoryIcon() {
				Type = CategoryIcon.IconType.Item
			};

			DataRecipesById[recipe.Id] = new DataRecipe(Mod, recipe);
		}
	}

	public bool TryGetRecipeById(string id, [NotNullWhen(true)] out DataRecipe? recipe) {
		LoadRecipes();
		return DataRecipesById.TryGetValue(id, out recipe);
	}

	#endregion

	#region Rule Loading

	public void LoadRules() {
		if (HasLoadedRules)
			return;

		HasLoadedRules = true;

		// Remove all the existing items.
		foreach (string key in RulesById.Keys)
			Mod.Recipes.UnregisterRuleHandler($"data:{key}");

		RulesById.Clear();

		var loaded = Mod.Helper.GameContent.Load<Dictionary<string, JsonDynamicRule>>(RULES_PATH);

		foreach (var entry in loaded) {
			// Create all new entries.
			entry.Value.Id = entry.Key;

			if (entry.Value.Rules is null || entry.Value.Rules.Length == 0) {
				Log($"Skipping empty data-based category rule '{entry.Value.Id}'.", LogLevel.Warn);
				continue;
			}

			entry.Value.Icon ??= new CategoryIcon() { Type = CategoryIcon.IconType.Item };

			var handler = RulesById[entry.Key] = new DataRuleHandler(Mod, entry.Value);
			Mod.Recipes.RegisterRuleHandler($"data:{entry.Key}", handler);
		}
	}

	#endregion

	#region IRecipeProvider

	public int RecipePriority => 0;

	public bool CacheAdditionalRecipes => false;

	public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking, GameStateQueryContext? context = null) {
		LoadRecipes();

		foreach (var recipe in DataRecipesById.Values) {
			if (!string.IsNullOrEmpty(recipe.Data.Condition)) {
				bool valid = context is null
					? GameStateQuery.CheckConditions(recipe.Data.Condition)
					: GameStateQuery.CheckConditions(recipe.Data.Condition, context.Value);

				if (!valid)
					continue;
			}


			if (cooking == recipe.Data.IsCooking)
				yield return recipe;
		}
	}

	public IRecipe? GetRecipe(CraftingRecipe recipe) {
		LoadRecipes();

		if (DataRecipesById.ContainsKey(recipe.name))
			return new InvalidRecipe(recipe.name);

		return null;
	}

	public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) {
		return GetAdditionalRecipes(cooking, null);
	}

	#endregion

}

public class InvalidRecipe : IRecipe {

	public InvalidRecipe(string id) {
		Name = id;
		Ingredients = [new ErrorIngredient()];
	}

	public string SortValue => "";

	public string Name { get; }

	public string DisplayName => "";

	public string? Description => null;

	public bool AllowRecycling => false;

	public CraftingRecipe? CraftingRecipe => null;

	public Texture2D Texture => Game1.mouseCursors;

	public Rectangle SourceRectangle => ErrorIngredient.SOURCE;

	public int GridHeight => 1;

	public int GridWidth => 1;

	public int QuantityPerCraft => 0;

	public IIngredient[]? Ingredients { get; }

	public bool Stackable => false;

	public bool CanCraft(Farmer who) {
		return false;
	}

	public Item? CreateItem() {
		return null;
	}

	public int GetTimesCrafted(Farmer who) {
		return 0;
	}

	public string? GetTooltipExtra(Farmer who) {
		return null;
	}

	public bool HasRecipe(Farmer who) {
		return false;
	}
}
