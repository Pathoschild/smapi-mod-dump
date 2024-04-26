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

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;

namespace Leclair.Stardew.BetterCrafting.Models;

public class MatcherIngredient : IOptimizedIngredient, IConsumptionTrackingIngredient, IConditionalIngredient, IRecyclable {

	public readonly Func<Item, bool> ItemMatcher;
	private readonly (Func<Item, bool>, int)[] IngList;

	public readonly float RecycleRate;

	private readonly Func<string> _displayName;
	private readonly Func<Texture2D> _texture;

	private readonly Func<Item?>? _recycleTo;

	private Rectangle? _source;

	private readonly bool IsFuzzyRecycle;

	public MatcherIngredient(Func<Item, bool> matcher, int quantity, Func<string> displayName, Func<Texture2D> texture, Rectangle? source = null, Func<Item?>? recycleTo = null, float recycleRate = 1f, string? condition = null) {
		ItemMatcher = matcher;
		Quantity = quantity;
		RecycleRate = recycleRate;

		_displayName = displayName;
		_texture = texture;
		_source = source;

		Condition = condition;

		IngList = new (Func<Item, bool>, int)[] {
			(ItemMatcher, Quantity)
		};

		RecycledSprite = new(item => SpriteHelper.GetSprite(item), () => RecycledItem?.Item1);
		if (recycleTo is not null) {
			IsFuzzyRecycle = false;
			_recycleTo = recycleTo;
		} else
			IsFuzzyRecycle = true;
	}

	#region IConditionalIngredient

	public string? Condition { get; }

	#endregion

	#region IRecyclable

	private Tuple<Item?>? RecycledItem;
	private readonly Cache<SpriteInfo?, Item?> RecycledSprite;

	[MemberNotNull(nameof(RecycledItem))]
	private void LoadRecycledItem() {
		if (RecycledItem is not null)
			return;

		if ( _recycleTo is not null ) {
			RecycledItem = new(_recycleTo());
			return;
		}

		Item? result = null;
		int price = 0;
		int count = 0;

		foreach(Item item in ModEntry.Instance.ItemCache.GetMatchingItems(ItemMatcher)) {
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
		return (int) (Quantity * RecycleRate);
	}

	public bool CanRecycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (RecycleRate <= 0f)
			return false;

		if (!fuzzyItems && IsFuzzyRecycle)
			return false;

		LoadRecycledItem();
		return RecycledItem.Item1 is not null;
	}

	public IEnumerable<Item>? Recycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (!fuzzyItems && IsFuzzyRecycle)
			return null;

		LoadRecycledItem();
		if (RecycledItem.Item1 is not null) {
			var output = IRecyclable.GetManyOf(RecycledItem.Item1, GetRecycleQuantity(who, recycledItem, fuzzyItems));

			if (_recycleTo is not null)
				// Reset it so it's different the next time.
				RecycledItem = null;

			return output;
		}

		return null;
	}

	#endregion

	#region IIngredient

	public bool SupportsQuality => true;

	public string DisplayName => _displayName();

	public Texture2D Texture => _texture();

	public Rectangle SourceRectangle {
		get {
			if (!_source.HasValue)
				_source = _texture().Bounds;
			return _source.Value;
		}
	}

	public int Quantity { get; }

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: maxQuality);
	}

	public bool HasAvailableQuantity(int quantity, Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: maxQuality, limit: quantity) >= quantity;
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool low_quality_first) {
		Consume(who, inventories, max_quality, low_quality_first, null);
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int maxQuality, bool lowQualityFirst, IList<Item>? consumedItems) {
		InventoryHelper.ConsumeItems(IngList, who, inventories, maxQuality, lowQualityFirst, consumedItems);
	}

	#endregion

}
