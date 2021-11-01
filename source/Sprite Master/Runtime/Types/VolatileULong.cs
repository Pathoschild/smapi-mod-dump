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

namespace SpriteMaster.Types {
	[DebuggerDisplay("{Value}")]

	[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong))]
	public struct VolatileULong :
		IComparable,
		IComparable<ulong>,
		IComparable<VolatileULong>,
		IEquatable<ulong>,
		IEquatable<VolatileULong>
	{
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public /*readonly*/ int CompareTo (object obj) => obj switch {
			ulong value => CompareTo(value),
			VolatileULong value => CompareTo(value),
			_ => throw new ArgumentException($"{obj} is neither type {typeof(ulong)} nor {typeof(VolatileULong)}"),
		};

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public /*readonly*/ int CompareTo (ulong other) => Value.CompareTo(other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public /*readonly*/ int CompareTo (/*in*/ VolatileULong other) => Value.CompareTo(other.Value);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public override /*readonly*/ int GetHashCode() => Value.GetHashCode();

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public override /*readonly*/ bool Equals (object obj) => obj switch {
			ulong value => Equals(value),
			VolatileULong value => Equals(value),
			_ => throw new ArgumentException($"{obj} is neither type {typeof(ulong)} nor {typeof(VolatileULong)}"),
		};

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public /*readonly*/ bool Equals (ulong other) => Value.Equals(other);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public /*readonly*/ bool Equals (/*in*/ VolatileULong other) => Value.Equals(other.Value);

		private long _Value;
		public ulong Value {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			/*readonly*/ get => unchecked((ulong)Interlocked.Read(ref _Value));
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set => Interlocked.Exchange(ref _Value, unchecked((long)value));
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public VolatileULong (ulong value = default) : this() => Value = value;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator ulong (in VolatileULong value) => value.Value;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static implicit operator VolatileULong (ulong value) => new VolatileULong(value);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in VolatileULong lhs, in VolatileULong rhs) => lhs.Value == rhs.Value;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in VolatileULong lhs, in VolatileULong rhs) => lhs.Value != rhs.Value;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (in VolatileULong lhs, ulong rhs) => lhs.Value == rhs;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (in VolatileULong lhs, ulong rhs) => lhs.Value != rhs;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator == (ulong lhs, in VolatileULong rhs) => lhs == rhs.Value;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool operator != (ulong lhs, in VolatileULong rhs) => lhs != rhs.Value;
	}
}
