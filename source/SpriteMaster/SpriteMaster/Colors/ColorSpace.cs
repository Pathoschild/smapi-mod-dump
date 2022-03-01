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
using System.Runtime.InteropServices;

namespace SpriteMaster.Colors;

readonly struct ColorSpace {
	internal readonly struct Double3 {
		internal readonly double R;
		internal readonly double G;
		internal readonly double B;

		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		internal Double3(double r, double g, double b) {
			R = r;
			G = g;
			B = b;
		}
	}

	internal readonly struct ScaleDouble {
		internal readonly double R;
		internal readonly double B;

		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		internal ScaleDouble(double r, double b) {
			R = 0.5 / (1.0 - r);
			B = 0.5 / (1.0 - b);
		}
	}

	internal readonly Double3 LumaCoefficient;
	internal readonly ScaleDouble LumaScale;

	private readonly byte[] LinearizeTable8;
	private readonly byte[] DelinearizeTable8;
	private readonly ushort[] LinearizeTable16;
	private readonly ushort[] DelinearizeTable16;

	internal delegate double CurveDelegateDouble(double scalar);
	internal readonly CurveDelegateDouble LinearizeScalar;
	internal readonly CurveDelegateDouble DelinearizeScalar;

	private delegate double ToScalar8(byte value);
	private delegate double ToScalar16(ushort value);

	private delegate byte FromScalar8(double value);
	private delegate ushort FromScalar16(double value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly byte Linearize(byte value) => LinearizeTable8[value];
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly byte Linearize(Fixed8 value) => Linearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Color8 Linearize(in Color8 color) {
		return new(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B),
			color.A
		);
	}
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly ushort Linearize(ushort value) => LinearizeTable16[value];
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Fixed16 Linearize(Fixed16 value) => Linearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Color16 Linearize(in Color16 color) {
		return new(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B),
			color.A
		);
	}
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly byte Delinearize(byte value) => DelinearizeTable8[value];
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly byte Delinearize(Fixed8 value) => Delinearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Color8 Delinearize(in Color8 color) {
		return new(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B),
			color.A
		);
	}
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly ushort Delinearize(ushort value) => DelinearizeTable16[value];
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Fixed16 Delinearize(Fixed16 value) => Delinearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Color16 Delinearize(in Color16 color) {
		return new(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B),
			color.A
		);
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	private static T ConvertTo<U, T>(U value) => (T)Convert.ChangeType(value, typeof(T))!;

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	private static (T[] LinearizeTable, T[] DelinearizeTable) InitializeTable<T>(CurveDelegateDouble linearize, CurveDelegateDouble delinearize, int maxValue) where T : unmanaged {
		Func<T, double> ToScalar;
		Func<double, T> FromScalar;

		switch (typeof(T)) {
			case var type when type == typeof(byte):
				ToScalar = value => value.ReinterpretAs<T, byte>().ValueToScalar();
				FromScalar = value => value.ScalarToValue8().ReinterpretAs<T>();
				break;
			case var type when type == typeof(ushort):
				ToScalar = value => value.ReinterpretAs<T, ushort>().ValueToScalar();
				FromScalar = value => value.ScalarToValue16().ReinterpretAs<T>();
				break;
			default:
				throw new ArgumentException($"Unknown Table Type: '{typeof(T).Name}'");
		}

		var linearizeTable = GC.AllocateUninitializedArray<T>(maxValue);
		var delinearizeTable = GC.AllocateUninitializedArray<T>(maxValue);
		for (int i = 0; i < maxValue; ++i) {
			linearizeTable[i] = FromScalar(linearize(ToScalar(ConvertTo<int, T>(i))));
			delinearizeTable[i] = FromScalar(delinearize(ToScalar(ConvertTo<int, T>(i))));
		}

		return (linearizeTable, delinearizeTable);
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal ColorSpace(double r, double g, double b, CurveDelegateDouble linearize, CurveDelegateDouble delinearize) {
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

		(LinearizeTable8, DelinearizeTable8) = InitializeTable<byte>(LinearizeScalar, DelinearizeScalar, byte.MaxValue + 1);
		(LinearizeTable16, DelinearizeTable16) = InitializeTable<ushort>(LinearizeScalar, DelinearizeScalar, ushort.MaxValue + 1);
	}

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.2020-2-201510-I!!PDF-E.pdf
	//internal static readonly Lazy<ColorSpace> BT_2020 = new(() => new(r: 0.2627, g: 0.6780, b: 0.0593));

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.2100-2-201807-I!!PDF-E.pdf
	//internal static readonly Lazy<ColorSpace> BT_2100 = BT_2020; // Same Coefficients as BT.2020

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
	//internal static readonly Lazy<ColorSpace> BT_709 = new(() => new(r: 0.2126, g: 0.7152, b: 0.0722));

	//internal static readonly Lazy<ColorSpace> BT_709_Precise = new(() => new(r: 0.212655, g: 0.715158, b: 0.072187));

	// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.601-7-201103-I!!PDF-E.pdf
	//internal static readonly Lazy<ColorSpace> BT_601 = new(() => new(r: 0.299, g: 0.587, b: 0.114));

	// https://www5.in.tum.de/lehre/vorlesungen/graphik/info/csc/COL_33.htm
	//internal static readonly Lazy<ColorSpace> SMPTE_240M = new(() => new(r: 0.2122, g: 0.7013, b: 0.0865));

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
