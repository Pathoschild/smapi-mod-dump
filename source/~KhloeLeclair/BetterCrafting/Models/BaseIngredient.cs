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

namespace Leclair.Stardew.BetterCrafting.Models;

public class BaseIngredient : IOptimizedIngredient, IRecyclable {

	private readonly int Index;
	private readonly KeyValuePair<int, int>[] IngList;

	public bool SupportsQuality => true;

	public BaseIngredient(int index, int quantity) {
		Index = index;
		Quantity = quantity;

		IngList = new KeyValuePair<int, int>[] {
			new(Index, Quantity)
		};
	}

	#region IRecyclable

	private Tuple<bool, Item?>? RecycledItem;

	[MemberNotNull(nameof(RecycledItem))]
	public void LoadRecycledItem() {
		if (RecycledItem is not null)
			return;

		int id = Index;

		// Wild Seeds (Any) are special.
		if (id == -777)
			id = Game1.Date.SeasonIndex switch {
				0 => 495, // Spring
				1 => 496, // Summer
				2 => 497, // Fall
				_ => 498  // Winter
			};

		if (id >= 0) {
			RecycledItem = new(false, InventoryHelper.CreateObjectOrRing(id));
			return;
		}

		// Fuzzy search.
		Item? result = null;
		int price = 0;
		int count = 0;

		foreach (Item item in ModEntry.Instance.GetMatchingItems(item => item.Category == id)) {
			int ip = item.salePrice();
			count++;
			if (result is null || ip < price) {
				result = item;
				price = ip;
			}
		}

		ModEntry.Instance.Log($"Item matches for \"{id}\": {count} -- Using: {result?.Name} (Price: {price})", LogLevel.Trace);
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
		return Quantity;
	}

	public bool CanRecycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
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

		return IRecyclable.GetManyOf(item, Quantity);
	}

	#endregion

	public string DisplayName {
		get {
			if (Index < 0)
				switch (Index) {
					// Specials

					case -777: // Wild Seeds (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574");

					// Categories

					case -2: // Gem (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");

					case -4: // Fish (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");

					case -5: // Egg (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");

					case -6: // Milk (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");

					case -7: // Cooking (Any)
						return I18n.Item_Category_Cooking();

					case -8: // Crafting (Any)
						return I18n.Item_Category_Crafting();

					case -12: // Minerals (Any)
						return I18n.Item_Category_Mineral();

					case -14: // Meat (Any)
						return I18n.Item_Category_Meat();

					case -19: // Fertilizer (Any)
						return I18n.Item_Category_Fertilizer();

					case -20: // Junk (Any)
						return I18n.Item_Category_Junk();

					case -74: // Seed (Any)
						return I18n.Item_Category_Seeds();

					case -75:
					case -3: // Vegetable (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");

					case -79: // Fruit (Any)
						return I18n.Item_Category_Fruit();

					case -80: // Flower (Any)
						return I18n.Item_Category_Flower();

					case -81:
					case -1: // Greens (Any)
						return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568");

					default:
						return "???";
				}

			if (Game1.objectInformation.ContainsKey(Index))
				return Game1.objectInformation[Index].Split('/')[4];
			return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.575");
		}
	}

	public int SpriteIndex {
		get {
			switch (Index) {
				// Specials

				case -777: // Wild Seeds
					return 495;

				// Categories

				case -2: // Gem Category
					return 80; // Quartz

				case -4: // Fish Category
					return 145; // Sunfish

				case -5: // Egg Category
					return 176; // Egg

				case -6: // Milk Category
					return 184; // Milk

				case -7: // Cooking Category
					return 662; // Copied from Love of Cooking, unused

				case -8: // Crafting Category
					return 298; // Hardwood Fence

				case -12: // Minerals Category
					return 546; // Geminite

				case -14: // Meat Category
					return 640; // Copied from Love of Cooking, unused

				case -19: // Fertilizer Category
					return 465; // Speed-Gro

				case -20: // Junk Category
					return 168; // Trash

				case -74: // Seed Category
					return 472; // Parsnip Seeds

				case -75: // Vegetables Category
				case -3: // Other Vegetables Category?
					return 24; // Parsnip

				case -79: // Fruits Category
					return 406; // Wild Plum

				case -80: // Flowers Category
					return 591; // Tulip

				case -81: // Greens Category
				case -1: // Other Greens Category?
					return 20; // Leek

				default:
					return Index;
			}
		}
	}

	public Texture2D Texture => Game1.objectSpriteSheet;

	public Rectangle SourceRectangle => Game1.getSourceRectForStandardTileSheet(Texture, SpriteIndex, 16, 16);

	public int Quantity { get; private set; }

	public void Consume(Farmer who, IList<IInventory>? inventories, int max_quality, bool low_quality_first) {
		InventoryHelper.ConsumeItems(IngList, who, inventories, max_quality, low_quality_first);
	}

	public bool HasAvailableQuantity(int quantity, Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int max_quality) {
		bool ItemMatcher(Item item) {
			return InventoryHelper.DoesItemMatchID(Index, item);
		}

		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: max_quality, limit: quantity) >= quantity;
	}

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int max_quality) {
		bool ItemMatcher(Item item) {
			return InventoryHelper.DoesItemMatchID(Index, item);
		}

		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: max_quality);
	}
}
