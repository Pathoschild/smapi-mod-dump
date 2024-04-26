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
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace Leclair.Stardew.BetterCrafting.Models;

public class BaseIngredient : IOptimizedIngredient, IConsumptionTrackingIngredient, IConditionalIngredient, IRecyclable {

	private readonly string ItemId;
	private readonly int NumericId;
	private readonly KeyValuePair<string, int>[] IngList;

	private readonly ParsedItemData? Data;

	public readonly float RecycleRate;

	public bool SupportsQuality => true;

	public BaseIngredient(int itemId, int quantity, float recycleRate = 1f) : this($"{itemId}", quantity, recycleRate) { }

	public BaseIngredient(string itemId, int quantity, float recycleRate = 1f, string? condition = null) {
		ItemId = itemId;
		Quantity = quantity;
		RecycleRate = recycleRate;
		Condition = condition;

		if (!int.TryParse(ItemId, out NumericId))
			NumericId = int.MaxValue;

		// Cache item data for this item.
		if (NumericId >= 0)
			Data = ItemRegistry.GetDataOrErrorItem(ItemId);

		IngList = new KeyValuePair<string, int>[] {
			new(ItemId, Quantity)
		};
	}

	#region IConditionalIngredient

	public string? Condition { get; }

	#endregion

	#region IRecyclable

	private Tuple<bool, Item?>? RecycledItem;

	[MemberNotNull(nameof(RecycledItem))]
	public void LoadRecycledItem() {
		if (RecycledItem is not null)
			return;

		// Handle non-category items.
		if (NumericId == -777 || NumericId >= 0) {
			string id = ItemId;

			// Wild Seeds (Any) are special.
			if (id == "-777")
				id = Game1.Date.SeasonIndex switch {
					0 => "(O)495", // Spring
					1 => "(O)496", // Summer
					2 => "(O)497", // Fall
					_ => "(O)498"  // Winter
				};

			RecycledItem = new(false, ItemRegistry.Create(id, 1));
			return;
		}

		// Fuzzy search for the rest.
		Item? result = null;
		int price = 0;
		int count = 0;

		foreach (Item item in ModEntry.Instance.ItemCache.GetMatchingItems(item => item.Category == NumericId)) {
			int ip = item.salePrice();
			count++;
			if (result is null || ip < price) {
				result = item;
				price = ip;
			}
		}

		ModEntry.Instance.Log($"Item matches for \"{ItemId}\": {count} -- Using: {result?.Name} (Price: {price})", LogLevel.Trace);
		RecycledItem = new(true, result);
	}

	public Texture2D GetRecycleTexture(Farmer who, Item? recycledItem, bool fuzzyItems) {
		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return Texture;
		return SpriteHelper.GetTexture(RecycledItem.Item2) ?? Texture;
	}

	public Rectangle GetRecycleSourceRect(Farmer who, Item? recycledItem, bool fuzzyItems) {
		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return SourceRectangle;
		return SpriteHelper.GetSourceRectangle(RecycledItem.Item2) ?? SourceRectangle;
	}

	public string GetRecycleDisplayName(Farmer who, Item? recycledItem, bool fuzzyItems) {
		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return DisplayName;
		return RecycledItem.Item2?.DisplayName ?? DisplayName;
	}

	public int GetRecycleQuantity(Farmer who, Item? recycledItem, bool fuzzyItems) {
		return (int) (Quantity * RecycleRate);
	}

	public bool CanRecycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (RecycleRate <= 0f)
			return false;

		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return false;

