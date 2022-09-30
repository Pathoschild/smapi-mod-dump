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
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SpriteMaster.Core;

internal static class OnDrawStringImpl {
	private const bool Continue = true;
	private const bool Stop = false;

	private delegate bool TryGetGlyphIndexDelegate(SpriteFont font, char c, out int index);
	private static readonly TryGetGlyphIndexDelegate? TryGetGlyphIndexFunc = typeof(SpriteFont).GetInstanceMethod("TryGetGlyphIndex")?.CreateDelegate<TryGetGlyphIndexDelegate>();


	private static readonly ConditionalWeakTable<SpriteFont, Dictionary<char, int?>> GlyphIndexCache = new();
	private static bool TryGetGlyphIndexCached(this SpriteFont font, char c, out int index) {
		if (!GlyphIndexCache.TryGetValue(font, out var indexCache)) {
			GlyphIndexCache.Add(font, indexCache = new());
		}
		else {
			if (indexCache.TryGetValue(c, out var indexOpt)) {
				index = indexOpt ?? 0;
				return indexOpt.HasValue;
			}
		}
		if (TryGetGlyphIndexFunc!(font, c, out index)) {
			indexCache.Add(c, index);
			return true;
		}

		indexCache.Add(c, null);
		return false;
	}

	internal static bool DrawString(
		XSpriteBatch __instance,
		SpriteFont spriteFont,
		string text,
		XVector2 position,
		XColor color,
		float rotation,
		XVector2 origin,
		XVector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		if (!Configuration.Config.IsEnabled || !Configuration.Config.Resample.IsEnabled) {
			return Continue;
		}

		if (TryGetGlyphIndexFunc is null) {
			return Continue;
		}

		return DrawString(__instance, spriteFont, new StringSource(text), position, color, rotation, origin, scale, effects, layerDepth);
	}

	internal static bool DrawString(
		XSpriteBatch __instance,
		SpriteFont spriteFont,
		StringBuilder text,
		XVector2 position,
		XColor color,
		float rotation,
		XVector2 origin,
		XVector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		if (!Configuration.Config.IsEnabled || !Configuration.Config.Resample.IsEnabled) {
			return Continue;
		}

		if (TryGetGlyphIndexFunc is null) {
			return Continue;
		}

		return DrawString(__instance, spriteFont, new BuilderSource(text), position, color, rotation, origin, scale, effects, layerDepth);
	}

	internal interface ICharSource : ILongHash, IEnumerable<char> {
		internal object Reference { get; }
		internal int Length { get; }

		internal char this[int index] { get; }
	}

