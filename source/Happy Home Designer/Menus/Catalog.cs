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
using HappyHomeDesigner.Patches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace HappyHomeDesigner.Menus
{
	public class Catalog : IClickableMenu
	{
		public static readonly PerScreen<Catalog> ActiveMenu = new();
		internal static Texture2D MenuTexture;

		public enum AvailableCatalogs
		{
			Furniture = 1,
			Wallpaper = 2,
			All = 3,
		}

		public static bool TryShowCatalog(AvailableCatalogs catalogs)
		{
			MenuTexture = ModEntry.helper.GameContent.Load<Texture2D>(ModEntry.uiPath);

			// catalog is open
			if (ActiveMenu.Value is Catalog catalog)
				// the same or more permissive
				if ((catalog.Catalogs | catalogs) == catalog.Catalogs)
					return false;
				else
					catalog.exitThisMenuNoSound();

			var menu = new Catalog(catalogs);
			Game1.onScreenMenus.Insert(0, menu);
			ActiveMenu.Value = menu;
			Game1.isTimePaused = ModEntry.config.PauseTime;
			return true;
		}

		public readonly AvailableCatalogs Catalogs;

		private List<ScreenPage> Pages = new();
		private int tab = 0;
		private List<ClickableTextureComponent> Tabs = new();
		private ClickableTextureComponent CloseButton;
		private readonly ClickableTextureComponent SettingsButton;
		private readonly ClickableTextureComponent ToggleButton;
		private bool Toggled = true;

		public Catalog(AvailableCatalogs catalogs)
		{
			Catalogs = catalogs;
			if ((catalogs & AvailableCatalogs.Furniture) is not 0)
				Pages.Add(new FurniturePage());
			if ((catalogs & AvailableCatalogs.Wallpaper) is not 0)
				Pages.Add(new WallFloorPage());

			if (Pages.Count is not 1)
				for (int i = 0; i < Pages.Count; i++)
					Tabs.Add(Pages[i].GetTab());

			CloseButton = new(new(0, 0, 48, 48), Game1.mouseCursors, new(337, 494, 12, 12), 3f, false);
			ToggleButton = new(new(0, 0, 48, 48), Game1.mouseCursors, new(352, 494, 12, 12), 3f, false);

			if (IGMCM.Installed)
				SettingsButton = new(new(0, 0, 48, 48), Game1.objectSpriteSheet, new(256, 64, 16, 16), 3f, true);

			var vp = Game1.uiViewport;
			Resize(new(vp.X, vp.Y, vp.Width, vp.Height));
			AltTex.forcePreviewDraw = true;
			AltTex.forceMenuDraw = true;

			Game1.playSound("bigSelect");
		}

		protected override void cleanupBeforeExit()
		{
			base.cleanupBeforeExit();
			AltTex.forcePreviewDraw = false;
			AltTex.forceMenuDraw = false;
			Game1.onScreenMenus.Remove(this);
			Game1.player.TemporaryItem = null;
			ActiveMenu.Value = null;
			Game1.isTimePaused = false;
			for (int i = 0;i < Pages.Count; i++)
				Pages[i].Exit();

			if (Game1.keyboardDispatcher.Subscriber is SearchBox)
				Game1.keyboardDispatcher.Subscriber = null;
		}
		public override void performHoverAction(int x, int y)
		{
			ToggleButton.tryHover(x, y);

			if (!Toggled)
				return;

			Pages[tab].performHoverAction(x, y);
		}
		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			Resize(newBounds);
		}
		public override void draw(SpriteBatch b)
		{
			ToggleButton.draw(b);

			if (!Toggled)
				return;

			// tab shadow
			b.Draw(MenuTexture, 
				new Rectangle(xPositionOnScreen + 92, yPositionOnScreen + 20, 64, 64), 
				new Rectangle(64, 24, 16, 16), 
				Color.Black * .4f);

			Pages[tab].draw(b);
			CloseButton.draw(b);
			SettingsButton?.draw(b);

			for (int i = 0; i < Tabs.Count; i++)
				Tabs[i].draw(b, i == tab ? Color.White : Color.DarkGray, 0f);
		}
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);

			if (ToggleButton.containsPoint(x, y))
				Toggle(playSound);

			if (!Toggled)
				return;

			Pages[tab].receiveLeftClick(x, y, playSound);
			for (int i = 0; i < Tabs.Count; i++)
			{
				if (Tabs[i].containsPoint(x, y))
				{
					if (playSound && tab != i)
						Game1.playSound("shwip");
					tab = i;
					break;
				}
			}

			if (CloseButton.containsPoint(x, y))
				exitThisMenu();

			if (SettingsButton is not null && SettingsButton.containsPoint(x, y))
			{
				IGMCM.API.OpenModMenu(ModEntry.manifest);
				if (playSound)
					Game1.playSound("bigSelect");
			}
		}
		public override bool isWithinBounds(int x, int y)
		{
			if (ToggleButton.containsPoint(x, y))
				return true;

			if (!Toggled)
				return false;

			for (int i = 0; i < Tabs.Count; i++)
				if (Tabs[i].containsPoint(x, y))
					return true;

			return 
				Pages[tab].isWithinBounds(x, y) || 
				CloseButton.containsPoint(x, y) || 
				(SettingsButton is not null && SettingsButton.containsPoint(x, y));
		}
		private void Resize(Rectangle bounds)
		{
			Rectangle region = new(32, 96, 400, bounds.Height - 160);
			for (int i = 0; i < Pages.Count; i++)
				Pages[i].Resize(region);

			int tabX = xPositionOnScreen + 96;
			int tabY = yPositionOnScreen + 16;
			for (int i = 0; i < Tabs.Count; i++)
			{
				var tabComp = Tabs[i];
				tabComp.setPosition(tabX, tabY);
				tabX += tabComp.bounds.Width;
			}
			SettingsButton?.setPosition(tabX + 8, tabY + 8);

			CloseButton.bounds.Location = new(40, 52);
			ToggleButton.bounds.Location = new(16, bounds.Height - 64);
		}

		public override void receiveScrollWheelAction(int direction)
		{
			if (!Toggled)
				return;

			Pages[tab].receiveScrollWheelAction(direction);
		}

		public void Toggle(bool playSound)
		{
			ToggleButton.sourceRect.X = Toggled ? 365 : 352;
			Toggled = !Toggled;

			if (playSound)
				Game1.playSound("shwip");
		}
	}
}
