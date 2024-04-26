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
using Leclair.Stardew.Common.UI.FlowNode;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class RuleEditorDialog : MenuSubscriber<ModEntry> {

	public delegate void FinishedDelegate(bool save, bool delete, DynamicRuleData data);

	public readonly BetterCraftingPage Menu;
	public readonly IDynamicRuleHandler Handler;

	public readonly FinishedDelegate OnFinished;

	private readonly DynamicRuleData Data;

	private ISimpleNode Layout;
	private Vector2 LayoutSize;

	public ClickableTextureComponent? btnSave;
	public ClickableTextureComponent? btnDelete;

	public TextBox? txtText;
	public ClickableComponent? btnText;

	// Dropdown

	public List<ClickableComponent>? FlowComponents;
	public ClickableTextureComponent? btnPageUp;
	public ClickableTextureComponent? btnPageDown;

	public ScrollableFlow? optionPicker;
	public ClickableComponent? btnPicker;

	public string? PickedOption;

	public RuleEditorDialog(ModEntry mod, BetterCraftingPage menu, IDynamicRuleHandler handler, object? obj, DynamicRuleData data, FinishedDelegate onFinished) : base(mod) {
		Menu = menu;
		Handler = handler;
		Data = data;
		OnFinished = onFinished;

		if (Handler is ISimpleInputRuleHandler) {
			string text = "";
			if (Data.Fields.TryGetValue("Input", out var token) && token.Type == JTokenType.String)
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

		else if (Handler is IOptionInputRuleHandler opts) {
			var options = opts.GetOptions(Menu.Cooking);
			PickedOption = options.First().Key;
			if (Data.Fields.TryGetValue("Input", out var token) && token.Type == JTokenType.String)
				PickedOption = (string?) token;

			optionPicker = new(
				this,
				0, 0, Math.Max(300, Math.Min(1000, Game1.uiViewport.Width - 128)), Math.Max(300, Math.Min(600, Game1.uiViewport.Height - 256))
			);

			if (Menu.Theme.CustomScroll && Menu.Background is not null)
				Sprites.CustomScroll.ApplyToScrollableFlow(optionPicker, Menu.Background);

			btnPageDown = optionPicker.btnPageDown;
			btnPageUp = optionPicker.btnPageUp;
			FlowComponents = optionPicker.DynamicComponents;

			var builder = FlowHelper.Builder();

			Dictionary<string, SelectableNode> nodes = [];

			void OnSelect(string? value) {
				PickedOption = value;
				foreach(var entry in nodes) {
					entry.Value.Selected = value == entry.Key;
				}
			}

			foreach(var entry in options) {
				var b2 = FlowHelper.Builder()
					.FormatText(entry.Value, align: Alignment.VCenter)
					.Build();

				string id = entry.Key;

				var node = nodes[id] = new SelectableNode(
					b2,
					onHover: (_, _, _) => {
						return true;
					},
					onClick: (_, _, _) => {
						OnSelect(id);
						return true;
					}
				) {
					Selected = id == PickedOption,
					SelectedTexture = Sprites.Buttons.Texture,
					SelectedSource = Sprites.Buttons.SELECT_BG,
					HoverTexture = Sprites.Buttons.Texture,
					HoverSource = Sprites.Buttons.SELECT_BG,
					HoverColor = Color.White * 0.4f
				};

				builder.Add(node);
			}

			optionPicker.Set(builder.Build());

			btnPicker = new ClickableComponent(
				bounds: new Rectangle(0, 0, optionPicker.Width, optionPicker.Height),
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
			texture: Menu.Background ?? Game1.mouseCursors,
			sourceRect: Menu.Background is null
				? new Rectangle(128, 256, 64, 64)
				: Sprites.Other.BTN_OK,
			scale: Menu.Background is null ? 1f : 4f
		) {
			myID = 1,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = 2
		};

		btnDelete = new ClickableTextureComponent(
			bounds: new Rectangle(0, 0, 64, 64),
			texture: Menu.Background ?? Game1.mouseCursors,
			sourceRect: Menu.Background is null
				? new Rectangle(192, 256, 64, 64)
				: Sprites.Other.BTN_CANCEL,
			scale: Menu.Background is null ? 1f : 4f
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

		if (optionPicker is not null && btnPicker is not null) {
			optionPicker.X = btnPicker.bounds.X;
			optionPicker.Y = btnPicker.bounds.Y;
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

		if (btnPicker is not null && Handler is IOptionInputRuleHandler opt) {
			if (!string.IsNullOrEmpty(opt.HelpText))
				builder
					.FormatText(opt.HelpText, wrapText: true);

			builder.Component(btnPicker, onDraw: OnComponentDraw);
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
		if (optionPicker is not null && btnPicker is not null) {
			optionPicker.X = btnPicker.bounds.X;
			optionPicker.Y = btnPicker.bounds.Y;
		}
	}

	public void Close() {
		OnFinished(false, false, Data);
		exitThisMenu(false);
	}

	public void Save() {
		if (txtText is not null)
			Data.Fields["Input"] = new JValue(txtText.Text);

		else if (optionPicker is not null)
			Data.Fields["Input"] = new JValue(PickedOption);

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

		if (optionPicker is not null && optionPicker.Scroll(direction > 0 ? -1 : 1))
			Game1.playSound("shwip");
	}

	public override void leftClickHeld(int x, int y) {
		base.leftClickHeld(x, y);
		optionPicker?.LeftClickHeld(x, y);
	}

	public override void releaseLeftClick(int x, int y) {
		base.releaseLeftClick(x, y);
		optionPicker?.ReleaseLeftClick();
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {

		txtText?.Update();

		if (optionPicker is not null && optionPicker.ReceiveLeftClick(x, y, playSound))
			return;

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

		if (optionPicker is not null) {
			if (optionPicker.PerformMiddleScroll(x, y))
				return;

			optionPicker.PerformHover(x, y);
		}

		txtText?.Hover(x, y);
		btnSave?.tryHover(x, y);
		btnDelete?.tryHover(x, y);
	}

	#endregion

	#region Drawing

	public override void draw(SpriteBatch b) {

		// Dim the Background
		b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		Texture2D? texture = Menu.Theme.CustomTooltip ? Menu.Background : null;

		// Background
		RenderHelper.DrawBox(
			b,
			texture: texture ?? Game1.menuTexture,
			sourceRect: texture is null
				? RenderHelper.Sprites.NativeDialogue.ThinBox
				: RenderHelper.Sprites.CustomBCraft.ThinBox,
			x: xPositionOnScreen,
			y: yPositionOnScreen,
			width: width,
			height: height,
			color: Color.White,
			scale: texture is null ? 1f : 4f
		);

		Layout?.Draw(
			b,
			new Vector2(xPositionOnScreen + 16, yPositionOnScreen + 16),
			LayoutSize,
			new Vector2(width, height),
			1f,
			Game1.smallFont,
			(texture is null ? null : Menu.Theme.TooltipTextColor ?? Menu.Theme.TextColor) ?? Game1.textColor,
			(texture is null ? null : Menu.Theme.TooltipTextShadowColor ?? Menu.Theme.TextShadowColor)
		);

		// Text
		txtText?.Draw(b);

		// Buttons
		btnSave?.draw(b);
		btnDelete?.draw(b);

		// Options
		optionPicker?.Draw(
			b,
			texture is null ? null : (Menu.Theme.TooltipTextColor ?? Menu.Theme.TextColor),
			texture is null ? null : (Menu.Theme.TooltipTextShadowColor ?? Menu.Theme.TextShadowColor)
		);

		// Base Menu Stuff
		base.draw(b);

		optionPicker?.DrawMiddleScroll(b);

		// Mouse
		Game1.mouseCursorTransparency = 1f;
		if (!Menu.Theme.CustomMouse || !RenderHelper.DrawMouse(b, Menu.Background, RenderHelper.Sprites.BCraftMouse))
			drawMouse(b);
	}

	#endregion

}
