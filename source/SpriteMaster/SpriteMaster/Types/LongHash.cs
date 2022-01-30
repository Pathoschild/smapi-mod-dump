/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;
static class LongHash {
	internal const ulong Null = Hashing.Null;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong GetLongHashCode<T>(this T obj) {
		if (obj is ILongHash hashable) {
			return hashable.GetLongHashCode();
		}
		return From(obj);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong From(int hashCode) => Hashing.Combine((ulong)hashCode, (ulong)(~hashCode) << 32);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong From(ILongHash obj) => obj.GetLongHashCode();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong From<T>(T obj) {
		if (obj is ILongHash hashable) {
			return hashable.GetLongHashCode();
		}
		if (obj is null) {
			return Null;
		}
		return From(obj.GetHashCode());
	}
}

internal interface ILongHash {
	internal ulong GetLongHashCode();
}
