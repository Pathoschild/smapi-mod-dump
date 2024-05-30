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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Leclair.Stardew.Common;

public enum MouseCursor {
	Auto = -1,
	Normal = 0,
	Busy = 1,
	Hand = 2,
	Gift = 3,
	Dialogue = 4,
	Search = 5,
	Plus = 6,
	Heart = 7,
	Pointer = 44
};

public record struct SourceSet(
	Rectangle Background,
	Rectangle TopLeft,
	Rectangle TopMiddle,
	Rectangle TopRight,
	Rectangle MiddleLeft,
	Rectangle MiddleRight,
	Rectangle BottomLeft,
	Rectangle BottomMiddle,
	Rectangle BottomRight,
	Rectangle VRuleTop,
	Rectangle VRuleMiddle,
	Rectangle VRuleBottom,
	Rectangle HRuleLeft,
	Rectangle HRuleMiddle,
	Rectangle HRuleRight,
	Rectangle ThinVRule,
	Rectangle ThinHRule,
	Rectangle ThinBox
);


public record struct MouseSources(
	Rectangle? Normal = null,
	Rectangle? Busy = null,
	Rectangle? Hand = null,
	Rectangle? Gift = null,
	Rectangle? Dialogue = null,
	Rectangle? Search = null,
	Rectangle? Plus = null,
	Rectangle? Heart = null,
	Rectangle? Pointer = null
);


public static class RenderHelper {

	public static class Sprites {

		public readonly static SourceSet NativeDialogue = new(
			Background: GetSourceByIndex(9),
			TopLeft: GetSourceByIndex(0),
			TopMiddle: GetSourceByIndex(2),
			TopRight: GetSourceByIndex(3),
			MiddleLeft: GetSourceByIndex(8),
			MiddleRight: GetSourceByIndex(11),
			BottomLeft: GetSourceByIndex(12),
			BottomMiddle: GetSourceByIndex(14),
			BottomRight: GetSourceByIndex(15),
			VRuleTop: GetSourceByIndex(1),
			VRuleMiddle: GetSourceByIndex(5),
			VRuleBottom: GetSourceByIndex(13),
			HRuleLeft: GetSourceByIndex(4),
			HRuleMiddle: GetSourceByIndex(6),
			HRuleRight: GetSourceByIndex(7),
			ThinVRule: GetSourceByIndex(26),
			ThinHRule: GetSourceByIndex(25),
			ThinBox: new Rectangle(0, 256, 60, 60)
		);

		public readonly static MouseSources NativeMouse = new(
			Normal: new(0, 0, 16, 16),
			Busy: new(16, 0, 16, 16),
			Hand: new(32, 0, 16, 16),
			Gift: new(48, 0, 16, 16),
			Dialogue: new(64, 0, 16, 16),
			Search: new(80, 0, 16, 16),
			Plus: new(96, 0, 16, 16),
			Heart: new(112, 0, 16, 16),
			Pointer: new(0, 16, 16, 16)
		);

		public readonly static SourceSet CustomBCraft = new(
			Background: GetSourceByIndex(9, 16),
			TopLeft: GetSourceByIndex(0, 16),
			TopMiddle: GetSourceByIndex(2, 16),
			TopRight: GetSourceByIndex(3, 16),
			MiddleLeft: GetSourceByIndex(8, 16),
			MiddleRight: GetSourceByIndex(11, 16),
			BottomLeft: GetSourceByIndex(12, 16),
			BottomMiddle: GetSourceByIndex(14, 16),
			BottomRight: GetSourceByIndex(15, 16),
			VRuleTop: GetSourceByIndex(1, 16),
			VRuleMiddle: GetSourceByIndex(5, 16),
			VRuleBottom: GetSourceByIndex(13, 16),
			HRuleLeft: GetSourceByIndex(4, 16),
			HRuleMiddle: GetSourceByIndex(6, 16),
			HRuleRight: GetSourceByIndex(7, 16),
			ThinVRule: GetSourceByIndex(18, 16),
			ThinHRule: GetSourceByIndex(17, 16),
			ThinBox: GetSourceByIndex(16, 16)
		);

		public readonly static MouseSources BCraftMouse = new(
			Normal: new(16, 96, 16, 16),
			Busy: new(32, 96, 16, 16),
			Pointer: new(48, 96, 16, 16)
		);

