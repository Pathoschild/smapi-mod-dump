/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class Integers {
	[DebuggerStepThrough, DebuggerHidden()]
	[Conditional("DEBUG")]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static unsafe void CheckBit<T>(int bit) where T : unmanaged {
#if DEBUG
		if (bit >= sizeof(T) * 8) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this bool value, int bit) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this sbyte value, int bit) {
		CheckBit<sbyte>(bit);
		return (sbyte)((sbyte)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this byte value, int bit) {
		CheckBit<byte>(bit);
		return (byte)((byte)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this short value, int bit) {
		CheckBit<short>(bit);
		return (short)((short)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this ushort value, int bit) {
		CheckBit<ushort>(bit);
		return (ushort)((ushort)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this int value, int bit) {
		CheckBit<int>(bit);
		return ((value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this uint value, int bit) {
		CheckBit<uint>(bit);
		return ((value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this long value, int bit) {
		CheckBit<long>(bit);
		return ((int)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this ulong value, int bit) {
		CheckBit<ulong>(bit);
		return ((int)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetBit(this in BigInteger value, int bit) => ((int)(value >> bit) & 1) != 0;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool SetBit(this ref bool value, int bit) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		value = true;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte SetBit(this ref sbyte value, int bit) {
		CheckBit<sbyte>(bit);
		value |= (sbyte)(1 << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte SetBit(this ref byte value, int bit) {
		CheckBit<byte>(bit);
		value |= (byte)(1U << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short SetBit(this ref short value, int bit) {
		CheckBit<short>(bit);
		value |= (short)(1 << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort SetBit(this ref ushort value, int bit) {
		CheckBit<ushort>(bit);
		value |= (ushort)(1U << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int SetBit(this ref int value, int bit) {
		CheckBit<int>(bit);
		value |= (int)(1 << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint SetBit(this ref uint value, int bit) {
		CheckBit<uint>(bit);
		value |= (uint)(1U << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SetBit(this ref long value, int bit) {
		CheckBit<long>(bit);
		value |= (long)(1L << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong SetBit(this ref ulong value, int bit) {
		CheckBit<ulong>(bit);
		value |= (ulong)(1UL << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger SetBit(this ref BigInteger value, int bit) {
		CheckBit<ulong>(bit);
		value |= BigInteger.One << bit;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool ClearBit(this ref bool value, int bit) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		value = false;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte ClearBit(this ref sbyte value, int bit) {
		CheckBit<sbyte>(bit);
		value &= (sbyte)~(1 << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte ClearBit(this ref byte value, int bit) {
		CheckBit<byte>(bit);
		value &= (byte)~(1U << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short ClearBit(this ref short value, int bit) {
		CheckBit<short>(bit);
		value &= (short)~(1 << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort ClearBit(this ref ushort value, int bit) {
		CheckBit<ushort>(bit);
		value &= (ushort)~(1U << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int ClearBit(this ref int value, int bit) {
		CheckBit<int>(bit);
		value &= (int)~(1 << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint ClearBit(this ref uint value, int bit) {
		CheckBit<uint>(bit);
		value &= (uint)~(1U << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long ClearBit(this ref long value, int bit) {
		CheckBit<long>(bit);
		value &= (long)~(1L << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong ClearBit(this ref ulong value, int bit) {
		CheckBit<ulong>(bit);
		value &= (ulong)~(1UL << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger ClearBit(this ref BigInteger value, int bit) {
		CheckBit<ulong>(bit);
		// TODO : I don't think this will necessarily work, as the BigInteger shifted might not be large eonugh to mask against the entirety of 'value'.
		value &= ~(BigInteger.One << bit);
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool SetBit(this ref bool value, int bit, bool condition) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		value = condition;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte SetBit(this ref sbyte value, int bit, bool condition) {
		CheckBit<sbyte>(bit);
		var mask = (byte)(1U << bit);
		var flag = condition ? byte.MaxValue : default;
		var Value = value;
		Value = (sbyte)((sbyte)(Value & (sbyte)~mask) | (sbyte)(flag & mask));
		return value = Value;
	}

	// https://graphics.stanford.edu/~seander/bithacks.html#ConditionalSetOrClearBitsWithoutBranching
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte SetBit(this ref byte value, int bit, bool condition) {
		CheckBit<byte>(bit);
		var mask = (byte)(1U << bit);
		var flag = condition ? byte.MaxValue : default;
		var Value = value;
		Value = (byte)((byte)(Value & (byte)~mask) | (byte)(flag & mask));
		return value = Value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short SetBit(this ref short value, int bit, bool condition) {
		CheckBit<short>(bit);
		var mask = (ushort)(1U << bit);
		var flag = condition ? ushort.MaxValue : default;
		var Value = (ushort)value;
		Value = (ushort)((ushort)(Value & (ushort)~mask) | (ushort)(flag & mask));
		return value = (short)Value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort SetBit(this ref ushort value, int bit, bool condition) {
		CheckBit<ushort>(bit);
		var mask = (ushort)(1U << bit);
		var flag = condition ? ushort.MaxValue : default;
		var Value = value;
		Value = (ushort)((ushort)(Value & (ushort)~mask) | (ushort)(flag & mask));
		return value = Value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int SetBit(this ref int value, int bit, bool condition) {
		CheckBit<int>(bit);
		var mask = (uint)(1U << bit);
		var flag = condition ? uint.MaxValue : default;
		var Value = (uint)value;
		Value = (uint)((uint)(Value & (uint)~mask) | (uint)(flag & mask));
		return value = (int)Value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint SetBit(this ref uint value, int bit, bool condition) {
		CheckBit<uint>(bit);
		var mask = (uint)(1U << bit);
		var flag = condition ? uint.MaxValue : default;
		var Value = value;
		Value = (uint)((uint)(Value & (uint)~mask) | (uint)(flag & mask));
		return value = Value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long SetBit(this ref long value, int bit, bool condition) {
		CheckBit<long>(bit);
		var mask = (ulong)(1UL << bit);
		var flag = condition ? ulong.MaxValue : default;
		var Value = (ulong)value;
		Value = (ulong)((ulong)(Value & (ulong)~mask) | (ulong)(flag & mask));
		return value = (long)Value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong SetBit(this ref ulong value, int bit, bool condition) {
		CheckBit<ulong>(bit);
		var mask = (ulong)(1UL << bit);
		var flag = condition ? ulong.MaxValue : default;
		var Value = value;
		Value = (ulong)((ulong)(Value & (ulong)~mask) | (ulong)(flag & mask));
		return value = Value;
	}

	// Unsigned Conversions

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Byte(this byte value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Byte(this ushort value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Byte(this uint value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Byte(this ulong value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Short(this byte value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Short(this ushort value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Short(this uint value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Short(this ulong value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Int(this byte value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Int(this ushort value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Int(this uint value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Int(this ulong value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Long(this byte value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Long(this ushort value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Long(this uint value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Long(this ulong value) => (ulong)value;

	// Signed Conversions

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Byte(this sbyte value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Byte(this short value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Byte(this int value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Byte(this long value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Short(this sbyte value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Short(this short value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Short(this int value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Short(this long value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Int(this sbyte value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Int(this short value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Int(this int value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Int(this long value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Long(this sbyte value) => (long)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Long(this short value) => (long)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Long(this int value) => (long)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Long(this long value) => (long)value;

	// Widen
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Widen(this byte value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Widen(this ushort value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Widen(this uint value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger Widen(this ulong value) => (BigInteger)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Widen(this sbyte value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Widen(this short value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Widen(this int value) => (long)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger Widen(this long value) => (BigInteger)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger Widen(this BigInteger value) => (BigInteger)value;

	// Narrow
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Narrow(this byte value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Narrow(this ushort value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Narrow(this uint value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Narrow(this ulong value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Narrow(this sbyte value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Narrow(this short value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Narrow(this int value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Narrow(this long value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Narrow(this BigInteger value) => (long)value;

	// Signed/Unsigned
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Unsigned(this byte value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Unsigned(this ushort value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Unsigned(this uint value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Unsigned(this ulong value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Unsigned(this sbyte value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Unsigned(this short value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Unsigned(this int value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Unsigned(this long value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Signed(this byte value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Signed(this ushort value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Signed(this uint value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Signed(this ulong value) => (long)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static sbyte Signed(this sbyte value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Signed(this short value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Signed(this int value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Signed(this long value) => (long)value;

	// Bitwise Fuse

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ushort Fuse(this byte lhs, byte rhs) => (lhs.Widen() | (rhs.Widen() << 8)).Unsigned().Short();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Fuse(this ushort lhs, ushort rhs) => (lhs.Widen() | (rhs.Widen() << 16)).Unsigned().Int();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Fuse(this uint lhs, uint rhs) => (lhs.Widen() | (rhs.Widen() << 32)).Unsigned().Long();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger Fuse(this ulong lhs, ulong rhs) => (lhs.Widen() | (rhs.Widen() << 64));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static short Fuse(this sbyte lhs, sbyte rhs) => (lhs.Unsigned().Widen() | (rhs.Unsigned().Widen() << 8)).Signed().Short();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Fuse(this short lhs, short rhs) => (lhs.Unsigned().Widen() | (rhs.Unsigned().Widen() << 16)).Signed().Int();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Fuse(this int lhs, int rhs) => (lhs.Unsigned().Widen() | (rhs.Unsigned().Widen() << 32)).Signed().Long();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BigInteger Fuse(this long lhs, long rhs) => (lhs.Widen() | (rhs.Widen() << 64));

	private enum IntRangeDirection {
		Forward,
		Reverse
	}

	private struct IntRangeEnumerator : IEnumerator<int>, IEnumerable<int> {
		public int Current { get; private set; }
		private readonly int Start;
		private readonly int End;

		object IEnumerator.Current => Current;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal IntRangeEnumerator(int start, int end, IntRangeDirection direction) {
			switch (direction) {
				case IntRangeDirection.Forward:
					--start; break;
				case IntRangeDirection.Reverse:
					++start; break;
			}
			Start = start;
			End = end;
			Current = Start;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public bool MoveNext() {
			if (Current == End) {
				return false;
			}
			if (Current < End) {
				++Current;
			}
			else {
				--Current;
			}
			return true;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Reset() => Current = Start;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public IEnumerator<int> GetEnumerator() => this;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		IEnumerator IEnumerable.GetEnumerator() => this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> RangeTo(this int from, int to) {
		IntRangeDirection direction;
		if (from < to) {
			--to;
			direction = IntRangeDirection.Forward;
		}
		else if (from > to) {
			++to;
			direction = IntRangeDirection.Reverse;
		}
		else {
			return EnumerableF.EmptyF<int>();
		}
		return new IntRangeEnumerator(from, to, direction);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> RangeToInclusive(this int from, int to) =>
		new IntRangeEnumerator(from, to, (from <= to) ? IntRangeDirection.Forward : IntRangeDirection.Reverse);

	private struct UIntRangeEnumerator : IEnumerator<uint>, IEnumerable<uint> {
		public uint Current { get; private set; }
		private readonly uint Start;
		private readonly uint End;

		object IEnumerator.Current => Current;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal UIntRangeEnumerator(uint start, uint end, IntRangeDirection direction) {
			switch (direction) {
				case IntRangeDirection.Forward:
					--start;
					break;
				case IntRangeDirection.Reverse:
					++start;
					break;
			}
			Start = start;
			End = end;
			Current = Start;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public bool MoveNext() {
			if (Current == End) {
				return false;
			}
			if (Current < End) {
				++Current;
			}
			else {
				--Current;
			}
			return true;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Reset() => Current = Start;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public IEnumerator<uint> GetEnumerator() => this;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		IEnumerator IEnumerable.GetEnumerator() => this;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<uint> RangeTo(this uint from, uint to) {
		IntRangeDirection direction;
		if (from < to) {
			--to;
			direction = IntRangeDirection.Forward;
		}
		else if (from > to) {
			++to;
			direction = IntRangeDirection.Reverse;
		}
		else {
			return EnumerableF.EmptyF<uint>();
		}
		return new UIntRangeEnumerator(from, to, direction);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<uint> RangeToInclusive(this uint from, uint to) =>
		new UIntRangeEnumerator(from, to, (from <= to) ? IntRangeDirection.Forward : IntRangeDirection.Reverse);

	//[MethodImpl(Runtime.MethodImpl.Optimize)]
	//internal static IEnumerable<long> RangeTo(this long from, long to) => EnumerableF.RangeF(from, to);
}
