/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.SimpleLayout;
using Leclair.Stardew.Common.UI.Overlay;


namespace Leclair.Stardew.Almanac.Overlays;

internal class InventoryOverlay : BaseOverlay<InventoryPage> {

	protected ClickableTextureComponent btnAlmanac;

	protected bool Hovering = false;
	protected ISimpleNode? Tip;

	#region Lifecycle

	internal InventoryOverlay(InventoryPage menu)
	: base(menu, ModEntry.Instance, true) {
		Texture2D? tex;
		try {
			tex = ModEntry.Instance.ThemeManager.Load<Texture2D>("Menu.png");
		} catch(Exception ex) {
			ModEntry.Instance.Log("Unable to load texture", LogLevel.Warn, ex);
			tex = null;
		}

		btnAlmanac = new(
			new Rectangle(
				Menu.xPositionOnScreen - 64 - 8,
				Menu.yPositionOnScreen + 128,
				64,
				64
			),
			tex,
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

	public override void Dispose(bool disposing) {
		Menu.allClickableComponents?.Remove(btnAlmanac);
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
		if (Menu.allClickableComponents == null)
			return;

		if (! Menu.allClickableComponents.Contains(btnAlmanac))
			Menu.allClickableComponents.Add(btnAlmanac);
	}

	public virtual void MoveUIElements() {

		var pos = ModEntry.Instance.Config.AlmanacButtonPos;

		// When set to Left, we set the position manually.

		if (pos == ButtonPosition.TopLeft || pos == ButtonPosition.BottomLeft) {

			btnAlmanac.bounds.X = Menu.xPositionOnScreen - 64 - 32;
			if (pos == ButtonPosition.TopLeft)
				btnAlmanac.bounds.Y = Menu.inventory.yPositionOnScreen;
			else
				btnAlmanac.bounds.Y = Menu.yPositionOnScreen + Menu.height - 64 - IClickableMenu.borderWidth;

			btnAlmanac.leftNeighborID = ClickableComponent.ID_ignore;
			btnAlmanac.upNeighborID = ClickableComponent.SNAP_AUTOMATIC;
			btnAlmanac.rightNeighborID = ClickableComponent.SNAP_AUTOMATIC;
			btnAlmanac.downNeighborID = ClickableComponent.SNAP_AUTOMATIC;

			foreach (var cmp in Menu.inventory.GetBorder(InventoryMenu.BorderSide.Left))
				if (cmp.leftNeighborID == ClickableComponent.ID_ignore)
					cmp.leftNeighborID = ClickableComponent.SNAP_AUTOMATIC;

			return;
		}

		// Relative to another component~

		ClickableComponent other;
		GUIHelper.Side side;

		switch(ModEntry.Instance.Config.AlmanacButtonPos) {
			case ButtonPosition.OrganizeRight:
				side = GUIHelper.Side.Right;
				other = Menu.organizeButton;
				break;
			case ButtonPosition.TrashRight:
				side = GUIHelper.Side.Right;
				other = Menu.trashCan;
				break;
			case ButtonPosition.TrashDown:
				side = GUIHelper.Side.Down;
				other = Menu.trashCan;
				break;
			default:
				return;
		}

		GUIHelper.LinkComponents(
			side,
			id => Menu.getComponentWithID(id),
			other, btnAlmanac
		);

		GUIHelper.MoveComponents(
			side, -1,
			other, btnAlmanac
		);
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

		if (Menu.allClickableComponents == null) {
			// Something's wrong. Abort!
			Dispose();
			return;
		}

		AddButtonsToMenu();
	}

	protected override void ReceiveGameWindowResized(xTile.Dimensions.Rectangle NewViewport) {
		base.ReceiveGameWindowResized(NewViewport);

		MoveUIElements();
	}

	bool ClickButton(int x, int y, bool right = false) {
		if (btnAlmanac.containsPoint(x, y)) {
			Game1.playSound("bigSelect");

			if (Menu.readyToClose()) {
#if DEBUG
				if (right) {
					ModEntry.Instance.OpenGMCM();
					return true;
				}
#endif
				Game1.activeClickableMenu = new Menus.AlmanacMenu(ModEntry.Instance, Game1.Date.Year);
			}

			return true;
		}

		return false;
	}

	protected override bool ReceiveRightClick(int x, int y) {
		if (!MenuActive)
			return false;

		if (ClickButton(x, y, true))
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
