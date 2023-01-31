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
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class IconPicker : MenuSubscriber<ModEntry> {

	public static readonly int[] OBJECT_SPRITES = new int[] {
		0,  // Weeds
		73, // Golden Walnut
		79, // Lost Note
	};

	public readonly Action<CategoryIcon> onPick;

	public List<ClickableComponent> FlowComponents;

	public ClickableTextureComponent btnPageUp;
	public ClickableTextureComponent btnPageDown;

	private readonly ScrollableFlow Flow;

	public IconPicker(ModEntry mod, int x, int y, int width, int height, Action<CategoryIcon> onPick)
	: base(mod) {

		this.onPick = onPick;

		initialize(x, y, width, height);

		Flow = new(
			this,
			(int) Math.Ceiling(xPositionOnScreen + 16.0),
			(int) Math.Ceiling(yPositionOnScreen + 16.0),
			(int) Math.Ceiling(width - 16.0),
			(int) Math.Ceiling(height - 32.0)
		);

		btnPageUp = Flow.btnPageUp;
		btnPageDown = Flow.btnPageDown;
		FlowComponents = Flow.DynamicComponents;

		var builder = FlowHelper.Builder();

		foreach(int id in OBJECT_SPRITES) { 
			Rectangle rect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, id, 16, 16);
			SpriteInfo sprite = new(Game1.objectSpriteSheet, rect);

			builder.Sprite(sprite, scale: 3, onClick: (_, _, _) => {
				Pick(GameTexture.Object, rect);
				return true;
			});
		}

		for(int i = 0; i < 17; i++) {
			Rectangle rect = new(10 * i, 428, 10, 10);
			SpriteInfo sprite = new(Game1.mouseCursors, rect);

			builder.Sprite(sprite, scale: 3, onClick: (_,_,_) => {
				Pick(GameTexture.MouseCursors, rect);
				return true;
			});
		}

		for(int iy = 0; iy < 5; iy++) {
			for(int ix = 0; ix < 6; ix++) {
				Rectangle rect = new(ix * 16, 624 + iy * 16, 16, 16);
				SpriteInfo sprite = new(Game1.mouseCursors, rect);

				builder.Sprite(sprite, scale: 3, onClick: (_, _, _) => {
					Pick(GameTexture.MouseCursors, rect);
					return true;
				});
			}
		}

		Texture2D? emoji = SpriteHelper.GetTexture(GameTexture.Emoji);

		if (emoji is not null)
			for (int iy = 0; iy < 14; iy++) {
				for (int ix = 0; ix < 14; ix++) {
					Rectangle rect = new(ix * 9, iy * 9, 9, 9);
					SpriteInfo sprite = new(emoji, rect);

					builder.Sprite(sprite, scale: 3, onClick: (_, _, _) => {
						Pick(GameTexture.Emoji, rect);
						return true;
					});
				}
			}

		Flow.Set(builder.Build());

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();
	}

	private void Pick(GameTexture texture, Rectangle source) {
		onPick(new() {
			Type = CategoryIcon.IconType.Texture,
			Source = texture,
			Rect = source
		});

		exitThisMenu();
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

		if (Flow.PerformMiddleScroll(x, y))
			return;

		Flow.PerformHover(x, y);
	}

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

		Flow.Draw(b);

		// Base Menu
		base.draw(b);

		Flow.DrawMiddleScroll(b);

		// Mouse
		Game1.mouseCursorTransparency = 1f;
		drawMouse(b);
	}

}
