/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
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

[DebuggerDisplay("{Value}")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(byte), Size = sizeof(byte))]
internal readonly struct Fixed8 : IEquatable<Fixed8>, IEquatable<byte>, ILongHash {
	internal static readonly Fixed8 Zero = new(0);
	internal static readonly Fixed8 Max = new(byte.MaxValue);

	internal readonly byte Value = 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static byte FromU16(ushort value) => value.Color16to8();

	internal readonly Fixed16 Widen => Value.Color8To16();
	internal readonly double Real => Value.ValueToScalar();
	internal readonly float RealF => Value.ValueToScalarF();

	[MethodImpl(MethodImpl.Inline)]
	internal Fixed8(byte value) => Value = value;
	[MethodImpl(MethodImpl.Inline)]
	internal Fixed8(Fixed8 value) => Value = value.Value;
	[MethodImpl(MethodImpl.Inline)]
	internal Fixed8(Fixed16 value) => Value = FromU16((ushort)value);

	[MethodImpl(MethodImpl.Inline)]
	private static uint InternalDivide(Fixed8 numerator, Fixed8 denominator) {
		uint numeratorWidened = ((uint)numerator.Value) << 16;
		numeratorWidened -= numerator.Value;
		return numeratorWidened / denominator.Value;
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed8 operator /(Fixed8 numerator, Fixed8 denominator) {
		if (denominator == Zero) {
			return numerator;
		}
		var result = InternalDivide(numerator, denominator);
		return (byte)(result >> 8);
	}

	[MethodImpl(MethodImpl.Inline)]
	internal readonly Fixed8 ClampedDivide(Fixed8 denominator) {
		if (denominator == Zero) {
			return 0;
		}
		var result = InternalDivide(this, denominator);
		// Check if it oversaturated the value
		//if ((result & 0xFFFF_0000) != 0) {
		//	return Fixed8.Max;
		//}
		result >>= 8;
		if (result > byte.MaxValue) {
			return byte.MaxValue;
		}
		return (byte)result;
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed8 operator %(Fixed8 numerator, Fixed8 denominator) {
		if (denominator == Zero) {
			return numerator;
		}
		var result = InternalDivide(numerator, denominator);
		return (byte)result;
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed8 operator *(Fixed8 lhs, Fixed8 rhs) {
		int intermediate = lhs.Value * rhs.Value;
		intermediate += byte.MaxValue;
		return new((byte)(intermediate >> 8));
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed8 operator +(Fixed8 lhs, Fixed8 rhs) => (byte)(lhs.Value + rhs.Value);

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed8 operator -(Fixed8 lhs, Fixed8 rhs) => (byte)(lhs.Value + rhs.Value);

	[MethodImpl(MethodImpl.Inline)]
	internal static Fixed8 AddClamped(Fixed8 lhs, Fixed8 rhs) => (byte)Math.Min(byte.MaxValue, lhs.Value + rhs.Value);
	[MethodImpl(MethodImpl.Inline)]
	internal readonly Fixed8 AddClamped(Fixed8 other) => AddClamped(this, other);
	[MethodImpl(MethodImpl.Inline)]
	internal static Fixed8 SubtractClamped(Fixed8 lhs, Fixed8 rhs) => (byte)Math.Max(byte.MinValue, lhs.Value - rhs.Value);
	[MethodImpl(MethodImpl.Inline)]
	internal readonly Fixed8 SubtractClamped(Fixed8 other) => SubtractClamped(this, other);

	[MethodImpl(MethodImpl.Inline)]
	public static bool operator ==(Fixed8 lhs, Fixed8 rhs) => lhs.Value == rhs.Value;
	[MethodImpl(MethodImpl.Inline)]
	public static bool operator !=(Fixed8 lhs, Fixed8 rhs) => lhs.Value != rhs.Value;

	[MethodImpl(MethodImpl.Inline)]
	public override readonly bool Equals(object? obj) {
		return obj switch {
			Fixed8 valueF => this == valueF,
			byte valueB => Value == valueB,
			_ => false
		};
	}

	[MethodImpl(MethodImpl.Inline)]
	public readonly bool Equals(Fixed8 other) => this == other;
	[MethodImpl(MethodImpl.Inline)]
	public readonly bool Equals(byte other) => this == (Fixed8)other;

	[MethodImpl(MethodImpl.Inline)]
	public override readonly int GetHashCode() => Value.GetHashCode();

	[MethodImpl(MethodImpl.Inline)]
	public static explicit operator byte(Fixed8 value) => value.Value;
	[MethodImpl(MethodImpl.Inline)]
	public static implicit operator Fixed8(byte value) => new(value);
	[MethodImpl(MethodImpl.Inline)]
	public static explicit operator Fixed16(Fixed8 value) => new(Fixed16.FromU8(value.Value));

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<double> ConvertToReal(ReadOnlySpan<Fixed8> values) {
		var result = SpanExt.Make<double>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].Real;
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<float> ConvertToRealF(ReadOnlySpan<Fixed8> values) {
		var result = SpanExt.Make<float>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].RealF;
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<Fixed8> ConvertFromReal(ReadOnlySpan<double> values) {
		var result = SpanExt.Make<Fixed8>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].ScalarToValue8();
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<Fixed8> ConvertFromReal(ReadOnlySpan<float> values) {
		var result = SpanExt.Make<Fixed8>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].ScalarToValue8();
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	public readonly ulong GetLongHashCode() => Value.GetLongHashCode();
}
