/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Types.Interlocking;

[DebuggerDisplay("{Value}")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong))]
internal struct InterlockedULong :
	IComparable,
	IComparable<ulong>,
	IComparable<InterlockedULong>,
	IEquatable<ulong>,
	IEquatable<InterlockedULong> {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(object? obj) => obj switch {
		ulong value => CompareTo(value),
		InterlockedULong value => CompareTo(value),
		_ => ThrowHelper.ThrowArgumentException<int>($"{obj} is neither type {typeof(ulong)} nor {typeof(InterlockedULong)}", nameof(obj))
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly void Set(ulong value) => Unsafe.AsRef(_Value) = value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly ulong Get() => Interlocked.Read(ref Unsafe.AsRef(_Value));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(ulong other) => Value.CompareTo(other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly int CompareTo(InterlockedULong other) => Value.CompareTo(other.Value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly int GetHashCode() => Value.GetHashCode();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override readonly bool Equals(object? obj) => obj switch {
		ulong value => Equals(value),
		InterlockedULong value => Equals(value),
		_ => ThrowHelper.ThrowArgumentException<bool>($"{obj} is neither type {typeof(ulong)} nor {typeof(InterlockedULong)}", nameof(obj))
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(ulong other) => Value.Equals(other);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly bool Equals(/*in*/ InterlockedULong other) => Value.Equals(other.Value);

	private ulong _Value;
	internal readonly ulong Value {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => Get();
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => Set(value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal InterlockedULong(ulong value = default) : this() => Value = value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator ulong(in InterlockedULong value) => value.Value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator InterlockedULong(ulong value) => new(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(in InterlockedULong lhs, in InterlockedULong rhs) => lhs.Value == rhs.Value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(in InterlockedULong lhs, in InterlockedULong rhs) => lhs.Value != rhs.Value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(in InterlockedULong lhs, ulong rhs) => lhs.Value == rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(in InterlockedULong lhs, ulong rhs) => lhs.Value != rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(ulong lhs, in InterlockedULong rhs) => lhs == rhs.Value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(ulong lhs, in InterlockedULong rhs) => lhs != rhs.Value;
}
