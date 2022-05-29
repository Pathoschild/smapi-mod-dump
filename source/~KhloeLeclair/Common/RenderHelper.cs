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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewValley.BellsAndWhistles;

#if HARMONY
using HarmonyLib;
#endif

namespace Leclair.Stardew.Common;

#if HARMONY
internal static class SpriteText_Patches {

	private static IMonitor? Monitor;

	internal static void Patch(Harmony harmony, IMonitor monitor) {
		Monitor = monitor;

		harmony.Patch(
			original: AccessTools.Method(typeof(SpriteText), nameof(SpriteText.getColorFromIndex)),
			prefix: new HarmonyMethod(typeof(SpriteText_Patches), nameof(getColorFromIndex__Prefix))
		);
	}

	static bool getColorFromIndex__Prefix(int index, ref Color __result) {
		try {
			if (index >= 100) {
				__result = CommonHelper.UnpackColor(index - 100);
				return false;
			}

		} catch (Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(getColorFromIndex__Prefix)}. Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}
}
#endif

public static class RenderHelper {

	private static IModHelper? Helper;

	[MemberNotNull(nameof(Helper))]
	public static void SetHelper(IModHelper helper) {
		Helper = helper;
	}

	public static Rectangle GetIntersection(this Rectangle self, Rectangle other) {
		return Rectangle.Intersect(self, other);
	}

	public static Rectangle Clone(this Rectangle self) {
		return self;
	}

	#region SpriteText Nonsense

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
		int cint;
		if (color.HasValue)
			cint = color.Value.PackColor() + 100;
		else
			cint = -1;

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
			color: cint,
			maxWidth: maxWidth
		);
	}

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
		int cint;
		if (color.HasValue)
			cint = color.Value.PackColor() + 100;
		else
			cint = -1;

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
			color: cint
		);
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

	public static void WithScissor(SpriteBatch b, SpriteSortMode mode, Rectangle rectangle, Action action, RenderTarget2D? target = null) {

		var smField = Helper?.Reflection.GetField<SpriteSortMode>(b, "_sortMode", false);
		SpriteSortMode old_sort = smField?.GetValue() ?? mode;

		var bsField = Helper?.Reflection.GetField<BlendState>(b, "_blendState", false);
		BlendState? old_blend = bsField?.GetValue();

		var ssField = Helper?.Reflection.GetField<SamplerState>(b, "_samplerState", false);
		SamplerState? old_sampler = ssField?.GetValue();

		var dsField = Helper?.Reflection.GetField<DepthStencilState>(b, "_depthStencilState", false);
		DepthStencilState? old_depth = dsField?.GetValue();

		var rsField = Helper?.Reflection.GetField<RasterizerState>(b, "_rasterizerState", false);
		RasterizerState? old_rasterizer = rsField?.GetValue();

		var efField = Helper?.Reflection.GetField<Effect>(b, "_effect", false);
		Effect? old_effect = efField?.GetValue();

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