	[StructLayout(LayoutKind.Auto)]
	internal readonly struct StringSource : ICharSource {
		internal readonly string String;
		internal readonly int Length;
		object ICharSource.Reference => String;
		int ICharSource.Length => Length;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal StringSource(string obj) {
			String = obj;
			Length = obj.Length;
		}

		readonly char ICharSource.this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => String[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CharEnumerator GetEnumerator() => String.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<char> IEnumerable<char>.GetEnumerator() => String.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override string ToString() => String;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => String.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ulong GetLongHashCode() => String.GetLongHashCode();
	}

	[StructLayout(LayoutKind.Auto)]
	internal readonly struct BuilderSource : ICharSource {
		internal readonly StringBuilder Builder;
		internal readonly int Length;
		object ICharSource.Reference => Builder;
		int ICharSource.Length => Length;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal BuilderSource(StringBuilder obj) {
			Builder = obj;
			Length = obj.Length;
		}

		readonly char ICharSource.this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Builder[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator() => new Enumerator(Builder);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<char> IEnumerable<char>.GetEnumerator() => new Enumerator(Builder);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override string ToString() => Builder.ToString();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(Builder);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ulong GetLongHashCode() => Builder.GetLongHashCode();

		[StructLayout(LayoutKind.Auto)]
		internal struct Enumerator : IEnumerator<char> {
			private readonly StringBuilder Builder;
			private StringBuilder.ChunkEnumerator Chunks;
			private ReadOnlyMemory<char> CurrentChunk = default;
			private int CurrentIndex = -1;

			public readonly char Current => CurrentChunk.Span[CurrentIndex];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator(StringBuilder builder) {
				Builder = builder;
				Chunks = builder.GetChunks();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool InitChunks() {
				while (CurrentChunk.IsEmpty) {
					if (!Chunks.MoveNext()) {
						return false;
					}

					CurrentChunk = Chunks.Current;
				}

				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool MoveNextChunk() {
				do {
					if (!Chunks.MoveNext()) {
						return false;
					}

					CurrentChunk = Chunks.Current;
				} while (CurrentChunk.IsEmpty);

				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext() {
				if (CurrentIndex == -1) {
					if (!InitChunks()) {
						return false;
					}
				}
				else if (CurrentIndex + 1 >= CurrentChunk.Length) {
					if (!MoveNextChunk()) {
						return false;
					}

					CurrentIndex = -1;
				}

				++CurrentIndex;
				return true;
			}

			public void Reset() {
				Chunks = Builder.GetChunks();
				CurrentChunk = default;
				CurrentIndex = -1;
			}

			readonly object IEnumerator.Current => Current;

			public readonly void Dispose() { }
		}
	}
	
	internal static bool DrawString<TCharSource>(
		XSpriteBatch __instance,
		SpriteFont spriteFont,
		in TCharSource text,
		XVector2 position,
		XColor color,
		float rotation,
		XVector2 origin,
		XVector2 scale,
		SpriteEffects effects,
		float layerDepth
	) where TCharSource : ICharSource {
		var textureFlags = spriteFont.Texture?.Meta().Flags ?? default;

		if (textureFlags.HasFlag(Texture2DMeta.TextureFlag.IsLargeFont) && !(Configuration.Preview.Override.Instance?.ResampleLargeText ?? Configuration.Config.Resample.EnabledLargeText)) {
			return true;
		}

		if (textureFlags.HasFlag(Texture2DMeta.TextureFlag.IsSmallFont) && !(Configuration.Preview.Override.Instance?.ResampleSmallText ?? Configuration.Config.Resample.EnabledSmallText)) {
			return true;
		}

		var flipAdjustment = Vector2F.Zero;
		Vector2B flipped = (effects.HasFlag(SpriteEffects.FlipHorizontally), effects.HasFlag(SpriteEffects.FlipVertically));

		Vector2F flippedScale = scale;

		if (flipped.Any) {
			Vector2F size = spriteFont.MeasureString(text.ToString());

			if (flipped.X) {
				flipAdjustment.X = -size.X;
				flippedScale.X = -flippedScale.X;
			}
			if (flipped.Y) {
				flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
				flippedScale.Y = -flippedScale.Y;
			}
		}

		XNA.Matrix? transform = null;
		if (rotation != 0.0f || scale != Vector2F.One || flipped.Any) {
			var tempTransform = XNA.Matrix.Identity;

			if (rotation == 0.0f) {
				tempTransform.M11 = flippedScale.X;
				tempTransform.M22 = flippedScale.Y;
				
				tempTransform.M41 = (flipAdjustment.X * tempTransform.M11) + position.X;
				tempTransform.M42 = (flipAdjustment.Y * tempTransform.M22) + position.Y;
			}
			else {
				(float sin, float cos) = rotation.SinCos();

				tempTransform.M11 = flippedScale.X * cos;
				tempTransform.M12 = flippedScale.X * sin;
				tempTransform.M21 = flippedScale.Y * -sin;
				tempTransform.M22 = flippedScale.Y * cos;
				tempTransform.M41 = (flipAdjustment.X * tempTransform.M11) + (flipAdjustment.Y * tempTransform.M21) + position.X;
				tempTransform.M42 = (flipAdjustment.X * tempTransform.M12) + (flipAdjustment.Y * tempTransform.M22) + position.Y;
			}
			transform = tempTransform;
		}

		Vector2F offset = Vector2F.Zero;
		bool firstGlyphOfLine = true;

		foreach (var c in text) {
			if (c == '\r') {
				continue;
			}

			if (c == '\n') {
				offset.X = 0.0f;
				offset.Y += spriteFont.LineSpacing;
				firstGlyphOfLine = true;
				continue;
			}

			if (!spriteFont.TryGetGlyphIndexCached(c, out var glyphIndex)) {
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
				XVector2 posTemp = pos;
				XVector2.Transform(ref posTemp, ref matrixTemp, out posTemp);
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
