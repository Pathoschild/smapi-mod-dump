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

[StructLayout(LayoutKind.Explicit, Pack = sizeof(ushort), Size = sizeof(ushort))]
struct Fixed16 : IEquatable<Fixed16>, IEquatable<ushort>, ILongHash {
	public static readonly Fixed16 Zero = new((ushort)0);
	public static readonly Fixed16 Max = new(ushort.MaxValue);

	[FieldOffset(0)]
	private ushort InternalValue = 0;

	internal readonly ushort Value => InternalValue;

	internal static ushort FromU8(byte value) => value.Color8To16();

	internal readonly Fixed8 Narrow => Value.Color16to8();
	internal readonly float Real => (float)ColorHelpers.ValueToScalar(Value);

	internal Fixed16(ushort value) => InternalValue = value;
	internal Fixed16(Fixed16 value) => InternalValue = value.InternalValue;
	internal Fixed16(Fixed8 value) => InternalValue = FromU8((byte)value);

	public static Fixed16 operator /(Fixed16 numerator, Fixed16 denominator) {
		if (denominator == Fixed16.Zero) {
			return numerator;
		}
		ulong numeratorWidened = ((ulong)numerator.InternalValue) << 32;
		numeratorWidened -= numerator.InternalValue;
		ulong result = numeratorWidened / denominator.InternalValue;
		return (ushort)(result >> 16);
	}

	public static Fixed16 operator *(Fixed16 lhs, Fixed16 rhs) {
		int intermediate = lhs.InternalValue * rhs.InternalValue;
		intermediate += ushort.MaxValue;
		return (ushort)(intermediate >> 16);
	}

	public static bool operator ==(Fixed16 lhs, Fixed16 rhs) => lhs.InternalValue == rhs.InternalValue;
	public static bool operator !=(Fixed16 lhs, Fixed16 rhs) => lhs.InternalValue != rhs.InternalValue;

	public override readonly bool Equals(object? obj) {
		if (obj is Fixed16 valueF) {
			return this == valueF;
		}
		if (obj is byte valueB) {
			return this.InternalValue == valueB;
		}
		return false;
	}

	internal readonly bool Equals(Fixed16 other) => this == other;
	internal readonly bool Equals(ushort other) => this == (Fixed16)other;

	readonly bool IEquatable<Fixed16>.Equals(Fixed16 other) => this.Equals(other);
	readonly bool IEquatable<ushort>.Equals(ushort other) => this.Equals(other);

	public override readonly int GetHashCode() => InternalValue.GetHashCode();

	public static explicit operator ushort(Fixed16 value) => value.InternalValue;
	public static implicit operator Fixed16(ushort value) => new(value);
	public static explicit operator Fixed8(Fixed16 value) => new(Fixed8.FromU16(value.InternalValue));

	internal static Span<float> ConvertToReal(ReadOnlySpan<Fixed16> values) {
		var result = SpanExt.MakeUninitialized<float>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = values[i].Real;
		}
		return result;
	}

	internal static Span<Fixed16> ConvertFromReal(ReadOnlySpan<float> values) {
		var result = SpanExt.MakeUninitialized<Fixed16>(values.Length);
		for (int i = 0; i < values.Length; ++i) {
			result[i] = ColorHelpers.ScalarToValue16(values[i]);
		}
		return result;
	}

	readonly ulong ILongHash.GetLongHashCode() => InternalValue.GetLongHashCode();
}
