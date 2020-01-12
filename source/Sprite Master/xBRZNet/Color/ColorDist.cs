using System;
using System.Runtime.CompilerServices;
using SpriteMaster.xBRZ.Color;
using SpriteMaster.xBRZ.Common;

namespace SpriteMaster.xBRZ.Color {
	internal class ColorDist {
		protected readonly Config Configuration;

		// TODO : Only sRGB presently has the linearizer/delinearizer implemented.
		private static readonly ColorSpace CurrentColorSpace = ColorSpace.sRGB_Precise;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ColorDist (in Config cfg) {
			Configuration = cfg;
		}

		private const bool MultiplyAlpha = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double TexelDiff (uint texel1, uint texel2, int shift) {
			unchecked {
				texel1 = (texel1 >> shift) & 0xFF;
				texel2 = (texel2 >> shift) & 0xFF;
			}

			return Math.Abs(texel1 - texel2);
			//return (unchecked((int)((uint)texel1 & mask)) - unchecked((int)((uint)texel2 & mask))) >> shift;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double TexelDiffGamma (uint texel1, uint texel2, int shift) {
			unchecked {
				texel1 = (texel1 >> shift) & 0xFF;
				texel2 = (texel2 >> shift) & 0xFF;
			}

			double t1 = CurrentColorSpace.Linearize(texel1 / 255.0);
			double t2 = CurrentColorSpace.Linearize(texel2 / 255.0);

			return Math.Abs(t1 - t2) * 255.0;
			//return (unchecked((int)((uint)texel1 & mask)) - unchecked((int)((uint)texel2 & mask))) >> shift;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double DistYCbCrImpl (uint pix1, uint pix2) {
			if ((pix1 & ~ColorConstant.Mask.Alpha) == (pix2 & ~ColorConstant.Mask.Alpha))
				return 0.0;

			// If it is a gamma-corrected texture, it must be linearized prior to luma conversion and comparison.
			var Difference = (Configuration.Gamma) ? 
				(Func<uint, uint, int, double>)TexelDiffGamma :
				TexelDiff;

			//http://en.wikipedia.org/wiki/YCbCr#ITU-R_BT.601_conversion
			//YCbCr conversion is a matrix multiplication => take advantage of linearity by subtracting first!
			var rDiff = Difference(pix1, pix2, ColorConstant.Shift.Red); //we may delay division by 255 to after matrix multiplication
			var gDiff = Difference(pix1, pix2, ColorConstant.Shift.Green);
			var bDiff = Difference(pix1, pix2, ColorConstant.Shift.Blue);  //subtraction for int is noticeable faster than for double

			// Alpha gives some interesting properties.
			// We technically cannot guarantee that the color is correct once we are in transparent areas, but we might still want to blend there.

			if (MultiplyAlpha) {
				var aDiff = 0xFF - TexelDiff(pix1, pix2, ColorConstant.Shift.Alpha);
				rDiff = (rDiff * aDiff) / 0xFF;
				gDiff = (gDiff * aDiff) / 0xFF;
				bDiff = (bDiff * aDiff) / 0xFF;
			}

			var coefficient = CurrentColorSpace.LumaCoefficient;
			var scale = CurrentColorSpace.LumaScale;

			var y = (coefficient.R * rDiff) + (coefficient.G * gDiff) + (coefficient.B * bDiff); //[!], analog YCbCr!
			var cB = scale.B * (bDiff - y);
			var cR = scale.R * (rDiff - y);

			// Skip division by 255.
			// Also skip square root here by pre-squaring the config option equalColorTolerance.
			return Math.Pow(Configuration.LuminanceWeight * y, 2) + Math.Pow(cB, 2) + Math.Pow(cR, 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double DistYCbCr (uint pix1, uint pix2) {
			if (pix1 == pix2)
				return 0;

			var distance = DistYCbCrImpl(pix1, pix2);

			// From reference xbrz.cpp
			if (Configuration.HasAlpha) {
				unchecked {
					double a1 = ((pix1 >> ColorConstant.Shift.Alpha) & 0xFF) / 255.0;
					double a2 = ((pix2 >> ColorConstant.Shift.Alpha) & 0xFF) / 255.0;

					distance /= 255.0;

					if (a1 <= a2) {
						return (a1 * distance) + (255.0 * (a2 - a1));
					}
					else {
						return (a2 * distance) + (255.0 * (a1 - a2));
					}
				}
			}

			return distance;
		}
	}
}
