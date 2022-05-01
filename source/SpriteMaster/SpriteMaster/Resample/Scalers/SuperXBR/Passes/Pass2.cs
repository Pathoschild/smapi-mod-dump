/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if !SHIPPING
using SpriteMaster.Resample.Scalers.SuperXBR.Cg;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

using static SpriteMaster.Resample.Scalers.SuperXBR.Cg.CgMath;

namespace SpriteMaster.Resample.Scalers.SuperXBR.Passes;

sealed class Pass2 : Pass {
	internal Pass2(Config config, Vector2I sourceSize, Vector2I targetSize) : base(config, sourceSize, targetSize) { }

	private float Weight1 => Configuration.Weight * 1.29633f / 10.0f;
	private float Weight2 => Configuration.Weight * 1.75068f / 10.0f / 2.0f;

	/*
                              P1
     |P0|B |C |P1|         C     F4          |a0|b1|c2|d3|
     |D |E |F |F4|      B     F     I4       |b0|c1|d2|e3|   |e1|i1|i2|e2|
     |G |H |I |I4|   P0    E  A  I     P3    |c0|d1|e2|f3|   |e3|i3|i4|e4|
     |P2|H5|I5|P3|      D     H     I5       |d0|e1|f2|g3|
                           G     H5
                              P2
	*/

	private static readonly float[] _PixelWeights = new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
	protected override float[] PixelWeights => _PixelWeights;

	//           X   Y   Z   W
	// VAR.t1 = -2, -2,  1,  1
	// VAR.t2 = -1, -2,  0,  1
	// VAR.t3 = -2, -1,  1,  0
	// VAR.t4 = -1, -1,  0,  0

	private static readonly Float4[] UV = new Float4[] {
		(-2.0f, -2.0f, 1.0f, 1.0f),
		(-1.0f, -2.0f, 0.0f, 1.0f),
		(-2.0f, -1.0f, 1.0f, 0.0f),
		(-1.0f, -1.0f, 0.0f, 0.0f)
	};

	// Pass-0 does not resize - it is a prefiltering pass
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Pass(ReadOnlySpan<Float4> sourceData, Span<Float4> target) {
		var source = new Texture(this, sourceData, SourceSize);

		for (int y = 0; y < TargetSize.Height; ++y) {
			int yOffset = GetY(y, TargetSize) * TargetSize.Width;
			for (int x = 0; x < TargetSize.Width; ++x) {
				Vector2I texCoord = (x, y);
				int targetOffset = yOffset + GetX(x, TargetSize);

				var P0 = source.Sample(texCoord + UV[0].XY.AsInt); // xy
				var P1 = source.Sample(texCoord + UV[0].ZY.AsInt); // zy
				var P2 = source.Sample(texCoord + UV[0].XW.AsInt); // xw
				var P3 = source.Sample(texCoord + UV[0].ZW.AsInt); // zw

				var B =  source.Sample(texCoord + UV[1].XY.AsInt); // xy
				var C =  source.Sample(texCoord + UV[1].ZY.AsInt); // zy
				var H5 = source.Sample(texCoord + UV[1].XW.AsInt); // xw
				var I5 = source.Sample(texCoord + UV[1].ZW.AsInt); // zw

				var D =  source.Sample(texCoord + UV[2].XY.AsInt); // xy
				var F4 = source.Sample(texCoord + UV[2].ZY.AsInt); // zy
				var G =  source.Sample(texCoord + UV[2].XW.AsInt); // xw
				var I4 = source.Sample(texCoord + UV[2].ZW.AsInt); // zw

				var E =  source.Sample(texCoord + UV[3].XY.AsInt); // xy
				var F =  source.Sample(texCoord + UV[3].ZY.AsInt); // zy
				var H =  source.Sample(texCoord + UV[3].XW.AsInt); // xw
				var I =  source.Sample(texCoord + UV[3].ZW.AsInt); // zw

				var b = CgMath.ToDiffTexel(B);
				var c = CgMath.ToDiffTexel(C);
				var d = CgMath.ToDiffTexel(D);
				var e = CgMath.ToDiffTexel(E);
				var f = CgMath.ToDiffTexel(F);
				var g = CgMath.ToDiffTexel(G);
				var h = CgMath.ToDiffTexel(H);
				var i = CgMath.ToDiffTexel(I);

				var i4 = CgMath.ToDiffTexel(I4); var p0 = CgMath.ToDiffTexel(P0);
				var i5 = CgMath.ToDiffTexel(I5); var p1 = CgMath.ToDiffTexel(P1);
				var h5 = CgMath.ToDiffTexel(H5); var p2 = CgMath.ToDiffTexel(P2);
				var f4 = CgMath.ToDiffTexel(F4); var p3 = CgMath.ToDiffTexel(P3);

				// Calc edgeness in diagonal directions.
				float dEdge =
					WeightedDifferenceDiagonal(d, b, g, e, c, p2, h, f, p1, h5, i, f4, i5, i4) -
					WeightedDifferenceDiagonal(c, f4, b, f, i4, p0, e, i, p3, d, h, i5, g, h5);

				// Calc edgeness in horizontal/vertical directions.
				float hvEdge =
					WeightedDifferenceHorizontalVertical(f, i, e, h, c, i5, b, h5) -
					WeightedDifferenceHorizontalVertical(e, f, h, i, d, f4, g, i4);

				float limits = Configuration.EdgeStrength + 0.000001f;
				float edgeStrength = SmoothStep(0.0f, limits, MathF.Abs(dEdge));

				// Filter weights. Two taps only.
				Float4 w1 = (-Weight1, Weight1 + 0.5f, Weight1 + 0.5f, -Weight1);
				Float4 w2 = (-Weight2, Weight2 + 0.25f, Weight2 + 0.25f, -Weight2);

				// Filtering and normalization in four direction generating four colors.
				Float4 c1 = CgMath.MatrixMul(w1, P2, H, F, P1);
				Float4 c2 = CgMath.MatrixMul(w1, P0, E, I, P3);
				Float4 c3 = CgMath.MatrixMul(w2, D + G, E + H, F + I, F4 + I4);
				Float4 c4 = CgMath.MatrixMul(w2, C + B, F + E, I + H, I5 + H5);

				// Smoothly blends the two strongest directions (one in diagonal and the other in vert/horiz direction).
				Float4 color = Lerp(
					Lerp(c1, c2, Step(0.0f, dEdge)),
					Lerp(c3, c4, Step(0.0f, hvEdge)),
					1.0f - edgeStrength
				);

				// Anti-ringing code.
				Float4 minSample =
					Min4(E, F, H, I) +
					(1.0f - Configuration.AntiRinging) *
					Lerp(
						(P2 - H) * (F - P1),
						(P0 - E) * (I - P3),
						Step(0.0f, dEdge)
					);
				Float4 maxSample =
					Max4(E, F, H, I) -
					(1.0f - Configuration.AntiRinging) *
					Lerp(
						(P2 - H) * (F - P1),
						(P0 - E) * (I - P3),
						Step(0.0f, dEdge)
					);
				color = color.Clamp(minSample, maxSample);

				target[targetOffset] = color;
			}
		}
	}
}
#endif
