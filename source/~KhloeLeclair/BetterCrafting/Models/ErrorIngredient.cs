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

using System.Collections.Generic;

using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models;

public class ErrorIngredient : IIngredient {

	public static readonly Rectangle SOURCE = new(268, 470, 16, 16);

	public bool SupportsQuality => true;

	public ErrorIngredient() {
			
	}

	public string DisplayName => I18n.Ingredient_Error();

	public Texture2D Texture => Game1.mouseCursors;

	public Rectangle SourceRectangle => SOURCE;

	public int Quantity => 1;

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool low_quality_first) {

	}

	public int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IBCInventory>? inventories, int max_quality) {
		return 0;
	}
}
