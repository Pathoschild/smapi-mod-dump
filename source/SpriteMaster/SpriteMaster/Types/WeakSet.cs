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

namespace SpriteMaster.Types;

class WeakSet<T> where T : class {
	private const object Sentinel = null;

	private readonly ConditionalWeakTable<T, object> InternalTable = new();
	private readonly SharedLock Lock = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal bool Contains(T obj) {
		using (Lock.Read) {
			return InternalTable.TryGetValue(obj, out var _);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal bool Remove(T obj) {
		using (Lock.Write) {
			return InternalTable.Remove(obj);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[SecuritySafeCritical]
	internal bool Add(T obj) {
		try {
			using (Lock.Write) {
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
