using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SpriteMaster.Types {
	[ComVisible(false)]
	public class WeakSet<T> where T : class {
		private const object Sentinel = null;

		private readonly ConditionalWeakTable<T, object> InternalTable = new ConditionalWeakTable<T, object>();
		private readonly SharedLock Lock = new SharedLock();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SecuritySafeCritical]
		private bool _Contains(T obj) {
			return InternalTable.TryGetValue(obj, out var _);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SecuritySafeCritical]
		public bool Contains(T obj) {
			using (Lock.Shared) {
				return _Contains(obj);
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
					if (_Contains(obj)) {
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
