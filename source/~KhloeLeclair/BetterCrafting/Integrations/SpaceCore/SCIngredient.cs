/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

using StardewValley.Objects;
using StardewValley;
using Leclair.Stardew.Common.Inventory;
using StardewValley.Inventories;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCIngredient : IIngredient {

	public readonly IIngredientMatcher Ingredient;

	public SCIngredient(IIngredientMatcher ingredient) {
		Ingredient = ingredient;
	}

	#region IIngredient

	public bool SupportsQuality => false;

	public string DisplayName => Ingredient.DispayName;

	public Texture2D Texture => Ingredient.IconTexture;

	public Rectangle SourceRectangle => Ingredient.IconSubrect ?? Texture.Bounds;

	public int Quantity => Ingredient.Quantity;

	public int GetAvailableQuantity(Farmer who, IList<Item?>? _, IList<IBCInventory>? inventories, int max_quality) {
		if (who != Game1.player)
			return 0;

		List<Item> items = new();
		items.AddRange(who.Items);

		// Rather than using the provided Item list, we need
		// a list that is only items from chests because
		// SpaceCore ingredient matchers only understand
		// how to consume items from chests.
		if (inventories != null)
			foreach (var inv in inventories) {
				if (inv.Object is Chest chest)
					items.AddRange(chest.Items);
			}

		return Ingredient.GetAmountInList(items);
	}

	public void Consume(Farmer who, IList<IBCInventory>? inventories, int max_quality, bool lower_quality_first) {
		// Unfortunately, we're always going to need chests for this
		// due to how SpaceCore is implemented.
		if (who == Game1.player)
			Ingredient.Consume(GetInventories(inventories));
	}

	private static List<IInventory> GetInventories(IList<IBCInventory>? inventories) {
		if (inventories is null)
			return new List<IInventory>();

		return inventories
			.Where(val => val.Inventory is not null)
			.Select(val => val.Inventory!)
			.ToList();
	}

	#endregion

}
