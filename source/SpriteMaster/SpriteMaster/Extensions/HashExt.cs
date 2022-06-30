/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Hashing;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster.Extensions;

internal static class HashExt {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong GetSafeHash64(this string value) => value.AsSpan().Hash();
	//internal static int GetSafeHash(this char[] value) => (int)Hashing.Hash(Encoding.Unicode.GetBytes(value ?? Array.Empty<char>()));
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong GetSafeHash64(this StringBuilder value) => value.ToString().GetSafeHash64();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this string value) => (int)value.AsSpan().Hash();
	//internal static int GetSafeHash(this char[] value) => (int)Hashing.Hash(Encoding.Unicode.GetBytes(value ?? Array.Empty<char>()));
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this StringBuilder value) => value.ToString().GetSafeHash();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this byte value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this sbyte value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this ushort value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this short value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this uint value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this int value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this ulong value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this long value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this float value) => value.GetHashCode();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash(this double value) => value.GetHashCode();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GetSafeHash<T>(this T? value) => value switch {
		null => HashUtility.Constants.Bits32.Null,
		string s => s.GetSafeHash(),
		StringBuilder s => s.ToString().GetSafeHash(),
		_ => value.GetHashCode()
	};
}
