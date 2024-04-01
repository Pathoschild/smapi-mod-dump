/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Colors;

[StructLayout(LayoutKind.Auto)]
internal readonly struct Converter {
	private readonly byte[] LinearizeTable8;
	private readonly byte[] DelinearizeTable8;
	private readonly ushort[] LinearizeTable16;
	private readonly ushort[] DelinearizeTable16;

	internal Converter(in ColorSpace colorSpace) {
		LinearizeTable8 = colorSpace.LinearizeTable8;
		DelinearizeTable8 = colorSpace.DelinearizeTable8;
		LinearizeTable16 = colorSpace.LinearizeTable16;
		DelinearizeTable16 = colorSpace.DelinearizeTable16;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly byte Linearize(byte value) => LinearizeTable8[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly byte Linearize(Fixed8 value) => Linearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Color8 Linearize(Color8 color) {
		color.SetRgb(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B)
		);
		return color;
	}
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly ushort Linearize(ushort value) => LinearizeTable16[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Fixed16 Linearize(Fixed16 value) => Linearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Color16 Linearize(Color16 color) {
		color.SetRgb(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B)
		);
		return color;
	}
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly byte Delinearize(byte value) => DelinearizeTable8[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly byte Delinearize(Fixed8 value) => Delinearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Color8 Delinearize(Color8 color) {
		color.SetRgb(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B)
		);
		return color;
	}
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly ushort Delinearize(ushort value) => DelinearizeTable16[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Fixed16 Delinearize(Fixed16 value) => Delinearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Color16 Delinearize(Color16 color) {
		color.SetRgb(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B)
		);
		return color;
	}
}

internal unsafe class ConverterRef {
	internal readonly byte[] LinearizeTable8;
	internal readonly byte[] DelinearizeTable8;
	internal readonly ushort[] LinearizeTable16;
	internal readonly ushort[] DelinearizeTable16;

	internal readonly ushort* LinearizeTable16Ptr;
	internal readonly ushort* DelinearizeTable16Ptr;

	internal ConverterRef(byte[] linearizeTable8, byte[] delinearizeTable8, ushort[] linearizeTable16, ushort[] delinearizeTable16) {
		LinearizeTable8 = linearizeTable8;
		DelinearizeTable8 = delinearizeTable8;
		LinearizeTable16 = linearizeTable16;
		DelinearizeTable16 = delinearizeTable16;

		LinearizeTable16Ptr = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(LinearizeTable16));
		DelinearizeTable16Ptr = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(DelinearizeTable16));
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Linearize(byte value) => LinearizeTable8[value];
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Linearize(Fixed8 value) => Linearize(value.Value);
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color8 Linearize(Color8 color) {
		color.SetRgb(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B)
		);
		return color;
	}
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal ushort Linearize(ushort value) => LinearizeTable16[value];
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal Fixed16 Linearize(Fixed16 value) => Linearize(value.Value);
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color16 Linearize(Color16 color) {
		color.SetRgb(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B)
		);
		return color;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Delinearize(byte value) => DelinearizeTable8[value];
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Delinearize(Fixed8 value) => Delinearize(value.Value);
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color8 Delinearize(Color8 color) {
		color.SetRgb(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B)
		);
		return color;
	}
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal ushort Delinearize(ushort value) => DelinearizeTable16[value];
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal Fixed16 Delinearize(Fixed16 value) => Delinearize(value.Value);
	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color16 Delinearize(Color16 color) {
		color.SetRgb(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B)
		);
		return color;
	}
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct ColorSpace {
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

	internal readonly byte[] LinearizeTable8;
	internal readonly byte[] DelinearizeTable8;
	internal readonly ushort[] LinearizeTable16;
	internal readonly ushort[] DelinearizeTable16;

	private readonly ConverterRef ConverterRef;

	internal readonly Converter GetConverter() => new(this);
	internal readonly ConverterRef GetConverterRef() => ConverterRef;

	internal delegate double CurveDelegateDouble(double scalar);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Linearize(byte value) => LinearizeTable8[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Linearize(Fixed8 value) => Linearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color8 Linearize(Color8 color) {
		color.SetRgb(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B)
		);
		return color;
	}
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal ushort Linearize(ushort value) => LinearizeTable16[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Fixed16 Linearize(Fixed16 value) => Linearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color16 Linearize(Color16 color) {
		color.SetRgb(
			Linearize(color.R),
			Linearize(color.G),
			Linearize(color.B)
		);
		return color;
	}
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Delinearize(byte value) => DelinearizeTable8[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal byte Delinearize(Fixed8 value) => Delinearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color8 Delinearize(Color8 color) {
		color.SetRgb(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B)
		);
		return color;
	}
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal ushort Delinearize(ushort value) => DelinearizeTable16[value];
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Fixed16 Delinearize(Fixed16 value) => Delinearize(value.Value);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Color16 Delinearize(Color16 color) {
		color.SetRgb(
			Delinearize(color.R),
			Delinearize(color.G),
			Delinearize(color.B)
		);
		return color;
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	private static T ConvertTo<U, T>(U value) => (T)Convert.ChangeType(value, typeof(T))!;

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	private static (T[] LinearizeTable, T[] DelinearizeTable) InitializeTable<T>(CurveDelegateDouble linearize, CurveDelegateDouble delinearize, int maxValue) where T : unmanaged {
		Func<T, double> toScalar;
		Func<double, T> fromScalar;

		switch (typeof(T)) {
			case var type when type == typeof(byte):
				toScalar = value => value.ReinterpretAsUnsafe<T, byte>().ValueToScalar();
				fromScalar = value => value.ScalarToValue8().ReinterpretAs<T>();
				break;
			case var type when type == typeof(ushort):
				toScalar = value => value.ReinterpretAsUnsafe<T, ushort>().ValueToScalar();
				fromScalar = value => value.ScalarToValue16().ReinterpretAs<T>();
				break;
			default:
				throw new ArgumentException($"Unknown Table Type: '{typeof(T).Name}'");
		}

		var linearizeTable = GC.AllocateUninitializedArray<T>(maxValue, pinned: true);
		var delinearizeTable = GC.AllocateUninitializedArray<T>(maxValue, pinned: true);
		for (int i = 0; i < maxValue; ++i) {
			linearizeTable[i] = fromScalar(linearize(toScalar(ConvertTo<int, T>(i))));
			delinearizeTable[i] = fromScalar(delinearize(toScalar(ConvertTo<int, T>(i))));
		}

		return (linearizeTable, delinearizeTable);
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal ColorSpace(double r, double g, double b, CurveDelegateDouble linearize, CurveDelegateDouble delinearize) {
		// Correct for precision error, just in case.
		// ReSharper disable once CompareOfFloatsByEqualityOperator
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

		(LinearizeTable8, DelinearizeTable8) = InitializeTable<byte>(linearize, delinearize, byte.MaxValue + 1);
		(LinearizeTable16, DelinearizeTable16) = InitializeTable<ushort>(linearize, delinearize, ushort.MaxValue + 1);

		ConverterRef = new(LinearizeTable8, DelinearizeTable8, LinearizeTable16, DelinearizeTable16);
	}

#if false
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
#endif

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
