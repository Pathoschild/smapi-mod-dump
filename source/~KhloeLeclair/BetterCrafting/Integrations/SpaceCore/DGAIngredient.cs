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

using HarmonyLib;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class DGAIngredient : IOptimizedIngredient, IRecyclable {

	private readonly ModEntry Mod;
	public readonly object Ingredient;
	public readonly IItemAbstraction? ItemAbstraction;

	public readonly string? IngredientType;

	public readonly Func<Item, bool> ItemMatcher;

	private readonly IReflectedProperty<string> DisplayNameProp;
	private readonly IReflectedProperty<Texture2D> TextureProp;
	private readonly IReflectedProperty<Rectangle?> SourceProp;
	private readonly IReflectedProperty<int> QuantityProp;
	private readonly IReflectedMethod MatchesMethod;

	public DGAIngredient(object ingredient, ModEntry mod, IItemAbstraction? itemAbstraction) {
		Mod = mod;
		Ingredient = ingredient;
		ItemAbstraction = itemAbstraction;

		DisplayNameProp = Mod.Helper.Reflection.GetProperty<string>(Ingredient, "DispayName");
		TextureProp = Mod.Helper.Reflection.GetProperty<Texture2D>(Ingredient, "IconTexture");
		SourceProp = Mod.Helper.Reflection.GetProperty<Rectangle?>(Ingredient, "IconSubrect");
		QuantityProp = Mod.Helper.Reflection.GetProperty<int>(Ingredient, "Quantity");

		MatchesMethod = Mod.Helper.Reflection.GetMethod(Ingredient, "ItemMatches");

		ItemMatcher = item => MatchesMethod.Invoke<bool>(item);

		// Ensure we can do stuff.
		DisplayNameProp.GetValue();
		TextureProp.GetValue();
		SourceProp.GetValue();
		QuantityProp.GetValue();

		RecycledSprite = new(item => SpriteHelper.GetSprite(item), () => RecycledItem?.Item2);

		// Try to get the matcher type.
		try {
			object? abstraction = mod.Helper.Reflection.GetField<object>(Ingredient, "ingred", true).GetValue();
			if (abstraction is not null) {
				var prop = AccessTools.Property(abstraction.GetType(), "Type");
				IngredientType = prop.GetValue(abstraction)?.ToString();
			} else
				IngredientType = null;
		} catch {
			IngredientType = null;
		}
	}

	#region IRecyclable

	private Tuple<bool, Item?>? RecycledItem;
	private readonly Cache<SpriteInfo?, Item?> RecycledSprite;

	[MemberNotNull(nameof(RecycledItem))]
	private void LoadRecycledItem() {
		if (RecycledItem is not null)
			return;

		// TODO: Make this better.
		switch(IngredientType) {
			case "DGAItem":
			case "VanillaObject":
			case "VanillaObjectColored":
			case "VanillaBigCraftable":
			case "VanillaWeapon":
			case "VanillaHat":
			case "VanillaClothing":
			case "VanillaBoots":
			case "VanillaFurniture":
				// TODO: Check for vanilla categories?
				RecycledItem = new(false, ItemAbstraction?.Create());
				return;

			case "DGARecipe":
				// Recipes are not supported.
				RecycledItem = new(false, null);
				return;
		}

		// If we got here, we're something odd. But probably just a context
		// tag match. Either way, we're fuzzy.

		Item? result = null;
		int price = 0;
		int count = 0;

		foreach (Item item in ModEntry.Instance.GetMatchingItems(ItemMatcher)) {
			int ip = item.salePrice();
			count++;
			if (result is null || ip < price) {
				result = item;
				price = ip;
			}
		}

		ModEntry.Instance.Log($"Item matches for \"{DisplayName}\": {count} -- Using: {result?.Name}", LogLevel.Trace);

		RecycledItem = new(true, result);
	}

	public Texture2D GetRecycleTexture(Farmer who, Item? recycledItem, bool fuzzyItems) {
		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return Texture;
		return RecycledSprite.Value?.Texture ?? Texture;
	}

	public Rectangle GetRecycleSourceRect(Farmer who, Item? recycledItem, bool fuzzyItems) {
		LoadRecycledItem();
		if (!fuzzyItems && RecycledItem.Item1)
			return SourceRectangle;
		return RecycledSprite.Value?.BaseSource ?? SourceRectangle;
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

		if (RecycledItem.Item2 is not null)
			return IRecyclable.GetManyOf(RecycledItem.Item2, Quantity);

		return null;
	}

	#endregion

	#region IIngredient

	public bool SupportsQuality => true;

	public string DisplayName => DisplayNameProp.GetValue();

	public Texture2D Texture => TextureProp.GetValue();

	public Rectangle SourceRectangle => SourceProp.GetValue() ?? Texture.Bounds;

	public int Quantity => QuantityProp.GetValue();

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: maxQuality);
	}

	public bool HasAvailableQuantity(int quantity, Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: maxQuality, limit: quantity) >= quantity;
	}

	public void Consume(Farmer who, IList<IInventory>? inventories, int maxQuality, bool lowQualityFirst) {
		InventoryHelper.ConsumeItems(new (Func<Item, bool>, int)[] {
			(ItemMatcher, Quantity)
		}, who, inventories, maxQuality, lowQualityFirst);
	}

	#endregion

}
