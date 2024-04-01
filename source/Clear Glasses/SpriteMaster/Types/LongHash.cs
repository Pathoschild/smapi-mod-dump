/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Hashing;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster.Types;

internal static class LongHash {
	internal const ulong Null = HashUtility.Constants.Bits64.Null;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong GetLongHashCode(this string str) {
		return str.AsSpan().Hash();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong GetLongHashCode(this StringBuilder str) {
		// TODO : need a proper streaming hash for this.
		return str.ToString().GetLongHashCode();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong GetLongHashCode<T>(this T obj) {
		if (obj is ILongHash hashable) {
			return hashable.GetLongHashCode();
		}
		return From(obj);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong From(int hashCode) => HashUtility.Combine((ulong)hashCode, (ulong)(~hashCode) << 32);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong From(ulong hashCode) => hashCode;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong From(ILongHash obj) => obj.GetLongHashCode();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong From<T>(T obj) {
		return obj switch {
			ILongHash hashable => hashable.GetLongHashCode(),
			null => Null,
			int => From(obj.GetHashCode()),
			uint => From(obj.GetHashCode()),
			_ => HashUtility.SubHash64(obj)
		};
	}
}

internal interface ILongHash {
	ulong GetLongHashCode();
}
