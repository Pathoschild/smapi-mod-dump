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

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models;

public class MatcherIngredient : IOptimizedIngredient {

	public readonly Func<Item, bool> ItemMatcher;
	private readonly (Func<Item, bool>, int)[] IngList;

	private readonly Func<string> _displayName;
	private readonly Func<Texture2D> _texture;

	private Rectangle? _source;

	public MatcherIngredient(Func<Item, bool> matcher, int quantity, Func<string> displayName, Func<Texture2D> texture, Rectangle? source = null) {
		ItemMatcher = matcher;
		Quantity = quantity;

		_displayName = displayName;
		_texture = texture;
		_source = source;

		IngList = new (Func<Item, bool>, int)[] {
			(ItemMatcher, Quantity)
		};
	}

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
