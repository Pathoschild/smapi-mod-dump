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

using Microsoft.Xna.Framework;

using StardewValley.GameData;

namespace Leclair.Stardew.BetterCrafting.Models;

public class JsonRecipeData {

	public string Id { get; set; } = string.Empty;

	public string? SortValue { get; set; }

	public string? Condition { get; set; }

	public bool Default { get; set; } = false;

	public bool IsCooking { get; set; } = false;

	public bool AllowRecycling { get; set; } = true;

	// Display

	public string? DisplayName { get; set; }

	public string? Description { get; set; }

	public CategoryIcon Icon { get; set; } = new CategoryIcon() { Type = CategoryIcon.IconType.Item };

	public Point? GridSize { get; set; }


	// Cost to Craft

	public JsonIngredientData[] Ingredients { get; set; } = Array.Empty<JsonIngredientData>();


	// Crafting

	public bool AllowBulk { get; set; } = true;

	public List<string>? ActionsOnCraft { get; set; }

	// Output

	public JsonRecipeOutput[] Output { get; set; } = Array.Empty<JsonRecipeOutput>();


}

public class JsonRecipeOutput : GenericSpawnItemDataWithCondition {

}


public enum IngredientType {
	Currency,
	Item
}

public class JsonIngredientData {

	public string Id { get; set; } = string.Empty;

	// Basic Properties

	public string? Condition { get; set; }

	public IngredientType Type { get; set; } = IngredientType.Item;

	public float RecycleRate { get; set; } = 1f;

	public int Quantity { get; set; } = 1;

	// Type: Currency

	public CurrencyType Currency { get; set; } = CurrencyType.Money;


	// Type: Item

	public string? ItemId { get; set; }

	public string[]? ContextTags { get; set; }

	public GenericSpawnItemData? Query { get; set; }

	public GenericSpawnItemDataWithCondition? RecycleItem { get; set; }

	// Item Display

	public string? DisplayName { get; set; }

	public CategoryIcon Icon { get; set; } = new CategoryIcon() {
		Type = CategoryIcon.IconType.Item
	};

}
