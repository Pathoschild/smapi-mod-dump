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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace HappyHomeDesigner.Menus
{
	internal abstract class VariantPage<T, TE> : ScreenPage 
		where TE: Item
		where T : VariantEntry<TE>
	{
		private readonly string KeyFavs;

		protected readonly List<T> entries = new();
		protected IReadOnlyList<VariantEntry<TE>> variants = Array.Empty<VariantEntry<TE>>();
		protected readonly List<T> Favorites = new();
		private bool showVariants = false;
		private int variantIndex = -1;
		private T variantItem;
		protected TE hovered;

		protected int iconRow;
		protected readonly GridPanel MainPanel = new(CELL_SIZE, CELL_SIZE, true);
		protected readonly GridPanel VariantPanel = new(CELL_SIZE, CELL_SIZE, false);
		protected readonly ClickableTextureComponent TrashSlot = new(new(0, 0, 48, 48), Catalog.MenuTexture, new(32, 48, 16, 16), 3f, true);

		private static readonly Rectangle FrameSource = new(0, 256, 60, 60);
		internal static HashSet<string> knownIDs = new();

		private static string[] preservedFavorites;

		public VariantPage(IEnumerable<ISalable> existing, string FavoritesKey, string typeName)
		{
			KeyFavs = FavoritesKey;

			var favorites = new HashSet<string>(
				Game1.player.modData.TryGetValue(KeyFavs, out var s) ?
				s.Split('	', StringSplitOptions.RemoveEmptyEntries) :
				Array.Empty<string>()
			);

			knownIDs.Clear();
			int skipped = 0;

			Init();

			var timer = Stopwatch.StartNew();

			foreach (var item in GetItemsFrom(existing, favorites))
			{
				if (knownIDs.Add(item.ToString()))
					entries.Add(item);
				else
					skipped++;
			}

			timer.Stop();
			ModEntry.monitor.Log($"Populated {entries.Count} {typeName} items in {timer.ElapsedMilliseconds} ms", LogLevel.Debug);
			if (skipped is not 0)
				ModEntry.monitor.Log($"Found and skipped {skipped} duplicate {typeName} items", LogLevel.Debug);

			MainPanel.DisplayChanged += UpdateDisplay;

			MainPanel.Items = entries;
			VariantPanel.Items = variants;

			preservedFavorites = favorites.ToArray();
		}

		public abstract void Init();

		public abstract IEnumerable<T> GetItemsFrom(IEnumerable<ISalable> source, ICollection<string> favorites);

		public override int Count() 
			=> entries.Count;

		public void UpdateDisplay()
		{
			variantIndex = MainPanel.FilteredItems.Find(variantItem);

			if (variantIndex is -1)
				variantItem = null;

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

			int bottom_margin = ModEntry.config.LargeVariants ? 0 : 240;
			const int top_margin = 256;

			MainPanel.Resize(width - 36, height - 64, xPositionOnScreen + 55, yPositionOnScreen);
			VariantPanel.Resize(
				CELL_SIZE * 3 + 32, 
				height - (bottom_margin + top_margin), 
				Game1.uiViewport.Width - CELL_SIZE * 3 - 80, 
				yPositionOnScreen + top_margin
			);
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
				(MainPanel.FilteredItems[index] as T).Item :
				null;
		}

		public abstract IReadOnlyList<IGridItem> ApplyFilter();

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);

			if (!MainPanel.isWithinBounds(x, y) && TrySelectFilter(x, y, playSound))
			{
				HideVariants();
				MainPanel.Items = ApplyFilter();
				return;
			}

			HandleGridClick(x, y, playSound, MainPanel, true);
			if (showVariants)
				HandleGridClick(x, y, playSound, VariantPanel, false);

			if (TrashSlot.containsPoint(x, y) && Game1.player.ActiveObject.CanDelete(knownIDs))
			{
				if (Game1.player.ActiveObject == Game1.player.TemporaryItem)
					Game1.player.TemporaryItem = null;
				else
					Game1.player.removeItemFromInventory(Game1.player.ActiveObject);

				if (playSound)
					Game1.playSound("trashcan");
			}
		}

		private void ShowVariantsFor(T entry, int index)
		{
			variantIndex = index;
			variantItem = entry;
			variants = entry.GetVariants();
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
				var entry = panel.FilteredItems[index] as T;

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

				if (!entry.CanPlace())
					return;

				if (Game1.player.ActiveObject.CanDelete(knownIDs))
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

		public override void Exit()
		{
			Game1.player.modData[KeyFavs] = string.Join('	', Favorites) + '	' + string.Join('	', preservedFavorites);
		}
	}
}
