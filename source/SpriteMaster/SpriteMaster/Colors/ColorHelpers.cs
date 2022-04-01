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
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Colors;

static class ColorHelpers {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(this byte value) => value / 255.0;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(this ushort value) => value / 65_535.0;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(this uint value) => value / 4_294_967_295.0;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(this Fixed8 value) => ValueToScalar(value.Value);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(this Fixed16 value) => ValueToScalar(value.Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte ScalarToValue8(this double scalar) => (byte)((scalar * 255.0) + 0.5);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte ScalarToValue8(this float scalar) => (byte)((scalar * 255.0f) + 0.5f);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort ScalarToValue16(this double scalar) => (ushort)((scalar * 655_35.0) + 0.5);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort ScalarToValue16(this float scalar) => (ushort)((scalar * 655_35.0f) + 0.5f);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint ScalarToValue32(this double scalar) => (uint)((scalar * 4_294_967_295.0) + 0.5);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint ScalarToValue32(this float scalar) => (uint)((scalar * 4_294_967_295.0f) + 0.5f);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Color8To16(this byte value) => (ushort)((value << 8) | value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Color16To8Fast(this ushort value) => (byte)((uint)((value * 0xFF01U) + 0x800000U) >> 24);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Color16To8Accurate(this ushort value) => (byte)(((uint)value + 128U) / 0x101U);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Color16to8(this ushort value) => Color16To8Accurate(value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static half Color8ToHalf(this byte value) {
		if (value == 0) {
			ushort shortValue = 0;
			return shortValue.ReinterpretAs<half>();
		}
		const uint InBits = sizeof(byte) * 8;
		uint uValue = (uint)value;
		int leadingZerosWithMSB = value.CountLeadingZeros() + 1;
		uValue = (byte)(uValue << leadingZerosWithMSB);
		uint remainingBits = (uint)(InBits - (leadingZerosWithMSB));
		int shiftRight = (leadingZerosWithMSB) + ((int)remainingBits - (int)Numeric.Float.Half.SignificandBits);
		if (InBits <= Numeric.Float.Half.SignificandBits || shiftRight < 0) {
			uValue <<= -shiftRight;
		}
		else {
			uValue >>= shiftRight;
		}
		uint mantissa = uValue;
		uint exponent = ((uint)-(int)(leadingZerosWithMSB + 1)) & (Numeric.Float.Half.ExponentMask >> 1);
		uint result = (mantissa | exponent << (int)Numeric.Float.Half.SignificandBits);
		return result.ReinterpretAs<half>();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static half Color16ToHalf(this ushort value) {
		if (value == 0) {
			return value.ReinterpretAs<half>();
		}
		const uint InBits = sizeof(ushort) * 8;
		uint uValue = (uint)value;
		int leadingZerosWithMSB = value.CountLeadingZeros() + 1;
		uValue = (byte)(uValue << leadingZerosWithMSB);
		uint remainingBits = (uint)(InBits - (leadingZerosWithMSB));
		int shiftRight = (leadingZerosWithMSB) + ((int)remainingBits - (int)Numeric.Float.Half.SignificandBits);
		if (InBits <= Numeric.Float.Half.SignificandBits || shiftRight < 0) {
			uValue <<= -shiftRight;
		}
		else {
			uValue >>= shiftRight;
		}
		uint mantissa = uValue;
		uint exponent = ((uint)-(int)(leadingZerosWithMSB + 1)) & (Numeric.Float.Half.ExponentMask >> 1);
		uint result = (mantissa | exponent << (int)Numeric.Float.Half.SignificandBits);
		return result.ReinterpretAs<half>();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static float Color8ToFloat(this byte value) {
		if (value == 0) {
			return 0.0f;
		}
		const uint InBits = sizeof(byte) * 8;
		uint uValue = (uint)value;
		int leadingZerosWithMSB = value.CountLeadingZeros() + 1;
		uValue = (byte)(uValue << leadingZerosWithMSB);
		uint remainingBits = (uint)(InBits - (leadingZerosWithMSB));
		int shiftRight = (leadingZerosWithMSB) + ((int)remainingBits - (int)Numeric.Float.Single.SignificandBits);
		if (InBits <= Numeric.Float.Single.SignificandBits || shiftRight < 0) {
			uValue <<= -shiftRight;
		}
		else {
			uValue >>= shiftRight;
		}
		uint mantissa = uValue;
		uint exponent = ((uint)-(int)(leadingZerosWithMSB + 1)) & (Numeric.Float.Single.ExponentMask >> 1);
		uint result = (mantissa | exponent << (int)Numeric.Float.Single.SignificandBits);
		return result.ReinterpretAs<float>();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static float Color16ToFloat(this ushort value) {
		if (value == 0) {
			return 0.0f;
		}
		const uint InBits = sizeof(ushort) * 8;
		uint uValue = (uint)value;
		int leadingZerosWithMSB = value.CountLeadingZeros() + 1;
		uValue = (byte)(uValue << leadingZerosWithMSB);
		uint remainingBits = (uint)(InBits - (leadingZerosWithMSB));
		int shiftRight = (leadingZerosWithMSB) + ((int)remainingBits - (int)Numeric.Float.Single.SignificandBits);
		if (InBits <= Numeric.Float.Single.SignificandBits || shiftRight < 0) {
			uValue <<= -shiftRight;
		}
		else {
			uValue >>= shiftRight;
		}
		uint mantissa = uValue;
		uint exponent = ((uint)-(int)(leadingZerosWithMSB + 1)) & (Numeric.Float.Single.ExponentMask >> 1);
		uint result = (mantissa | exponent << (int)Numeric.Float.Single.SignificandBits);
		return result.ReinterpretAs<float>();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint RedmeanDifference(this in Color8 colorA, in Color8 colorB, bool linear, bool alpha = true) {
		static int Square(int value) => value * value;

		if (colorA == colorB) {
			return 0;
		}

		Fixed8 alphaScalar = Fixed8.Max;
		if (alpha) {
			alphaScalar = Math.Min(colorA.A.Value, colorB.A.Value);

			if (alphaScalar == 0) {
				return (byte)Math.Abs(colorB.A.Value - colorA.A.Value);
			}
		}

		Fixed8 colorDistance;
		if (colorA.NoAlpha == colorB.NoAlpha) {
			colorDistance = Fixed8.Zero;
		}
		else {
			var refColorA = colorA;
			var refColorB = colorB;

			if (linear) {
				refColorA = ColorSpace.sRGB_Precise.Delinearize(refColorA);
				refColorB = ColorSpace.sRGB_Precise.Delinearize(refColorB);
			}

			// https://en.wikipedia.org/wiki/Color_difference#sRGB

			// redmean
			var redMean = ((uint)refColorA.R + refColorB.R.Value) / 2;
			var redMeanFloat = (float)redMean;

			var rDiff = Square(refColorB.R.Value - refColorA.R.Value);
			var gDiff = Square(refColorB.G.Value - refColorA.G.Value);
			var bDiff = Square(refColorB.B.Value - refColorA.B.Value);

			// TODO : use integer arithmetic
			// https://stackoverflow.com/a/2103422

			var rWeight = 2f + (redMeanFloat / 256f);
			var gWeight = 4f;
			var bWeight = 2f + ((255f - redMeanFloat) / 256f);


			var rFac = rWeight * rDiff;
			var gFac = gWeight * gDiff;
			var bFac = bWeight * bDiff;
			var sumSq = rFac + gFac + bFac;
			var sum = MathF.Sqrt(sumSq);
			colorDistance = (Fixed8)MathExt.RoundToInt(Math.Clamp(sum, 0, 255));
		}

		if (alphaScalar == Fixed8.Max) {
			return colorDistance.Value;
		}
		else {
			return (alphaScalar * colorDistance).AddClamped((byte)Math.Abs(colorB.A.Value - colorA.A.Value)).Value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint RedmeanDifference(this in Color16 colorA, in Color16 colorB, bool linear, bool alpha = true) {
		static int Square(int value) => value * value;

		if (colorA == colorB) {
			return 0;
		}

		Fixed16 alphaScalar = Fixed16.Max;
		if (alpha) {
			alphaScalar = Math.Min(colorA.A.Value, colorB.A.Value);

			if (alphaScalar == 0) {
				return (ushort)Math.Abs(colorB.A.Value - colorA.A.Value);
			}
		}

		Fixed16 colorDistance;
		if (colorA.NoAlpha == colorB.NoAlpha) {
			colorDistance = Fixed16.Zero;
		}
		else {
			var refColorA = colorA;
			var refColorB = colorB;

			if (linear) {
				refColorA = ColorSpace.sRGB_Precise.Delinearize(refColorA);
				refColorB = ColorSpace.sRGB_Precise.Delinearize(refColorB);
			}

			// https://en.wikipedia.org/wiki/Color_difference#sRGB

			// redmean
			var redMean = ((uint)refColorA.R + refColorB.R.Value) / 2;
			var redMeanFloat = (float)redMean;

			var rDiff = Square(refColorB.R.Value - refColorA.R.Value);
			var gDiff = Square(refColorB.G.Value - refColorA.G.Value);
			var bDiff = Square(refColorB.B.Value - refColorA.B.Value);

			// TODO : use integer arithmetic
			// https://stackoverflow.com/a/2103422

			var rWeight = 2f + (redMeanFloat / 65_536f);
			var gWeight = 4f;
			var bWeight = 2f + ((65_535f - redMeanFloat) / 65_536f);


			var rFac = rWeight * rDiff;
			var gFac = gWeight * gDiff;
			var bFac = bWeight * bDiff;
			var sumSq = rFac + gFac + bFac;
			var sum = MathF.Sqrt(sumSq);
			colorDistance = (Fixed16)MathExt.RoundToInt(Math.Clamp(sum, 0, 65_535));
		}

		if (alphaScalar == Fixed16.Max) {
			return colorDistance.Value;
		}
		else {
			return (alphaScalar * colorDistance).AddClamped((ushort)Math.Abs(colorB.A.Value - colorA.A.Value)).Value;
		}
	}

	internal readonly struct YccConfig {
		internal double LuminanceWeight { get; init; }
		internal double ChrominanceWeight { get; init; }
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint YccDifference(this in Color16 colorA, in Color16 colorB, in YccConfig config, bool linear, bool alpha = true) {
		if (colorA == colorB) {
			return 0;
		}

		static double YccColorDifference(in Color16 pix1, in Color16 pix2, in YccConfig config) {
			// See if the colors are the same
			if (pix1.NoAlpha == pix2.NoAlpha) {
				return 0.0;
			}

			static double Square(double value) => value * value;
			static Fixed16 TexelDiff(Fixed16 texel1, Fixed16 texel2) => (Fixed16)Math.Abs(texel1.Value - texel2.Value);

			//http://en.wikipedia.org/wiki/YCbCr#ITU-R_BT.601_conversion
			//YCbCr conversion is a matrix multiplication => take advantage of linearity by subtracting first!
			var rDiff = TexelDiff(pix1.R, pix2.R);
			var gDiff = TexelDiff(pix1.G, pix2.G);
			var bDiff = TexelDiff(pix1.B, pix2.B);

			var coefficient = ColorSpace.sRGB_Precise.LumaCoefficient;
			var scale = ColorSpace.sRGB_Precise.LumaScale;

			// TODO : integer math?
			var y = coefficient.R * rDiff.Value + coefficient.G * gDiff.Value + coefficient.B * bDiff.Value; //[!], analog YCbCr!
			var cB = scale.B * (bDiff.Value - y);
			var cR = scale.R * (rDiff.Value - y);

			// Skip division by 255.
			var luminance = Square(config.LuminanceWeight * y);
			var chrominance = config.ChrominanceWeight * (Square(cB) + Square(cR));
			return Math.Sqrt(luminance + chrominance);
		}

		var refColorA = colorA;
		var refColorB = colorB;

		if (linear) {
			refColorA = ColorSpace.sRGB_Precise.Delinearize(refColorA);
			refColorB = ColorSpace.sRGB_Precise.Delinearize(refColorB);
		}

		var distance = YccColorDifference(refColorA, refColorB, config);

		// From reference xbrz.cpp
		if (alpha) {
			var a1 = refColorA.A;
			var a2 = refColorB.A;

			// TODO : integer math?
			var alphaScalar = a1.Min(a2).ValueToScalar();
			distance = alphaScalar * distance + Math.Abs(a2.Value - a1.Value);
		}

		return (uint)MathExt.RoundToInt(Math.Clamp(distance, 0.0, 65_535.0));
	}
}
