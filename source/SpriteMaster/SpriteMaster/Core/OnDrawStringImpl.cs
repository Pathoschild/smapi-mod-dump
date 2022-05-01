/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SpriteMaster.Core;

static partial class OnDrawStringImpl {
	private const bool Continue = true;
	private const bool Stop = false;

	private delegate bool TryGetGlyphIndexDelegate(SpriteFont font, char c, out int index);
	private static readonly TryGetGlyphIndexDelegate? TryGetGlyphIndexFunc = typeof(SpriteFont).GetMethod(
		"TryGetGlyphIndex",
		System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public
	)?.CreateDelegate<TryGetGlyphIndexDelegate>();

	private static readonly Dictionary<SpriteFont, Dictionary<char, int?>> GlyphIndexCache = new();
	private static bool TryGetGlyphIndex(this SpriteFont font, char c, out int index) {
		/*
		if (!GlyphIndexCache.TryGetValue(font, out var indexCache)) {
			GlyphIndexCache.Add(font, indexCache = new());
		}
		else {
			if (indexCache.TryGetValue(c, out var indexOpt)) {
				index = indexOpt ?? 0;
				return indexOpt.HasValue;
			}
		}
		*/
		if (TryGetGlyphIndexFunc!(font, c, out index)) {
			//indexCache.Add(c, index);
			return true;
		}
		else {
			//indexCache.Add(c, null);
			return false;
		}
	}

	internal static bool DrawString(
		SpriteBatch __instance,
		SpriteFont spriteFont,
		string text,
		XNA.Vector2 position,
		XNA.Color color,
		float rotation,
		XNA.Vector2 origin,
		XNA.Vector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		if (!Configuration.Config.IsEnabled || !Configuration.Config.Resample.IsEnabled) {
			return Continue;
		}

		if (TryGetGlyphIndexFunc is null) {
			return Continue;
		}

		return DrawString(__instance, spriteFont, new CharSource(text), position, color, rotation, origin, scale, effects, layerDepth);
	}

	internal static bool DrawString(
		SpriteBatch __instance,
		SpriteFont spriteFont,
		StringBuilder text,
		XNA.Vector2 position,
		XNA.Color color,
		float rotation,
		XNA.Vector2 origin,
		XNA.Vector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		if (!Configuration.Config.IsEnabled || !Configuration.Config.Resample.IsEnabled) {
			return Continue;
		}

		if (TryGetGlyphIndexFunc is null) {
			return Continue;
		}

		return DrawString(__instance, spriteFont, new CharSource(text), position, color, rotation, origin, scale, effects, layerDepth);
	}

	// Ripped from MonoGame SpriteFont.cs
	internal ref struct CharSource {
		private readonly string? String = null;
		private readonly StringBuilder? Builder = null;
		internal readonly int Length;

		private CharSource(int length) => Length = length;

		internal CharSource(string s) : this(s.Length) {
			String = s;
		}

		internal CharSource(StringBuilder builder) : this(builder.Length) {
			Builder = builder;
		}

		internal char this[int index] {
			get {
				if (String is not null) {
					return String[index];
				}
				return Builder![index];
			}
		}

		public override string ToString() {
			if (String is not null) {
				return String.ToString();
}
			return Builder!.ToString();
		}
	}

	// internal float TexelWidth { get; private set; }
	// internal float TexelHeight { get; private set; }

	internal static bool DrawString(
		SpriteBatch __instance,
		SpriteFont spriteFont,
		in CharSource text,
		XNA.Vector2 position,
		XNA.Color color,
		float rotation,
		XNA.Vector2 origin,
		XNA.Vector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		if (!(Configuration.Preview.Override.Instance?.ResampleBasicText ?? Configuration.Config.Resample.EnabledBasicText)) {
			return true;
		}

		var flipAdjustment = Vector2F.Zero;
		Vector2B flipped = (effects.HasFlag(SpriteEffects.FlipHorizontally), effects.HasFlag(SpriteEffects.FlipVertically));
		
		if (flipped.Any) {
			Vector2F size = spriteFont.MeasureString(text.ToString());

			if (flipped.X) {
				origin.X *= -1;
				flipAdjustment.X = -size.X;
			}
			if (flipped.Y) {
				origin.Y *= -1;
				flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
			}
		}

		XNA.Matrix? transform = null;
		if (rotation != 0.0f || scale != Vector2F.Zero) {
			var tempTransform = XNA.Matrix.Identity;

			Vector2F flippedScale = (
				flipped.X ? -scale.X : scale.X,
				flipped.Y ? -scale.Y : scale.Y
			);
			Vector2F flippedOrigin = flipAdjustment - (Vector2F)origin;

			if (rotation == 0.0f) {
				tempTransform.M11 = flippedScale.X;
				tempTransform.M22 = flippedScale.Y;
				
				tempTransform.M41 = (flippedOrigin.X * tempTransform.M11) + position.X;
				tempTransform.M42 = (flippedOrigin.Y * tempTransform.M22) + position.Y;
			}
			else {
				(float sin, float cos) = MathExt.SinCos(rotation);

				tempTransform.M11 = flippedScale.X * cos;
				tempTransform.M12 = flippedScale.X * sin;
				tempTransform.M21 = flippedScale.Y * -sin;
				tempTransform.M22 = flippedScale.Y * cos;
				tempTransform.M41 = ((flippedOrigin.X * tempTransform.M11) + (flippedOrigin.Y * tempTransform.M21)) + position.X;
				tempTransform.M42 = ((flippedOrigin.X * tempTransform.M12) + (flippedOrigin.Y * tempTransform.M22)) + position.Y;
			}
			transform = tempTransform;
		}

		Vector2F offset = Vector2F.Zero;
		bool firstGlyphOfLine = true;

		for (int i = 0; i < text.Length; ++i) {
			var c = text[i];

			if (c == '\r') {
				continue;
			}

			if (c == '\n') {
				offset.X = 0.0f;
				offset.Y += spriteFont.LineSpacing;
				firstGlyphOfLine = true;
				continue;
			}

			if (!spriteFont.TryGetGlyphIndex(c, out var glyphIndex)) {
				return Continue;
			}

			var glyph = spriteFont.Glyphs[glyphIndex];

			if (firstGlyphOfLine) {
				offset.X = Math.Max(glyph.LeftSideBearing, 0.0f);
				firstGlyphOfLine = false;
			}
			else {
				offset.X += spriteFont.Spacing + glyph.LeftSideBearing;
			}

			var pos = offset;
			if (flipped.X) {
				pos.X += glyph.BoundsInTexture.Width;
			}
			if (flipped.Y) {
				pos.Y += glyph.BoundsInTexture.Height - spriteFont.LineSpacing;
			}
			pos += glyph.Cropping.Location;

			if (transform.HasValue) {
				var matrixTemp = transform.Value;
				XNA.Vector2 posTemp = pos;
				XNA.Vector2.Transform(ref posTemp, ref matrixTemp, out posTemp);
				pos = posTemp;
			}
			else {
				pos += (Vector2F)position;
			}

			var textureBounds = (Bounds)glyph.BoundsInTexture;

			// DRAW
			__instance.Draw(
				texture: spriteFont.Texture,
				position: pos,
				sourceRectangle: textureBounds,
				color: color,
				rotation: rotation,
				origin: origin,
				scale: scale,
				effects: effects,
				layerDepth: layerDepth
			);

			offset.X += glyph.Width + glyph.RightSideBearing;
		}

		return Stop;
	}
}
