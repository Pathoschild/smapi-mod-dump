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

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class SearchBox : MenuSubscriber<ModEntry> {

	public readonly Action<string?> onSearch;

	public readonly BetterCraftingPage Menu;

	public ClickableTextureComponent btnSearch;

	public TextBox txtInput;
	public ClickableComponent btnInput;

	public bool UsingKB = false;

	public SearchBox(ModEntry mod, BetterCraftingPage menu, int x, int y, int width, int height, Action<string?> onSearch, string? old = null)
	: base(mod) {

		Menu = menu;

		initialize(x, y, width, height, true);

		this.onSearch = onSearch;

		txtInput = new TextBox(
			textBoxTexture: Game1.content.Load<Texture2D>(@"LooseSprites\textBox"),
			null,
			Game1.smallFont,
			Game1.textColor
		) {
			X = 0,
			Y = 0,
			Width = width - 64 - 16 - 16 - 16,
			Text = old ?? ""
		};

		txtInput.OnEnterPressed += sender => {
			sender.Selected = false;
			DoSearch();
		};
		txtInput.OnTabPressed += sender => {
			sender.Selected = false;
			currentlySnappedComponent = btnSearch;
			snapCursorToCurrentSnappedComponent();
		};

		btnInput = new ClickableComponent(
			bounds: new Rectangle(0, 0, txtInput.Width, txtInput.Height),
			name: ""
		) {
			myID = 1,
			upNeighborID = ClickableComponent.ID_ignore,
			leftNeighborID = ClickableComponent.ID_ignore,
			rightNeighborID = 2,
			downNeighborID = ClickableComponent.ID_ignore
		};

		btnSearch = new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			menu.ButtonTexture ?? Sprites.Buttons.Texture,
			Sprites.Buttons.SEARCH_ON,
			4f
		) {
			myID = 2,
			upNeighborID = ClickableComponent.ID_ignore,
			leftNeighborID = 1,
			rightNeighborID = ClickableComponent.ID_ignore,
			downNeighborID = ClickableComponent.ID_ignore
		};

		UpdateComponents();

		txtInput.Selected = true;

		if (Game1.options.SnappyMenus) {
			snapToDefaultClickableComponent();

			if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse) {
				Game1.showTextEntry(txtInput);
				UsingKB = true;
			}
		}
	}

	private void DoSearch() {
		string? text = txtInput.Text;
		if (string.IsNullOrEmpty(text) || text.Trim() == "")
			text = null;

		onSearch(text);
		exitThisMenu();
	}

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = btnInput;
		snapCursorToCurrentSnappedComponent();
	}

	public void UpdateComponents() {
		if (upperRightCloseButton != null) {
			upperRightCloseButton.bounds.X = xPositionOnScreen + width + 16;
			upperRightCloseButton.bounds.Y = yPositionOnScreen + (height - upperRightCloseButton.bounds.Height) / 2;
		}

		btnSearch.bounds.X = xPositionOnScreen + width - btnSearch.bounds.Width - 16;
		btnSearch.bounds.Y = yPositionOnScreen + (height - btnSearch.bounds.Height) / 2;

		txtInput.X = xPositionOnScreen + 16;
		txtInput.Y = yPositionOnScreen + (height - txtInput.Height) / 2;

		btnInput.bounds.X = txtInput.X;
		btnInput.bounds.Y = txtInput.Y;
	}

	#region Events and Input

	public override void receiveGamePadButton(Buttons b) {
		base.receiveGamePadButton(b);

		if (txtInput.Selected) {
			switch (b) {
				case Buttons.DPadUp:
				case Buttons.DPadDown:
				case Buttons.DPadLeft:
				case Buttons.DPadRight:
				case Buttons.LeftThumbstickUp:
				case Buttons.LeftThumbstickDown:
				case Buttons.LeftThumbstickLeft:
				case Buttons.LeftThumbstickRight:
				case Buttons.B:
					txtInput.Selected = false;
					break;
			}
		}
	}

	public override void receiveKeyPress(Keys key) {
		if (key == Keys.Escape && Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose()) {
			exitThisMenu();
			return;
		}

		if (txtInput.Selected)
			return;

		base.receiveKeyPress(key);
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		base.receiveLeftClick(x, y, playSound);

		if (btnSearch.containsPoint(x, y)) {
			DoSearch();
			return;
		}

		txtInput.Update();

		if (x < xPositionOnScreen || x > (xPositionOnScreen + width) || y < yPositionOnScreen || y > (yPositionOnScreen + height))
			exitThisMenu();
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		btnSearch.tryHover(x, y);
		txtInput.Hover(x, y);
	}

	#endregion

	#region Drawing

	public override void draw(SpriteBatch b) {
		if (UsingKB && Game1.textEntry == null) {
			exitThisMenu();
			return;
		}

		// Dim the Background
		b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		// Tip
		bool kb_open = Game1.textEntry != null;

		if (!kb_open) {
			var layout = SimpleHelper.Builder()
				.FormatText(
					I18n.Tooltip_Search_Tip(FlowHelper.EscapeFormatText(I18n.Search_IngredientPrefix()), FlowHelper.EscapeFormatText(I18n.Search_LikePrefix()), FlowHelper.EscapeFormatText(I18n.Search_LovePrefix())),
					wrapText: true,
					minWidth: width
				)
				.GetLayout();

			Menu.DrawSimpleNodeHover(
				layout,
				b,
				overrideX: xPositionOnScreen,
				overrideY: yPositionOnScreen + height + 16
			);
		}

		// Background
		RenderHelper.DrawBox(
			b,
			texture: Menu.Background ?? Game1.menuTexture,
			sourceRect: Menu.Background is null
				? RenderHelper.Sprites.NativeDialogue.ThinBox // new Rectangle(0, 256, 60, 60),
				: RenderHelper.Sprites.CustomBCraft.ThinBox,
			x: xPositionOnScreen,
			y: yPositionOnScreen,
			width: width,
			height: height,
			color: Color.White,
			scale: Menu.Background is null ? 1f : 4f
		);

		txtInput.Draw(b);
		btnSearch.draw(b);

		base.draw(b);

		// Mouse
		Game1.mouseCursorTransparency = 1f;
		drawMouse(b);
	}

	#endregion

}
