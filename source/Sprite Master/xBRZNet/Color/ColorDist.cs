/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

// #define MULTIPLY_ALPHA

using SpriteMaster.Colors;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Color;

class ColorDist {
	protected readonly Config Configuration;
	private unsafe readonly delegate*<uint, uint, int, byte> DifferenceFunction;

	// TODO : Only sRGB presently has the linearizer/delinearizer implemented.
	private static readonly ColorSpace CurrentColorSpace = ColorSpace.sRGB_Precise;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ColorDist(Config cfg) {
		Configuration = cfg;

		// If it is a gamma-corrected texture, it must be linearized prior to luma conversion and comparison.
		unsafe {
			DifferenceFunction = Configuration.Gamma ? &TexelDiffGamma : &TexelDiff;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte TexelDiff(uint texel1, uint texel2, int shift) {
		texel1 = (texel1 >> shift) & 0xFF;
		texel2 = (texel2 >> shift) & 0xFF;

		return (byte)Math.Abs((byte)texel1 - (byte)texel2);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte TexelDiffGamma(uint texel1, uint texel2, int shift) {
		texel1 = (texel1 >> shift) & 0xFF;
		texel2 = (texel2 >> shift) & 0xFF;

		byte t1 = CurrentColorSpace.Linearize((byte)texel1);
		byte t2 = CurrentColorSpace.Linearize((byte)texel2);

		return (byte)Math.Abs(t1 - t2);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static double Square(double value) => value * value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static int Square(int value) => value * value;

	// TODO : This can be SIMD-ized using https://docs.microsoft.com/en-us/dotnet/api/system.runtime.intrinsics.x86
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private unsafe double DistYCbCrImpl(uint pix1, uint pix2) {
		if ((pix1 & ~ColorConstant.Mask.Alpha) == (pix2 & ~ColorConstant.Mask.Alpha)) {
			return 0.0;
		}

		//http://en.wikipedia.org/wiki/YCbCr#ITU-R_BT.601_conversion
		//YCbCr conversion is a matrix multiplication => take advantage of linearity by subtracting first!
		var rDiff = DifferenceFunction(pix1, pix2, ColorConstant.Shift.Red); //we may delay division by 255 to after matrix multiplication
		var gDiff = DifferenceFunction(pix1, pix2, ColorConstant.Shift.Green);
		var bDiff = DifferenceFunction(pix1, pix2, ColorConstant.Shift.Blue);  //subtraction for int is noticeable faster than for double

		var coefficient = CurrentColorSpace.LumaCoefficient;
		var scale = CurrentColorSpace.LumaScale;

		var y = (coefficient.R * rDiff) + (coefficient.G * gDiff) + (coefficient.B * bDiff); //[!], analog YCbCr!
		var cB = scale.B * (bDiff - y);
		var cR = scale.R * (rDiff - y);

		// Skip division by 255.
		// Also skip square root here by pre-squaring the config option equalColorTolerance.
		return Square(Configuration.LuminanceWeight * y) + (Configuration.ChrominanceWeight * (Square(cB) + Square(cR)));
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal double DistYCbCr(uint pix1, uint pix2) {
		if (pix1 == pix2) {
			return 0.0;
		}

		var distance = DistYCbCrImpl(pix1, pix2);

		// From reference xbrz.cpp
		if (Configuration.HasAlpha) {
			byte a1 = (byte)((pix1 >> ColorConstant.Shift.Alpha) & 0xFF);
			byte a2 = (byte)((pix2 >> ColorConstant.Shift.Alpha) & 0xFF);

			distance = ColorConstant.ValueToScalar(Math.Min(a1, a2)) * distance + Square(Math.Abs(a2 - a1));
		}

		return distance;
	}
}