		public static Rectangle GetSourceByIndex(int idx, int tileWidth = 64, int? tileHeight = null, int? textureWidth = null) {

			tileHeight ??= tileWidth;
			textureWidth ??= tileWidth * 4;

			int tiles_wide = textureWidth.Value / tileWidth;

			int column = idx % tiles_wide;
			int row = idx / tiles_wide;

			return new Rectangle(
				column * tileWidth,
				row * tileHeight.Value,
				tileWidth,
				tileHeight.Value
			);
		}

	}

	public static Rectangle? GetSourceForMouse(this MouseSources sources, MouseCursor which = MouseCursor.Normal) {
		return which switch {
			MouseCursor.Auto => (Game1.options.SnappyMenus && Game1.options.gamepadControls) ? sources.Pointer : sources.Normal,
			MouseCursor.Normal => sources.Normal,
			MouseCursor.Busy => sources.Busy,
			MouseCursor.Hand => sources.Hand,
			MouseCursor.Gift => sources.Gift,
			MouseCursor.Dialogue => sources.Dialogue,
			MouseCursor.Search => sources.Search,
			MouseCursor.Plus => sources.Plus,
			MouseCursor.Heart => sources.Heart,
			MouseCursor.Pointer => sources.Pointer,
			_ => null
		};
	}


	private static IModHelper? Helper;

	[MemberNotNull(nameof(Helper))]
	public static void SetHelper(IModHelper helper) {
		Helper = helper;
	}

	public static Rectangle GetIntersection(this Rectangle self, Rectangle other) {
		return Rectangle.Intersect(self, other);
	}

	public static Rectangle ToXna(this xTile.Dimensions.Rectangle self) {
		return new Rectangle(self.X, self.Y, self.Width, self.Height);
	}

	public static Rectangle Clone(this Rectangle self) {
		return self;
	}

	#region SpriteText Nonsense

	[Obsolete("Just use SpriteText directly it uses normal Color? now")]
	public static void DrawCenteredSpriteText(
		SpriteBatch b,
		string text,
		int x, int y,
		int characterPosition = 999999,
		int width = -1,
		int height = 999999,
		float alpha = 1f,
		float layerDepth = 0.88f,
		bool junimoText = false,
		Color? color = null,
		int maxWidth = 99999
	) {
		SpriteText.drawStringHorizontallyCenteredAt(
			b,
			s: text,
			x: x,
			y: y,
			characterPosition: characterPosition,
			width: width,
			height: height,
			alpha: alpha,
			layerDepth: layerDepth,
			junimoText: junimoText,
			color: color,
			maxWidth: maxWidth
		);
	}

	[Obsolete("Just use SpriteText directly it uses normal Color? now")]
	public static void DrawSpriteText(
		SpriteBatch b,
		string text,
		int x, int y,
		int characterPosition = 999999,
		int width = -1,
		int height = 999999,
		float alpha = 1f,
		float layerDepth = 0.88f,
		bool junimoText = false,
		Color? color = null
	) {
		SpriteText.drawString(
			b: b,
			s: text,
			x: x,
			y: y,
			characterPosition: characterPosition,
			width: width,
			height: height,
			alpha: alpha,
			layerDepth: layerDepth,
			junimoText: junimoText,
			color: color
		);
	}

	#endregion

	#region Inventory Custom Slots

	public static void DrawCustomSlots(
		this InventoryMenu inventory,
		SpriteBatch b,
		Texture2D texture,
		Rectangle source,
		Rectangle? disabledSource = null,
		Color? color = null
	) {
		if (texture is null)
			return;
		if (source.IsEmpty)
			source = texture.Bounds;

		int columns = inventory.capacity / inventory.rows;

		float scale = 64f / source.Width;

		Color c = color ?? Color.White;

		for (int i = 0; i < inventory.capacity; i++) {
			int col = i % columns;
			int row = i / columns;

			Vector2 pos = new(
				inventory.xPositionOnScreen + col * (64 + inventory.horizontalGap),
				inventory.yPositionOnScreen + row * (64 + inventory.verticalGap) + (row - 1) * 4 - ((i < columns && inventory.playerInventory && inventory.verticalGap == 0) ? 12 : 0)
			);

			b.Draw(texture, pos, source, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);

			if ((inventory.playerInventory || inventory.showGrayedOutSlots) && i >= Game1.player.MaxItems && disabledSource.HasValue)
				b.Draw(texture, pos, disabledSource.Value, c * 0.5f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);

			if (!Game1.options.gamepadControls && i < 12 && inventory.playerInventory) {
				string label = i switch {
					11 => "=",
					10 => "-",
					9 => "0",
					_ => $"{i + 1}"
				};

				Vector2 labelSize = Game1.tinyFont.MeasureString(label);
				b.DrawString(Game1.tinyFont, label, pos + new Vector2(32f - labelSize.X / 2f, 0f - labelSize.Y), i == Game1.player.CurrentToolIndex ? Color.Red : Color.DimGray);


			}

		}

	}

