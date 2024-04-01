/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.EPX;

internal sealed partial class Scaler : AbstractScaler<Config, Scaler.ValueScale> {
	private const uint MinScale = 2;
	private const uint MaxScale = Config.MaxScale;

	internal readonly struct ValueScale : IScale {
		public readonly uint Minimum => MinScale;
		public readonly uint Maximum => MaxScale;
	}

	private static uint ClampScale(uint scale) => Math.Clamp(scale, MinScale, MaxScale);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static Span<Color16> Apply(
		Config config,
		uint scaleMultiplier,
		ReadOnlySpan<Color16> sourceData,
		Vector2I sourceSize,
		Span<Color16> targetData,
		Vector2I targetSize
	) {
		Common.ApplyValidate(config, scaleMultiplier, sourceData, sourceSize, ref targetData, targetSize);

		var scalerInstance = new Scaler(
			configuration: config,
			scaleMultiplier: scaleMultiplier,
			sourceSize: sourceSize,
			targetSize: targetSize
		);

		scalerInstance.Scale(sourceData, targetData);
		return targetData;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private Scaler(
		Config configuration,
		uint scaleMultiplier,
		Vector2I sourceSize,
		Vector2I targetSize
	) : base(configuration, scaleMultiplier, sourceSize, targetSize) {
	}

	private interface IGetter {
		public uint GetStride(Vector2I sourceSize);
		public uint Get(int offset, Vector2I sourceSize, bool wrapped);
	}

	private readonly struct XGetter : IGetter {
		public uint GetStride(Vector2I sourceSize) => 1U;

		public uint Get(int offset, Vector2I sourceSize, bool wrapped) {
			if (wrapped) {
				offset = (offset + sourceSize.Width) % sourceSize.Width;
			}
			else {
				offset = Math.Clamp(offset, 0, sourceSize.Width - 1);
			}

			return (uint)offset;
		}
	}

	private readonly struct YGetter : IGetter {
		public uint GetStride(Vector2I sourceSize) => (uint)sourceSize.X;

		public uint Get(int offset, Vector2I sourceSize, bool wrapped) {
			if (wrapped) {
				offset = (offset + sourceSize.Height) % sourceSize.Height;
			}
			else {
				offset = Math.Clamp(offset, 0, sourceSize.Height - 1);
			}

			return (uint)offset;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static Color16 GetPixel(ReadOnlySpan<Color16> src, uint stride, uint offset) {
		return src[(int)(stride + offset)];
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void SetPixel(Span<Color16> dst, uint stride, uint offset, Color16 color) {
		dst[(int)(stride + offset)] = color;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private bool ColorEq(Color16 a, Color16 b) {
		if (Configuration.SmoothCompare) {
			return Common.ColorDistance(
				useRedmean: Configuration.UseRedmean,
				gammaCorrected: Configuration.GammaCorrected,
				hasAlpha: Configuration.HasAlpha,
				pix1: a,
				pix2: b,
				yccConfig: YccConfiguration
			) < Configuration.EqualColorTolerance;
		}

		return a == b;
	}

	// https://en.wikipedia.org/wiki/Pixel-art_scaling_algorithms#EPX/Scale2%C3%97/AdvMAME2%C3%97
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void Scale(ReadOnlySpan<Color16> source, Span<Color16> destination) {
		switch (ScaleMultiplier) {
			case 2:
				Scale2(source, destination, SourceSize, TargetSize);
				break;
			case 3:
				Scale3(source, destination, SourceSize, TargetSize);
				break;
			case 4: {
				var intermediateDestination = SpanExt.Make<Color16>(destination.Length / 4);
				var intermediateSize = TargetSize / 2;
				Scale2(source, intermediateDestination, SourceSize, intermediateSize);
				Scale2(intermediateDestination, destination, intermediateSize, TargetSize);
				break;
			}
			case 6: {
				var intermediateDestination = SpanExt.Make<Color16>(destination.Length / 4);
				var intermediateSize = TargetSize / 2;
				Scale3(source, intermediateDestination, SourceSize, intermediateSize);
				Scale2(intermediateDestination, destination, intermediateSize, TargetSize);
				break;
			}
			default:
				ThrowHelper.ThrowNotImplementedException($"EPX Scale '{ScaleMultiplier}' unimplemented");
				break;
		}
	}

	private readonly struct Offset<TGetter> where TGetter : struct, IGetter {
		private readonly uint Negative;
		private readonly uint Center;
		private readonly uint Positive;

		internal readonly uint this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => index switch {
				-1 => Negative,
				1 => Positive,
				_ => Center
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Offset(uint offset, bool wrapped, Vector2I sourceSize, TGetter getter = default) {
			uint stride = getter.GetStride(sourceSize);
			Negative = stride * getter.Get((int)offset - 1, sourceSize, wrapped);
			Center = stride * getter.Get((int)offset, sourceSize, wrapped);
			Positive = stride * getter.Get((int)offset + 1, sourceSize, wrapped);
		}
	}

	/*
	 *
	 *    │A│
	 *   ─╬═╬─      1│2
	 *   C║P║B  ═>  ─┼─
	 *   ─╬═╬─      3│4
	 *    │D│
	 *
	 */

	private void Scale2(ReadOnlySpan<Color16> source, Span<Color16> destination, Vector2I sourceSize, Vector2I targetSize) {
		if (sourceSize.Y <= 0 || sourceSize.X <= 0) {
			return;
		}

		(sourceSize * 2).AssertEqual(targetSize);
		(source.Length * 4).AssertEqual(destination.Length);

		for (uint y = 0; y < (uint)sourceSize.Y; ++y) {
			Offset<YGetter> yOffsets = new(y, Configuration.Wrapped.Y, sourceSize);

			// The Y offset/stride - where we are outputting in the target
			var baseYOffset = y * 2U;
			var yOutOffset12 = (uint)targetSize.X * baseYOffset;
			var yOutOffset34 = (uint)targetSize.X * (baseYOffset + 1U);

			for (uint x = 0; x < (uint)sourceSize.X; ++x) {
				Offset<XGetter> xOffsets = new(x, Configuration.Wrapped.X, sourceSize);

				var a = GetPixel(source, yOffsets[-1], xOffsets[ 0]);
				var c = GetPixel(source, yOffsets[ 0], xOffsets[-1]);
				var p = GetPixel(source, yOffsets[ 0], xOffsets[ 0]);
				var b = GetPixel(source, yOffsets[ 0], xOffsets[ 1]);
				var d = GetPixel(source, yOffsets[ 1], xOffsets[ 0]);

				var o1 = p;
				var o2 = p;
				var o3 = p;
				var o4 = p;

				bool ac = ColorEq(a, c);
				bool cd = ColorEq(c, d);
				bool ab = ColorEq(a, b);
				if (ac && !cd && !ab) {
					o1 = a;
				}
				bool bd = ColorEq(b, d);
				if (ab && !ac && !bd) {
					o2 = b;
				}
				if (cd && !bd && !ac) {
					o3 = c;
				}
				if (bd && !ab && !cd) {
					o4 = d;
				}

				//o1 = o2 = o3 = o4 = p;

				// The X offset where we are outputting in the target
				var baseXOffset = x * 2U;
				var xOutOffset13 = baseXOffset;
				var xOutOffset24 = baseXOffset + 1U;

				SetPixel(destination, yOutOffset12, xOutOffset13, o1);
				SetPixel(destination, yOutOffset12, xOutOffset24, o2);
				SetPixel(destination, yOutOffset34, xOutOffset13, o3);
				SetPixel(destination, yOutOffset34, xOutOffset24, o4);
			}
		}
	}

	/*
	 *
	 *   A│B│C      1│2│3
	 *   ─╬═╬─      ─┼─┼─
	 *   D║E║F  ═>  4│5│6
	 *   ─╬═╬─      ─┼─┼─
	 *   G│H│I      7│8│9
	 *
	 */

	private void Scale3(ReadOnlySpan<Color16> source, Span<Color16> destination, Vector2I sourceSize, Vector2I targetSize) {
		if (sourceSize.Y <= 0 || sourceSize.X <= 0) {
			return;
		}

		(sourceSize * 3).AssertEqual(targetSize);
		(source.Length * 9).AssertEqual(destination.Length);

		for (uint y = 0; y < (uint)sourceSize.Y; ++y) {
			Offset<YGetter> yOffsets = new(y, Configuration.Wrapped.Y, sourceSize);

			// The Y offset/stride - where we are outputting in the target
			var baseYOffset = y * 3U;
			var yOutOffset123 = (uint)targetSize.X * baseYOffset;
			var yOutOffset456 = (uint)targetSize.X * (baseYOffset + 1U);
			var yOutOffset789 = (uint)targetSize.X * (baseYOffset + 2U);

			for (uint x = 0; x < (uint)sourceSize.X; ++x) {
				Offset<XGetter> xOffsets = new(x, Configuration.Wrapped.X, sourceSize);

				var a = GetPixel(source, yOffsets[-1], xOffsets[-1]);
				var b = GetPixel(source, yOffsets[-1], xOffsets[ 0]);
				var c = GetPixel(source, yOffsets[-1], xOffsets[ 1]);
				var d = GetPixel(source, yOffsets[ 0], xOffsets[-1]);
				var e = GetPixel(source, yOffsets[ 0], xOffsets[ 0]);
				var f = GetPixel(source, yOffsets[ 0], xOffsets[ 1]);
				var g = GetPixel(source, yOffsets[ 1], xOffsets[-1]);
				var h = GetPixel(source, yOffsets[ 1], xOffsets[ 0]);
				var i = GetPixel(source, yOffsets[ 1], xOffsets[ 1]);

				var o1 = e;
				var o2 = e;
				var o3 = e;
				var o4 = e;
				var o5 = e;
				var o6 = e;
				var o7 = e;
				var o8 = e;
				var o9 = e;

				bool bd = ColorEq(b, d);
				bool dh = ColorEq(d, h);
				bool bf = ColorEq(b, f);
				// IF D==B AND D!=H AND B!=F => 1=D
				if (bd && !dh && !bf) {
					o1 = d;
				}

				bool ce = ColorEq(c, e);
				bool fh = ColorEq(f, h);
				bool ae = ColorEq(a, e);
				// IF (D==B AND D!=H AND B!=F AND E!=C) OR (B==F AND B!=D AND F!=H AND E!=A) => 2=B
				if ((bd && !dh && !bf && !ce) || (bf && !bd && !fh && !ae)) {
					o2 = b;
				}
				// IF B==F AND B!=D AND F!=H => 3=F
				if (bf && !bd && !fh) {
					o3 = f;
				}

				bool eg = ColorEq(e, g);
				// IF (H==D AND H!=F AND D!=B AND E!=A) OR (D==B AND D!=H AND B!=F AND E!=G) => 4=D
				if ((dh && !fh && !bd && !ae) || (bd && !dh && !bf && !eg)) {
					o4 = d;
				}

				bool ei = ColorEq(e, i);
				// IF (B==F AND B!=D AND F!=H AND E!=I) OR (F==H AND F!=B AND H!=D AND E!=C) => 6=F
				if ((bf && !bd && !fh && !ei) || (fh && !bf && !dh && !ce)) {
					o6 = f;
				}
				// IF H==D AND H!=F AND D!=B => 7=D
				if (dh && !fh && !bd) {
					o7 = d;
				}
				// IF (F==H AND F!=B AND H!=D AND E!=G) OR (H==D AND H!=F AND D!=B AND E!=I) => 8=H
				if ((fh && !bf && !dh && !eg) || (dh && !fh && !bd && !ei)) {
					o8 = h;
				}
				// IF F==H AND F!=B AND H!=D => 9=F
				if (fh && !bf && !dh) {
					o9 = f;
				}

				// The X offset where we are outputting in the target
				var baseXOffset = x * 3U;
				var xOutOffset147 = (baseXOffset);
				var xOutOffset258 = (baseXOffset) + 1U;
				var xOutOffset369 = (baseXOffset) + 2U;

				SetPixel(destination, yOutOffset123, xOutOffset147, o1);
				SetPixel(destination, yOutOffset123, xOutOffset258, o2);
				SetPixel(destination, yOutOffset123, xOutOffset369, o3);

				SetPixel(destination, yOutOffset456, xOutOffset147, o4);
				SetPixel(destination, yOutOffset456, xOutOffset258, o5);
				SetPixel(destination, yOutOffset456, xOutOffset369, o6);

				SetPixel(destination, yOutOffset789, xOutOffset147, o7);
				SetPixel(destination, yOutOffset789, xOutOffset258, o8);
				SetPixel(destination, yOutOffset789, xOutOffset369, o9);
			}
		}
	}
}
