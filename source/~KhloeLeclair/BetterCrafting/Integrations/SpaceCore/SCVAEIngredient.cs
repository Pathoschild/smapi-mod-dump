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

using Nanoray.Pintail;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCVAEIngredient : IOptimizedIngredient, IConsumptionPreTrackingIngredient, IIngredient, IRecyclable {

	private static Func<object, Item, bool>? CallMatchDelegate;
	private static Func<object, object>? GetIngredientDataDelegate;

	private readonly object UnwrappedIngredient;
	public readonly IIngredientMatcher Ingredient;
	public readonly IVAEIngredientData Data;

	private readonly bool IsFuzzyRecycle;

	public SCVAEIngredient(SCIntegration integration, object unwrapped, IIngredientMatcher ingredient) {
		UnwrappedIngredient = unwrapped;
		Ingredient = ingredient;

		RecycledSprite = new(item => SpriteHelper.GetSprite(item), () => RecycledItem?.Item1);

		if (CallMatchDelegate is null) {
			var method = AccessTools.Method(unwrapped.GetType(), "Matches");
			CallMatchDelegate = ReflectionHelper.CreateFunc<object, Item, bool>(method);
		}

		if (GetIngredientDataDelegate is null) {
			var method = AccessTools.PropertyGetter(unwrapped.GetType(), "Data");
			if (method is null) {
				var field = AccessTools.Field(unwrapped.GetType(), "data");
				GetIngredientDataDelegate = ReflectionHelper.CreateGetter<object, object>(field);
			} else
				GetIngredientDataDelegate = ReflectionHelper.CreateFunc<object, object>(method);
		}

		Data = integration.ProxyMan!.ObtainProxy<IVAEIngredientData>(GetIngredientDataDelegate(unwrapped));
		IsFuzzyRecycle = Data.Type != VAEIngredientType.Item;
		IsFuzzyIngredient = IsFuzzyRecycle;
	}

	private bool Matches(Item item) {
		return CallMatchDelegate?.Invoke(UnwrappedIngredient, item) ?? false;
	}

	#region IIngredient

	public bool IsFuzzyIngredient { get; }

	public bool SupportsQuality => true;

	public string DisplayName => Ingredient.DispayName;

	public Texture2D Texture => Ingredient.IconTexture;

	public Rectangle SourceRectangle => Ingredient.IconSubrect ?? Texture.Bounds;

	public int Quantity => Ingredient.Quantity;

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(Matches, who, items, out bool _, max_quality: maxQuality);
	}

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality, IList<Item>? matchingItems) {
		return InventoryHelper.CountItem(Matches, who, items, out bool _, max_quality: maxQuality, matchingItems: matchingItems);
	}

	public bool HasAvailableQuantity(int quantity, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(Matches, who, items, out bool _, max_quality: maxQuality, limit: quantity) >= quantity;
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool low_quality_first) {
		Consume(who, inventories, max_quality, low_quality_first, null, null);
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, IList<Item>? consumedItems) {
		Consume(who, inventories, maxQuality, lowQualityFirst, null, consumedItems);
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, IList<Item>? matchedItems, IList<Item>? consumedItems) {
		(Func<Item, bool>, int)[] ingredients = [
			(Matches, Quantity)
		];

		InventoryHelper.ConsumeItems(ingredients, who, inventories, maxQuality, lowQualityFirst, matchedItems, consumedItems);
	}

	#endregion

	#region IRecyclable

	private Tuple<Item?>? RecycledItem;
	private readonly Cache<SpriteInfo?, Item?> RecycledSprite;

	[MemberNotNull(nameof(RecycledItem))]
	private void LoadRecycledItem() {
		if (RecycledItem is not null)
			return;

		if (Data.Type == VAEIngredientType.Item) {
			RecycledItem = new(ItemRegistry.Create(Data.Value, allowNull: true));
			return;
		}

		Item? result = null;
		int price = 0;
		int count = 0;

		foreach (Item item in ModEntry.Instance.ItemCache.GetMatchingItems(Matches)) {
			int ip = item.salePrice();
			count++;
			if (result is null || ip < price) {
				result = item;
				price = ip;
			}
		}

		ModEntry.Instance.Log($"Item matches for \"{DisplayName}\": {count} -- Using: {result?.Name}", LogLevel.Debug);
		RecycledItem = new(result);
	}

	public Texture2D GetRecycleTexture(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (!fuzzyItems && IsFuzzyRecycle)
			return Texture;
		LoadRecycledItem();
		return RecycledSprite.Value?.Texture ?? Texture;
	}

	public Rectangle GetRecycleSourceRect(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (!fuzzyItems && IsFuzzyRecycle)
			return SourceRectangle;
		LoadRecycledItem();
		return RecycledSprite.Value?.BaseSource ?? SourceRectangle;
	}

	public string GetRecycleDisplayName(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (!fuzzyItems && IsFuzzyRecycle)
			return DisplayName;
		LoadRecycledItem();
		return RecycledItem.Item1?.DisplayName ?? DisplayName;
	}

	public int GetRecycleQuantity(Farmer who, Item? recycledItem, bool fuzzyItems) {
		return Quantity;
	}

	public bool CanRecycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (!fuzzyItems && IsFuzzyRecycle)
			return false;

		LoadRecycledItem();
		return RecycledItem.Item1 is not null;
	}

	public IEnumerable<Item>? Recycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (!fuzzyItems && IsFuzzyRecycle)
			return null;

		LoadRecycledItem();
		if (RecycledItem.Item1 is not null)
			return IRecyclable.GetManyOf(RecycledItem.Item1, GetRecycleQuantity(who, recycledItem, fuzzyItems));

		return null;
	}

	#endregion

}
