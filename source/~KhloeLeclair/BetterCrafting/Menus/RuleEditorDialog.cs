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

using Leclair.Stardew.Common.Events;

using Leclair.Stardew.BetterCrafting.DynamicRules;
using StardewValley;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Leclair.Stardew.Common.UI.SimpleLayout;
using System.Diagnostics.CodeAnalysis;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common;
using Leclair.Stardew.BetterCrafting.Models;
using StardewValley.Menus;
using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class RuleEditorDialog : MenuSubscriber<ModEntry> {

	public delegate void FinishedDelegate(bool save, bool delete, DynamicRuleData data);

	public readonly BetterCraftingPage Menu;
	public readonly IDynamicRuleHandler Handler;

	public readonly FinishedDelegate OnFinished;

	private DynamicRuleData Data;
	private object? Obj;

	private ISimpleNode Layout;
	private Vector2 LayoutSize;

	public ClickableTextureComponent? btnSave;
	public ClickableTextureComponent? btnDelete;

	public TextBox? txtText;
	public ClickableComponent? btnText;

	public RuleEditorDialog(ModEntry mod, BetterCraftingPage menu, IDynamicRuleHandler handler, object? obj, DynamicRuleData data, FinishedDelegate onFinished) : base(mod) {
		Menu = menu;
		Handler = handler;
		Obj = obj;
		Data = data;
		OnFinished = onFinished;

		if (Handler is ISimpleInputRuleHandler) {
			string text = "";
			if (Data.Fields.TryGetValue("Input", out var token) && token.Type == Newtonsoft.Json.Linq.JTokenType.String)
				text = (string?) token ?? "";

			txtText = new TextBox(
				textBoxTexture: Game1.content.Load<Texture2D>(@"LooseSprites\textBox"),
				null,
				Game1.smallFont,
				Game1.textColor
			) {
				X = 0,
				Y = 0,
				Width = 400,
				Text = text
			};

			txtText.OnTabPressed += sender => {
				sender.Selected = false;
				currentlySnappedComponent = btnSave;
				snapCursorToCurrentSnappedComponent();
			};

			btnText = new ClickableComponent(
				bounds: new Rectangle(0, 0, txtText.Width, txtText.Height),
				name: ""
			) {
				myID = 3,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};
		}

		UpdateLayout();

		int width = (int) LayoutSize.X + 32;
		int height = (int) LayoutSize.Y + 32 +
			32; // Half Button

		Vector2 point = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

		initialize((int) point.X, (int) point.Y, width, height, true);

		btnSave = new ClickableTextureComponent(
			bounds: new Rectangle(0, 0, 64, 64),
			texture: Game1.mouseCursors,
			sourceRect: new Rectangle(128, 256, 64, 64),
			scale: 1f
		) {
			myID = 1,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = 2
		};

		btnDelete = new ClickableTextureComponent(
			bounds: new Rectangle(0, 0, 64, 64),
			texture: Game1.mouseCursors,
			sourceRect: new Rectangle(192, 256, 64, 64),
			scale: 1f
		) {
			myID = 2,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = 1,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
		};

		UpdateComponents();

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();
	}

	#region Controls

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = btnText ?? btnSave ?? btnDelete;
		base.snapToDefaultClickableComponent();
	}

	public void UpdateComponents() {
		width = (int) LayoutSize.X + 32;
		height = (int) LayoutSize.Y + 32 +
			32; // Half Button

		Vector2 point = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

		xPositionOnScreen = (int) point.X;
		yPositionOnScreen = (int) point.Y;

		if (upperRightCloseButton != null) {
			upperRightCloseButton.bounds.X = xPositionOnScreen + width - 32;
			upperRightCloseButton.bounds.Y = yPositionOnScreen - 16;
		}

		if (txtText is not null && btnText is not null) {
			txtText.X = btnText.bounds.X;
			txtText.Y = btnText.bounds.Y;
		}

		var first = btnSave ?? btnDelete;
		if (first is null)
			return;

		int buttons = 0;

		if (btnSave is not null)
			buttons++;
		if (btnDelete is not null)
			buttons++;

		int bWidth = (buttons * 64) + ((buttons - 1) * 16);

		first.bounds.X = xPositionOnScreen + (width - bWidth) / 2;
		first.bounds.Y = yPositionOnScreen + height - 32;

		if (buttons > 1)
			GUIHelper.MoveComponents(GUIHelper.Side.Right, 16, btnSave, btnDelete);
	}

	[MemberNotNull(nameof(Layout))]
	public void UpdateLayout() {

		float scale = 48f / Handler.Source.Height;
		if (scale >= 3)
			scale = 3f;
		else if (scale >= 1)
			scale = MathF.Round(scale);

		var builder = SimpleHelper.Builder(minSize: new Vector2(4 * 80, 0))
			.Group(margin: 8)
				.Space()
				.Texture(Handler.Texture, Handler.Source, scale: scale, align: Alignment.VCenter)
				.Space(expand: false)
				.Text(Handler.DisplayName, font: Game1.dialogueFont, align: Alignment.VCenter)
				.Space()
			.EndGroup();

		builder.Divider();

		if (btnText is not null && Handler is ISimpleInputRuleHandler sir) {
			if (!string.IsNullOrEmpty(sir.HelpText))
				builder
					.FormatText(sir.HelpText, wrapText: true);

			builder.Component(btnText, onDraw: OnComponentDraw);
		}

		Layout = builder.GetLayout();
		LayoutSize = Layout.GetSize(Game1.smallFont, new Vector2(400, 0));
	}

	#endregion

	#region Logic

	private void OnComponentDraw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
		if (txtText is not null && btnText is not null) {
			txtText.X = btnText.bounds.X;
			txtText.Y = btnText.bounds.Y;
		}
	}

	public void Close() {
		OnFinished(false, false, Data);
		exitThisMenu(false);
	}

	public void Save() {
		if (txtText is not null)
			Data.Fields["Input"] = new JValue(txtText.Text);

		OnFinished(true, false, Data);
		exitThisMenu(false);
	}

	public void Delete() {
		OnFinished(false, true, Data);
		exitThisMenu(false);
	}

	#endregion

	#region Events and Input

	public override void receiveKeyPress(Keys key) {
		if (txtText is not null && txtText.Selected)
			return;

		base.receiveKeyPress(key);
	}

	public override void receiveScrollWheelAction(int direction) {
		base.receiveScrollWheelAction(direction);
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {

		txtText?.Update();

		if (btnSave is not null && btnSave.containsPoint(x, y)) {
			Save();
			if (playSound)
				Game1.playSound("bigSelect");
			return;
		}

		if (btnDelete is not null && btnDelete.containsPoint(x, y)) {
			Delete();
			if (playSound)
				Game1.playSound("trashcan");
			return;
		}

		base.receiveLeftClick(x, y, playSound);
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		txtText?.Hover(x, y);
		btnSave?.tryHover(x, y);
		btnDelete?.tryHover(x, y);
	}

	#endregion

	#region Drawing

	public override void draw(SpriteBatch b) {

		// Dim the Background
		b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		// Background
		RenderHelper.DrawBox(
			b,
			texture: Game1.menuTexture,
			sourceRect: new Rectangle(0, 256, 60, 60),
			x: xPositionOnScreen,
			y: yPositionOnScreen,
			width: width,
			height: height,
			color: Color.White,
			scale: 1f
		);

		Layout?.Draw(
			b,
			new Vector2(xPositionOnScreen + 16, yPositionOnScreen + 16),
			LayoutSize,
			new Vector2(width, height),
			1f,
			Game1.smallFont,
			Game1.textColor,
			null
		);

		// Text
		txtText?.Draw(b);

		// Buttons
		btnSave?.draw(b);
		btnDelete?.draw(b);

		// Base Menu Stuff
		base.draw(b);

		// Mouse
		Game1.mouseCursorTransparency = 1f;
		drawMouse(b);
	}

	#endregion

}
