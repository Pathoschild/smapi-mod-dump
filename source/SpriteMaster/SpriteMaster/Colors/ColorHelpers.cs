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
using SpriteMaster.Types.Fixed;
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
	internal static ushort ScalarToValue16(this double scalar) => (ushort)((scalar * 655_35.0) + 0.5);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint ScalarToValue32(this double scalar) => (uint)((scalar * 4_294_967_295.0) + 0.5);

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
}
