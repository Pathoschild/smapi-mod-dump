/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using MusicMaster.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace MusicMaster.Types.Interlocking;

using UnderlyingInt = Int32;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(UnderlyingInt), Size = sizeof(UnderlyingInt))]
internal struct InterlockedBool :
	IComparable,
	IConvertible,
	IComparable<InterlockedBool>, IComparable<bool>,
	IEquatable<InterlockedBool>, IEquatable<bool>,
	ILongHash
{
	private const UnderlyingInt TrueValue = 1;
	private const UnderlyingInt FalseValue = 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static UnderlyingInt Convert(bool value) =>
		value.ReinterpretAs<byte>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool Convert(UnderlyingInt value) =>
		((byte)value).ReinterpretAs<bool>();

	internal volatile UnderlyingInt ValueInternal = FalseValue;

	internal readonly bool Value {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => Convert(ValueInternal);
		[MethodImpl(Runtime.MethodImpl.Inline)]
#pragma warning disable CS0420
		set => Unsafe.AsRef(in ValueInternal) = Convert(value);
#pragma warning restore CS0420
	}

[MethodImpl(Runtime.MethodImpl.Inline)]
	public InterlockedBool() { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal InterlockedBool(bool value) {
		ValueInternal = Convert(value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal InterlockedBool(InterlockedBool value) : this((bool)value) {
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool Exchange(bool value) =>
#pragma warning disable CS0420
		Convert(Interlocked.Exchange(ref Unsafe.AsRef(in ValueInternal), Convert(value)));
#pragma warning restore CS0420

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool CompareExchange(bool value, bool comparand) =>
#pragma warning disable CS0420
		Convert(Interlocked.CompareExchange(ref Unsafe.AsRef(in ValueInternal), Convert(value), Convert(comparand)));
#pragma warning restore CS0420

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator bool(InterlockedBool value) => Convert(value.ValueInternal);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator InterlockedBool(bool value) => new(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(InterlockedBool lhs, InterlockedBool rhs) => lhs.ValueInternal == rhs.ValueInternal;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(InterlockedBool lhs, InterlockedBool rhs) => lhs.ValueInternal != rhs.ValueInternal;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(InterlockedBool lhs, bool rhs) => lhs.ValueInternal == Convert(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(InterlockedBool lhs, bool rhs) => lhs.ValueInternal != Convert(rhs);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(bool lhs, InterlockedBool rhs) => Convert(lhs) == rhs.ValueInternal;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(bool lhs, InterlockedBool rhs) => Convert(lhs) != rhs.ValueInternal;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(InterlockedBool other) => this == other;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(bool other) => this == other;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly bool Equals(object? other) => other switch {
		InterlockedBool interlockedBool => this == interlockedBool,
		bool boolValue => this == boolValue,
		_ => false
	};

	/*
	  public int CompareTo(bool value)
    {
      if (this == value)
        return 0;
      return !this ? -1 : 1;
    }
	*/

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(InterlockedBool other) => ValueInternal - other.ValueInternal;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(bool other) => ValueInternal - Convert(other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(object? other) => other switch {
		InterlockedBool interlockedBool => CompareTo(interlockedBool),
		bool boolValue => CompareTo(boolValue),
		_ => ThrowHelper.ThrowArgumentException<int>(other?.GetType().FullName ?? "<null>", nameof(other))
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly int GetHashCode() {
		// The hash code of a bool is 0 or 1 - it's an identity hash
		return ValueInternal;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly ulong GetLongHashCode() {
		return Convert(ValueInternal).GetLongHashCode();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly string ToString() => Convert(ValueInternal).ToString();

	#region IConvertible

	readonly TypeCode IConvertible.GetTypeCode() => TypeCode.Boolean;

	readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => Convert(ValueInternal);

	readonly byte IConvertible.ToByte(IFormatProvider? provider) => (byte)ValueInternal;

	readonly char IConvertible.ToChar(IFormatProvider? provider) => System.Convert.ToChar(ValueInternal, provider);

	readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => System.Convert.ToDateTime(ValueInternal, provider);

	readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => System.Convert.ToDecimal(ValueInternal, provider);

	readonly double IConvertible.ToDouble(IFormatProvider? provider) => System.Convert.ToDouble(ValueInternal, provider);

	readonly short IConvertible.ToInt16(IFormatProvider? provider) => (short)ValueInternal;

	readonly int IConvertible.ToInt32(IFormatProvider? provider) => ValueInternal;

	readonly long IConvertible.ToInt64(IFormatProvider? provider) => ValueInternal;

	readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => (sbyte)ValueInternal;

	readonly float IConvertible.ToSingle(IFormatProvider? provider) => System.Convert.ToSingle(ValueInternal, provider);

	readonly string IConvertible.ToString(IFormatProvider? provider) => ToString();

	readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider) =>
		System.Convert.ChangeType(ValueInternal, conversionType, provider);

	readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => (ushort)ValueInternal;

	readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => (uint)ValueInternal;

	ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)ValueInternal;

	#endregion
}
