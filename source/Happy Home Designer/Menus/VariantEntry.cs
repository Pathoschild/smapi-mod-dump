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
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace HappyHomeDesigner.Menus
{
	internal abstract class VariantEntry<T> : IGridItem where T : Item
	{
		internal const int CELL_SIZE = 80;

		// 384 396 15 15 cursors
		// 256 256 10 10 cursors
		// 128 128 64 64 menus

		private static readonly Rectangle background = new(128, 128, 64, 64);
		private static readonly Rectangle star = new(6, 38, 7, 7);
		private static readonly Rectangle favRibbon = new(0, 38, 6, 6);

		public bool Favorited;
		public bool HasVariants;
		public readonly T Item;

		protected readonly Season season = default;

		/// <summary>Standard constructor. Used for main catalog page.</summary>
		/// <param name="Item">The contained item.</param>
		/// <param name="season">The local season. Required to accurately check for AT variants.</param>
		/// <param name="seasonName">The name of the local season. Required to accurately check for AT variants.</param>
		/// <param name="favorites">A list of favorites, to determine if this item is favorited</param>
		public VariantEntry(T Item, Season season, string seasonName, ICollection<string> favorites, string prefix)
		{
			this.Item = Item;
			this.season = season;
			HasVariants =
				AlternativeTextures.Installed &&
				AlternativeTextures.HasVariant(prefix + Item.ItemId, prefix + Item.Name, seasonName);
			Favorited = favorites.Remove(ToString());
		}

		/// <summary>Used for AT variant entries.</summary>
		/// <param name="Item">The contained item, with AT tags applied.</param>
		public VariantEntry(T Item)
		{
			this.Item = Item;
			HasVariants = false;
		}

		public abstract IReadOnlyList<VariantEntry<T>> GetVariants();

		public void Draw(SpriteBatch b, int x, int y)
		{
			IClickableMenu.drawTextureBox(b, Game1.menuTexture, background, x, y, CELL_SIZE, CELL_SIZE, Color.White, 1f, false);
			Item.drawInMenu(b, new(x + 8, y + 8), 1f);

			if (HasVariants)
				b.Draw(Catalog.MenuTexture, new Rectangle(x + CELL_SIZE - 22, y + 1, 21, 21), star, Color.White);

			if (Favorited)
				b.Draw(Catalog.MenuTexture, new Rectangle(x + 5, y + 5, 18, 18), favRibbon, Color.White);
		}

		public virtual string GetName()
		{
			return Item.DisplayName + '|' + Item.ItemId;
		}

		public override string ToString()
		{
			return Item.QualifiedItemId;
		}

		public virtual bool ToggleFavorite(bool playSound)
		{
			Favorited = !Favorited;

			if (playSound)
				Game1.playSound(Favorited ? "jingle1" : "cancel");

			return Favorited;
		}

		public abstract T GetOne();
		public abstract bool CanPlace();
	}
}
