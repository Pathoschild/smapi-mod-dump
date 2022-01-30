/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Colors;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Color;

class ColorDist {
	protected readonly Config Configuration;

	// TODO : Only sRGB presently has the linearizer/delinearizer implemented.
	private static readonly ColorSpace CurrentColorSpace = ColorSpace.sRGB_Precise;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ColorDist(Config cfg) => Configuration = cfg;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static Fixed16 TexelDiff(Fixed16 texel1, Fixed16 texel2) => (Fixed16)Math.Abs(texel1.Value - texel2.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static double Square(double value) => value * value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static int Square(int value) => value * value;

	// TODO : This can be SIMD-ized using https://docs.microsoft.com/en-us/dotnet/api/system.runtime.intrinsics.x86
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private unsafe double DistYCbCrImpl(Color16 pix1, Color16 pix2) {
		// See if the colors are the same
		if (pix1.NoAlpha == pix2.NoAlpha) {
			return 0.0;
		}

		//http://en.wikipedia.org/wiki/YCbCr#ITU-R_BT.601_conversion
		//YCbCr conversion is a matrix multiplication => take advantage of linearity by subtracting first!
		var rDiff = TexelDiff(pix1.R, pix2.R); //we may delay division by 255 to after matrix multiplication
		var gDiff = TexelDiff(pix1.G, pix2.G);
		var bDiff = TexelDiff(pix1.B, pix2.B); //subtraction for int is noticeable faster than for double

		var coefficient = CurrentColorSpace.LumaCoefficient;
		var scale = CurrentColorSpace.LumaScale;

		// TODO : integer math?
		var y = coefficient.R * rDiff.Value + coefficient.G * gDiff.Value + coefficient.B * bDiff.Value; //[!], analog YCbCr!
		var cB = scale.B * (bDiff.Value - y);
		var cR = scale.R * (rDiff.Value - y);

		// Skip division by 255.
		var luminance = Square(Configuration.LuminanceWeight * y);
		var chrominance = Configuration.ChrominanceWeight * (Square(cB) + Square(cR));
		return Math.Sqrt(luminance + chrominance);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal double DistYCbCr(Color16 pix1, Color16 pix2) {
		if (pix1 == pix2) {
			return 0.0;
		}

		var distance = DistYCbCrImpl(pix1, pix2);

		// From reference xbrz.cpp
		if (Configuration.HasAlpha) {
			var a1 = pix1.A;
			var a2 = pix2.A;

			// TODO : integer math?
			var alphaScalar = a1.Min(a2).ValueToScalar();
			distance = alphaScalar * distance + Math.Abs(a2.Value - a1.Value);
		}

		return distance;
	}
}