		return RecycledItem.Item2 is not null;
	}

	public IEnumerable<Item>? Recycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return null;

		Item? item = RecycledItem.Item2;
		if (item is null)
			return null;

		return IRecyclable.GetManyOf(item, GetRecycleQuantity(who, recycledItem, fuzzyItems));
	}

	#endregion

	public string DisplayName {
		get {
			if (Data is not null)
				return Data.DisplayName;

			switch (NumericId) {
				// Specials

				case -777: // Wild Seeds (Any)
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574");

				// Categories

				case SObject.GemCategory: // Gem (Any)
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");

				case SObject.FishCategory: // Fish (Any)
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");

				case SObject.EggCategory: // Egg (Any)
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");

				case SObject.MilkCategory: // Milk (Any)
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");

				case SObject.CookingCategory: // Cooking (Any)
					return I18n.Item_Category_Cooking();

				case SObject.CraftingCategory: // Crafting (Any)
					return I18n.Item_Category_Crafting();

				case SObject.mineralsCategory: // Minerals (Any)
					return I18n.Item_Category_Mineral();

				case SObject.meatCategory: // Meat (Any)
					return I18n.Item_Category_Meat();

				case SObject.fertilizerCategory: // Fertilizer (Any)
					return I18n.Item_Category_Fertilizer();

				case SObject.junkCategory: // Junk (Any)
					return I18n.Item_Category_Junk();

				case SObject.SeedsCategory: // Seed (Any)
					return I18n.Item_Category_Seeds();

				case SObject.VegetableCategory: // Vegetable (Any)
				case -3:
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");

				case SObject.FruitsCategory: // Fruit (Any)
					return I18n.Item_Category_Fruit();

				case SObject.flowersCategory: // Flower (Any)
					return I18n.Item_Category_Flower();

				case SObject.GreensCategory: // Greens (Any)
				case -1:
					return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568");

				default:
					return "???";
			}
		}
	}

	public int SpriteIndex {
		get {
			if (Data is not null)
				return Data.SpriteIndex;

			switch (NumericId) {
				// Specials

				case -777: // Wild Seeds
					return 495;

				// Categories

				case SObject.GemCategory:
					return 80; // Quartz

				case SObject.FishCategory:
					return 145; // Sunfish

				case SObject.EggCategory:
					return 176; // Egg

				case SObject.MilkCategory:
					return 184; // Milk

				case SObject.CookingCategory:
					return 662; // Copied from Love of Cooking, unused

				case SObject.CraftingCategory:
					return 298; // Hardwood Fence

				case SObject.mineralsCategory:
					return 546; // Geminite

				case SObject.meatCategory:
					return 640; // Copied from Love of Cooking, unused

				case SObject.fertilizerCategory:
					return 465; // Speed-Gro

				case SObject.junkCategory:
					return 168; // Trash

				case SObject.SeedsCategory:
					return 472; // Parsnip Seeds

				case SObject.VegetableCategory:
				case -3: // Other Vegetables Category?
					return 24; // Parsnip

				case SObject.FruitsCategory:
					return 406; // Wild Plum

				case SObject.flowersCategory:
					return 591; // Tulip

				case SObject.GreensCategory:
				case -1: // Other Greens Category?
					return 20; // Leek

				default:
					// Just get an error item at this point.
					return ItemRegistry.GetDataOrErrorItem("(O)THISdoesNOTexistEVER").SpriteIndex;
			}
		}
	}

	public Texture2D Texture => Data?.GetTexture() ?? Game1.objectSpriteSheet;

	public Rectangle SourceRectangle => Data?.GetSourceRect() ?? Game1.getSourceRectForStandardTileSheet(Texture, SpriteIndex, 16, 16);

	public int Quantity { get; private set; }

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool low_quality_first) {
		Consume(who, inventories, max_quality, low_quality_first, null);
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool low_quality_first, IList<Item>? consumedItems) {
		InventoryHelper.ConsumeItems(IngList, who, inventories, max_quality, low_quality_first, consumedItems);
	}


	public bool HasAvailableQuantity(int quantity, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int max_quality) {
		bool ItemMatcher(Item item) {
			return CraftingRecipe.ItemMatchesForCrafting(item, ItemId);
		}

		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: max_quality, limit: quantity) >= quantity;
	}

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int max_quality) {
		bool ItemMatcher(Item item) {
			return CraftingRecipe.ItemMatchesForCrafting(item, ItemId);
		}

		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: max_quality);
	}
}
