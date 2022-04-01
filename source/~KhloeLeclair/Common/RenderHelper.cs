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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;


namespace Leclair.Stardew.Common
{
	public static class RenderHelper {

		private static IModHelper Helper;

		public static void SetHelper(IModHelper helper) {
			Helper = helper;
		}

		public static Rectangle GetIntersection(this Rectangle self, Rectangle other) {
			return Rectangle.Intersect(self, other);
		}

		public static Rectangle Clone(this Rectangle self) {
			return self;
		}

		// SpriteText Reflection
		private static bool HasReflected = false;
		private static IReflectedMethod _SetupCharacterMap;
		private static IReflectedMethod _IsSpecialCharacter;
		private static IReflectedMethod _getSourceRectForChar;
		private static IReflectedField<IDictionary> _CharacterMap;
		private static IReflectedField<List<Texture2D>> _FontPages;

		private static void AssertReflection() {
			if (HasReflected)
				return;

			HasReflected = true;
			_SetupCharacterMap = Helper.Reflection.GetMethod(typeof(SpriteText), "setUpCharacterMap");
			_IsSpecialCharacter = Helper.Reflection.GetMethod(typeof(SpriteText), "IsSpecialCharacter");
			_getSourceRectForChar = Helper.Reflection.GetMethod(typeof(SpriteText), "getSourceRectForChar");
			_CharacterMap = Helper.Reflection.GetField<IDictionary>(typeof(SpriteText), "_characterMap");
			_FontPages = Helper.Reflection.GetField<List<Texture2D>>(typeof(SpriteText), "fontPages");
		}

		public static IDictionary SpriteText_CharacterMap {
			get {
				AssertReflection();
				return _CharacterMap.GetValue();
			}
		}

		public static List<Texture2D> SpriteText_FontPages {
			get {
				AssertReflection();
				return _FontPages.GetValue();
			}
		}

		public static bool SpriteText_IsSpecialCharacter(char c) {
			AssertReflection();
			return _IsSpecialCharacter.Invoke<bool>(c);
		}

		public static Rectangle SpriteText_getSourceRectForChar(char c, bool junimoText) {
			AssertReflection();
			return _getSourceRectForChar.Invoke<Rectangle>(c, junimoText);
		}

		public static int SpriteText_LineHeight() {
			object font = Helper.Reflection.GetField<object>(typeof(SpriteText), "FontFile", false)?.GetValue();
			if (font == null)
				return 0;

			object common = Helper.Reflection.GetProperty<object>(font, "Common", false)?.GetValue();
			if (common == null)
				return 0;

			return Helper.Reflection.GetProperty<int>(common, "LineHeight", false)?.GetValue() ?? 0;
		}

		public static Rectangle FontChar_GetSourceRect(object c) {
			return new Rectangle(
				Helper.Reflection.GetProperty<int>(c, "X").GetValue(),
				Helper.Reflection.GetProperty<int>(c, "Y").GetValue(),
				Helper.Reflection.GetProperty<int>(c, "Width").GetValue(),
				Helper.Reflection.GetProperty<int>(c, "Height").GetValue()
			);
		}

		public static Vector2 FontChar_GetOffset(object c) {
			return new Vector2(
				Helper.Reflection.GetProperty<int>(c, "XOffset").GetValue(),
				Helper.Reflection.GetProperty<int>(c, "YOffset").GetValue()
			);
		}

		public static int FontChar_GetPage(object c) {
			return Helper.Reflection.GetProperty<int>(c, "Page").GetValue();
		}

