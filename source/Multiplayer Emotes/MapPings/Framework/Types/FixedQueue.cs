/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Collections.Concurrent;

namespace MapPings.Framework.Types {

	public class FixedQueue<T> : ConcurrentQueue<T> {

		private readonly object syncObject = new object();

		public int Size { get; private set; }

		public FixedQueue(int size) {
			Size = size;
		}

		public new void Enqueue(T obj) {
			base.Enqueue(obj);
			lock(syncObject) {
				while(base.Count > Size) {
					base.TryDequeue(out T outObj);
				}
			}
		}

	}

}
