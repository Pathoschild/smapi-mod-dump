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
[StructLayout(LayoutKind.Explicit, Pack = sizeof(byte), Size = sizeof(byte))]
struct Fixed8 : IEquatable<Fixed8>, IEquatable<byte>, ILongHash {
	internal static readonly Fixed8 Zero = new(0);
	internal static readonly Fixed8 Max = new(byte.MaxValue);

	[FieldOffset(0)]
	private byte InternalValue = 0;

	internal readonly byte Value => InternalValue;

	[MethodImpl(MethodImpl.Hot)]
	internal static byte FromU16(ushort value) => value.Color16to8();

	internal readonly Fixed16 Widen => Value.Color8To16();
	internal readonly float Real => Value.Color8ToFloat();

	[MethodImpl(MethodImpl.Hot)]
	internal Fixed8(byte value) => InternalValue = value;
	[MethodImpl(MethodImpl.Hot)]
	internal Fixed8(Fixed8 value) => InternalValue = value.InternalValue;
	[MethodImpl(MethodImpl.Hot)]
	internal Fixed8(Fixed16 value) => InternalValue = FromU16((ushort)value);

	[MethodImpl(MethodImpl.Hot)]
	private static uint InternalDivide(Fixed8 numerator, Fixed8 denominator) {
		uint numeratorWidened = ((uint)numerator.InternalValue) << 16;
		numeratorWidened -= numerator.InternalValue;
		return numeratorWidened / denominator.InternalValue;
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed8 operator /(Fixed8 numerator, Fixed8 denominator) {
		if (denominator == Fixed8.Zero) {
			return numerator;
		}
		var result = InternalDivide(numerator, denominator);
		return (byte)(result >> 8);
	}

	[MethodImpl(MethodImpl.Hot)]
	internal Fixed8 ClampedDivide(Fixed8 denominator) {
		if (denominator == Fixed8.Zero) {
			return 0;
		}
		var result = InternalDivide(this, denominator);
		// Check if it oversaturated the value
		//if ((result & 0xFFFF_0000) != 0) {
		//	return Fixed8.Max;
		//}
		return (byte)(result >> 8);
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed8 operator %(Fixed8 numerator, Fixed8 denominator) {
		if (denominator == Fixed8.Zero) {
			return numerator;
		}
		var result = InternalDivide(numerator, denominator);
		return (byte)result;
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed8 operator *(Fixed8 lhs, Fixed8 rhs) {
		int intermediate = lhs.InternalValue * rhs.InternalValue;
		intermediate += byte.MaxValue;
		return new((byte)(intermediate >> 8));
	}

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed8 operator +(Fixed8 lhs, Fixed8 rhs) => (byte)(lhs.InternalValue + rhs.InternalValue);

	[MethodImpl(MethodImpl.Hot)]
	public static Fixed8 operator -(Fixed8 lhs, Fixed8 rhs) => (byte)(lhs.InternalValue + rhs.InternalValue);

	[MethodImpl(MethodImpl.Hot)]
	internal static Fixed8 AddClamped(Fixed8 lhs, Fixed8 rhs) => (byte)Math.Min(byte.MaxValue, lhs.InternalValue + rhs.InternalValue);
	[MethodImpl(MethodImpl.Hot)]
	internal readonly Fixed8 AddClamped(Fixed8 other) => AddClamped(this, other);
	[MethodImpl(MethodImpl.Hot)]
	internal static Fixed8 SubtractClamped(Fixed8 lhs, Fixed8 rhs) => (byte)Math.Max(byte.MinValue, lhs.InternalValue - rhs.InternalValue);
	[MethodImpl(MethodImpl.Hot)]
	internal readonly Fixed8 SubtractClamped(Fixed8 other) => SubtractClamped(this, other);

	[MethodImpl(MethodImpl.Hot)]
	public static bool operator ==(Fixed8 lhs, Fixed8 rhs) => lhs.InternalValue == rhs.InternalValue;
	[MethodImpl(MethodImpl.Hot)]
	public static bool operator !=(Fixed8 lhs, Fixed8 rhs) => lhs.InternalValue != rhs.InternalValue;

	[MethodImpl(MethodImpl.Hot)]
	public override readonly bool Equals(object? obj) {
		if (obj is Fixed8 valueF) {
			return this == valueF;
		}
		if (obj is byte valueB) {
			return this.InternalValue == valueB;
		}
		return false;
	}

	[MethodImpl(MethodImpl.Hot)]
	internal readonly bool Equals(Fixed8 other) => this == other;
	[MethodImpl(MethodImpl.Hot)]
	internal readonly bool Equals(byte other) => this == (Fixed8)other;

	[MethodImpl(MethodImpl.Hot)]
	readonly bool IEquatable<Fixed8>.Equals(Fixed8 other) => this.Equals(other);
	[MethodImpl(MethodImpl.Hot)]
	readonly bool IEquatable<byte>.Equals(byte other) => this.Equals(other);

	[MethodImpl(MethodImpl.Hot)]
	public override readonly int GetHashCode() => InternalValue.GetHashCode();

	[MethodImpl(MethodImpl.Hot)]
	public static explicit operator byte(Fixed8 value) => value.InternalValue;
	[MethodImpl(MethodImpl.Hot)]
	public static implicit operator Fixed8(byte value) => new(value);
	[MethodImpl(MethodImpl.Hot)]
	public static explicit operator Fixed16(Fixed8 value) => new(Fixed16.FromU8(value.InternalValue));

	[MethodImpl(MethodImpl.Hot)]
	internal static Span<float> ConvertToReal(ReadOnlySpan<Fixed8> values) {
		var result = SpanExt.MakeUninitialized<float>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].Real;
		}
		return result;
	}

	[MethodImpl(MethodImpl.Hot)]
	internal static Span<Fixed8> ConvertFromReal(ReadOnlySpan<float> values) {
		var result = SpanExt.MakeUninitialized<Fixed8>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = ColorHelpers.ScalarToValue8(values[i]);
		}
		return result;
	}

	[MethodImpl(MethodImpl.Hot)]
	readonly ulong ILongHash.GetLongHashCode() => InternalValue.GetLongHashCode();
}
