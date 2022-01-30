/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Numerics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static partial class Integer {
	// Unsigned Conversions

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe byte ToByte(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe sbyte ToSByte(this bool value) => value.ReinterpretAs<sbyte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe short ToShort(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe ushort ToUShort(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe int ToInt(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe uint ToUInt(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe long ToLong(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe ulong ToULong(this bool value) => value.ReinterpretAs<byte>();

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
}
