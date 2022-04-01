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
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static partial class Integer {
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
}
