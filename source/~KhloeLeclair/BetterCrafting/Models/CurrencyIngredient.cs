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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models;

/// <summary>
/// The various currency types supported by <see cref="CreateCurrencyIngredient(string, int)"/>
/// </summary>
public enum CurrencyType {
	/// <summary>
	/// The player's gold.
	/// </summary>
	Money,
	/// <summary>
	/// The player's earned points at the current festival. This should likely
	/// never actually be used, since players can't craft while they're at a
	/// festival in the first place.
	/// </summary>
	FestivalPoints,
	/// <summary>
	/// The player's casino points.
	/// </summary>
	ClubCoins,
	/// <summary>
	/// The player's Qi Gems.
	/// </summary>
	QiGems
};

public class CurrencyIngredient : IIngredient, IConditionalIngredient, IRecyclable {

	public readonly static Rectangle ICON_MONEY = new(193, 373, 9, 10);
	public readonly static Rectangle ICON_FESTIVAL_POINTS = new(202, 373, 9, 10);
	public readonly static Rectangle ICON_CLUB_COINS = new(211, 373, 9, 10);

	public readonly CurrencyType Type;
	public readonly float RecycleRate;

	public bool SupportsQuality => true;


	public CurrencyIngredient(CurrencyType type, int quantity, float recycleRate = 1f, string? condition = null) {
		Type = type;
		Quantity = quantity;
		RecycleRate = recycleRate;
		Condition = condition;
	}

	#region IConditionalIngredient

	public string? Condition { get; }

	#endregion

	#region IRecyclable

	public Texture2D GetRecycleTexture(Farmer who, Item? recycledItem, bool fuzzyItems) {
		return Texture;
	}

	public Rectangle GetRecycleSourceRect(Farmer who, Item? recycledItem, bool fuzzyItems) {
		return SourceRectangle;
	}

	public string GetRecycleDisplayName(Farmer who, Item? recycledItem, bool fuzzyItems) {
		return DisplayName;
	}

	public int GetRecycleQuantity(Farmer who, Item? recycledItem, bool fuzzyItems) {
		return (int) (Quantity * RecycleRate);
	}

	public bool CanRecycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		if (RecycleRate <= 0f)
			return false;

		switch (Type) {
			case CurrencyType.Money:
			case CurrencyType.FestivalPoints:
			case CurrencyType.ClubCoins:
			case CurrencyType.QiGems:
				return true;
		}

		return false;
	}

	public IEnumerable<Item>? Recycle(Farmer who, Item? recycledItem, bool fuzzyItems) {
		int quantity = GetRecycleQuantity(who, recycledItem, fuzzyItems);

		switch (Type) {
			case CurrencyType.Money:
				who.Money += quantity;
				break;
			case CurrencyType.FestivalPoints:
				who.festivalScore += quantity;
				break;
			case CurrencyType.ClubCoins:
				who.clubCoins += quantity;
				break;
			case CurrencyType.QiGems:
				who.QiGems += quantity;
				break;
		}

		return null;
	}

	#endregion

	public string DisplayName {
		get {
			switch (Type) {
				case CurrencyType.Money:
					return I18n.Currency_Gold();
				case CurrencyType.FestivalPoints:
					return I18n.Currency_FestivalPoints();
				case CurrencyType.ClubCoins:
					return I18n.Currency_ClubCoins();
				case CurrencyType.QiGems:
					return I18n.Currency_QiGems();
				default:
					return "???";
			}
		}
	}

	public Texture2D Texture => Type switch {
		CurrencyType.QiGems => Game1.objectSpriteSheet,
		_ => Game1.mouseCursors
	};

	public Rectangle SourceRectangle {
		get {
			switch (Type) {
				case CurrencyType.Money:
					return ICON_MONEY;
				case CurrencyType.FestivalPoints:
					return ICON_FESTIVAL_POINTS;
				case CurrencyType.ClubCoins:
					return ICON_CLUB_COINS;
				case CurrencyType.QiGems:
					return Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16);
				default:
					return Rectangle.Empty;
			}
		}
	}

	public int Quantity { get; }

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool low_quality_first) {
		switch (Type) {
			case CurrencyType.Money:
				who.Money -= Quantity;
				break;
			case CurrencyType.FestivalPoints:
				who.festivalScore -= Quantity;
				break;
			case CurrencyType.ClubCoins:
				who.clubCoins -= Quantity;
				break;
			case CurrencyType.QiGems:
				who.QiGems -= Quantity;
				break;
		}
	}

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int max_quality) {
		switch (Type) {
			case CurrencyType.Money:
				return who.Money;
			case CurrencyType.FestivalPoints:
				return who.festivalScore;
			case CurrencyType.ClubCoins:
				return who.clubCoins;
			case CurrencyType.QiGems:
				return who.QiGems;
		}

		return 0;
	}
}
