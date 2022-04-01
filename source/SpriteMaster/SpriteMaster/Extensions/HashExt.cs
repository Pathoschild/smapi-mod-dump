/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Text;

namespace SpriteMaster.Extensions;

static class HashExt {
	internal static int GetSafeHash(this string value) => (int)Hashing.Hash(Encoding.Unicode.GetBytes(value ?? ""));
	//internal static int GetSafeHash(this char[] value) => (int)Hashing.Hash(Encoding.Unicode.GetBytes(value ?? Array.Empty<char>()));
	internal static int GetSafeHash(this StringBuilder value) => value.ToString().GetSafeHash();

	internal static int GetSafeHash(this byte value) => value.GetHashCode();
	internal static int GetSafeHash(this sbyte value) => value.GetHashCode();
	internal static int GetSafeHash(this ushort value) => value.GetHashCode();
	internal static int GetSafeHash(this short value) => value.GetHashCode();
	internal static int GetSafeHash(this uint value) => value.GetHashCode();
	internal static int GetSafeHash(this int value) => value.GetHashCode();
	internal static int GetSafeHash(this ulong value) => value.GetHashCode();
	internal static int GetSafeHash(this long value) => value.GetHashCode();

	internal static int GetSafeHash<T>(this T? value) => value switch {
		null => Hashing.Null32,
		string s => s.GetSafeHash(),
		StringBuilder s => s.ToString().GetSafeHash(),
		_ => value.GetHashCode()
	};
}
