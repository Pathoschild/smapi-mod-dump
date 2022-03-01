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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.SimpleLayout;
using Leclair.Stardew.Common.UI.Overlay;

namespace Leclair.Stardew.MoveToConnected.Overlay {
	internal abstract class BaseAddOverlay<T> : BaseOverlay<T> where T : IClickableMenu {

		protected ClickableTextureComponent btnExtract;
		protected ClickableTextureComponent btnInsert;

		protected Item HoverItem = null;
		protected bool HoverModified = false;
		protected TransferDirection HoverDir = TransferDirection.None;
		protected ISimpleNode Tip;

		private List<ItemGrabMenu.TransferredItemSprite> tSprites = new();

		protected List<object> CachedInventories;
		protected bool Working = false;

		#region Lifecycle

		internal BaseAddOverlay(T menu, Farmer who) : base(menu, ModEntry.instance, assumeUI: true) {

			btnInsert = new(new Rectangle(0, 0, 64, 64), ModEntry.instance.Buttons, new Rectangle(0, 0, 16, 16), 4f) {
				myID = 118001,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnExtract = new(new Rectangle(0, 0, 64, 64), ModEntry.instance.Buttons, new Rectangle(0, 32, 16, 16), 4f) {
				myID = 118002,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};


			DiscoverInventories();

			AddButtonsToMenu();
			MoveUIElements();
		}

		public override void Dispose() {
			base.Dispose();

			Menu.allClickableComponents.Remove(btnExtract);
			Menu.allClickableComponents.Remove(btnInsert);
		}

		public virtual void AddButtonsToMenu() {
			if (!Menu.allClickableComponents.Contains(btnExtract))
				Menu.allClickableComponents.Add(btnExtract);

			if (!Menu.allClickableComponents.Contains(btnInsert))
				Menu.allClickableComponents.Add(btnInsert);
		}

		#endregion

		#region Inventory Stuff

		protected void DiscoverInventories() {

		}

		#endregion

		#region Events

		protected void UpdateTooltip() {
			bool holding = HoverItem != null;

			if (HoverDir == TransferDirection.None) {
				Tip = null;
				return;
			}

			var builder = SimpleHelper.Builder();
			TransferBehavior behavior;

			if (HoverDir == TransferDirection.Insert) {
				builder
					.Text(holding ? I18n.Tooltip_Insert_Held() : I18n.Tooltip_Insert())
					.Divider();

			} else {
				builder
					.Text(holding ? I18n.Tooltip_Extract_Held() : I18n.Tooltip_Extract())
					.Divider();
			}

		}

		protected override void PreDrawUI(SpriteBatch batch) {
			base.PreDrawUI(batch);

			btnInsert.draw(batch);
			btnExtract.draw(batch);
		}

		protected override void DrawUI(SpriteBatch batch) {
			base.DrawUI(batch);

			foreach (var sprite in tSprites)
				sprite.Draw(batch);

			// TODO: pSprites

			if (Tip != null)
				SimpleHelper.DrawHover(Tip, batch, Game1.smallFont);
		}

		protected override void HandleUpdateTicked() {
			base.HandleUpdateTicked();
			AddButtonsToMenu();

			GameTime time = Game1.currentGameTime;

			for (int i = 0; i < tSprites.Count; i++) {
				if (tSprites[i].Update(time)) {
					tSprites.RemoveAt(i);
					i--;
				}
			}

			// TODO: Update pSprites.
		}

		protected override void ReceiveGameWindowResized(Rectangle NewViewport) {
			base.ReceiveGameWindowResized(NewViewport);
			MoveUIElements();
		}

		protected override void ReceiveButtonsChanged(object sender, ButtonsChangedEventArgs e) {
			base.ReceiveButtonsChanged(sender, e);

		}

		protected override bool ReceiveLeftClick(int x, int y) {
			return ReceiveClick(x, y, false);
		}

		protected override bool ReceiveRightClick(int x, int y) {
			return ReceiveClick(x, y, true);
		}

		private bool ReceiveClick(int x, int y, bool isAction) {
			return false;
		}

		protected override bool ReceiveCursorHover(int x, int y) {
			TransferDirection dir = TransferDirection.None;

			btnInsert.tryHover(x, y, 0.25f);
			btnExtract.tryHover(x, y, 0.25f);

			if (btnInsert.containsPoint(x, y))
				dir = TransferDirection.Insert;
			else if (btnExtract.containsPoint(y, x))
				dir = TransferDirection.Extract;

			Item item = GetHeldItem();
			if (dir != HoverDir || item != HoverItem) {
				HoverDir = dir;
				HoverItem = item;
				UpdateTooltip();
			}

			return dir != TransferDirection.None;
		}



		#endregion

		#region API

		protected abstract Vector2? GetTilePosition();

		protected abstract Rectangle? GetMultiTileRegion();

		protected abstract InventoryMenu GetInventoryMenu();

		protected abstract Item GetHeldItem();

		protected abstract bool SetHeldItem(Item item);

		protected virtual Item GetItemAt(int x, int y) {
			InventoryMenu menu = GetInventoryMenu();
			return menu?.getItemAt(x, y);
		}

		protected virtual Rectangle? GetItemBoundsAt(int x, int y) {
			InventoryMenu menu = GetInventoryMenu();
			int idx = menu?.getInventoryPositionOfClick(x, y) ?? -1;
			if (idx == -1)
				return null;

			return menu.inventory[idx]?.bounds;
		}

		protected virtual bool SetItemAt(Item item, int x, int y) {
			InventoryMenu menu = GetInventoryMenu();
			int idx = menu?.getInventoryPositionOfClick(x, y) ?? -1;
			if (idx == -1)
				return false;

			menu.actualInventory[idx] = item;
			return true;
		}

		protected abstract void MoveUIElements();

		#endregion

	}
}
