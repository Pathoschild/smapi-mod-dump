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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.SimpleLayout;
using Leclair.Stardew.Common.UI.Overlay;


namespace Leclair.Stardew.Almanac.Overlays {
	internal class InventoryOverlay : BaseOverlay<InventoryPage> {

		protected ClickableTextureComponent btnAlmanac;

		protected bool Hovering = false;
		protected ISimpleNode Tip;

		#region Lifecycle

		internal InventoryOverlay(InventoryPage menu, Farmer who)
		: base(menu, ModEntry.instance, true) {
			btnAlmanac = new(
				new Rectangle(
					Menu.xPositionOnScreen - 64 - 8,
					Menu.yPositionOnScreen + 128,
					64,
					64
				),
				ModEntry.instance.Helper.Content.Load<Texture2D>("assets/Menu.png"),
				new Rectangle(240, 352, 16, 16),
				4f
			) {
				myID = 11022000,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			AddButtonsToMenu();
			MoveUIElements();
		}

		public override void Dispose() {
			base.Dispose();

			Menu.allClickableComponents.Remove(btnAlmanac);
		}

		public bool MenuActive {
			get {
				if (Game1.activeClickableMenu == Menu)
					return true;

				if (Game1.activeClickableMenu is GameMenu gm) {
					if (gm.GetCurrentPage() == Menu)
						return true;
				}

				return false;
			}
		}

		#endregion

		#region UI Updates

		public virtual void AddButtonsToMenu() {
			if (! Menu.allClickableComponents.Contains(btnAlmanac))
				Menu.allClickableComponents.Add(btnAlmanac);
		}

		public virtual void MoveUIElements() {

			GUIHelper.LinkComponents(GUIHelper.Side.Right, id => Menu.getComponentWithID(id), Menu.organizeButton, btnAlmanac);
			GUIHelper.MoveComponents(GUIHelper.Side.Right, -1, Menu.organizeButton, btnAlmanac);
		}

		#endregion

		#region Events

		protected override void PreDrawUI(SpriteBatch batch) {
			base.PreDrawUI(batch);
		}

		protected override void DrawUI(SpriteBatch batch) {
			base.DrawUI(batch);

			if (!MenuActive)
				return;

			btnAlmanac.draw(batch);
			Menu.drawMouse(batch);

			if (Tip != null)
				SimpleHelper.DrawHover(Tip, batch, Game1.smallFont);
		}

		protected override void HandleUpdateTicked() {
			base.HandleUpdateTicked();

			if (!MenuActive)
				return;

			AddButtonsToMenu();
		}

		protected override void ReceiveGameWindowResized(Rectangle NewViewport) {
			base.ReceiveGameWindowResized(NewViewport);

			MoveUIElements();
		}

		bool ClickButton(int x, int y) {
			if (btnAlmanac.containsPoint(x, y)) {
				Game1.playSound("bigSelect");

				if (Menu.readyToClose())
					Game1.activeClickableMenu = new Menus.AlmanacMenu(Game1.Date.Year);

				return true;
			}

			return false;
		}

		protected override bool ReceiveRightClick(int x, int y) {
			if (!MenuActive)
				return false;

			if (ClickButton(x, y))
				return true;

			return base.ReceiveRightClick(x, y);
		}

		protected override bool ReceiveLeftClick(int x, int y) {
			if (!MenuActive)
				return false;

			if (ClickButton(x, y))
				return true;

			return base.ReceiveLeftClick(x, y);
		}

		protected override bool ReceiveCursorHover(int x, int y) {
			if (!MenuActive)
				return false;

			btnAlmanac.tryHover(x, y, 0.25f);

			Hovering = btnAlmanac.containsPoint(x, y);
			if (Hovering && Tip == null)
				Tip = SimpleHelper.Builder().Text(I18n.Almanac_Open()).GetLayout();
			else if (!Hovering && Tip != null)
				Tip = null;

			return Hovering;
		}

		#endregion

	}
}
