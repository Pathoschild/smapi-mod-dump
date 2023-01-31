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

public class MatcherIngredient : IOptimizedIngredient, IRecyclable {

	public readonly Func<Item, bool> ItemMatcher;
	private readonly (Func<Item, bool>, int)[] IngList;

	private readonly Func<string> _displayName;
	private readonly Func<Texture2D> _texture;

	private Rectangle? _source;

	private readonly bool IsFuzzyRecycle;

	public MatcherIngredient(Func<Item, bool> matcher, int quantity, Func<string> displayName, Func<Texture2D> texture, Rectangle? source = null, Item? recycleTo = null) {
		ItemMatcher = matcher;
		Quantity = quantity;

		_displayName = displayName;
		_texture = texture;
		_source = source;

		IngList = new (Func<Item, bool>, int)[] {
			(ItemMatcher, Quantity)
		};

		RecycledSprite = new(item => SpriteHelper.GetSprite(item), () => RecycledItem?.Item1);
		if (recycleTo is not null) {
			IsFuzzyRecycle = false;
			RecycledItem = new(recycleTo);
		} else
			IsFuzzyRecycle = true;
	}

	#region IRecyclable

	private Tuple<Item?>? RecycledItem;
	private readonly Cache<SpriteInfo?, Item?> RecycledSprite;

	[MemberNotNull(nameof(RecycledItem))]
	private void LoadRecycledItem() {
		if (RecycledItem is not null)
			return;

		Item? result = null;
		int price = 0;
		int count = 0;

		foreach(Item item in ModEntry.Instance.GetMatchingItems(ItemMatcher)) {
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
			return IRecyclable.GetManyOf(RecycledItem.Item1, Quantity);

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

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: maxQuality);
	}

	public bool HasAvailableQuantity(int quantity, Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int maxQuality) {
		return InventoryHelper.CountItem(ItemMatcher, who, items, out bool _, max_quality: maxQuality, limit: quantity) >= quantity;
	}

	public void Consume(Farmer who, IList<IInventory>? inventories, int maxQuality, bool lowQualityFirst) {
		InventoryHelper.ConsumeItems(IngList, who, inventories, maxQuality, lowQualityFirst);
	}

	#endregion

}
