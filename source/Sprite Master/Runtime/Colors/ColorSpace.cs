/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Colors;

readonly struct ColorSpace {
	internal readonly struct Double3 {
		internal readonly double R;
		internal readonly double G;
		internal readonly double B;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal Double3(double r, double g, double b) {
			R = r;
			G = g;
			B = b;
		}
	}

	internal readonly struct ScaleDouble {
		internal readonly double R;
		internal readonly double B;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal ScaleDouble(double r, double b) {
			R = 0.5 / (1.0 - r);
			B = 0.5 / (1.0 - b);
		}
	}

	internal readonly Double3 LumaCoefficient;
	internal readonly ScaleDouble LumaScale;

	private readonly byte[] LinearizeTable8 = null;
	private readonly byte[] DelinearizeTable8 = null;
	private readonly ushort[] LinearizeTable16 = null;
	private readonly ushort[] DelinearizeTable16 = null;

	internal delegate double CurveDelegateDouble(double scalar);
	internal readonly CurveDelegateDouble LinearizeScalar;
	internal readonly CurveDelegateDouble DelinearizeScalar;

	internal readonly byte Linearize(byte value) => LinearizeTable8[value];
	internal readonly ushort Linearize(ushort value) => LinearizeTable16[value];
	internal readonly byte Delinearize(byte value) => DelinearizeTable8[value];
	internal readonly ushort Delinearize(ushort value) => DelinearizeTable16[value];

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal ColorSpace(double r, double g, double b, CurveDelegateDouble linearize = null, CurveDelegateDouble delinearize = null) {
		// Correct for precision error, just in case.
		if ((r + g + b) != 1.0) {
			// Recalculate from the smallest two values, hoping to retain precision? My logic might be backwards.
			if (r >= g && r >= b) {
				r = 1.0 - (g + b);
			}
			else if (g >= r && g >= b) {
				g = 1.0 - (r + b);
			}
			else {
				b = 1.0 - (r + g);
			}
		}

		LumaCoefficient = new Double3(r, g, b);
		LumaScale = new ScaleDouble(r, b);

		LinearizeScalar = linearize;
		DelinearizeScalar = delinearize;

		if (LinearizeScalar is not null && DelinearizeScalar is not null) {
			const int max8 = byte.MaxValue + 1;
			LinearizeTable8 = new byte[max8];
			DelinearizeTable8 = new byte[max8];
			for (int i = 0; i < max8; ++i) {
				LinearizeTable8[i] = ColorConstant.ScalarToValue8(LinearizeScalar(ColorConstant.ValueToScalar((byte)i)));
				DelinearizeTable8[i] = ColorConstant.ScalarToValue8(DelinearizeScalar(ColorConstant.ValueToScalar((byte)i)));
			}

			const int max16 = ushort.MaxValue + 1;
			LinearizeTable16 = new ushort[max16];
			DelinearizeTable16 = new ushort[max16];
			for (int i = 0; i < max16; ++i) {
				LinearizeTable16[i] = ColorConstant.ScalarToValue16(LinearizeScalar(ColorConstant.ValueToScalar((ushort)i)));
				DelinearizeTable16[i] = ColorConstant.ScalarToValue16(DelinearizeScalar(ColorConstant.ValueToScalar((ushort)i)));
			}
		}
	}

	private static double _GetCoefficient(in double? v, in double? v1, in double? v2) => v.HasValue ? v.Value : 1.0 - (v1.Value + v2.Value);

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.2020-2-201510-I!!PDF-E.pdf
	internal static readonly Lazy<ColorSpace> BT_2020 = new(() => new(r: 0.2627, g: 0.6780, b: 0.0593));

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.2100-2-201807-I!!PDF-E.pdf
	internal static readonly Lazy<ColorSpace> BT_2100 = BT_2020; // Same Coefficients as BT.2020

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
	internal static readonly Lazy<ColorSpace> BT_709 = new(() => new(r: 0.2126, g: 0.7152, b: 0.0722));

	internal static readonly Lazy<ColorSpace> BT_709_Precise = new(() => new(r: 0.212655, g: 0.715158, b: 0.072187));

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.601-7-201103-I!!PDF-E.pdf
	internal static readonly Lazy<ColorSpace> BT_601 = new(() => new(r: 0.299, g: 0.587, b: 0.114));

	// https://www5.in.tum.de/lehre/vorlesungen/graphik/info/csc/COL_33.htm
	internal static readonly Lazy<ColorSpace> SMPTE_240M = new(() => new(r: 0.2122, g: 0.7013, b: 0.0865));

	// https://www.sis.se/api/document/preview/562720/
	internal static readonly ColorSpace sRGB = new(
		r: 0.2126, g: 0.7152, b: 0.0722,
		linearize: (s) => (s <= 0.04045) ? (s / 12.92) : Math.Pow((s + 0.055) / 1.055, 2.4),
		delinearize: (l) => (l <= 0.0031308) ? (l * 12.92) : 1.055 * Math.Pow(l, 1 / 2.4) - 0.055
	);

	internal static readonly ColorSpace sRGB_Precise = new(
		r: 0.212655, g: 0.715158, b: 0.072187,
		linearize: (s) => (s <= 0.0404482362771082) ? (s / 12.92) : Math.Pow((s + 0.055) / 1.055, 2.4),
		delinearize: (l) => (l <= 0.00313066844250063) ? (l * 12.92) : 1.055 * Math.Pow(l, 1 / 2.4) - 0.055
	);
}
