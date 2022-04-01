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
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SpriteMaster.Runtime;

namespace SpriteMaster.Types.Fixed;

[DebuggerDisplay("{InternalValue}")]
[StructLayout(LayoutKind.Explicit, Pack = sizeof(ushort), Size = sizeof(ushort))]
struct Fixed16 : IEquatable<Fixed16>, IEquatable<ushort>, ILongHash {
	internal static readonly Fixed16 Zero = new((ushort)0);
	internal static readonly Fixed16 Max = new(ushort.MaxValue);

	[FieldOffset(0)]
	private ushort InternalValue = 0;

	internal readonly ushort Value => InternalValue;

	[MethodImpl(MethodImpl.Hot)]
	internal static ushort FromU8(byte value) => value.Color8To16();

	internal readonly Fixed8 Narrow => Value.Color16to8();
	internal readonly float Real => (float)ColorHelpers.ValueToScalar(Value);

	internal static Fixed16 FromReal(float value) => ColorHelpers.ScalarToValue16(value);

	[MethodImpl(MethodImpl.Hot)]
	internal Fixed16(ushort value) => InternalValue = value;
	[MethodImpl(MethodImpl.Hot)]
	internal Fixed16(Fixed16 value) => InternalValue = value.InternalValue;
	[MethodImpl(MethodImpl.Hot)]
	internal Fixed16(Fixed8 value) => InternalValue = FromU8((byte)value);

	[MethodImpl(MethodImpl.Hot)]
	private static ulong InternalDivide(Fixed16 numerator, Fixed16 denominator) {
		ulong numeratorWidened = ((ulong)numerator.InternalValue) << 32;
		numeratorWidened -= numerator.InternalValue;
		return numeratorWidened / denominator.InternalValue;
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed16 operator /(Fixed16 numerator, Fixed16 denominator) {
		if (denominator == Fixed16.Zero) {
			return 0;
		}
		var result = InternalDivide(numerator, denominator);
		return (ushort)(result >> 16);
	}

	[MethodImpl(MethodImpl.Hot)]
	internal Fixed16 ClampedDivide(Fixed16 denominator) {
		if (denominator == Fixed16.Zero) {
			return 0;
		}
		var result = InternalDivide(this, denominator);
		// Check if it oversaturated the value
		//if ((result & 0xFFFF_FFFF_0000_0000) != 0) {
		//	return (Value <= (32U << 8)) ? Fixed16.Zero : Fixed16.Zero;
		//}
		return (ushort)(result >> 16);
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed16 operator %(Fixed16 numerator, Fixed16 denominator) {
		if (denominator == Fixed16.Zero) {
			return 0;
		}
		var result = InternalDivide(numerator, denominator);
		return (ushort)result;
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed16 operator *(Fixed16 lhs, Fixed16 rhs) {
		int intermediate = lhs.InternalValue * rhs.InternalValue;
		intermediate += ushort.MaxValue;
		return (ushort)(intermediate >> 16);
	}

	[MethodImpl(MethodImpl.Hot)]
	public static bool operator ==(Fixed16 lhs, Fixed16 rhs) => lhs.InternalValue == rhs.InternalValue;
	[MethodImpl(MethodImpl.Hot)]
	public static bool operator !=(Fixed16 lhs, Fixed16 rhs) => lhs.InternalValue != rhs.InternalValue;

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed16 operator +(Fixed16 lhs, Fixed16 rhs) => (ushort)(lhs.InternalValue + rhs.InternalValue);

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed16 operator -(Fixed16 lhs, Fixed16 rhs) => (ushort)(lhs.InternalValue + rhs.InternalValue);

	[MethodImpl(MethodImpl.Hot)]
	internal static Fixed16 AddClamped(Fixed16 lhs, Fixed16 rhs) => (ushort)Math.Min(ushort.MaxValue, lhs.InternalValue + rhs.InternalValue);
	[MethodImpl(MethodImpl.Hot)]
	internal readonly Fixed16 AddClamped(Fixed16 other) => AddClamped(this, other);
	[MethodImpl(MethodImpl.Hot)]
	internal static Fixed16 SubtractClamped(Fixed16 lhs, Fixed16 rhs) => (ushort)Math.Max(ushort.MinValue, lhs.InternalValue - rhs.InternalValue);
	[MethodImpl(MethodImpl.Hot)]
	internal readonly Fixed16 SubtractClamped(Fixed16 other) => SubtractClamped(this, other);

	[MethodImpl(MethodImpl.Hot)]
	public override readonly bool Equals(object? obj) {
		if (obj is Fixed16 valueF) {
			return this == valueF;
		}
		if (obj is byte valueB) {
			return this.InternalValue == valueB;
		}
		return false;
	}

	[MethodImpl(MethodImpl.Hot)]
	internal readonly bool Equals(Fixed16 other) => this == other;
	[MethodImpl(MethodImpl.Hot)]
	internal readonly bool Equals(ushort other) => this == (Fixed16)other;

	[MethodImpl(MethodImpl.Hot)]
	readonly bool IEquatable<Fixed16>.Equals(Fixed16 other) => this.Equals(other);
	[MethodImpl(MethodImpl.Hot)]
	readonly bool IEquatable<ushort>.Equals(ushort other) => this.Equals(other);

	[MethodImpl(MethodImpl.Hot)]
	public override readonly int GetHashCode() => InternalValue.GetHashCode();

	[MethodImpl(MethodImpl.Hot)]
	public static explicit operator ushort(Fixed16 value) => value.InternalValue;
	[MethodImpl(MethodImpl.Hot)]
	public static implicit operator Fixed16(ushort value) => new(value);
	[MethodImpl(MethodImpl.Hot)]
	public static explicit operator Fixed8(Fixed16 value) => new(Fixed8.FromU16(value.InternalValue));

	[MethodImpl(MethodImpl.Hot)]
	internal static Span<float> ConvertToReal(ReadOnlySpan<Fixed16> values) {
		var result = SpanExt.MakeUninitialized<float>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].Real;
		}
		return result;
	}

	[MethodImpl(MethodImpl.Hot)]
	internal static Span<Fixed16> ConvertFromReal(ReadOnlySpan<float> values) {
		var result = SpanExt.MakeUninitialized<Fixed16>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = ColorHelpers.ScalarToValue16(values[i]);
		}
		return result;
	}

	[MethodImpl(MethodImpl.Hot)]
	readonly ulong ILongHash.GetLongHashCode() => InternalValue.GetLongHashCode();
}