		public static int FontChar_GetXAdvance(object c) {
			return Helper.Reflection.GetProperty<int>(c, "XAdvance").GetValue();
		}

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
			DrawSpriteText(
				b: b,
				text: text,
				x: x - SpriteText.getWidthOfString(text, maxWidth) / 2,
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
			int originalX = x;
			int originalY = y;
			int originalWidth = width;
			float originalScale = SpriteText.fontPixelZoom;

			try {
				AssertReflection();
				_SetupCharacterMap.Invoke();

				if (width == -1)
					width = Game1.graphics.GraphicsDevice.Viewport.Width - x;

				if (SpriteText.fontPixelZoom < 4f && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ko)
					y += (int) ((4f - SpriteText.fontPixelZoom) * 4f);

				Vector2 position = new(x, y);
				int accumulatedHorizontalSpaceBetweenCharacters = 0;

				// if ( drawBGScroll != 1)
				if (position.X + (float) width > (float) (Game1.graphics.GraphicsDevice.Viewport.Width - 4))
					position.X = Game1.graphics.GraphicsDevice.Viewport.Width - width - 4;

				if (position.X < 0f)
					position.X = 0f;
				// endif

				// Trimmed out a bunch of background stuff we don't care about here.

				if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
					position.Y -= 8f;

				text = text.Replace(Environment.NewLine, "");
				if (!junimoText && (
					LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja ||
					LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh ||
					LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th ||
					(LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.mod && LocalizedContentManager.CurrentModLanguage.FontApplyYOffset)
				))
					position.Y -= (4f - SpriteText.fontPixelZoom) * 4f;

				text = text.Replace('♡', '<');
				for (int i = 0; i < Math.Min(text.Length, characterPosition); i++) {
					char c = text[i];
					bool special = SpriteText_IsSpecialCharacter(c);
					if (LocalizedContentManager.CurrentLanguageLatin || special || junimoText || SpriteText.forceEnglishFont) {
						float old_zoom = SpriteText.fontPixelZoom;
						if (special || junimoText || SpriteText.forceEnglishFont)
							SpriteText.fontPixelZoom = 3f;

						if (c == '^') {
							position.Y += 18f * SpriteText.fontPixelZoom;
							position.X = x;
							accumulatedHorizontalSpaceBetweenCharacters = 0;
							SpriteText.fontPixelZoom = old_zoom;
							continue;
						}

						accumulatedHorizontalSpaceBetweenCharacters = (int) (0f * SpriteText.fontPixelZoom);
						bool upper = char.IsUpper(c) || c == 'ß';
						Vector2 offset = new(0f, -1 + ((!junimoText && upper) ? -3 : 0));
						if (c == 'Ç')
							offset.Y += 2f;

						if (SpriteText.positionOfNextSpace(text, i, (int) position.X - x, accumulatedHorizontalSpaceBetweenCharacters) >= width) {
							position.Y += 18f * SpriteText.fontPixelZoom;
							accumulatedHorizontalSpaceBetweenCharacters = 0;
							position.X = x;
							if (c == ' ') {
								SpriteText.fontPixelZoom = old_zoom;
								continue;
							}
						}

						bool use_color = !special && !junimoText && color.HasValue;

						b.Draw(
							use_color ? SpriteText.coloredTexture : SpriteText.spriteTexture,
							position + offset * SpriteText.fontPixelZoom,
							SpriteText_getSourceRectForChar(c, junimoText),
							(use_color ? (color ?? SpriteText.getColorFromIndex(-1)) : Color.White) * alpha,
							0f,
							Vector2.Zero,
							SpriteText.fontPixelZoom,
							SpriteEffects.None,
							layerDepth
						);

						if (i < text.Length - 1)
							position.X += 8f * SpriteText.fontPixelZoom + (float) accumulatedHorizontalSpaceBetweenCharacters + (float) SpriteText.getWidthOffsetForChar(text[i + 1]) * SpriteText.fontPixelZoom;

						if (c != '^')
							position.X += (float) SpriteText.getWidthOffsetForChar(c) * SpriteText.fontPixelZoom;

						SpriteText.fontPixelZoom = old_zoom;
						continue;
					}

					if (c == '^') {
						position.Y += (float)(SpriteText_LineHeight() + 2f) * SpriteText.fontPixelZoom;
						position.X = x;
						accumulatedHorizontalSpaceBetweenCharacters = 0;
						continue;
					}

					if (i > 0 && SpriteText_IsSpecialCharacter(text[i - 1]))
						position.X += 24f;

					var map = SpriteText_CharacterMap;

					if (map.Contains(c)) {
						object fchar = map[c];
						if (fchar != null) {
							Rectangle box = FontChar_GetSourceRect(fchar);
							Texture2D texture = SpriteText_FontPages[FontChar_GetPage(fchar)];
							if (SpriteText.positionOfNextSpace(text, i, (int) position.X, accumulatedHorizontalSpaceBetweenCharacters) >= x + width - 4) {
								position.Y += (float) (SpriteText_LineHeight() + 2) * SpriteText.fontPixelZoom;
								accumulatedHorizontalSpaceBetweenCharacters = 0;
								position.X = x;
							}

							Vector2 offset = FontChar_GetOffset(fchar);
							offset.X = position.X + offset.X * SpriteText.fontPixelZoom;
							offset.Y = position.Y + offset.Y * SpriteText.fontPixelZoom;

							if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru) {
								Vector2 off2 = new Vector2(-1f, 1f) * SpriteText.fontPixelZoom;
								b.Draw(
									texture, offset + off2, box,
									(color ?? SpriteText.getColorFromIndex(-1)) * alpha * SpriteText.shadowAlpha,
									0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth
								);

								b.Draw(
									texture, offset + new Vector2(0f, off2.Y), box,
									(color ?? SpriteText.getColorFromIndex(-1)) * alpha * SpriteText.shadowAlpha,
									0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth
								);

								b.Draw(
									texture, offset + new Vector2(off2.X, 0f), box,
									(color ?? SpriteText.getColorFromIndex(-1)) * alpha * SpriteText.shadowAlpha,
									0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth
								);
							}

							b.Draw(
								texture, offset, box, (color ?? SpriteText.getColorFromIndex(-1)) * alpha,
								0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth
							);

							position.X += (float) FontChar_GetXAdvance(fchar) * SpriteText.fontPixelZoom;
						}
					}
				}

				SpriteText.fontPixelZoom = originalScale;

			} catch {
				SpriteText.fontPixelZoom = originalScale;
				SpriteText.drawString(
					b,
					s: text,
					x: originalX,
					y: originalY,
					characterPosition: characterPosition,
					width: originalWidth,
					height: height,
					alpha: alpha,
					layerDepth: layerDepth,
					junimoText: junimoText
				);
			}
		}


