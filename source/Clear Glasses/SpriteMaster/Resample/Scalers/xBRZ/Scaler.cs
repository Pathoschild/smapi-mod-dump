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
using SpriteMaster.Resample.Scalers.xBRZ.Blend;
using SpriteMaster.Resample.Scalers.xBRZ.Color;
using SpriteMaster.Resample.Scalers.xBRZ.Common;
using SpriteMaster.Resample.Scalers.xBRZ.Scalers;
using SpriteMaster.Resample.Scalers.xBRZ.Structures;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.xBRZ;

using PreprocessType = Byte;

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
		Resample.Scalers.Common.ApplyValidate(config, scaleMultiplier, sourceData, sourceSize, ref targetData, targetSize);

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
		Scalerer = scaleMultiplier.ToIScaler(configuration);
		Comparer = new(Configuration);
	}

	private readonly AbstractScaler Scalerer;

	private readonly ColorComparer Comparer;

	//fill block with the given color
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void FillBlock(Span<Color16> target, int targetOffset, int pitch, Color16 color, int blockSize) {
		for (
			var y = 0;
			y < blockSize;
			++y, targetOffset += pitch
		) {
			target.Slice(targetOffset, blockSize).Fill(color);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static int SharpenOffset(int offset, int max) {
		var offsetD = (double)offset / (double)max;
		offsetD = offsetD switch {
			< 0.5 => Math.Pow(offsetD, 5.0),
			> 0.5 => Math.Pow(offsetD, 1.0 / 5.0),
			_ => offsetD
		};
		return (offsetD * max).RoundToInt();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static (int R, int G, int B, int A) BlendColor(int offset, int max, Color16 a, Color16 b) {
		offset = SharpenOffset(offset, max);
		var aCount = max - offset;
		(int r, int g, int b, int a) color32 = (
			(a.R.Value * aCount + b.R.Value * offset),
			(a.G.Value * aCount + b.G.Value * offset),
			(a.B.Value * aCount + b.B.Value * offset),
			(a.A.Value * aCount + b.A.Value * offset)
		);
		return color32;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static (int R, int G, int B, int A) BlendColor(int offset, int max, (int R, int G, int B, int A) a, (int R, int G, int B, int A) b) {
		offset = SharpenOffset(offset, max);
		int aCount = max - offset;
		(int r, int g, int b, int a) color32 = (
			(a.R * aCount + b.R * offset),
			(a.G * aCount + b.G * offset),
			(a.B * aCount + b.B * offset),
			(a.A * aCount + b.A * offset)
		);
		return color32;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void BlendBlock(Span<Color16> target, int targetOffset, int pitch, Color16 topLeft, Color16 topRight, Color16 bottomLeft, Color16 bottomRight, int blockSize) {
		if (topLeft == topRight && topLeft == bottomLeft && topLeft == bottomRight) {
			FillBlock(target, targetOffset, pitch, topLeft, blockSize);
			return;
		}

		int blockSizeSq = (blockSize + 1) * (blockSize + 1);

		for (
			var y = 0;
			y < blockSize;
			++y, targetOffset += pitch
		) {
			var colorLeft32 = BlendColor(y, blockSize + 1, topLeft, bottomLeft);
			var colorRight32 = BlendColor(y, blockSize + 1, topRight, bottomRight);
			for (int x = 0; x < blockSize; ++x) {
				var color32 = BlendColor(x, blockSize + 1, colorLeft32, colorRight32);
				color32.R /= blockSizeSq;
				color32.G /= blockSizeSq;
				color32.B /= blockSizeSq;
				color32.A /= blockSizeSq;

				target.Slice(targetOffset, blockSize)[x] = new((ushort)color32.R, (ushort)color32.G, (ushort)color32.B, (ushort)color32.A);
			}
		}
	}

	//detect blend direction
	[Pure]
	private BlendResult PreProcessCorners(in Kernel4X4 ker) {
		BlendResult result = default;

		if (ker.F == ker.G && ker.J == ker.K || ker.F == ker.J && ker.G == ker.K) {
			return result;
		}

		var comparer = Comparer;

		var jg =
			comparer.ColorDistance(ker.I, ker.F) +
			comparer.ColorDistance(ker.F, ker.C) +
			comparer.ColorDistance(ker.N, ker.K) +
			comparer.ColorDistance(ker.K, ker.H) +
			(Configuration.CenterDirectionBias * comparer.ColorDistance(ker.J, ker.G)).RoundToInt();
		var fk =
			comparer.ColorDistance(ker.E, ker.J) +
			comparer.ColorDistance(ker.J, ker.O) +
			comparer.ColorDistance(ker.B, ker.G) +
			comparer.ColorDistance(ker.G, ker.L) +
			(Configuration.CenterDirectionBias * comparer.ColorDistance(ker.F, ker.K)).RoundToInt();

		if (jg < fk) {
			var dominantGradient = (Configuration.DominantDirectionThreshold * jg).RoundToInt() < fk ? BlendType.Dominant : BlendType.Normal;
			if (ker.F != ker.G && ker.F != ker.J) {
				result.F = dominantGradient;
			}
			if (ker.K != ker.J && ker.K != ker.G) {
				result.K = dominantGradient;
			}
		}
		else if (fk < jg) {
			var dominantGradient = (Configuration.DominantDirectionThreshold * fk).RoundToInt() < jg ? BlendType.Dominant : BlendType.Normal;
			if (ker.J != ker.F && ker.J != ker.K) {
				result.J = dominantGradient;
			}
			if (ker.G != ker.F && ker.G != ker.K) {
				result.G = dominantGradient;
			}
		}

		/*
		const uint testComp = 16_386u;

		var fb = comparer.ColorDistance(ker.F, ker.B);
		var fe = comparer.ColorDistance(ker.F, ker.E);
		var fj = comparer.ColorDistance(ker.F, ker.J);
		var fg = comparer.ColorDistance(ker.F, ker.G);
		*/

		/*
		 * 			---------
						| F | G | //evaluate corner between F, G, J, K
						----|---| //current input pixel is at position F
						| J | K |
						---------
		 */

		/*
		uint testEdges = 0;
		if (fb < testComp && fe < testComp) {
			if (comparer.ColorDistance(ker.F, ker.A) >= testComp) {
				result.F = BlendType.None;
			}
		}

		if (fb < testComp && fg < testComp) {
			if (comparer.ColorDistance(ker.F, ker.C) >= testComp) {
				result.G = BlendType.None;
			}
		}
		if (fj < testComp && fe < testComp) {
			if (comparer.ColorDistance(ker.F, ker.I) >= testComp) {
				result.J = BlendType.None;
			}
		}
		if (fj < testComp && fg < testComp) {
			if (comparer.ColorDistance(ker.F, ker.K) >= testComp) {
				result.K = BlendType.None;
			}
		}*/

		/*
		var kernel = new Color16[4,4] {
			{
				ker.A, ker.B, ker.C, ker.D
			},
			{
				ker.E, ker.F, ker.G, ker.H
			},
			{
				ker.I, ker.J, ker.K, ker.L
			},
			{
				ker.M, ker.N, ker.O, ker.P
			}
		};

		unsafe bool HasSolidEdge(int x, int y) {
			var reference = kernel[x, y];
			var comps = stackalloc uint[8] {
				comparer.ColorDistance(reference, kernel[x - 1, y - 1]),
				comparer.ColorDistance(reference, kernel[x    , y - 1]),
				comparer.ColorDistance(reference, kernel[x + 1, y - 1]),

				comparer.ColorDistance(reference, kernel[x - 1, y    ]),
				comparer.ColorDistance(reference, kernel[x + 1, y    ]),

				comparer.ColorDistance(reference, kernel[x - 1, y + 1]),
				comparer.ColorDistance(reference, kernel[x    , y + 1]),
				comparer.ColorDistance(reference, kernel[x + 1, y + 1])
			};
		}
		*/

		return result;
	}

	/*
			input kernel area naming convention:
			-------------
			| A | B | C |
			----|---|---|
			| D | E | F | //input pixel is at position E
			----|---|---|
			| G | H | I |
			-------------
			blendInfo: result of preprocessing all four corners of pixel "e"
	*/
	private void ScalePixel(AbstractScaler abstractScaler, RotationDegree rotDeg, in Kernel3X3 ker, ref OutputMatrix outputMatrix, int targetIndex, PreprocessType blendInfo) {
		var blend = blendInfo.Rotate(rotDeg);

		if (blend.GetBottomR() == BlendType.None) {
			return;
		}

		// int a = ker._[Rot._[(0 << 2) + rotDeg]];
		var b = ker[Rotator.Get((1 << 2) + (int)rotDeg)];
		var c = ker[Rotator.Get((2 << 2) + (int)rotDeg)];
		var d = ker[Rotator.Get((3 << 2) + (int)rotDeg)];
		var e = ker[Rotator.Get((4 << 2) + (int)rotDeg)];
		var f = ker[Rotator.Get((5 << 2) + (int)rotDeg)];
		var g = ker[Rotator.Get((6 << 2) + (int)rotDeg)];
		var h = ker[Rotator.Get((7 << 2) + (int)rotDeg)];
		var i = ker[Rotator.Get((8 << 2) + (int)rotDeg)];

		var comparer = Comparer;

		bool doLineBlend;

		if (blend.GetBottomR() >= BlendType.Dominant) {
			doLineBlend = true;
		}
		//make sure there is no second blending in an adjacent
		//rotation for this pixel: handles insular pixels, mario eyes
		//but support double-blending for 90° corners
		else if (blend.GetTopR() != BlendType.None && !comparer.IsColorEqual(e, g)) {
			doLineBlend = false;
		}
		else if (blend.GetBottomL() != BlendType.None && !comparer.IsColorEqual(e, c)) {
			doLineBlend = false;
		}
		//no full blending for L-shapes; blend corner only (handles "mario mushroom eyes")
		else if (comparer.IsColorEqual(g, h) && comparer.IsColorEqual(h, i) && comparer.IsColorEqual(i, f) && comparer.IsColorEqual(f, c) && !comparer.IsColorEqual(e, i)) {
			doLineBlend = false;
		}
		else {
			doLineBlend = true;
		}

		//choose most similar color
		var px = comparer.ColorDistance(e, f) <= comparer.ColorDistance(e, h) ? f : h;

		outputMatrix.Move(rotDeg, targetIndex);

		if (!doLineBlend) {
			abstractScaler.BlendCorner(px, ref outputMatrix);
			return;
		}

		//test sample: 70% of values max(fg, hc) / min(fg, hc)
		//are between 1.1 and 3.7 with median being 1.9
		var fg = comparer.ColorDistance(f, g);
		var hc = comparer.ColorDistance(h, c);

		var haveShallowLine = (Configuration.SteepDirectionThreshold * fg).RoundToInt() <= hc && e != g && d != g;
		var haveSteepLine = (Configuration.SteepDirectionThreshold * hc).RoundToInt() <= fg && e != c && b != c;

		if (haveShallowLine) {
			if (haveSteepLine) {
				abstractScaler.BlendLineSteepAndShallow(px, ref outputMatrix);
			}
			else {
				abstractScaler.BlendLineShallow(px, ref outputMatrix);
			}
		}
		else {
			if (haveSteepLine) {
				abstractScaler.BlendLineSteep(px, ref outputMatrix);
			}
			else {
				abstractScaler.BlendLineDiagonal(px, ref outputMatrix);
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private int GetX(int x) {
		if (Configuration.Wrapped.X) {
			x = (x + SourceSize.Width) % SourceSize.Width;
		}
		else {
			x = Math.Clamp(x, 0, SourceSize.Width - 1);
		}
		return x;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private int GetY(int y) {
		if (Configuration.Wrapped.Y) {
			y = (y + SourceSize.Height) % SourceSize.Height;
		}
		else {
			y = Math.Clamp(y, 0, SourceSize.Height - 1);
		}
		return y;
	}

	//scaler policy: see "Scaler2x" reference implementation
	private void Scale(ReadOnlySpan<Color16> source, Span<Color16> destination) {
		int targetStride = TargetSize.Width * Scalerer.Scale;
		int yLast = SourceSize.Height;

		if (0 >= yLast) {
			return;
		}

		//temporary buffer for "on the fly preprocessing"
		Span<PreprocessType> preProcBuffer = stackalloc PreprocessType[SourceSize.Width];
		preProcBuffer.Fill(0);

		[MethodImpl(Runtime.MethodImpl.Inline)]
		static Color16 GetPixel(ReadOnlySpan<Color16> src, int stride, int offset) {
			// We can try embedded a distance calculation as well. Perhaps instead of a negative stride/offset, we provide a 
			// negative distance from the edge and just recalculate the stride/offset in that case.
			// We can scale the alpha reduction by the distance to hopefully correct the edges.

			// Alternatively, for non-wrapping textures (or for wrapping ones that only have one wrapped axis) we embed them in a new target
			// which is padded with alpha, and after resampling we manually clamp the colors on it. This will give a normal-ish effect for drawing, and will make it so we might get a more correct edge since it can overdraw.
			// If we do this, we draw the entire texture, with the padding, but we slightly enlarge the target area for _drawing_ to account for the extra padding.
			// This will effectively cause a filtering effect and hopefully prevent the hard edge problems

			if (stride < 0) {
				Debug.Warning($"xBRZ GetPixel out of range: stride: {stride}, value clamped");
				stride = 0;
			}

			if (offset < 0) {
				Debug.Warning($"xBRZ GetPixel out of range: offset: {offset}, value clamped");
				offset = 0;
			}

			return src[stride + offset];
		}

		//initialize preprocessing buffer for first row:
		//detect upper left and right corner blending
		//this cannot be optimized for adjacent processing
		//stripes; we must not allow for a memory race condition!
		/*if (yFirst > 0)*/
		{
			var y = -1;

			var sM1 = SourceSize.X * GetY(y - 1);
			var s0 = SourceSize.X * GetY(y); //center line
			var sP1 = SourceSize.X * GetY(y + 1);
			var sP2 = SourceSize.X * GetY(y + 2);

			for (var x = 0; x < SourceSize.Width; ++x) {
				var xM1 = GetX(x - 1);
				var xP1 = GetX(x + 1);
				var xP2 = GetX(x + 2);

				//read sequentially from memory as far as possible
				var ker4 = new Kernel4X4(
					GetPixel(source, sM1, xM1),
					GetPixel(source, sM1, x),
					GetPixel(source, sM1, xP1),
					GetPixel(source, sM1, xP2),

					GetPixel(source, s0, xM1),
					GetPixel(source, s0, x),
					GetPixel(source, s0, xP1),
					GetPixel(source, s0, xP2),

					GetPixel(source, sP1, xM1),
					GetPixel(source, sP1, x),
					GetPixel(source, sP1, xP1),
					GetPixel(source, sP1, xP2),

					GetPixel(source, sP2, xM1),
					GetPixel(source, sP2, x),
					GetPixel(source, sP2, xP1),
					GetPixel(source, sP2, xP2)
				);

				var blendResult = PreProcessCorners(in ker4); // writes to blendResult
				/*
				preprocessing blend result:
				---------
				| F | G | //evalute corner between F, G, J, K
				----|---| //input pixel is at position F
				| J | K |
				---------
				*/

				preProcBuffer[x].SetTopR(blendResult.J);

				int preProcIndex;
				if (x + 1 < SourceSize.Width) {
					preProcIndex = x + 1;
				}
				else if (Configuration.Wrapped.X) {
					preProcIndex = 0;
				}
				else {
					preProcIndex = x;
				}
				preProcBuffer[preProcIndex].SetTopL(blendResult.K);
			}
		}

		var outputMatrix = new OutputMatrix(Scalerer.Scale, destination, TargetSize.Width);

		for (var y = 0; y < yLast; ++y) {
			//consider MT "striped" access
			var targetIndex = y * targetStride;

			var sM1 = SourceSize.X * GetY(y - 1);
			var s0 = SourceSize.X * y; //center line
			var sP1 = SourceSize.X * GetY(y + 1);
			var sP2 = SourceSize.X * GetY(y + 2);

			PreprocessType blendXY1 = 0;

			for (var x = 0; x < SourceSize.Width; ++x, targetIndex += Scalerer.Scale) {
				var xM1 = GetX(x - 1);
				var xP1 = GetX(x + 1);
				var xP2 = GetX(x + 2);

				//evaluate the four corners on bottom-right of current pixel
				//blend_xy for current (x, y) position

				//read sequentially from memory as far as possible
				var ker4 = new Kernel4X4(
					GetPixel(source, sM1, xM1),
					GetPixel(source, sM1, x),
					GetPixel(source, sM1, xP1),
					GetPixel(source, sM1, xP2),

					GetPixel(source, s0, xM1),
					source[s0 + x],
					GetPixel(source, s0, xP1),
					GetPixel(source, s0, xP2),

					GetPixel(source, sP1, xM1),
					GetPixel(source, sP1, x),
					GetPixel(source, sP1, xP1),
					GetPixel(source, sP1, xP2),

					GetPixel(source, sP2, xM1),
					GetPixel(source, sP2, x),
					GetPixel(source, sP2, xP1),
					GetPixel(source, sP2, xP2)
				);

				var blendResult = PreProcessCorners(in ker4); // writes to blendResult

				/*
						preprocessing blend result:
						---------
						| F | G | //evaluate corner between F, G, J, K
						----|---| //current input pixel is at position F
						| J | K |
						---------
				*/

				//all four corners of (x, y) have been determined at
				//this point due to processing sequence!
				var blendXY = preProcBuffer[x];
				blendXY.SetBottomR(blendResult.F);
				blendXY1.SetTopR(blendResult.J);
				preProcBuffer[x] = blendXY1;

				blendXY1 = 0;
				blendXY1.SetTopL(blendResult.K);

				//set 3rd known corner for (x + 1, y)
				int preProcIndex;
				if (x + 1 < SourceSize.Width) {
					preProcIndex = x + 1;
				}
				else if (Configuration.Wrapped.X) {
					preProcIndex = 0;
				}
				else {
					preProcIndex = x;
				}

				preProcBuffer[preProcIndex].SetBottomL(blendResult.G);

				//fill block of size scale * scale with the given color
				//  //place *after* preprocessing step, to not overwrite the
				//  //results while processing the the last pixel!

				if (SMConfig.Resample.xBRZ.UseGradientBlockCopy) {
					BlendBlock(
						target: destination,
						targetOffset: targetIndex,
						pitch: TargetSize.Width,
						topLeft: source[s0 + x],
						topRight: source[s0 + xP1],
						bottomLeft: source[sP1 + x],
						bottomRight: source[sP1 + xP1],
						blockSize: Scalerer.Scale
					);
				}
				else {
					FillBlock(
						target: destination,
						targetOffset: targetIndex,
						pitch: TargetSize.Width,
						color: source[s0 + x],
						blockSize: Scalerer.Scale
					);
				}

				//blend four corners of current pixel
				if (!blendXY.BlendingNeeded()) {
					continue;
				}

				//read sequentially from memory as far as possible
				var ker3 = new Kernel3X3(
					ker4.A,
					ker4.B,
					ker4.C,

					ker4.E,
					ker4.F,
					ker4.G,

					ker4.I,
					ker4.J,
					ker4.K
				);

				ScalePixel(Scalerer, RotationDegree.R0, in ker3, ref outputMatrix, targetIndex, blendXY);
				ScalePixel(Scalerer, RotationDegree.R90, in ker3, ref outputMatrix, targetIndex, blendXY);
				ScalePixel(Scalerer, RotationDegree.R180, in ker3, ref outputMatrix, targetIndex, blendXY);
				ScalePixel(Scalerer, RotationDegree.R270, in ker3, ref outputMatrix, targetIndex, blendXY);
			}
		}
	}
}
