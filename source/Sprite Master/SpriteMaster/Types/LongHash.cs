/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types {
	public static class LongHash {
		public const ulong Null = 0UL;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong GetLongHashCode<T>(this T obj) {
			if (obj is ILongHash hashable) {
				return hashable.GetLongHashCode();
			}
			return From(obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong From (int hashCode) => Hash.Combine(hashCode, hashCode << 32);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong From (ILongHash obj) => obj.GetLongHashCode();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong From<T> (T obj) {
			if (obj is ILongHash hashable) {
				return hashable.GetLongHashCode();
			}
			return From(obj.GetHashCode());
		}
	}

	public interface ILongHash {
		public ulong GetLongHashCode ();
	}
}
