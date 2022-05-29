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

internal static partial class Integer {
	// Unsigned Conversions

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool ToBool(this byte value) => value.ReinterpretAs<bool>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool ToBool(this int value) => value.ReinterpretAs<bool>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte ToByte(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte ToSByte(this bool value) => value.ReinterpretAs<sbyte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short ToShort(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort ToUShort(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int ToInt(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint ToUInt(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long ToLong(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong ToULong(this bool value) => value.ReinterpretAs<byte>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Byte(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Byte(this ushort value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Byte(this uint value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Byte(this ulong value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Short(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Short(this ushort value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Short(this uint value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Short(this ulong value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Int(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Int(this ushort value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Int(this uint value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Int(this ulong value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Long(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Long(this ushort value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Long(this uint value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Long(this ulong value) => value;

	// Signed Conversions

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Byte(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Byte(this short value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Byte(this int value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Byte(this long value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Short(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Short(this short value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Short(this int value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Short(this long value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Int(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Int(this short value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Int(this int value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Int(this long value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Long(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Long(this short value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Long(this int value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Long(this long value) => value;

	// Widen
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Widen(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Widen(this ushort value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Widen(this uint value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BigInteger Widen(this ulong value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Widen(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Widen(this short value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Widen(this int value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BigInteger Widen(this long value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BigInteger Widen(this BigInteger value) => value;

	// Narrow
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Narrow(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Narrow(this ushort value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Narrow(this uint value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Narrow(this ulong value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Narrow(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Narrow(this short value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Narrow(this int value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Narrow(this long value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Narrow(this BigInteger value) => (long)value;

	// Signed/Unsigned
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Unsigned(this byte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Unsigned(this ushort value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Unsigned(this uint value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Unsigned(this ulong value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte Unsigned(this sbyte value) => (byte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Unsigned(this short value) => (ushort)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Unsigned(this int value) => (uint)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Unsigned(this long value) => (ulong)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Signed(this byte value) => (sbyte)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Signed(this ushort value) => (short)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Signed(this uint value) => (int)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Signed(this ulong value) => (long)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static sbyte Signed(this sbyte value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Signed(this short value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Signed(this int value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Signed(this long value) => value;

	// Bitwise Fuse

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ushort Fuse(this byte lhs, byte rhs) => (lhs.Widen() | (rhs.Widen() << 8)).Unsigned().Short();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static uint Fuse(this ushort lhs, ushort rhs) => (lhs.Widen() | (rhs.Widen() << 16)).Unsigned().Int();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Fuse(this uint lhs, uint rhs) => (lhs.Widen() | (rhs.Widen() << 32)).Unsigned().Long();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BigInteger Fuse(this ulong lhs, ulong rhs) => (lhs.Widen() | (rhs.Widen() << 64));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static short Fuse(this sbyte lhs, sbyte rhs) => (lhs.Unsigned().Widen() | (rhs.Unsigned().Widen() << 8)).Signed().Short();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Fuse(this short lhs, short rhs) => (lhs.Unsigned().Widen() | (rhs.Unsigned().Widen() << 16)).Signed().Int();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long Fuse(this int lhs, int rhs) => (lhs.Unsigned().Widen() | (rhs.Unsigned().Widen() << 32)).Signed().Long();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BigInteger Fuse(this long lhs, long rhs) => (lhs.Widen() | (rhs.Widen() << 64));
}
