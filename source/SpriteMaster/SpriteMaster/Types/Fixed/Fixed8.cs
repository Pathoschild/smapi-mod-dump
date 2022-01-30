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
using System.Runtime.InteropServices;

namespace SpriteMaster.Types.Fixed;

[StructLayout(LayoutKind.Explicit, Pack = sizeof(byte), Size = sizeof(byte))]
struct Fixed8 : IEquatable<Fixed8>, IEquatable<byte>, ILongHash {
	public static readonly Fixed8 Zero = new(0);
	public static readonly Fixed8 Max = new(byte.MaxValue);

	[FieldOffset(0)]
	private byte InternalValue = 0;

	internal readonly byte Value => InternalValue;

	internal static byte FromU16(ushort value) => value.Color16to8();

	internal readonly Fixed16 Widen => Value.Color8To16();
	internal readonly float Real => Value.Color8ToFloat();

	internal Fixed8(byte value) => InternalValue = value;
	internal Fixed8(Fixed8 value) => InternalValue = value.InternalValue;
	internal Fixed8(Fixed16 value) => InternalValue = FromU16((ushort)value);

	public static Fixed8 operator /(Fixed8 numerator, Fixed8 denominator) {
		if (denominator == Fixed8.Zero) {
			return numerator;
		}
		uint numeratorWidened = ((uint)numerator.InternalValue) << 16;
		numeratorWidened -= numerator.InternalValue;
		uint result = numeratorWidened / denominator.InternalValue;
		return (byte)(result >> 8);
	}

	public static Fixed8 operator *(Fixed8 lhs, Fixed8 rhs) {
		int intermediate = lhs.InternalValue * rhs.InternalValue;
		intermediate += byte.MaxValue;
		return new((byte)(intermediate >> 8));
	}

	public static bool operator ==(Fixed8 lhs, Fixed8 rhs) => lhs.InternalValue == rhs.InternalValue;
	public static bool operator !=(Fixed8 lhs, Fixed8 rhs) => lhs.InternalValue != rhs.InternalValue;

	public override readonly bool Equals(object? obj) {
		if (obj is Fixed8 valueF) {
			return this == valueF;
		}
		if (obj is byte valueB) {
			return this.InternalValue == valueB;
		}
		return false;
	}

	internal readonly bool Equals(Fixed8 other) => this == other;
	internal readonly bool Equals(byte other) => this == (Fixed8)other;

	readonly bool IEquatable<Fixed8>.Equals(Fixed8 other) => this.Equals(other);
	readonly bool IEquatable<byte>.Equals(byte other) => this.Equals(other);

	public override readonly int GetHashCode() => InternalValue.GetHashCode();

	public static explicit operator byte(Fixed8 value) => value.InternalValue;
	public static implicit operator Fixed8(byte value) => new(value);
	public static explicit operator Fixed16(Fixed8 value) => new(Fixed16.FromU8(value.InternalValue));

	internal static Span<float> ConvertToReal(ReadOnlySpan<Fixed8> values) {
		var result = SpanExt.MakeUninitialized<float>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].Real;
		}
		return result;
	}

	internal static Span<Fixed8> ConvertFromReal(ReadOnlySpan<float> values) {
		var result = SpanExt.MakeUninitialized<Fixed8>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = ColorHelpers.ScalarToValue8(values[i]);
		}
		return result;
	}

	readonly ulong ILongHash.GetLongHashCode() => InternalValue.GetLongHashCode();
}
