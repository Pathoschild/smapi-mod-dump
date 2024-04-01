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
[StructLayout(LayoutKind.Sequential, Pack = sizeof(ushort), Size = sizeof(ushort))]
internal readonly struct Fixed16 : IEquatable<Fixed16>, IEquatable<ushort>, ILongHash {
	internal static readonly Fixed16 Zero = new((ushort)0);
	internal static readonly Fixed16 Max = new(ushort.MaxValue);

	internal readonly ushort Value = 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static ushort FromU8(byte value) => value.Color8To16();

	internal readonly Fixed8 Narrow => Value.Color16to8();
	internal readonly double Real => Value.ValueToScalar();
	internal readonly float RealF => Value.ValueToScalarF();

	internal static Fixed16 FromReal(float value) => value.ScalarToValue16();
	internal static Fixed16 FromReal(double value) => value.ScalarToValue16();

	[MethodImpl(MethodImpl.Inline)]
	internal Fixed16(ushort value) => Value = value;
	[MethodImpl(MethodImpl.Inline)]
	internal Fixed16(Fixed16 value) => Value = value.Value;
	[MethodImpl(MethodImpl.Inline)]
	internal Fixed16(Fixed8 value) => Value = FromU8((byte)value);

	[MethodImpl(MethodImpl.Inline)]
	private static ulong InternalDivide(Fixed16 numerator, Fixed16 denominator) {
		ulong numeratorWidened = ((ulong)numerator.Value) << 32;
		numeratorWidened -= numerator.Value;
		return numeratorWidened / denominator.Value;
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed16 operator /(Fixed16 numerator, Fixed16 denominator) {
		if (denominator == Zero) {
			return 0;
		}
		var result = InternalDivide(numerator, denominator);
		return (ushort)(result >> 16);
	}

	[MethodImpl(MethodImpl.Inline)]
	internal readonly Fixed16 ClampedDivide(Fixed16 denominator) {
		if (denominator == Zero) {
			return 0;
		}
		var result = InternalDivide(this, denominator);
		// Check if it oversaturated the value
		//if ((result & 0xFFFF_FFFF_0000_0000) != 0) {
		//	return (Value <= (32U << 8)) ? Fixed16.Zero : Fixed16.Zero;
		//}
		result >>= 16;
		if (result > ushort.MaxValue) {
			return ushort.MaxValue;
		}
		return (ushort)result;
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed16 operator %(Fixed16 numerator, Fixed16 denominator) {
		if (denominator == Zero) {
			return 0;
		}
		var result = InternalDivide(numerator, denominator);
		return (ushort)result;
	}

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed16 operator *(Fixed16 lhs, Fixed16 rhs) {
		int intermediate = lhs.Value * rhs.Value;
		intermediate += ushort.MaxValue;
		return (ushort)(intermediate >> 16);
	}

	[MethodImpl(MethodImpl.Inline)]
	public static bool operator ==(Fixed16 lhs, Fixed16 rhs) => lhs.Value == rhs.Value;
	[MethodImpl(MethodImpl.Inline)]
	public static bool operator !=(Fixed16 lhs, Fixed16 rhs) => lhs.Value != rhs.Value;

	[MethodImpl(MethodImpl.Inline)]
	public static bool operator >=(Fixed16 lhs, Fixed16 rhs) => lhs.Value >= rhs.Value;
	[MethodImpl(MethodImpl.Inline)]
	public static bool operator <=(Fixed16 lhs, Fixed16 rhs) => lhs.Value <= rhs.Value;

	[MethodImpl(MethodImpl.Inline)]
	public static bool operator >(Fixed16 lhs, Fixed16 rhs) => lhs.Value > rhs.Value;
	[MethodImpl(MethodImpl.Inline)]
	public static bool operator <(Fixed16 lhs, Fixed16 rhs) => lhs.Value < rhs.Value;

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed16 operator +(Fixed16 lhs, Fixed16 rhs) => (ushort)(lhs.Value + rhs.Value);

	[MethodImpl(MethodImpl.Inline)]
	public static Fixed16 operator -(Fixed16 lhs, Fixed16 rhs) => (ushort)(lhs.Value + rhs.Value);

	[MethodImpl(MethodImpl.Inline)]
	internal static Fixed16 AddClamped(Fixed16 lhs, Fixed16 rhs) => (ushort)Math.Min(ushort.MaxValue, lhs.Value + rhs.Value);
	[MethodImpl(MethodImpl.Inline)]
	internal readonly Fixed16 AddClamped(Fixed16 other) => AddClamped(this, other);
	[MethodImpl(MethodImpl.Inline)]
	internal static Fixed16 SubtractClamped(Fixed16 lhs, Fixed16 rhs) => (ushort)Math.Max(ushort.MinValue, lhs.Value - rhs.Value);
	[MethodImpl(MethodImpl.Inline)]
	internal readonly Fixed16 SubtractClamped(Fixed16 other) => SubtractClamped(this, other);

	[MethodImpl(MethodImpl.Inline)]
	public override readonly bool Equals(object? obj) {
		return obj switch {
			Fixed16 valueF => this == valueF,
			ushort valueS => Value == valueS,
			_ => false
		};
	}

	[MethodImpl(MethodImpl.Inline)]
	public readonly bool Equals(Fixed16 other) => this == other;
	[MethodImpl(MethodImpl.Inline)]
	public readonly bool Equals(ushort other) => this == (Fixed16)other;

	[MethodImpl(MethodImpl.Inline)]
	public override readonly int GetHashCode() => Value.GetHashCode();

	[MethodImpl(MethodImpl.Inline)]
	public static explicit operator ushort(Fixed16 value) => value.Value;
	[MethodImpl(MethodImpl.Inline)]
	public static implicit operator Fixed16(ushort value) => new(value);
	[MethodImpl(MethodImpl.Inline)]
	public static explicit operator Fixed8(Fixed16 value) => new(Fixed8.FromU16(value.Value));

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<double> ConvertToReal(ReadOnlySpan<Fixed16> values) {
		var result = SpanExt.Make<double>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].Real;
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<float> ConvertToRealF(ReadOnlySpan<Fixed16> values) {
		var result = SpanExt.Make<float>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].RealF;
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<Fixed16> ConvertFromReal(ReadOnlySpan<double> values) {
		var result = SpanExt.Make<Fixed16>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].ScalarToValue16();
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	internal static Span<Fixed16> ConvertFromReal(ReadOnlySpan<float> values) {
		var result = SpanExt.Make<Fixed16>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].ScalarToValue16();
		}
		return result;
	}

	[MethodImpl(MethodImpl.Inline)]
	public readonly ulong GetLongHashCode() => Value.GetLongHashCode();
}
