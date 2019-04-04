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
