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
using HappyHomeDesigner.Integration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HappyHomeDesigner.Menus
{
	internal class FurniturePage : ScreenPage
	{
		private const int FURNITURE_MAX = 18;
		private const string KeyFavs = "tlitookilakin.HappyHomeDesigner/FurnitureFavorites";
		private const int DEFAULT_EXTENDED = 2;
		private const int DEFAULT_DEFAULT = 4;

		private readonly List<FurnitureEntry> entries = new();
		private readonly List<FurnitureEntry> variants = new();
		private bool showVariants = false;
		private int variantIndex = -1;
		private Furniture hovered;

		private readonly int iconRow;
		private readonly GridPanel MainPanel = new(CELL_SIZE, CELL_SIZE, true);
		private readonly GridPanel VariantPanel = new(CELL_SIZE, CELL_SIZE, false);
		private readonly List<FurnitureEntry>[] Filters;
		private readonly List<FurnitureEntry> Favorites = new();
		private readonly ClickableTextureComponent TrashSlot = new(new(0, 0, 48, 48), Catalog.MenuTexture, new(32, 48, 16, 16), 3f, true);

		private static readonly Rectangle FrameSource = new(0, 256, 60, 60);
		private static readonly int[] ExtendedTabMap = {0, 0, 1, 1, 2, 3, 4, 5, 6, 2, 2, 3, 7, 8, 2, 9, 5, 8};
		private static readonly int[] DefaultTabMap = {1, 1, 1, 1, 0, 0, 2, 4, 4, 4, 4, 0, 3, 2, 4, 5, 4, 4};
		internal static HashSet<string> knownFurnitureIDs;

		public FurniturePage(ShopMenu existing = null)
		{
			int[] Map;
			int default_slot;

			if (ModEntry.config.ExtendedCategories)
			{
				Map = ExtendedTabMap;
				default_slot = DEFAULT_EXTENDED;
				iconRow = 0;
			}
			else
			{
				Map = DefaultTabMap;
				default_slot = DEFAULT_DEFAULT;
				iconRow = 8;
			}

			filter_count = Map.Max() + 1;
			Filters = new List<FurnitureEntry>[filter_count];
			for (int i = 0; i < Filters.Length; i++)
				Filters[i] = new();
			filter_count += 2;

			var favorites = Game1.player.modData.TryGetValue(KeyFavs, out var s) ? s.Split('	') : Array.Empty<string>();
			var season = Game1.player.currentLocation.GetSeason();
			var seasonName = season.ToString();

			bool populateIds = knownFurnitureIDs is null;
			if (populateIds)
				knownFurnitureIDs = new();

			var timer = Stopwatch.StartNew();

			foreach (var item in ModUtilities.GetCatalogItems(true, existing))
			{
				if (item is Furniture furn)
				{
					var entry = new FurnitureEntry(furn, season, seasonName, favorites);
					var type = furn.furniture_type.Value;

					entries.Add(entry);
					if (type is < FURNITURE_MAX and >= 0)
						Filters[Map[type]].Add(entry);
					else
						Filters[default_slot].Add(entry);

					if (entry.Favorited)
						Favorites.Add(entry);

					if (populateIds)
						knownFurnitureIDs.Add(furn.ItemId);
				}
			}

			timer.Stop();
			ModEntry.monitor.Log($"Populated {entries.Count} furniture items in {timer.ElapsedMilliseconds} ms", LogLevel.Debug);

			MainPanel.DisplayChanged += UpdateDisplay;

			MainPanel.Items = entries;
			VariantPanel.Items = variants;
		}
		public void UpdateDisplay()
		{
			if (variantIndex is not -1)
				variantIndex = MainPanel.FilteredItems.Find(MainPanel.LastFiltered[variantIndex]);
			showVariants = variantIndex is not -1;
		}
		public override void draw(SpriteBatch b)
		{
			MainPanel.DrawShadow(b);
			if (showVariants)
				VariantPanel.DrawShadow(b);

			base.draw(b);
			DrawFilters(b, iconRow, 1, xPositionOnScreen, yPositionOnScreen);
			MainPanel.draw(b);
			TrashSlot.draw(b);

			if (variantIndex is >= 0)
			{
				int cols = MainPanel.Columns;
				int variantDrawIndex = variantIndex - MainPanel.Offset;
				if (variantDrawIndex >= 0 && variantDrawIndex < MainPanel.VisibleCells)
				b.DrawFrame(Game1.menuTexture, new(
					xPositionOnScreen + variantDrawIndex % cols * CELL_SIZE - 8 + 55,
					yPositionOnScreen + variantDrawIndex / cols * CELL_SIZE - 8,
					CELL_SIZE + 16, CELL_SIZE + 16),
					FrameSource, 13, 1, Color.White, 0);
			}

			if (hovered is not null && ModEntry.config.FurnitureTooltips)
				drawToolTip(b, hovered.getDescription(), hovered.DisplayName, hovered);

			//AltTex.forceMenuDraw = true;
			if (showVariants)
				VariantPanel.draw(b);
			//AltTex.forceMenuDraw = false;
		}
		public override void Resize(Rectangle region)
		{
			base.Resize(region);

			MainPanel.Resize(width - 36, height - 64, xPositionOnScreen + 55, yPositionOnScreen);
			VariantPanel.Resize(CELL_SIZE * 3 + 32, height - 496, Game1.uiViewport.Width - CELL_SIZE * 3 - 64, yPositionOnScreen + 256);
			TrashSlot.setPosition(
				MainPanel.xPositionOnScreen + MainPanel.width - 48 + GridPanel.BORDER_WIDTH, 
				MainPanel.yPositionOnScreen + MainPanel.height + GridPanel.BORDER_WIDTH + GridPanel.MARGIN_BOTTOM
			);
		}
		public override void performHoverAction(int x, int y)
		{
			MainPanel.performHoverAction(x, y);
			VariantPanel.performHoverAction(x, y);

			hovered = MainPanel.TrySelect(x, y, out int index) ?
				(MainPanel.FilteredItems[index] as FurnitureEntry).Item : 
				null;
		}
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);

			if (!MainPanel.isWithinBounds(x, y) && TrySelectFilter(x, y, playSound))
			{
				HideVariants();
				MainPanel.Items = 
					// all items
					(current_filter is 0) ? entries :
					// categories
					(current_filter <= Filters.Length) ? Filters[current_filter - 1] :
					// favorites
					Favorites;
				return;
			}

			HandleGridClick(x, y, playSound, MainPanel, true);
			if (showVariants)
				HandleGridClick(x, y, playSound, VariantPanel, false);

			if (TrashSlot.containsPoint(x, y) && Game1.player.ActiveObject.CanDelete())
			{
				if (Game1.player.ActiveObject == Game1.player.TemporaryItem)
					Game1.player.TemporaryItem = null;
				else
					Game1.player.removeItemFromInventory(Game1.player.ActiveObject);

				if (playSound)
					Game1.playSound("trashcan");
			}
		}

		private void ShowVariantsFor(FurnitureEntry entry, int index)
		{
			variantIndex = index;
			var vars = entry.GetVariants();
			variants.Clear();
			for(int i = 0; i < vars.Count; i++)
				variants.Add(new(vars[i]));
			VariantPanel.Items = variants;
			showVariants = true;
		}

		private void HideVariants()
		{
			variantIndex = -1;
			showVariants = false;
		}

		public override void receiveScrollWheelAction(int direction)
		{
			var pos = Game1.getMousePosition(true);
			if (MainPanel.isWithinBounds(pos.X, pos.Y))
				MainPanel.receiveScrollWheelAction(direction);
			else if (VariantPanel.isWithinBounds(pos.X, pos.Y))
				VariantPanel.receiveScrollWheelAction(direction);
		}

		private void HandleGridClick(int mx, int my, bool playSound, GridPanel panel, bool allowVariants)
		{
			panel.receiveLeftClick(mx, my, playSound);

			if (panel.TrySelect(mx, my, out int index))
			{
				var entry = panel.FilteredItems[index] as FurnitureEntry;

				if (allowVariants)
				{
					if (ModEntry.config.FavoriteModifier.IsDown())
					{
						if (entry.ToggleFavorite(playSound))
							Favorites.Add(entry);
						else
							Favorites.Remove(entry);

						if (MainPanel.Items == Favorites)
							MainPanel.UpdateCount();

						return;
					}

					if (entry.HasVariants)
					{
						ShowVariantsFor(entry, index);
						if (playSound)
							Game1.playSound("shwip");

						return;
					}
					HideVariants();
				}

				if (ModEntry.config.GiveModifier.IsDown())
				{
					if (Game1.player.addItemToInventoryBool(entry.GetOne()) && playSound)
						Game1.playSound("pickUpItem");
					return;
				}

				if (!entry.CanPlaceHere())
					return;

				if (Game1.player.ActiveObject.CanDelete())
					if (Game1.player.ActiveObject != Game1.player.TemporaryItem)
						Game1.player.removeItemFromInventory(Game1.player.ActiveObject);

				Game1.player.TemporaryItem = entry.GetOne();
				if (playSound)
					Game1.playSound("stoneStep");
			}
		}
		public override bool isWithinBounds(int x, int y)
		{
			return 
				base.isWithinBounds(x, y) || 
				MainPanel.isWithinBounds(x, y) || 
				(showVariants && VariantPanel.isWithinBounds(x, y)) ||
				TrashSlot.containsPoint(x, y);
		}
		public override ClickableTextureComponent GetTab()
		{
			return new(new(0, 0, 64, 64), Catalog.MenuTexture, new(64, 24, 16, 16), 4f);
		}

		public override void Exit()
		{
			Game1.player.modData[KeyFavs] = string.Join('	', Favorites);
		}
	}
}
