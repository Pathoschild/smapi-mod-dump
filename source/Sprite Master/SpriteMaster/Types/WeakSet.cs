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
using System.Security;

namespace SpriteMaster.Types {
	public class WeakSet<T> where T : class {
		private const object Sentinel = null;

		private readonly ConditionalWeakTable<T, object> InternalTable = new ConditionalWeakTable<T, object>();
		private readonly SharedLock Lock = new SharedLock();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SecuritySafeCritical]
		public bool Contains(T obj) {
			using (Lock.Shared) {
				return InternalTable.TryGetValue(obj, out var _);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SecuritySafeCritical]
		public bool Remove(T obj) {
			using (Lock.Exclusive) {
				return InternalTable.Remove(obj);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SecuritySafeCritical]
		public bool Add(T obj) {
			try {
				using (Lock.Exclusive) {
					if (InternalTable.TryGetValue(obj, out var _)) {
						return false;
					}

					InternalTable.Add(obj, Sentinel);
					return true;
				}
			}
			catch {
				return false;
			}
		}
	}
}
