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

using SpaceCore;

using StardewValley;
using SObject = StardewValley.Object;

using StardewModdingAPI;

namespace Leclair.Stardew.BCSpaceCore {
	public class DGAIngredient : IIngredient {

		private readonly CustomCraftingRecipe.IngredientMatcher Ingredient;
		private readonly IReflectedMethod Matcher;

		private readonly Tuple<Func<Item, bool>, int>[] IngList;

		public DGAIngredient(IReflectedMethod matcher, CustomCraftingRecipe.IngredientMatcher ingredient) {
			Ingredient = ingredient;
			Matcher = matcher;

			IngList = new Tuple<Func<Item, bool>, int>[] {
				new(ItemMatches, Quantity)
			};
		}

		public bool SupportsQuality => true;

		public string DisplayName => Ingredient.DispayName;
		public Texture2D Texture => Ingredient.IconTexture;
		public Rectangle SourceRectangle => Ingredient.IconSubrect ?? Texture.Bounds;
		public int Quantity => Ingredient.Quantity;

		private bool ItemMatches(Item item) {
			return Matcher.Invoke<bool>(item);
		}

		public void Consume(Farmer who, IList<IInventory> inventories, int max_quality, bool lower_quality_first) {
			InventoryHelper.ConsumeItems(IngList, who, inventories, max_quality, lower_quality_first);
		}

		public int GetAvailableQuantity(Farmer who, IList<Item> items, IList<IInventory> inventories, int max_quality) {
			int amount = 0;

			if (who != null)
				foreach(var item in who.Items) {
					int quality = item is SObject obj ? obj.Quality : 0;
					if (quality <= max_quality && ItemMatches(item))
						amount += item.Stack;
				}

			if (items != null)
				foreach (var item in items) {
					int quality = item is SObject obj ? obj.Quality : 0;
					if (quality <= max_quality && ItemMatches(item))
						amount += item.Stack;
				}

			return amount;
		}
	}
}
