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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class RulePickerDialog : MenuSubscriber<ModEntry> {

	public readonly Action<DynamicRuleData?, bool> OnPick;

	public readonly BetterCraftingPage Menu;

	public List<ClickableComponent> FlowComponents;

	public ClickableTextureComponent btnPageUp;
	public ClickableTextureComponent btnPageDown;

	private readonly ScrollableFlow Flow;

	private string? HoverText;

	public RulePickerDialog(ModEntry mod, BetterCraftingPage menu, int x, int y, int width, int height, HashSet<string> existing, Action<DynamicRuleData?, bool> onPick) : base(mod) {
		OnPick = onPick;
		Menu = menu;

		initialize(x, y, width, height);

		Flow = new(
			this,
			(int) Math.Ceiling(xPositionOnScreen + 16.0),
			(int) Math.Ceiling(yPositionOnScreen + 16.0),
			(int) Math.Ceiling(width - 16.0),
			(int) Math.Ceiling(height - 32.0)
		);

		btnPageDown = Flow.btnPageDown;
		btnPageUp = Flow.btnPageUp;
		FlowComponents = Flow.DynamicComponents;

		if (menu.Theme.CustomScroll && menu.Background is not null)
			Sprites.CustomScroll.ApplyToScrollableFlow(Flow, menu.Background);

		var builder = FlowHelper.Builder();

		var entries = Mod.Recipes.GetRuleHandlers().ToList();
		entries.Sort((a, b) => {
			return a.Value.DisplayName.CompareTo(b.Value.DisplayName);
		});

		foreach (var entry in entries) {
			string id = entry.Key;
			var handler = entry.Value;

			if (!handler.AllowMultiple && existing.Contains(id))
				continue;

			float scale = 48f / handler.Source.Height;
			if (scale >= 3)
				scale = 3f;
			else if (scale >= 1)
				scale = MathF.Round(scale);

			var b2 = FlowHelper.Builder()
				.Texture(
					texture: handler.Texture,
					source: handler.Source,
					scale: scale,
					align: Alignment.VCenter
				)
				.Text(" ")
				.FormatText(handler.DisplayName, align: Alignment.VCenter);

			var node = new SelectableNode(
				b2.Build(),
				onHover: (_, _, _) => {
					HoverText = handler.Description;
					return true;
				},
				onClick: (_, _, _) => {
					Pick(id, handler.HasEditor);
					return false;
				}
			) {
				SelectedTexture = Sprites.Buttons.Texture,
				SelectedSource = Sprites.Buttons.SELECT_BG,
				HoverTexture = Sprites.Buttons.Texture,
				HoverSource = Sprites.Buttons.SELECT_BG,
				HoverColor = Color.White * 0.4f
			};

			builder.Add(node);
		}

		Flow.Set(builder.Build());

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();
	}

	private void Pick(string? id, bool openEditor) {
		exitThisMenu();

		OnPick(string.IsNullOrEmpty(id) ? null : new DynamicRuleData {
			Id = id
		}, openEditor);
	}

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = FlowComponents.Count > 0 ? FlowComponents[0] : null;
		if (currentlySnappedComponent != null)
			snapCursorToCurrentSnappedComponent();
	}

	public override void receiveScrollWheelAction(int direction) {
		base.receiveScrollWheelAction(direction);

		if (Flow.Scroll(direction > 0 ? -1 : 1))
			Game1.playSound("shwip");
	}

	public override void releaseLeftClick(int x, int y) {
		base.releaseLeftClick(x, y);
		Flow.ReleaseLeftClick();
	}

	public override void leftClickHeld(int x, int y) {
		base.leftClickHeld(x, y);
		Flow.LeftClickHeld(x, y);
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		base.receiveLeftClick(x, y, playSound);

		if (Flow.ReceiveLeftClick(x, y, playSound))
			return;

		if (x < xPositionOnScreen || x > (xPositionOnScreen + width) || y < yPositionOnScreen || y > (yPositionOnScreen + height))
			exitThisMenu();
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		HoverText = null;

		if (Flow.PerformMiddleScroll(x, y))
			return;

		Flow.PerformHover(x, y);
	}

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
			scale: texture is null
				? 1f
				: 4f
		);

		Flow.Draw(
			b,
			texture is null ? null : (Menu.Theme.TooltipTextColor ?? Menu.Theme.TextColor),
			texture is null ? null : (Menu.Theme.TooltipTextShadowColor ?? Menu.Theme.TextShadowColor)
		);

		// Base Menu
		base.draw(b);

		Flow.DrawMiddleScroll(b);

		// Mouse
		Game1.mouseCursorTransparency = 1f;
		if (!Menu.Theme.CustomMouse || !RenderHelper.DrawMouse(b, Menu.Background, RenderHelper.Sprites.BCraftMouse))
			drawMouse(b);

		// Hover Text
		if (!string.IsNullOrEmpty(HoverText)) {
			var layout = SimpleHelper.Builder(minSize: new Vector2(400, 0))
				.FormatText(HoverText, wrapText: true)
				.GetLayout();

			Menu.DrawSimpleNodeHover(layout, b);
		}
	}

}
