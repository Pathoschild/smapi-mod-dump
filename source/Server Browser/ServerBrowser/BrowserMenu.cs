/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerBrowser
{
	class BrowserMenu : IClickableMenu
	{
		uint slotIndex = 0;

		int SlotsPerPage => (height - 17 * 2) / BrowserSlot.height;
		IEnumerable<BrowserSlot> VisibleSlots => slots.Values.Skip((int)slotIndex).Take(SlotsPerPage);
		
		Dictionary<CSteamID, BrowserSlot> slots = new Dictionary<CSteamID, BrowserSlot>();

		ClickableTextureComponent forwardButton;
		ClickableTextureComponent backButton;

		ClickableTextureComponent searchButton;
		ClickableTextureComponent refreshButton;

		Checkbox showFullServersCheckbox;
		Checkbox showPasswordProtectedCheckbox;
		Checkbox showNoEmptyCabinsCheckbox;

		public BrowserMenu(int x, int y, int width, int height, List<CSteamID> servers, IClickableMenu fallBackMenu) : base(x, y, width, 17 * 2 + ((height - 17 * 2) / BrowserSlot.height - 1) * BrowserSlot.height, true)
		{
			if (Game1.viewport.Height - base.height <= 150)
				base.height -= BrowserSlot.height;

			exitFunction = delegate
			{
				Game1.activeClickableMenu = fallBackMenu;
			};

			int i = 0;
			foreach (var steamID in servers)
			{
				var slot = new BrowserSlot(xPositionOnScreen + 15, yPositionOnScreen + 17 + i * BrowserSlot.height, width - 30, 0, 0, "0",
										"Loading...", $"Server ID {steamID}");
				slots.Add(steamID, slot);
				
				i++;
			}

			showFullServersCheckbox = new Checkbox(xPositionOnScreen + 23, yPositionOnScreen + base.height + 27, "Show full", ModEntry.SearchOptions.ShowFullServers);
			showPasswordProtectedCheckbox = new Checkbox(xPositionOnScreen + 23, yPositionOnScreen + base.height + 27 + 40 + 12, "Show password protected", ModEntry.SearchOptions.ShowPasswordProtectedSerers);
			showNoEmptyCabinsCheckbox = new Checkbox(xPositionOnScreen + 260, yPositionOnScreen + base.height + 27, "Show full cabins", ModEntry.SearchOptions.ShowFullCabinServers);

			searchButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 185, yPositionOnScreen + base.height + 46, 14*4, 15*4), Game1.mouseCursors, new Rectangle(208, 321, 14, 15), 4f, true);
			refreshButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 107, yPositionOnScreen + base.height + 41, 18 * 4, 18 * 4), ModEntry.RefreshTexture, new Rectangle(0, 0, 18, 18), 4f, true);


			forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 28, yPositionOnScreen + base.height - 16, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);
			backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 24, yPositionOnScreen + base.height - 16, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false);
		}

		public BrowserSlot GetSlot(CSteamID steamID)
		{
			return slots[steamID];
		}

		public void RemoveSlot(CSteamID steamID)
		{
			slots.Remove(steamID);
		}

		public override void update(GameTime time)
		{
			var vs = VisibleSlots.ToArray();
			for (int i = 0; i < vs.Length; i++)
			{
				vs[i].Y = yPositionOnScreen + 17 + i * BrowserSlot.height;
			}

			base.update(time);
		}

		public override void draw(SpriteBatch spriteBatch)
		{
			drawBackground(spriteBatch);
			
			drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f, false);
			
			foreach (var slot in VisibleSlots.Reverse())//Reverse order fixes hover boxes clipping under other slots
			{
				slot.Draw(spriteBatch);
			}
			
			//Filters, search button etc
			drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen + height, width, Game1.viewport.Height - (yPositionOnScreen + height) - 15, Color.White, 4f, false);
			showFullServersCheckbox.Draw(spriteBatch);
			showPasswordProtectedCheckbox.Draw(spriteBatch);
			showNoEmptyCabinsCheckbox.Draw(spriteBatch);

			if (slotIndex + SlotsPerPage < slots.Count)
				forwardButton.draw(spriteBatch);

			if (slotIndex - SlotsPerPage >= 0)
				backButton.draw(spriteBatch);

			searchButton.draw(spriteBatch);
			refreshButton.draw(spriteBatch);

			if (searchButton.bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
				drawHoverText(Game1.spriteBatch, string.IsNullOrWhiteSpace(ModEntry.SearchOptions.SearchQuery) ? "Search" : $"Search query: \"{ModEntry.SearchOptions.SearchQuery}\"", Game1.smallFont, 0, 0);

			if (refreshButton.bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
				drawHoverText(Game1.spriteBatch, "Refresh", Game1.smallFont, 0, 0);

			base.draw(spriteBatch);
			drawMouse(spriteBatch);
		}

		public override void performHoverAction(int x, int y)
		{
			foreach (var slot in VisibleSlots)
			{
				if (slot.Bounds.Contains(x,y) && !upperRightCloseButton.containsPoint(x, y))
				{
					slot.TryHover(x, y);
					break;
				}
			}

			forwardButton.tryHover(x, y, 0.2f);
			backButton.tryHover(x, y, 0.2f);
			searchButton.tryHover(x, y, 0.2f);
			refreshButton.tryHover(x, y, 0.2f);

			base.performHoverAction(x, y);
		}

		public override void releaseLeftClick(int x, int y)
		{
			foreach (var slot in VisibleSlots)
			{
				if (slot.Bounds.Contains(x, y) && !upperRightCloseButton.containsPoint(x,y))
				{
					slot.Clicked(x, y);
					break;
				}
			}

			if (showFullServersCheckbox.Bounds.Contains(x,y))
				showFullServersCheckbox.Clicked(x, y);

			if (showPasswordProtectedCheckbox.Bounds.Contains(x, y))
				showPasswordProtectedCheckbox.Clicked(x, y);

			if (showNoEmptyCabinsCheckbox.Bounds.Contains(x, y))
				showNoEmptyCabinsCheckbox.Clicked(x, y);

			ModEntry.SearchOptions.ShowFullServers = showFullServersCheckbox.IsChecked;
			ModEntry.SearchOptions.ShowFullCabinServers = showNoEmptyCabinsCheckbox.IsChecked;
			ModEntry.SearchOptions.ShowPasswordProtectedSerers = showPasswordProtectedCheckbox.IsChecked;

			if (forwardButton.bounds.Contains(x, y) && slotIndex + SlotsPerPage < slots.Count)
				slotIndex += (uint)SlotsPerPage;

			if (backButton.bounds.Contains(x, y) && slotIndex - SlotsPerPage >= 0)
				slotIndex -= (uint)SlotsPerPage;

			if (refreshButton.bounds.Contains(x, y))
			{
				ModEntry.OpenServerBrowser();
			}

			if (searchButton.bounds.Contains(x, y))
			{
				Console.WriteLine("Showing search box");
				Game1.activeClickableMenu = new TextMenu("Please enter your search query", false, (searchInput) =>
				{
					ModEntry.SearchOptions.SearchQuery = searchInput;
					ModEntry.OpenServerBrowser();
				}, () => Game1.activeClickableMenu = this);
			}

			base.releaseLeftClick(x, y);
		}
	}
}
