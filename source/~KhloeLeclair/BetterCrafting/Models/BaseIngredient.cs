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

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using SObject = StardewValley.Object;

namespace Leclair.Stardew.BetterCrafting.Models {
	public class BaseIngredient : IIngredient {

		private readonly int Index;
		private readonly KeyValuePair<int, int>[] IngList;

		public bool SupportsQuality => true;

		public BaseIngredient(int index, int quantity) {
			Index = index;
			Quantity = quantity;

			IngList = new KeyValuePair<int, int>[] {
				new(Index, Quantity)
			};
		}

		public string DisplayName {
			get {
				if (Index < 0)
					switch (Index) {
						case -777:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574");
						case -6:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
						case -5:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
						case -4:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
						case -3:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
						case -2:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
						case -1:
							return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568");
						default:
							return "???";
					}

				if (Game1.objectInformation.ContainsKey(Index))
					return Game1.objectInformation[Index].Split('/')[4];
				return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.575");
			}
		}

		public int SpriteIndex {
			get {
				switch (Index) {
					case -777:
						return 495;
					case -6:
						return 184;
					case -5:
						return 176;
					case -4:
						return 145;
					case -3:
						return 24;
					case -2:
						return 80;
					case -1:
						return 20;
					default:
						return Index;
				}
			}
		}

		public Texture2D Texture => Game1.objectSpriteSheet;

		public Rectangle SourceRectangle => Game1.getSourceRectForStandardTileSheet(Texture, SpriteIndex, 16, 16);

		public int Quantity { get; private set; }

		public void Consume(Farmer who, IList<IInventory> inventories, int max_quality, bool low_quality_first) {
			InventoryHelper.ConsumeItems(IngList, who, inventories, max_quality, low_quality_first);
		}

		public int GetAvailableQuantity(Farmer who, IList<Item> items, IList<IInventory> inventories, int max_quality) {
			int amount = 0;

			if (who != null)
				foreach (var item in who.Items) {
					int quality = item is SObject obj ? obj.Quality : 0;
					if (quality <= max_quality && InventoryHelper.DoesItemMatchID(Index, item))
						amount += item.Stack;
				}

			if (items != null)
				foreach (var item in items) {
					int quality = item is SObject obj ? obj.Quality : 0;
					if (quality <= max_quality && InventoryHelper.DoesItemMatchID(Index, item))
						amount += item.Stack;
				}

			return amount;
		}
	}
}
