/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HappyHomeDesigner.Menus
{
	internal class WallEntry : IGridItem
	{
		public bool Favorited;

		private readonly Wallpaper item;
		private Texture2D sheet;
		private readonly Rectangle region;
		private readonly int CellHeight;
		private readonly int CellWidth;
		private readonly string id;
		private readonly float Scale;

		private static readonly Rectangle background = new(128, 128, 64, 64);
		private static readonly Rectangle favRibbon = new(0, 38, 6, 6);

		public WallEntry(Wallpaper wallPaper, IList<string> favorites)
		{
			item = wallPaper;

			var modData = item.GetSetData();

			id = modData is not null ?
				modData.Id + ':' + item.ParentSheetIndex.ToString() :
				item.ParentSheetIndex.ToString();

			Favorited = favorites.Contains(id);

			if (item.isFloor.Value)
			{
				region = new(item.ParentSheetIndex % 8 * 32, 336 + item.ParentSheetIndex / 8 * 32, 32, 32);
				CellHeight = 72;
				CellWidth = 72;
				Scale = 2f;

				if (modData is not null)
					region.Y -= 336;

			} else
			{
				region = new(item.ParentSheetIndex % 16 * 16, item.ParentSheetIndex / 16 * 48, 16, 44);
				CellHeight = 140;
				CellWidth = 56;
				Scale = 3f;
			}
		}

		public void Draw(SpriteBatch batch, int x, int y)
		{
			// defer texture load to prevent Lag Spike Of Doom
			sheet ??= GetTexture();

			//IClickableMenu.drawTextureBox(batch, Game1.menuTexture, background, x, y, 56, CellHeight, Color.White, 1f, false);
			batch.Draw(sheet, new Vector2(x + 4, y + 4), region, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
			batch.DrawFrame(Game1.menuTexture, new(x, y, CellWidth, CellHeight), background, 4, 1, Color.White);

			if (Favorited)
				batch.Draw(Catalog.MenuTexture, new Rectangle(x + 4, y + 4, 18, 18), favRibbon, Color.White);
		}

		public bool TryApply(bool playSound, out WallFloorState state)
		{
			state = default;

			if (Game1.currentLocation is not DecoratableLocation where)
				return false;

			(var x, var y) = Game1.player.TilePoint;

			if (item.isFloor.Value)
			{
				var id = where.GetFloorID(x, y);
				if (id is null)
					return false;

				var existing = where.appliedFloor.TryGetValue(id, out var xid) ? xid : "0";

				var modData = item.GetSetData();
				var name = modData is null ?
					item.ParentSheetIndex.ToString() :
					$"{modData.Id}:{item.ParentSheetIndex}";

				where.SetFloor(name, id);

				state = new() { area = id, isFloor = true, old = existing, which = name};
			} else
			{
				string id = where.GetWallpaperID(x, y);
				while (id is null)
				{
					y--;
					if (y is < 0)
						return false;
					id = where.GetWallpaperID(x, y);
				}

				var existing = where.appliedWallpaper.TryGetValue(id, out var xid) ? xid : "0";

				var modData = item.GetSetData();
				var name = modData is null ?
					item.ParentSheetIndex.ToString() :
					$"{modData.Id}:{item.ParentSheetIndex}";

				where.SetWallpaper(name, id);

				state = new() { area = id, isFloor = false, old = existing, which = name };
			}

			if (playSound)
				Game1.playSound("stoneStep");

			return true;
		}
		public Wallpaper GetOne()
		{
			return item.getOne() as Wallpaper;
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
			return id;
		}

		public string GetName()
		{
			return id;
		}

		private Texture2D GetTexture()
		{
			var modData = item.GetSetData();
			if (modData is not null)
			{
				try
				{
					return ModEntry.helper.GameContent.Load<Texture2D>(modData.Texture);
				}
				catch (Exception)
				{
					return ModEntry.helper.GameContent.Load<Texture2D>("Maps/walls_and_floors");
				}
			}
			return ModEntry.helper.GameContent.Load<Texture2D>("Maps/walls_and_floors");
		}
	}
}
