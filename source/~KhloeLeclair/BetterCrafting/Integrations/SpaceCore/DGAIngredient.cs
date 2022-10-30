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

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class DGAIngredient : IOptimizedIngredient {

	private readonly ModEntry Mod;
	public readonly object Ingredient;

	public readonly Func<Item, bool> ItemMatcher;

	private readonly IReflectedProperty<string> DisplayNameProp;
	private readonly IReflectedProperty<Texture2D> TextureProp;
	private readonly IReflectedProperty<Rectangle?> SourceProp;
	private readonly IReflectedProperty<int> QuantityProp;
	private readonly IReflectedMethod MatchesMethod;

	public DGAIngredient(object ingredient, ModEntry mod) {
		Mod = mod;
		Ingredient = ingredient;

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
	}

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
