/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.Common;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class TemporaryCraftingPage : CraftingPage {

	private readonly ModEntry Mod;
	private bool HasStartedLoad = false;
	private bool HasRenderedOnce = false;

	public TemporaryCraftingPage(ModEntry mod, CraftingPage oldPage)
		: base(
			oldPage.xPositionOnScreen,
			oldPage.yPositionOnScreen,
			oldPage.width,
			oldPage.height,
			oldPage.cooking,
			false,
			oldPage._materialContainers is null ? null : new(oldPage._materialContainers)
		) {
		Mod = mod;
	}

	private void ReplaceSelf() {
		if (HasStartedLoad || !HasRenderedOnce)
			return;

		HasStartedLoad = true;

		GameMenu gm;
		if (GetParentMenu() is GameMenu parent)
			gm = parent;
		else if (Game1.activeClickableMenu is GameMenu active)
			gm = active;
		else {
			exitThisMenu();
			return;
		}

		int idx = gm.pages.IndexOf(this);
		if (idx == -1) {
			exitThisMenu();
			return;
		}

		// Make a copy of the existing chests.
		List<object>? containers = _materialContainers is null ? null : new(_materialContainers);

		// Close our own menu.
		CommonHelper.YeetMenu(this);

		gm.pages[idx] = BetterCraftingPage.Open(
			mod: Mod,
			location: Game1.player.currentLocation,
			position: null,
			width: gm.width,
			height: gm.height,
			cooking: false,
			standalone_menu: false,
			material_containers: containers,
			x: gm.xPositionOnScreen,
			y: gm.yPositionOnScreen
		);

		gm.AddTabsToClickableComponents(gm.pages[idx]);
		if (Game1.options.SnappyMenus)
			gm.snapToDefaultClickableComponent();
	}

	public override void RepositionElements() {

	}

	public override void snapToDefaultClickableComponent() {

	}

	protected override List<string> GetRecipesToDisplay() {
		return ["Torch", "Fried Egg"];
	}

	public override void receiveKeyPress(Keys key) {

		if (key != 0) {
			if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose()) {
				exitThisMenu();
				return;

			} else if (Game1.options.snappyMenus && Game1.options.gamepadControls && !overrideSnappyMenuCursorMovementBan())
				applyMovementKey(key);
		}

		ReplaceSelf();
	}

	public override void receiveScrollWheelAction(int direction) {

	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		if (upperRightCloseButton != null && readyToClose() && upperRightCloseButton.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound(closeSound);
			exitThisMenu();
			return;
		}

		ReplaceSelf();
	}

	public override void receiveRightClick(int x, int y, bool playSound = true) {
		ReplaceSelf();
	}

	public override void performHoverAction(int x, int y) {

	}

	public override void draw(SpriteBatch b) {

		string msg = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3689");

		SpriteText.drawStringHorizontallyCenteredAt(
			b,
			msg,
			xPositionOnScreen + (width / 2),
			yPositionOnScreen + ((height - SpriteText.getHeightOfString(msg)) / 2)
		);

		ReplaceSelf();
		HasRenderedOnce = true;
	}

}
