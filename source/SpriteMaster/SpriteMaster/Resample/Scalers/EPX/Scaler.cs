/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using static SpriteMaster.Colors.ColorHelpers;

namespace SpriteMaster.Resample.Scalers.EPX;

sealed partial class Scaler {
	private const uint MinScale = 3;
	private const uint MaxScale = Config.MaxScale;

	private static uint ClampScale(uint scale) => Math.Clamp(scale, MinScale, MaxScale);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static Span<Color16> Apply(
		Config? config,
		uint scaleMultiplier,
		ReadOnlySpan<Color16> sourceData,
		Vector2I sourceSize,
		Span<Color16> targetData,
		Vector2I targetSize
	) {
		if (config is null) {
			throw new ArgumentNullException(nameof(config));
		}

		if (sourceSize.X * sourceSize.Y > sourceData.Length) {
			throw new ArgumentOutOfRangeException(nameof(sourceData));
		}

		var targetSizeCalculated = sourceSize * scaleMultiplier;
		if (targetSize != targetSizeCalculated) {
			throw new ArgumentOutOfRangeException(nameof(targetSize));
		}

		if (targetData == Span<Color16>.Empty) {
			targetData = SpanExt.MakeUninitialized<Color16>(targetSize.Area);
		}
		else {
			if (targetSize.Area > targetData.Length) {
				throw new ArgumentOutOfRangeException(nameof(targetData));
			}
		}

		var scalerInstance = new Scaler(
			configuration: in config,
			scaleMultiplier: scaleMultiplier,
			sourceSize: sourceSize,
			targetSize: targetSize
		);

		scalerInstance.Scale(sourceData, targetData);
		return targetData;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private Scaler(
		in Config configuration,
		uint scaleMultiplier,
		Vector2I sourceSize,
		Vector2I targetSize
	) {
		if (scaleMultiplier < MinScale || scaleMultiplier > MaxScale) {
			throw new ArgumentOutOfRangeException(nameof(scaleMultiplier));
		}
		/*
		if (sourceData is null) {
			throw new ArgumentNullException(nameof(sourceData));
		}
		if (targetData is null) {
			throw new ArgumentNullException(nameof(targetData));
		}
		*/
		if (sourceSize.X <= 0 || sourceSize.Y <= 0) {
			throw new ArgumentOutOfRangeException(nameof(sourceSize));
		}

		ScaleMultiplier = scaleMultiplier;
		Configuration = configuration;
		SourceSize = sourceSize;
		TargetSize = targetSize;
		YccConfiguration = new() {
			LuminanceWeight = Configuration.LuminanceWeight,
			ChrominanceWeight = Configuration.ChrominanceWeight
		};
	}

	private readonly uint ScaleMultiplier;
	private readonly Config Configuration;
	private readonly YccConfig YccConfiguration;

	private readonly Vector2I SourceSize;
	private readonly Vector2I TargetSize;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private int GetX(int x) {
		if (Configuration.Wrapped.X) {
			x = (x + SourceSize.Width) % SourceSize.Width;
		}
		else {
			x = Math.Clamp(x, 0, SourceSize.Width - 1);
		}
		return x;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private int GetY(int y) {
		if (Configuration.Wrapped.Y) {
			y = (y + SourceSize.Height) % SourceSize.Height;
		}
		else {
			y = Math.Clamp(y, 0, SourceSize.Height - 1);
		}
		return y;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static Color16 GetPixel(ReadOnlySpan<Color16> src, int stride, int offset) {
		// We can try embedded a distance calculation as well. Perhaps instead of a negative stride/offset, we provide a 
		// negative distance from the edge and just recalculate the stride/offset in that case.
		// We can scale the alpha reduction by the distance to hopefully correct the edges.

		// Alternatively, for non-wrapping textures (or for wrapping ones that only have one wrapped axis) we embed them in a new target
		// which is padded with alpha, and after resampling we manually clamp the colors on it. This will give a normal-ish effect for drawing, and will make it so we might get a more correct edge since it can overdraw.
		// If we do this, we draw the entire texture, with the padding, but we slightly enlarge the target area for _drawing_ to account for the extra padding.
		// This will effectively cause a filtering effect and hopefully prevent the hard edge problems

		if (stride < 0) {
			Debug.Warning($"EPX GetPixel out of range: stride: {stride}, value clamped");
			stride = Math.Max(0, stride);
		}

		if (offset < 0) {
			Debug.Warning($"EPX GetPixel out of range: offset: {offset}, value clamped");
			offset = Math.Max(0, offset);
		}

		return src[stride + offset];
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static void SetPixel(Span<Color16> dst, int stride, int offset, in Color16 color) {
		// We can try embedded a distance calculation as well. Perhaps instead of a negative stride/offset, we provide a 
		// negative distance from the edge and just recalculate the stride/offset in that case.
		// We can scale the alpha reduction by the distance to hopefully correct the edges.

		// Alternatively, for non-wrapping textures (or for wrapping ones that only have one wrapped axis) we embed them in a new target
		// which is padded with alpha, and after resampling we manually clamp the colors on it. This will give a normal-ish effect for drawing, and will make it so we might get a more correct edge since it can overdraw.
		// If we do this, we draw the entire texture, with the padding, but we slightly enlarge the target area for _drawing_ to account for the extra padding.
		// This will effectively cause a filtering effect and hopefully prevent the hard edge problems

		if (stride < 0) {
			return;
		}

		if (offset < 0) {
			return;
		}

		dst[stride + offset] = color;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	bool ColorEq(in Color16 a, in Color16 b) {
		return a == b;
		/*
		return Resample.Scalers.Common.ColorDistance(
			useRedmean: Configuration.UseRedmean,
			gammaCorrected: Configuration.GammaCorrected,
			hasAlpha: Configuration.HasAlpha,
			pix1: a,
			pix2: b,
			yccConfig: YccConfiguration
		) < Configuration.EqualColorTolerance;
		*/
	}

	// https://en.wikipedia.org/wiki/Pixel-art_scaling_algorithms#EPX/Scale2%C3%97/AdvMAME2%C3%97
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void Scale(ReadOnlySpan<Color16> source, Span<Color16> destination) {
		switch (ScaleMultiplier) {
			case 2:
				Scale2(source, destination);
				break;
			case 3:
				Scale3(source, destination);
				break;
			default:
				throw new NotImplementedException($"EPX Scale '{ScaleMultiplier}' unimplemented");
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void Scale2(ReadOnlySpan<Color16> source, Span<Color16> destination) {
		var last = SourceSize;

		if (last.Y <= 0 || last.X <= 0) {
			return;
		}

		for (int y = 0; y < last.Y; ++y) {
			var yM1 = SourceSize.X * GetY(y - 1);
			var y0 = SourceSize.X * GetY(y);
			var yP1 = SourceSize.X * GetY(y + 1);

			var yo12 = TargetSize.X * (y * 2);
			var yo34 = TargetSize.X * ((y * 2) + 1);

			for (int x = 0; x < last.X; ++x) {
				var xM1 = GetX(x - 1);
				var x0 = GetX(x);
				var xP1 = GetX(x + 1);

				var A = GetPixel(source, yM1, x0);
				var C = GetPixel(source, y0, xM1);
				var P = GetPixel(source, y0, x0);
				var B = GetPixel(source, y0, xP1);
				var D = GetPixel(source, yP1, x0);

				var o1 = P;
				var o2 = P;
				var o3 = P;
				var o4 = P;

				bool AC = ColorEq(A, C);
				bool CD = ColorEq(C, D);
				bool AB = ColorEq(A, B);
				if (AC && !CD && !AB) {
					o1 = A;
				}
				bool BD = ColorEq(B, D);
				if (AB && !AC && !BD) {
					o2 = B;
				}
				if (CD && !BD && !AC) {
					o3 = C;
				}
				if (BD && !AB && !CD) {
					o4 = D;
				}

				var x13 = x * 2;
				var x24 = x * 2 + 1;

				SetPixel(destination, yo12, x13, in o1);
				SetPixel(destination, yo12, x24, in o2);
				SetPixel(destination, yo34, x13, in o3);
				SetPixel(destination, yo34, x24, in o4);
			}
		}
	}



	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void Scale3(ReadOnlySpan<Color16> source, Span<Color16> destination) {
		var last = SourceSize;

		if (last.Y <= 0 || last.X <= 0) {
			return;
		}

		for (int y = 0; y < last.Y; ++y) {
			var yM1 = SourceSize.X * GetY(y - 1);
			var y0 =  SourceSize.X * GetY(y);
			var yP1 = SourceSize.X * GetY(y + 1);

			var yo123 = TargetSize.X * (y * 3);
			var yo456 = TargetSize.X * ((y * 3) + 1);
			var yo789 = TargetSize.X * ((y * 3) + 2);

			for (int x = 0; x < last.X; ++x) {
				var xM1 = GetX(x - 1);
				var x0 =  GetX(x);
				var xP1 = GetX(x + 1);

				var A = GetPixel(source, yM1, xM1);
				var B = GetPixel(source, yM1, x0 );
				var C = GetPixel(source, yM1, xP1);
				var D = GetPixel(source, y0 , xM1);
				var E = GetPixel(source, y0 , x0 );
				var F = GetPixel(source, y0 , xP1);
				var G = GetPixel(source, yP1, xM1);
				var H = GetPixel(source, yP1, x0 );
				var I = GetPixel(source, yP1, xP1);

				var o1 = E;
				var o2 = E;
				var o3 = E;
				var o4 = E;
				var o5 = E;
				var o6 = E;
				var o7 = E;
				var o8 = E;
				var o9 = E;

				bool BD = ColorEq(B, D);
				bool DH = ColorEq(D, H);
				bool BF = ColorEq(B, F);
				// IF D==B AND D!=H AND B!=F => 1=D
				if (BD && !DH && !BF) {
					o1 = D;
				}

				bool CE = ColorEq(C, E);
				bool FH = ColorEq(F, H);
				bool AE = ColorEq(A, E);
				// IF (D==B AND D!=H AND B!=F AND E!=C) OR (B==F AND B!=D AND F!=H AND E!=A) => 2=B
				if ((BD && !DH && !BF && !CE) || (BF && !BD && !FH && !AE)) {
					o2 = B;
				}
				// IF B==F AND B!=D AND F!=H => 3=F
				if (BF && !BD && !FH) {
					o3 = F;
				}

				bool EG = ColorEq(E, G);
				// IF (H==D AND H!=F AND D!=B AND E!=A) OR (D==B AND D!=H AND B!=F AND E!=G) => 4=D
				if ((DH && !FH && !BD && !AE) || (BD && !DH && !BF && !EG)) {
					o4 = D;
				}

				bool EI = ColorEq(E, I);
				// IF (B==F AND B!=D AND F!=H AND E!=I) OR (F==H AND F!=B AND H!=D AND E!=C) => 6=F
				if ((BF && !BD && !FH && !EI) || (FH && !BF && !DH && !CE)) {
					o6 = F;
				}
				// IF H==D AND H!=F AND D!=B => 7=D
				if (DH && !FH && !BD) {
					o7 = D;
				}
				// IF (F==H AND F!=B AND H!=D AND E!=G) OR (H==D AND H!=F AND D!=B AND E!=I) => 8=H
				if ((FH && !BF && !DH && !EG) || (DH && !FH && !BD && !EI)) {
					o8 = H;
				}
				// IF F==H AND F!=B AND H!=D => 9=F
				if (FH && !BF && !DH) {
					o9 = F;
				}

				var x147 = x * 3;
				var x258 = x * 3 + 1;
				var x369 = x * 3 + 2;

				SetPixel(destination, yo123, x147, in o1);
				SetPixel(destination, yo123, x258, in o2);
				SetPixel(destination, yo123, x369, in o3);

				SetPixel(destination, yo456, x147, in o4);
				SetPixel(destination, yo456, x258, in o5);
				SetPixel(destination, yo456, x369, in o6);

				SetPixel(destination, yo789, x147, in o7);
				SetPixel(destination, yo789, x258, in o8);
				SetPixel(destination, yo789, x369, in o9);
			}
		}
	}
}
