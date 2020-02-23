using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Color {
	internal struct ColorSpace {
		public struct Double3 {
			public readonly double R;
			public readonly double G;
			public readonly double B;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Double3 (double r, double g, double b) {
				R = r;
				G = g;
				B = b;
			}
		}

		public struct ScaleDouble {
			public readonly double R;
			public readonly double B;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal ScaleDouble (double r, double b) {
				R = 0.5 / (1.0 - r);
				B = 0.5 / (1.0 - b);
			}
		}

		public readonly Double3 LumaCoefficient;
		public readonly ScaleDouble LumaScale;

		public readonly Func<double, double> Linearize;
		public readonly Func<double, double> Delinearize;

		internal ColorSpace (double r, double g, double b, Func<double, double> linearize = null, Func<double, double> delinearize = null) {
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

			Linearize = linearize;
			Delinearize = delinearize;
		}
		
		private static double _GetCoefficient(double? v, double? v1, double? v2) {
			return v.HasValue ? v.Value : 1.0 - (v1.Value + v2.Value);
		}

		// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.2020-2-201510-I!!PDF-E.pdf
		internal static readonly ColorSpace BT_2020 = new ColorSpace(r: 0.2627, g: 0.6780, b: 0.0593);

		// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.2100-2-201807-I!!PDF-E.pdf
		internal static readonly ColorSpace BT_2100 = BT_2020; // Same Coefficients as BT.2020

		// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
		internal static readonly ColorSpace BT_709 = new ColorSpace(r: 0.2126, g: 0.7152, b: 0.0722);

		internal static readonly ColorSpace BT_709_Precise = new ColorSpace(r: 0.212655, g: 0.715158, b: 0.072187);

		// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.601-7-201103-I!!PDF-E.pdf
		internal static readonly ColorSpace BT_601 = new ColorSpace(r: 0.299, g: 0.587, b: 0.114);

		// https://www5.in.tum.de/lehre/vorlesungen/graphik/info/csc/COL_33.htm
		internal static readonly ColorSpace SMPTE_240M = new ColorSpace(r: 0.2122, g: 0.7013, b: 0.0865);

		// https://www.sis.se/api/document/preview/562720/
		internal static readonly ColorSpace sRGB = new ColorSpace(
			r: 0.2126, g: 0.7152, b: 0.0722,
			linearize: (s) => (s <= 0.04045) ? (s / 12.92) : Math.Pow((s + 0.055) / 1.055, 2.4),
			delinearize: (l) => (l <= 0.0031308) ? (l * 12.92) : 1.055 * Math.Pow(l, 1 / 2.4) - 0.055
		);

		internal static readonly ColorSpace sRGB_Precise = new ColorSpace(
			r: 0.212655, g: 0.715158, b: 0.072187,
			linearize: (s) => (s <= 0.0404482362771082) ? (s / 12.92) : Math.Pow((s + 0.055) / 1.055, 2.4),
			delinearize: (l) => (l <= 0.00313066844250063) ? (l * 12.92) : 1.055 * Math.Pow(l, 1 / 2.4) - 0.055
		);
	}
}