		public static void DrawBox(
			SpriteBatch b,
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
					b: b,
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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
			b.Draw(
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


		public static void WithScissor(SpriteBatch b, SpriteSortMode mode, Rectangle rectangle, Action action, RenderTarget2D target = null) {

			var smField = Helper.Reflection.GetField<SpriteSortMode>(b, "_sortMode", false);
			SpriteSortMode old_sort = smField?.GetValue() ?? mode;

			var bsField = Helper.Reflection.GetField<BlendState>(b, "_blendState", false);
			BlendState old_blend = bsField?.GetValue();

			var ssField = Helper.Reflection.GetField<SamplerState>(b, "_samplerState", false);
			SamplerState old_sampler = ssField?.GetValue();

			var dsField = Helper.Reflection.GetField<DepthStencilState>(b, "_depthStencilState", false);
			DepthStencilState old_depth = dsField?.GetValue();

			var rsField = Helper.Reflection.GetField<RasterizerState>(b, "_rasterizerState", false);
			RasterizerState old_rasterizer = rsField?.GetValue();

			var efField = Helper.Reflection.GetField<Effect>(b, "_effect", false);
			Effect old_effect = efField?.GetValue();

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
						old_targets[0].RenderTarget : null;
					b.GraphicsDevice.SetRenderTarget((RenderTarget2D) rt);
				}

				b.GraphicsDevice.ScissorRectangle = old_scissor;
			}
		}


    }
}
