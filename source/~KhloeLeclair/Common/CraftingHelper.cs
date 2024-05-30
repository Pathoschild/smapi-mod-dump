/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_CRAFTING

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;

namespace Leclair.Stardew.Common;

public static class CraftingHelper {

	public static bool PassesConditionQuery(this IIngredient ingredient, GameStateQueryContext ctx) {
		return ingredient is not IConditionalIngredient cing ||
			string.IsNullOrEmpty(cing.Condition) ||
			GameStateQuery.CheckConditions(cing.Condition, ctx);
	}

	public static bool HasIngredients(IIngredient[]? ingredients, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality, Dictionary<IIngredient, List<Item>>? matchingItems = null) {
		if (ingredients == null || ingredients.Length == 0)
			return true;

		GameStateQueryContext ctx = new(Game1.player.currentLocation, Game1.player, null, null, Game1.random);

		foreach (var entry in ingredients)
			if (entry.Quantity < 1 || !entry.PassesConditionQuery(ctx))
				continue;
			else if (matchingItems is not null && entry is IConsumptionPreTrackingIngredient cpt) {
				List<Item> matching = [];
				matchingItems[entry] = matching;
				if (cpt.GetAvailableQuantity(who, items, inventories, maxQuality, matching) < entry.Quantity)
					return false;

			} else if (entry is IOptimizedIngredient opti) {
				if (!opti.HasAvailableQuantity(entry.Quantity, who, items, inventories, maxQuality))
					return false;

			} else {
				if (entry.GetAvailableQuantity(who, items, inventories, maxQuality) < entry.Quantity)
					return false;
			}

		return true;
	}

	public static bool HasIngredients(this IRecipe recipe, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality, Dictionary<IIngredient, List<Item>>? matchingItems = null) {
		if (recipe.Ingredients == null)
			return true;

		return HasIngredients(recipe.Ingredients, who, items, inventories, maxQuality, matchingItems);
	}

	public static void ConsumeIngredients(IIngredient[]? ingredients, Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, Dictionary<IIngredient, List<Item>>? matchingItems, IList<Item>? consumedItems, bool[]? modifiedInventories) {
		if (ingredients != null) {
			GameStateQueryContext ctx = new(Game1.player.currentLocation, Game1.player, null, null, Game1.random);

			InventoryHelper.GlobalModified = inventories != null && modifiedInventories != null ? (inventories, modifiedInventories) : null;

			foreach (var entry in ingredients) {
				if (entry.Quantity < 1 || !entry.PassesConditionQuery(ctx))
					continue;

				if (entry is IConsumptionPreTrackingIngredient cpt) {
					var matched = matchingItems?.GetValueOrDefault(entry);
					cpt.Consume(who, inventories, maxQuality, lowQualityFirst, matched, consumedItems);

				} else if (entry is IConsumptionTrackingIngredient cst)
					cst.Consume(who, inventories, maxQuality, lowQualityFirst, consumedItems);
				else
					entry.Consume(who, inventories, maxQuality, lowQualityFirst);
			}

			InventoryHelper.GlobalModified = null;
		}
	}

	public static void Consume(this IRecipe recipe, Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, Dictionary<IIngredient, List<Item>>? matchingItems, IList<Item>? consumedItems, bool[]? modifiedInventories) {
		if (recipe.Ingredients != null)
			ConsumeIngredients(recipe.Ingredients, who, inventories, maxQuality, lowQualityFirst, matchingItems, consumedItems, modifiedInventories);
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

#endif
