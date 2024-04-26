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
using System.Collections.Generic;

using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using StardewValley;
using StardewModdingAPI;
using StardewValley.Delegates;

namespace Leclair.Stardew.Common;

public static class CraftingHelper {

	public static bool PassesConditionQuery(this IIngredient ingredient, GameStateQueryContext ctx) {
		return ingredient is not IConditionalIngredient cing ||
			string.IsNullOrEmpty(cing.Condition) ||
			GameStateQuery.CheckConditions(cing.Condition, ctx);
	}

	public static bool HasIngredients(IIngredient[]? ingredients, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality) {
		if (ingredients == null || ingredients.Length == 0)
			return true;

		GameStateQueryContext ctx = new(Game1.player.currentLocation, Game1.player, null, null, Game1.random);

		foreach (var entry in ingredients)
			if (entry.Quantity < 1 || ! entry.PassesConditionQuery(ctx))
				continue;
			else if (entry is IOptimizedIngredient opti) {
				if (!opti.HasAvailableQuantity(entry.Quantity, who, items, inventories, maxQuality))
					return false;
			} else {
				if (entry.GetAvailableQuantity(who, items, inventories, maxQuality) < entry.Quantity)
					return false;
			}

		return true;
	}

	public static bool HasIngredients(this IRecipe recipe, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality) {
		if (recipe.Ingredients == null)
			return true;

		return HasIngredients(recipe.Ingredients, who, items, inventories, maxQuality);
	}

	public static void ConsumeIngredients(IIngredient[]? ingredients, Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, IList<Item>? consumedItems) {
		if (ingredients != null) {
			GameStateQueryContext ctx = new(Game1.player.currentLocation, Game1.player, null, null, Game1.random);

			foreach (var entry in ingredients) {
				if (entry.Quantity < 1 || ! entry.PassesConditionQuery(ctx))
					continue;

				if (entry is IConsumptionTrackingIngredient cst)
					cst.Consume(who, inventories, maxQuality, lowQualityFirst, consumedItems);
				else
					entry.Consume(who, inventories, maxQuality, lowQualityFirst);
			}
		}
	}

	public static void Consume(this IRecipe recipe, Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, IList<Item>? consumedItems) {
		if (recipe.Ingredients != null)
			ConsumeIngredients(recipe.Ingredients, who, inventories, maxQuality, lowQualityFirst, consumedItems);
	}


	public delegate void LogMethod(string message, LogLevel level = LogLevel.Debug, Exception? ex = null);

	public static Item? CreateItemSafe(this IRecipe recipe, LogMethod? log = null) {
		try {
			return recipe.CreateItem();
		} catch (Exception ex) {
			log?.Invoke($"Unable to create item instance for recipe \"{recipe.Name}\".", LogLevel.Warn, ex);
		}

		return null;
	}

}
