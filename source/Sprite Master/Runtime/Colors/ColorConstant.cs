/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;

namespace SpriteMaster.Colors;

static class ColorConstant {
	internal static class Shift {
		internal const int Alpha = 24;
		internal const int Red = 0;
		internal const int Green = 8;
		internal const int Blue = 16;
	}

	internal static class Mask {
		internal const uint Alpha = 0xFFU << Shift.Alpha;
		internal const uint Red = 0xFFU << Shift.Red;
		internal const uint Green = 0xFFU << Shift.Green;
		internal const uint Blue = 0xFFU << Shift.Blue;
	}

	internal const double ScalarFactor8 = 255.999_999_999_999_9;
	internal const double ScalarFactor16 = 65_535.999_999_999_99;
	internal const double ScalarFactor32 = 4_294_967_295.999_999;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(byte value) => value / ScalarFactor8;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(ushort value) => value / ScalarFactor16;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double ValueToScalar(uint value) => value / ScalarFactor32;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte ScalarToValue8(double scalar) => (byte)(scalar * ScalarFactor8);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort ScalarToValue16(double scalar) => (byte)(scalar * ScalarFactor8);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint ScalarToValue32(double scalar) => (byte)(scalar * ScalarFactor8);

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	static ColorConstant() {
		if ((int)ScalarFactor8 == 256) {
			throw new System.Exception($"ScalarFactor8 == 2^8");
		}
		if ((int)ScalarFactor16 == 65_536) {
			throw new System.Exception($"ScalarFactor16 == 2^16");
		}
		if ((long)ScalarFactor32 == 4_294_967_296) {
			throw new System.Exception($"ScalarFactor32 == 2^32");
		}
	}
}
