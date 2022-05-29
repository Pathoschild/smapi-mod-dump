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

internal static partial class Integer {
	[DebuggerStepThrough, DebuggerHidden]
	[Conditional("DEBUG")]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static unsafe void CheckBit<T>(int bit) where T : unmanaged {
		if (bit >= sizeof(T) * 8) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this bool value, int bit) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this sbyte value, int bit) {
		CheckBit<sbyte>(bit);
		return (sbyte)((sbyte)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this byte value, int bit) {
		CheckBit<byte>(bit);
		return (byte)((byte)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this short value, int bit) {
		CheckBit<short>(bit);
		return (short)((short)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this ushort value, int bit) {
		CheckBit<ushort>(bit);
		return (ushort)((ushort)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this int value, int bit) {
		CheckBit<int>(bit);
		return ((value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this uint value, int bit) {
		CheckBit<uint>(bit);
		return ((value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this long value, int bit) {
		CheckBit<long>(bit);
		return ((int)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this ulong value, int bit) {
		CheckBit<ulong>(bit);
		return ((int)(value >> bit) & 1) != 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool GetBit(this in BigInteger value, int bit) => ((int)(value >> bit) & 1) != 0;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	// ReSharper disable once RedundantAssignment
	internal static bool SetBit(this ref bool value, int bit) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		return value = true;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte SetBit(this ref sbyte value, int bit) {
		CheckBit<sbyte>(bit);
		return value |= (sbyte)(1 << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte SetBit(this ref byte value, int bit) {
		CheckBit<byte>(bit);
		return value |= (byte)(1U << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short SetBit(this ref short value, int bit) {
		CheckBit<short>(bit);
		return value |= (short)(1 << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort SetBit(this ref ushort value, int bit) {
		CheckBit<ushort>(bit);
		return value |= (ushort)(1U << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int SetBit(this ref int value, int bit) {
		CheckBit<int>(bit);
		return value |= 1 << bit;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint SetBit(this ref uint value, int bit) {
		CheckBit<uint>(bit);
		return value |= 1U << bit;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long SetBit(this ref long value, int bit) {
		CheckBit<long>(bit);
		return value |= 1L << bit;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong SetBit(this ref ulong value, int bit) {
		CheckBit<ulong>(bit);
		return value |= 1UL << bit;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	// ReSharper disable once RedundantAssignment
	internal static bool ClearBit(this ref bool value, int bit) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		return value = false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte ClearBit(this ref sbyte value, int bit) {
		CheckBit<sbyte>(bit);
		return value &= (sbyte)~(1 << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte ClearBit(this ref byte value, int bit) {
		CheckBit<byte>(bit);
		return value &= (byte)~(1U << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short ClearBit(this ref short value, int bit) {
		CheckBit<short>(bit);
		return value &= (short)~(1 << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort ClearBit(this ref ushort value, int bit) {
		CheckBit<ushort>(bit);
		return value &= (ushort)~(1U << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int ClearBit(this ref int value, int bit) {
		CheckBit<int>(bit);
		return value &= ~(1 << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint ClearBit(this ref uint value, int bit) {
		CheckBit<uint>(bit);
		return value &= ~(1U << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long ClearBit(this ref long value, int bit) {
		CheckBit<long>(bit);
		return value &= ~(1L << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong ClearBit(this ref ulong value, int bit) {
		CheckBit<ulong>(bit);
		return value &= ~(1UL << bit);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	// ReSharper disable once RedundantAssignment
	internal static bool SetBit(this ref bool value, int bit, bool condition) {
#if DEBUG
		if (bit != 0) {
			throw new ArgumentOutOfRangeException(nameof(bit));
		}
#endif
		return value = condition;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte SetBit(this ref sbyte value, int bit, bool condition) {
		CheckBit<sbyte>(bit);
		var mask = (byte)(1U << bit);
		var flag = condition ? byte.MaxValue : default;
		var localValue = value;
		localValue = (sbyte)((sbyte)(localValue & (sbyte)~mask) | (sbyte)(flag & mask));
		return value = localValue;
	}

	// https://graphics.stanford.edu/~seander/bithacks.html#ConditionalSetOrClearBitsWithoutBranching
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte SetBit(this ref byte value, int bit, bool condition) {
		CheckBit<byte>(bit);
		var mask = (byte)(1U << bit);
		var flag = condition ? byte.MaxValue : default;
		var localValue = value;
		localValue = (byte)((byte)(localValue & (byte)~mask) | (byte)(flag & mask));
		return value = localValue;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short SetBit(this ref short value, int bit, bool condition) {
		CheckBit<short>(bit);
		var mask = (ushort)(1U << bit);
		var flag = condition ? ushort.MaxValue : default;
		var localValue = (ushort)value;
		localValue = (ushort)((ushort)(localValue & (ushort)~mask) | (ushort)(flag & mask));
		return value = (short)localValue;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort SetBit(this ref ushort value, int bit, bool condition) {
		CheckBit<ushort>(bit);
		var mask = (ushort)(1U << bit);
		var flag = condition ? ushort.MaxValue : default;
		var casted = value;
		casted = (ushort)((ushort)(casted & (ushort)~mask) | (ushort)(flag & mask));
		return value = casted;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int SetBit(this ref int value, int bit, bool condition) {
		CheckBit<int>(bit);
		var mask = 1U << bit;
		var flag = condition ? uint.MaxValue : default;
		var casted = (uint)value;
		casted = (casted & ~mask) | (flag & mask);
		return value = (int)casted;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint SetBit(this ref uint value, int bit, bool condition) {
		CheckBit<uint>(bit);
		var mask = 1U << bit;
		var flag = condition ? uint.MaxValue : default;
		var casted = value;
		casted = (casted & ~mask) | (flag & mask);
		return value = casted;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long SetBit(this ref long value, int bit, bool condition) {
		CheckBit<long>(bit);
		var mask = 1UL << bit;
		var flag = condition ? ulong.MaxValue : default;
		var casted = (ulong)value;
		casted = (casted & ~mask) | (flag & mask);
		return value = (long)casted;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong SetBit(this ref ulong value, int bit, bool condition) {
		CheckBit<ulong>(bit);
		var mask = 1UL << bit;
		var flag = condition ? ulong.MaxValue : default;
		var casted = value;
		casted = (casted & ~mask) | (flag & mask);
		return value = casted;
	}
}
