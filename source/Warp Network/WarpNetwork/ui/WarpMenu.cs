/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using WarpNetwork.api;
using WarpNetwork.models;

namespace WarpNetwork.ui
{
	class WarpMenu : IClickableMenu
	{
		const int buttonH = 72;

		private readonly Rectangle panel = new(384, 373, 18, 18);
		private readonly List<IWarpNetAPI.IDestinationHandler> locs;
		private readonly string title;
		private readonly int titleW;
		private static readonly Color shadow = Color.Black * 0.33f;

		private List<WarpButton> buttons = new();
		private ClickableTextureComponent upArrow;
		private ClickableTextureComponent downArrow;
		private int index = 0;
		private bool autoAlign = false;
		private Rectangle mainPanel = new(0, 27, 0, 0);
		private Farmer who;
		internal bool hovering = false;

		internal static Texture2D defaultIcon;

		public WarpMenu(List<IWarpNetAPI.IDestinationHandler> locs, int x = 0, int y = 0, int width = 0, int height = 0)
	  : base(x, y, width, height, true)
		{
			if (locs.Count < 1)
			{
				ModEntry.monitor.Log("Warp menu created with no destinations!", LogLevel.Warn);
				exitThisMenuNoSound();
			}

			autoAlign = x == 0 && y == 0;
			this.width = width != 0 ? width : 600;
			this.height = height != 0 ? height : 380;
			this.locs = locs;
			who = Game1.player;
			title = ModEntry.helper.Translation.Get("ui-label");
			titleW = (int)Game1.dialogueFont.MeasureString(title).X + 33 + 36;
			upArrow = new(new Rectangle(0, 0, 33, 36), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 3f);
			downArrow = new(new Rectangle(0, 0, 33, 36), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 3f);
			defaultIcon = ModEntry.helper.GameContent.Load<Texture2D>(ModEntry.AssetPath + "/Icons/DEFAULT");

			if (autoAlign)
				align();

			resized();

			if (Game1.options.SnappyMenus)
				snapToDefaultClickableComponent();
		}
		public override void snapToDefaultClickableComponent()
		{
			if (buttons.Count < 1)
			{
				exitThisMenu();
				return;
			}
			currentlySnappedComponent = buttons[0];
			snapCursorToCurrentSnappedComponent();
		}
		private void setIndex(int what, bool playSound = true)
		{
			int l_index = index;
			index = Math.Clamp(what, 0, Math.Max(0, locs.Count - buttons.Count));
			if (index == l_index)
				return;
			if (playSound)
				Game1.playSound("shwip");
			for (int i = 0; i < buttons.Count; i++)
				buttons[i].location = locs[i + index];
		}
		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			if (autoAlign)
			{
				align();
				resized();
			}
		}
		public override void receiveScrollWheelAction(int direction)
		{
			base.receiveScrollWheelAction(direction);
			if (direction > 0)
				setIndex(index - 1);
			else if (direction < 0)
				setIndex(index + 1);
		}
		public override void applyMovementKey(int direction)
		{
			if (currentlySnappedComponent == null)
				snapToDefaultClickableComponent();
			switch (direction)
			{
				case 0:
					if (currentlySnappedComponent is WarpButton w && w.index == 0 && index > 0)
						setIndex(index - 1);
					else if (currentlySnappedComponent is not null)
						setCurrentlySnappedComponentTo(currentlySnappedComponent.upNeighborID);
					break;
				case 2:
					if (currentlySnappedComponent is WarpButton w2 && w2.index == buttons.Count - 1 && index < locs.Count - 1)
						setIndex(index + 1);
					else if (currentlySnappedComponent != null)
						setCurrentlySnappedComponentTo(currentlySnappedComponent.downNeighborID);
					break;
			}
		}
		public override void setCurrentlySnappedComponentTo(int id)
		{
			ModEntry.monitor.Log(id.ToString());
			if (id >= 0 && id < buttons.Count)
				currentlySnappedComponent = buttons[id];
			else
				snapToDefaultClickableComponent();
			snapCursorToCurrentSnappedComponent();
		}
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (mainPanel.Contains(x - xPositionOnScreen, y - yPositionOnScreen))
			{
				foreach (WarpButton button in buttons)
				{
					if (button.containsPoint(x, y))
					{
						if (button.location != null)
						{
							ModEntry.monitor.Log("Destination selected! Closing menu and warping...");
							button.location.Activate(who.currentLocation, who);
							exitThisMenuNoSound();
						}
						else
						{
							ModEntry.monitor.Log("Something went wrong! A warp menu button had no location bound!", LogLevel.Warn);
						}
					}
				}
			}
			else if (upArrow.containsPoint(x, y))
			{
				setIndex(index - 1);
			}
			else if (downArrow.containsPoint(x, y))
			{
				setIndex(index + 1);
			}
			base.receiveLeftClick(x, y, playSound);
		}
		public void align()
		{
			var port = Game1.uiViewport.Size;
			xPositionOnScreen = port.Width / 2 - width / 2;
			yPositionOnScreen = port.Height / 2 - height / 2;
		}
		public void resized()
		{
			mainPanel.Width = width;
			mainPanel.Height = (height - mainPanel.Y - 27) / buttonH * buttonH + 27;
			upperRightCloseButton.bounds.Location = new(xPositionOnScreen + mainPanel.X + mainPanel.Width - 32, yPositionOnScreen + mainPanel.Y - 16);
			upArrow.bounds.Location = new(xPositionOnScreen + mainPanel.X + mainPanel.Width + 6, yPositionOnScreen + mainPanel.Y + 33);
			downArrow.bounds.Location = new(xPositionOnScreen + mainPanel.X + mainPanel.Width + 6, yPositionOnScreen + mainPanel.Y + mainPanel.Height - 48);
			for (int i = 0; i * buttonH < mainPanel.Height - buttonH - 12; i += 1)
			{
				Rectangle bound = new(xPositionOnScreen + 12 + mainPanel.X, yPositionOnScreen + i * buttonH + 15 + mainPanel.Y, mainPanel.Width - 24, buttonH);
				if (buttons.Count <= i && locs.Count > i + index)
				{
					buttons.Add(new(bound, locs[i + index], i, who) { scale = 3f, myID = i });
				}
				else
				{
					if (locs.Count <= i + index)
					{
						if (buttons.Count > i)
						{
							buttons.RemoveAt(i);
						}
					}
					else
					{
						buttons[i].bounds = bound;
						buttons[i].myID = i;
						buttons[i].updateLabel();
					}
				}
			}
			ClickableComponent.ChainNeighborsUpDown(buttons);
		}
		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);
			upArrow.tryHover(x, y, .2f);
			downArrow.tryHover(x, y, .2f);
		}
		public override void draw(SpriteBatch b)
		{
			if (!Game1.options.showMenuBackground)
				b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
			else
				drawBackground(b);
			drawTextureBox(b, Game1.mouseCursors, panel,
				xPositionOnScreen + mainPanel.X,
				yPositionOnScreen + mainPanel.Y,
				mainPanel.Width,
				mainPanel.Height,
				Color.White, 3f);
			foreach (WarpButton button in buttons)
				button.draw(b);
			drawTitleBox(b, title);
			upArrow.draw(b);
			downArrow.draw(b);
			base.draw(b);
			drawMouse(b, false);
		}
		private void drawTitleBox(SpriteBatch b, string text)
		{
			int offset = (width - titleW) / 2;
			//shadows
			b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset - 6, yPositionOnScreen - 4, 36, 54), new Rectangle(325, 318, 12, 18), shadow);
			b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset + 36 - 6, yPositionOnScreen - 4, titleW - 36 - 36, 54), new Rectangle(337, 318, 1, 18), shadow);
			b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + width - offset - 42, yPositionOnScreen - 4, 36, 54), new Rectangle(338, 318, 12, 18), shadow);
			//scroll
			b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset, yPositionOnScreen - 10, 36, 54), new Rectangle(325, 318, 12, 18), Color.White);
			b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset + 36, yPositionOnScreen - 10, titleW - 36 - 36, 54), new Rectangle(337, 318, 1, 18), Color.White);
			b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + width - offset - 36, yPositionOnScreen - 10, 36, 54), new Rectangle(338, 318, 12, 18), Color.White);
			//text
			Utility.drawTextWithShadow(b, title, Game1.dialogueFont, new Vector2(xPositionOnScreen + offset + 36, yPositionOnScreen - 8), Game1.textColor);
		}
	}
}