	#endregion

	#region Mouse

	public static bool DrawMouse(
		SpriteBatch b,
		Texture2D? texture,
		MouseSources sources,
		MouseCursor cursor = MouseCursor.Auto,
		bool ignore_transparency = false
	) {
		Rectangle? source = sources.GetSourceForMouse(cursor);
		if (texture is null || !source.HasValue || Game1.options.hardwareCursor)
			return false;

		float transparency = ignore_transparency ? 1 : Game1.mouseCursorTransparency;

		b.Draw(
			texture: texture,
			position: new Vector2(Game1.getMouseX(), Game1.getMouseY()),
			sourceRectangle: source.Value,
			color: Color.White * transparency,
			rotation: 0f,
			origin: Vector2.Zero,
			scale: 4f + Game1.dialogueButtonScale / 150f,
			effects: SpriteEffects.None,
			layerDepth: 1f
		);

		return true;
	}

	#endregion

	#region Dialogue Box

	public static void DrawDialogueBox(
		SpriteBatch batch,
		int x, int y,
		int width, int height,
		Color? color = null,
		Texture2D? texture = null,
		SourceSet? sources = null
	) {
		color ??= Color.White;
		texture ??= color == Color.White ? Game1.menuTexture : Game1.uncoloredMenuTexture;
		var source = sources ?? Sprites.NativeDialogue;

		// Determine the scale automatically.
		float scale = 64f / source.TopLeft.Width;

		// Middle
		batch.Draw(texture!, new Rectangle(28 + x, 28 + y, width - 64, height - 64), source.Background, color.Value);

		// Top Left and Right
		batch.Draw(texture!, new Vector2(x, y), source.TopLeft, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
		batch.Draw(texture!, new Vector2(x + width - 64, y), source.TopRight, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

		// Bottom Left and Right
		batch.Draw(texture!, new Vector2(x, y + height - 64), source.BottomLeft, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
		batch.Draw(texture!, new Vector2(x + width - 64, y + height - 64), source.BottomRight, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

		// Top Middle
		batch.Draw(texture!, new Rectangle(x + 64, y, width - 128, 64), source.TopMiddle, color.Value);

		// Bottom Middle
		batch.Draw(texture!, new Rectangle(x + 64, y + height - 64, width - 128, 64), source.BottomMiddle, color.Value);

		// Left Middle
		batch.Draw(texture!, new Rectangle(x, y + 64, 64, height - 128), source.MiddleLeft, color.Value);

		// Right Middle
		batch.Draw(texture!, new Rectangle(x + width - 64, y + 64, 64, height - 128), source.MiddleRight, color.Value);

	}

	public static void DrawHorizontalPartition(
		SpriteBatch batch,
		int x,
		int y,
		int width,
		Color? color = null,
		Texture2D? texture = null,
		SourceSet? sources = null
	) {
		color ??= Color.White;
		texture ??= color == Color.White ? Game1.menuTexture : Game1.uncoloredMenuTexture;
		var source = sources ?? Sprites.NativeDialogue;

		// Determine the scale automatically.
		float scale = 64f / source.TopLeft.Width;

		// Left
		batch.Draw(texture!, new Vector2(x, y), source.HRuleLeft, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

		// Middle
		batch.Draw(texture!, new Rectangle(x + 64, y, width - 128, 64), source.HRuleMiddle, color.Value);

		// Right
		batch.Draw(texture!, new Vector2(x + width - 64, y), source.HRuleRight, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

	}


	public static void DrawVerticalPartition(
		SpriteBatch batch,
		int x,
		int y,
		int height,
		bool ends = true,
		Color? color = null,
		Texture2D? texture = null,
		SourceSet? sources = null
	) {
		color ??= Color.White;
		texture ??= color == Color.White ? Game1.menuTexture : Game1.uncoloredMenuTexture;
		var source = sources ?? Sprites.NativeDialogue;

		// Determine the scale automatically.
		float scale = 64f / source.TopLeft.Width;

		// Middle
		batch.Draw(
			texture!,
			new Rectangle(
				x, ends ? y + 64 : y, 64, ends ? height - 128 : height
			),
			source.VRuleMiddle,
			color.Value
		);

		if (ends) {
			// Top
			batch.Draw(texture!, new Vector2(x, y), source.VRuleTop, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

			// Bottom
			batch.Draw(texture!, new Vector2(x, y + height - 64), source.VRuleBottom, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
		}

	}


	#endregion

	#region 9-Sliced Boxes

	public static void DrawBox(
		SpriteBatch batch,
		Texture2D texture,
		Rectangle sourceRect,
		int x, int y,
		int width, int height,
		Color color,
		int topSlice = -1,
		int leftSlice = -1,
		int rightSlice = -1,
		int bottomSlice = -1,
		float scale = 1f,
		bool drawShadow = true,
		float draw_layer = -1f
	) {
		if (topSlice == -1)
			topSlice = sourceRect.Height / 3;
		if (bottomSlice == -1)
			bottomSlice = sourceRect.Height / 3;
		if (leftSlice == -1)
			leftSlice = sourceRect.Width / 3;
		if (rightSlice == -1)
			rightSlice = sourceRect.Width / 3;

		float layerDepth = draw_layer - 0.03f;
		if (draw_layer < 0f) {
			draw_layer = 0.8f - y * 1E-06f;
			layerDepth = 0.77f;
		}

		int sTop = (int) (topSlice * scale);
		int sLeft = (int) (leftSlice * scale);
		int sRight = (int) (rightSlice * scale);
		int sBottom = (int) (bottomSlice * scale);

		// Not doing recursive calls would, frankly, be stupid.
		if (drawShadow)
			DrawBox(
				batch: batch,
				texture: texture,
				sourceRect: sourceRect,
				x: x - 8,
				y: y + 8,
				width: width,
				height: height,
				color: Color.Black * 0.4f,
				topSlice: topSlice,
				leftSlice: leftSlice,
				rightSlice: rightSlice,
				bottomSlice: bottomSlice,
				scale: scale,
				drawShadow: false,
				draw_layer: layerDepth
			);

		// Base
		batch.Draw(
			texture,
			new Rectangle(
				x + sLeft, y + sTop,
				width - sLeft - sRight,
				height - sTop - sBottom
			),
			new Rectangle(
				x: sourceRect.X + leftSlice,
				y: sourceRect.Y + topSlice,
				width: sourceRect.Width - leftSlice - rightSlice,
				height: sourceRect.Height - topSlice - bottomSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Top Left
		batch.Draw(
			texture,
			new Rectangle(x, y, sLeft, sTop),
			new Rectangle(
				x: sourceRect.X,
				y: sourceRect.Y,
				width: leftSlice,
				height: topSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Top Middle
		batch.Draw(
			texture,
			new Rectangle(x + sLeft, y, width - sLeft - sRight, sTop),
			new Rectangle(
				x: sourceRect.X + leftSlice,
				y: sourceRect.Y,
				width: sourceRect.Width - leftSlice - rightSlice,
				height: topSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Top Right
		batch.Draw(
			texture,
			new Rectangle(x + width - sRight, y, sRight, sTop),
			new Rectangle(
				x: sourceRect.X + sourceRect.Width - rightSlice,
				y: sourceRect.Y,
				width: rightSlice,
				height: topSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Left
		batch.Draw(
			texture,
			new Rectangle(x, y + sTop, sLeft, height - sTop - sBottom),
			new Rectangle(
				x: sourceRect.X,
				y: sourceRect.Y + topSlice,
				width: leftSlice,
				height: sourceRect.Height - topSlice - bottomSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Right
		batch.Draw(
			texture,
			new Rectangle(
				x: x + width - sRight,
				y: y + sTop,
				width: sRight,
				height: height - sTop - sBottom),
			new Rectangle(
				x: sourceRect.X + sourceRect.Width - rightSlice,
				y: sourceRect.Y + topSlice,
				width: rightSlice,
				height: sourceRect.Height - topSlice - bottomSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Bottom Left
		batch.Draw(
			texture,
			new Rectangle(
				x: x,
				y: y + height - sBottom,
				width: sLeft,
				height: sBottom
			),
			new Rectangle(
				x: sourceRect.X,
				y: sourceRect.Y + sourceRect.Height - bottomSlice,
				width: leftSlice,
				height: bottomSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Bottom Middle
		batch.Draw(
			texture,
			new Rectangle(
				x: x + sLeft,
				y: y + height - sBottom,
				width: width - sLeft - sRight,
				height: sBottom
			),
			new Rectangle(
				x: sourceRect.X + leftSlice,
				y: sourceRect.Y + sourceRect.Height - bottomSlice,
				width: sourceRect.Width - leftSlice - rightSlice,
				height: bottomSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

		// Bottom Right
		batch.Draw(
			texture,
			new Rectangle(
				x: x + width - sRight,
				y: y + height - sBottom,
				width: sRight,
				height: sBottom
			),
			new Rectangle(
				x: sourceRect.X + sourceRect.Width - rightSlice,
				y: sourceRect.Y + sourceRect.Height - bottomSlice,
				width: rightSlice,
				height: bottomSlice
			),
			color,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			draw_layer
		);

	}

	#endregion

	#region Scissor Rendering

	private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

	[MemberNotNull(nameof(GetSortMode))]
	[MemberNotNull(nameof(GetBlendState))]
	[MemberNotNull(nameof(GetSamplerState))]
	[MemberNotNull(nameof(GetDepthStencilState))]
	[MemberNotNull(nameof(GetRasterizerState))]
	[MemberNotNull(nameof(GetEffect))]
	private static void LoadFields() {
		Type SB = typeof(SpriteBatch);

		GetSortMode ??= SB.GetField("_sortMode", Flags)!.CreateGetter<SpriteBatch, SpriteSortMode>();
		GetBlendState ??= SB.GetField("_blendState", Flags)!.CreateGetter<SpriteBatch, BlendState>();
		GetSamplerState ??= SB.GetField("_samplerState", Flags)!.CreateGetter<SpriteBatch, SamplerState>();
		GetDepthStencilState ??= SB.GetField("_depthStencilState", Flags)!.CreateGetter<SpriteBatch, DepthStencilState>();
		GetRasterizerState ??= SB.GetField("_rasterizerState", Flags)!.CreateGetter<SpriteBatch, RasterizerState>();
		GetEffect ??= SB.GetField("_effect", Flags)!.CreateGetter<SpriteBatch, Effect>();

	}

	private static Func<SpriteBatch, SpriteSortMode>? GetSortMode;
	private static Func<SpriteBatch, BlendState>? GetBlendState;
	private static Func<SpriteBatch, SamplerState>? GetSamplerState;
	private static Func<SpriteBatch, DepthStencilState>? GetDepthStencilState;
	private static Func<SpriteBatch, RasterizerState>? GetRasterizerState;
	private static Func<SpriteBatch, Effect>? GetEffect;


	public static void WithScissor(SpriteBatch b, SpriteSortMode mode, Rectangle rectangle, Action action, RenderTarget2D? target = null) {

		LoadFields();

		SpriteSortMode old_sort = GetSortMode(b);
		BlendState? old_blend = GetBlendState(b);
		SamplerState? old_sampler = GetSamplerState(b);
		DepthStencilState? old_depth = GetDepthStencilState(b);
		RasterizerState? old_rasterizer = GetRasterizerState(b);
		Effect? old_effect = GetEffect(b);

		var old_scissor = b.GraphicsDevice.ScissorRectangle;

		var old_targets = b.GraphicsDevice.GetRenderTargets();

		RasterizerState state = new() {
			ScissorTestEnable = true
		};

		if (old_rasterizer != null) {
			state.CullMode = old_rasterizer.CullMode;
			state.FillMode = old_rasterizer.FillMode;
			state.DepthBias = old_rasterizer.DepthBias;
			state.MultiSampleAntiAlias = old_rasterizer.MultiSampleAntiAlias;
			state.SlopeScaleDepthBias = old_rasterizer.SlopeScaleDepthBias;
			state.DepthClipEnable = old_rasterizer.DepthClipEnable;
		}

		b.End();

		b.Begin(
			sortMode: mode,
			blendState: old_blend,
			samplerState: old_sampler,
			depthStencilState: old_depth,
			rasterizerState: state,
			effect: old_effect,
			transformMatrix: null
		);

		if (target != null)
			b.GraphicsDevice.SetRenderTarget(target);

		b.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(rectangle, old_scissor);

		try {
			action?.Invoke();
		} finally {
			b.End();

			b.Begin(
				sortMode: old_sort,
				blendState: old_blend,
				samplerState: old_sampler,
				depthStencilState: old_depth,
				rasterizerState: old_rasterizer,
				effect: old_effect,
				transformMatrix: null
			);

			if (target != null) {
				var rt = (old_targets?.Length ?? 0) > 0 ?
					old_targets![0].RenderTarget : null;
				b.GraphicsDevice.SetRenderTarget((RenderTarget2D?) rt);
			}

			b.GraphicsDevice.ScissorRectangle = old_scissor;
		}
	}

	#endregion

}

