using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.CompilerServices;
using SpriteMaster.Types;
using SpriteMaster.xBRZ.Blend;
using SpriteMaster.xBRZ.Color;
using SpriteMaster.xBRZ.Common;
using SpriteMaster.xBRZ.Scalers;

namespace SpriteMaster.xBRZ {
	// ReSharper disable once InconsistentNaming
	public sealed class Scaler {
		public const uint MinScale = 2;
		public const uint MaxScale = Config.MaxScale;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Scaler (
			uint scaleMultiplier,
			in Span<uint> sourceData,
			in Point sourceSize,
			in Rectangle? sourceTarget,
			ref Span<uint> targetData,
			in Config configuration
		) {
			if (scaleMultiplier < MinScale || scaleMultiplier > MaxScale) {
				throw new ArgumentOutOfRangeException(nameof(scaleMultiplier));
			}
			/*
			if (sourceData == null) {
				throw new ArgumentNullException(nameof(sourceData));
			}
			if (targetData == null) {
				throw new ArgumentNullException(nameof(targetData));
			}
			*/
			if (sourceSize.X <= 0 || sourceSize.Y <= 0) {
				throw new ArgumentOutOfRangeException(nameof(sourceSize));
			}
			if (sourceSize.X * sourceSize.Y > sourceData.Length) {
				throw new ArgumentOutOfRangeException(nameof(sourceData));
			}
			this.sourceTarget = sourceTarget.GetValueOrDefault(new Rectangle(0, 0, sourceSize.X, sourceSize.Y));
			if (this.sourceTarget.Right > sourceSize.X || this.sourceTarget.Bottom > sourceSize.Y) {
				throw new ArgumentOutOfRangeException(nameof(sourceTarget));
			}
			this.targetWidth = this.sourceTarget.Width * (int)scaleMultiplier;
			this.targetHeight = this.sourceTarget.Height * (int)scaleMultiplier;
			if (targetWidth * targetHeight > targetData.Length) {
				throw new ArgumentOutOfRangeException(nameof(targetData));
			}

			this.scaler = scaleMultiplier.ToIScaler();
			this.configuration = configuration;
			this.ColorDistance = new ColorDist(this.configuration);
			this.ColorEqualizer = new ColorEq(this.configuration);
			this.sourceSize = sourceSize;
			Scale(in sourceData, ref targetData);
		}

		private readonly Config configuration;
		private readonly IScaler scaler;
		private OutputMatrix outputMatrix;

		private readonly ColorDist ColorDistance;
		private readonly ColorEq ColorEqualizer;

		private readonly Point sourceSize;
		private readonly Rectangle sourceTarget;
		private readonly int targetWidth;
		private readonly int targetHeight;

		//fill block with the given color
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void FillBlock<T> (ref Span<T> trg, int trgi, int pitch, T col, int blockSize) where T : unmanaged {
			for (var y = 0; y < blockSize; ++y, trgi += pitch) {
				for (var x = 0; x < blockSize; ++x) {
					trg[trgi + x] = col;
				}
			}
		}

