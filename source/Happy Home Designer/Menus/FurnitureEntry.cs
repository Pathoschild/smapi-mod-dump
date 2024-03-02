/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Integration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace HappyHomeDesigner.Menus
{
	internal class FurnitureEntry : IGridItem
	{
		internal const int CELL_SIZE = 80;

		public Furniture Item;
		public bool HasVariants;
		public bool Favorited;
		private readonly string season = string.Empty;

		// 384 396 15 15 cursors
		// 256 256 10 10 cursors
		// 128 128 64 64 menus

		private static readonly Rectangle background = new(128, 128, 64, 64);
		private static readonly Rectangle star = new(6, 38, 7, 7);
		private static readonly Rectangle favRibbon = new(0, 38, 6, 6);

		/// <summary>Standard constructor. Used for main catalog page.</summary>
		/// <param name="Item">The contained furniture item.</param>
		/// <param name="season">The local season. Required to accurately check for AT variants.</param>
		public FurnitureEntry(Furniture Item, string season, IList<string> favorites)
		{
			this.Item = Item;
			this.season = season;
			HasVariants = AlternativeTextures.Installed && AlternativeTextures.HasVariant("Furniture_" + Item.ItemId, season);
			// 1.6: port to id
			Favorited = favorites.Contains(Item.Name);
		}

		/// <summary>Used for AT variant entries.</summary>
		/// <param name="Item">The contained furniture item, with AT tags applied.</param>
		public FurnitureEntry(Furniture Item)
		{
			this.Item = Item;
			Item.currentRotation.Value = 0;
			Item.updateRotation();
			HasVariants = false;
		}

		public IList<Furniture> GetVariants()
		{
			if (!HasVariants)
				return new[] {Item};

			List<Furniture> skins = new() { Item };
			AlternativeTextures.VariantsOf(Item, season, skins);
			return skins;
		}

		public void Draw(SpriteBatch b, int x, int y)
		{
			IClickableMenu.drawTextureBox(b, Game1.menuTexture, background, x, y, CELL_SIZE, CELL_SIZE, Color.White, 1f, false);
			Item?.drawInMenu(b, new(x + 8, y + 8), 1f);

			if (HasVariants)
				b.Draw(Catalog.MenuTexture, new Rectangle(x + CELL_SIZE - 22, y + 1, 21, 21), star, Color.White);

			if (Favorited)
				b.Draw(Catalog.MenuTexture, new Rectangle(x + 5, y + 5, 18, 18), favRibbon, Color.White);
		}

		public Furniture GetOne()
		{
			var item = Item.getOne() as Furniture;
			item.Price = 0;
			item.currentRotation.Value = 0;
			item.updateRotation();
			return item;
		}

		public bool CanPlaceHere()
		{
			if (Item is BedFurniture bed)
			{
				var location = Game1.currentLocation;

				if (!bed.CanModifyBed(Game1.player))
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_CantMoveOthersBeds"));
					return false;
				}

				if (location is FarmHouse house)
				{
					if (house.upgradeLevel < (int)bed.bedType)
					{
						Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_NeedsUpgrade"));
						return false;
					}
				}
			}

			return true;
		}

		public bool ToggleFavorite(bool playSound)
		{
			Favorited = !Favorited;

			if (playSound)
				Game1.playSound(Favorited ? "jingle1" : "cancel");

			return Favorited;
		}

		public override string ToString()
		{
			// 1.6: replace with ID
			return Item?.Name ?? string.Empty;
		}

		public string GetName()
		{
			return Item?.DisplayName;
		}
	}
}
