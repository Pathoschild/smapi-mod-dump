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
		public static ulong From (int hashCode) {
			return Hash.Combine(hashCode, hashCode << 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong From (ILongHash obj) {
			return obj.GetLongHashCode();
		}

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