		//detect blend direction
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private BlendResult PreProcessCorners (in Kernel4x4 ker) {
			var result = new BlendResult();

			if ((ker.F == ker.G && ker.J == ker.K) || (ker.F == ker.J && ker.G == ker.K))
				return result;

			var dist = ColorDistance;

			var jg = dist.DistYCbCr(ker.I, ker.F) + dist.DistYCbCr(ker.F, ker.C) + dist.DistYCbCr(ker.N, ker.K) + dist.DistYCbCr(ker.K, ker.H) + configuration.CenterDirectionBias * dist.DistYCbCr(ker.J, ker.G);
			var fk = dist.DistYCbCr(ker.E, ker.J) + dist.DistYCbCr(ker.J, ker.O) + dist.DistYCbCr(ker.B, ker.G) + dist.DistYCbCr(ker.G, ker.L) + configuration.CenterDirectionBias * dist.DistYCbCr(ker.F, ker.K);

			if (jg < fk) {
				var dominantGradient = (configuration.DominantDirectionThreshold * jg < fk) ? BlendType.Dominant : BlendType.Normal;
				if (ker.F != ker.G && ker.F != ker.J) {
					result.F = dominantGradient;
				}
				if (ker.K != ker.J && ker.K != ker.G) {
					result.K = dominantGradient;
				}
			}
			else if (fk < jg) {
				var dominantGradient = (configuration.DominantDirectionThreshold * fk < jg) ? BlendType.Dominant : BlendType.Normal;
				if (ker.J != ker.F && ker.J != ker.K) {
					result.J = dominantGradient;
				}
				if (ker.G != ker.F && ker.G != ker.K) {
					result.G = dominantGradient;
				}
			}

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ScalePixel (IScaler scaler, RotationDegree rotDeg, in Kernel3x3 ker, int trgi, byte blendInfo) {
			var blend = blendInfo.Rotate(rotDeg);

			if (blend.GetBottomR() == BlendType.None)
				return;

			// int a = ker._[Rot._[(0 << 2) + rotDeg]];
			var b = ker[Rotator.Get((1 << 2) + (int)rotDeg)];
			var c = ker[Rotator.Get((2 << 2) + (int)rotDeg)];
			var d = ker[Rotator.Get((3 << 2) + (int)rotDeg)];
			var e = ker[Rotator.Get((4 << 2) + (int)rotDeg)];
			var f = ker[Rotator.Get((5 << 2) + (int)rotDeg)];
			var g = ker[Rotator.Get((6 << 2) + (int)rotDeg)];
			var h = ker[Rotator.Get((7 << 2) + (int)rotDeg)];
			var i = ker[Rotator.Get((8 << 2) + (int)rotDeg)];

			var eq = ColorEqualizer;
			var dist = ColorDistance;

			bool doLineBlend;

			if (blend.GetBottomR() >= BlendType.Dominant) {
				doLineBlend = true;
			}
			//make sure there is no second blending in an adjacent
			//rotation for this pixel: handles insular pixels, mario eyes
			//but support double-blending for 90� corners
			else if (blend.GetTopR() != BlendType.None && !eq.IsColorEqual(e, g)) {
				doLineBlend = false;
			}
			else if (blend.GetBottomL() != BlendType.None && !eq.IsColorEqual(e, c)) {
				doLineBlend = false;
			}
			//no full blending for L-shapes; blend corner only (handles "mario mushroom eyes")
			else if (eq.IsColorEqual(g, h) && eq.IsColorEqual(h, i) && eq.IsColorEqual(i, f) && eq.IsColorEqual(f, c) && !eq.IsColorEqual(e, i)) {
				doLineBlend = false;
			}
			else {
				doLineBlend = true;
			}

			//choose most similar color
			var px = dist.DistYCbCr(e, f) <= dist.DistYCbCr(e, h) ? f : h;

			var out_ = outputMatrix;
			out_.Move(rotDeg, trgi);

			if (!doLineBlend) {
				scaler.BlendCorner(px, out_);
				return;
			}

			//test sample: 70% of values max(fg, hc) / min(fg, hc)
			//are between 1.1 and 3.7 with median being 1.9
			var fg = dist.DistYCbCr(f, g);
			var hc = dist.DistYCbCr(h, c);

			var haveShallowLine = configuration.SteepDirectionThreshold * fg <= hc && e != g && d != g;
			var haveSteepLine = configuration.SteepDirectionThreshold * hc <= fg && e != c && b != c;

			if (haveShallowLine) {
				if (haveSteepLine) {
					scaler.BlendLineSteepAndShallow(px, out_);
				}
				else {
					scaler.BlendLineShallow(px, out_);
				}
			}
			else {
				if (haveSteepLine) {
					scaler.BlendLineSteep(px, out_);
				}
				else {
					scaler.BlendLineDiagonal(px, out_);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int clampX (int x) {
			x -= sourceTarget.Left;
			if (configuration.Wrapped.X) {
				x = (x + sourceTarget.Width) % sourceTarget.Width;
			}
			else {
				x = Math.Min(Math.Max(x, 0), sourceTarget.Width - 1);
			}
			return x + sourceTarget.Left;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int clampY (int y) {
			y -= sourceTarget.Top;
			if (configuration.Wrapped.Y) {
				y = (y + sourceTarget.Height) % sourceTarget.Height;
			}
			else {
				y = Math.Min(Math.Max(y, 0), sourceTarget.Height - 1);
			}
			return y + sourceTarget.Top;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool legalX (int x) {
			return true;
			/*
			if (configuration.Wrapped.X) {
				return true;
			}
			return x >= sourceTarget.Left && x < sourceTarget.Right;
			*/
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool legalY (int y) {
			return true;
			/*
			if (configuration.Wrapped.Y) {
				return true;
			}
			return y >= sourceTarget.Top && y < sourceTarget.Bottom;
			*/
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int getX (int x) {
			return legalX(x) ? clampX(x) : -clampX(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int getY (int y) {
			return legalY(y) ? clampY(y) : -clampY(y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Mask (uint value, uint mask) {
			return value & mask;
		}

		//scaler policy: see "Scaler2x" reference implementation
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void Scale (in Span<uint> source, ref Span<uint> destination) {
			fixed (uint* destinationPtr = &destination.GetPinnableReference()) {

				int yFirst = sourceTarget.Top;
				int yLast = sourceTarget.Bottom;

				if (yFirst >= yLast)
					return;

				//temporary buffer for "on the fly preprocessing"
				var preProcBuffer = stackalloc byte[sourceTarget.Width];

				static uint GetPixel (in Span<uint> src, int stride, int offset) {
					// We can try embedded a distance calculation as well. Perhaps instead of a negative stride/offset, we provide a 
					// negative distance from the edge and just recalculate the stride/offset in that case.
					// We can scale the alpha reduction by the distance to hopefully correct the edges.

					// Alternatively, for non-wrapping textures (or for wrapping ones that only have one wrapped axis) we embed them in a new target
					// which is padded with alpha, and after resampling we manually clamp the colors on it. This will give a normal-ish effect for drawing, and will make it so we might get a more correct edge since it can overdraw.
					// If we do this, we draw the entire texture, with the padding, but we slightly enlarge the target area for _drawing_ to account for the extra padding.
					// This will effectively cause a filtering effect and hopefully prevent the hard edge problems

					if (stride >= 0 && offset >= 0)
						return src[stride + offset];
					stride = (stride < 0) ? -stride : stride;
					offset = (offset < 0) ? -offset : offset;
					uint sample = src[stride + offset];
					const uint mask = 0x00_FF_FF_FFU;
					return Mask(sample, mask);
				}

				//initialize preprocessing buffer for first row:
				//detect upper left and right corner blending
				//this cannot be optimized for adjacent processing
				//stripes; we must not allow for a memory race condition!
				if (yFirst > 0) {
					var y = yFirst - 1;

					var sM1 = sourceSize.X * getY(y - 1);
					var s0 = sourceSize.X * y; //center line
					var sP1 = sourceSize.X * getY(y + 1);
					var sP2 = sourceSize.X * getY(y + 2);

					for (var x = sourceTarget.Left; x < sourceTarget.Right; ++x) {
						var xM1 = getX(x - 1);
						var xP1 = getX(x + 1);
						var xP2 = getX(x + 2);

						//read sequentially from memory as far as possible
						var ker4 = new Kernel4x4(
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
						| F | G | //evalute corner between F, G, J, K
						----|---| //input pixel is at position F
						| J | K |
						---------
						*/

						int adjustedX = x - sourceTarget.Left;

						preProcBuffer[adjustedX] = preProcBuffer[adjustedX].SetTopR(blendResult.J);

						if (x + 1 < sourceTarget.Right) {
							preProcBuffer[adjustedX + 1] = preProcBuffer[adjustedX + 1].SetTopL(blendResult.K);
						}
						else if (configuration.Wrapped.X) {
							preProcBuffer[0] = preProcBuffer[0].SetTopL(blendResult.K);
						}
					}
				}

				outputMatrix = new OutputMatrix(scaler.Scale, destinationPtr, targetWidth);

				for (var y = yFirst; y < yLast; ++y) {
					//consider MT "striped" access
					var trgi = scaler.Scale * (y - yFirst) * targetWidth;

					var sM1 = sourceSize.X * getY(y - 1);
					var s0 = sourceSize.X * y; //center line
					var sP1 = sourceSize.X * getY(y + 1);
					var sP2 = sourceSize.X * getY(y + 2);

					byte blendXy1 = 0;

					for (var x = sourceTarget.Left; x < sourceTarget.Right; ++x, trgi += scaler.Scale) {
						var xM1 = getX(x - 1);
						var xP1 = getX(x + 1);
						var xP2 = getX(x + 2);

						//evaluate the four corners on bottom-right of current pixel
						//blend_xy for current (x, y) position

						//read sequentially from memory as far as possible
						var ker4 = new Kernel4x4(
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

						int adjustedX = x - sourceTarget.Left;

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
						var blendXy = preProcBuffer[adjustedX].SetBottomR(blendResult.F);

						//set 2nd known corner for (x, y + 1)
						blendXy1 = blendXy1.SetTopR(blendResult.J);
						//store on current buffer position for use on next row
						preProcBuffer[adjustedX] = blendXy1;

						//set 1st known corner for (x + 1, y + 1) and
						//buffer for use on next column
						blendXy1 = ((byte)0).SetTopL(blendResult.K);

						if (x + 1 < sourceTarget.Right) {
							//set 3rd known corner for (x + 1, y)
							preProcBuffer[adjustedX + 1] = preProcBuffer[adjustedX + 1].SetBottomL(blendResult.G);
						}
						else if (configuration.Wrapped.X) {
							preProcBuffer[0] = preProcBuffer[0].SetBottomL(blendResult.G);
						}

						//fill block of size scale * scale with the given color
						//  //place *after* preprocessing step, to not overwrite the
						//  //results while processing the the last pixel!
						FillBlock(ref destination, trgi, targetWidth, source[s0 + x], scaler.Scale);

						//blend four corners of current pixel
						if (blendXy == 0)
							continue;

						//read sequentially from memory as far as possible
						var ker3 = new Kernel3x3(
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

						ScalePixel(scaler, RotationDegree.R0, in ker3, trgi, blendXy);
						ScalePixel(scaler, RotationDegree.R90, in ker3, trgi, blendXy);
						ScalePixel(scaler, RotationDegree.R180, in ker3, trgi, blendXy);
						ScalePixel(scaler, RotationDegree.R270, in ker3, trgi, blendXy);
					}
				}
			}
		}
	}
}
