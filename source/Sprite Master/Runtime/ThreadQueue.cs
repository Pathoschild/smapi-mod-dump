/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster {
	// C#'s Thread Queue can block when inserting a task if no free threads are free. This prevents said blocking.
	public static class ThreadQueue {
		private const ThreadPriority QueueHandlerPriority = ThreadPriority.BelowNormal;

		public delegate void QueueFunctor<T> (T state) where T : class;

		private struct Functor {
			public readonly WaitCallback Callback;
			public readonly object State;

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			private Functor(WaitCallback callback, object state) {
				Callback = callback;
				State = state;
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			internal static Functor Of (WaitCallback callback, object state) => new Functor(callback, state);

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			internal static Functor Of<T> (QueueFunctor<T> callback, T state) where T : class => new Functor((object o) => callback((T)o), state);

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			internal static Functor Of (WaitCallback callback) => new Functor(callback, null);
		}

#if THREADQUEUE_PARALLEL
		private static readonly Thread QueueThread;
		private static readonly DoubleBuffer<List<Functor>> QueueList = new DoubleBuffer<List<Functor>>();
		private static readonly AutoResetEvent PendingEvent = new AutoResetEvent(false);

		static ThreadQueue() {
			QueueThread = new Thread(() => {
				if (Thread.CurrentThread.Priority > QueueHandlerPriority) {
					Thread.CurrentThread.Priority = QueueHandlerPriority;
				}
				Thread.CurrentThread.Name = "ThreadQueue Thread";
				while (true) {
					PendingEvent.WaitOne();

					var CurrentQueue = QueueList.Current;
					lock (CurrentQueue) {
						if (!CurrentQueue.Any()) {
							continue;
						}
					}

					QueueList.Swap();

					lock (CurrentQueue) {
						foreach (var functor in CurrentQueue) {
							ThreadPool.UnsafeQueueUserWorkItem(functor.Callback, functor.State);
						}
						CurrentQueue.Clear();
					}
				}
			});
			QueueThread.Start();
		}
#endif

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private static void Enqueue(Functor functor) {
#if THREADQUEUE_PARALLEL
			// TODO : PROFILE MAIN THREAD
			var CurrentQueue = QueueList.Current;
			lock (CurrentQueue) {
				CurrentQueue.Add(functor);
			}
			PendingEvent.Set();
#else
			ThreadPool.UnsafeQueueUserWorkItem((obj) => {
				if (Thread.CurrentThread.Priority > QueueHandlerPriority) {
					Thread.CurrentThread.Priority = QueueHandlerPriority;
				}
				functor.Callback.Invoke(obj);
			}, functor.State);
#endif
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static void Queue<T> (QueueFunctor<T> functor, T argument) where T : class => Enqueue(Functor.Of(functor, argument));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static void Queue (WaitCallback functor, object argument) => Enqueue(Functor.Of(functor, argument));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static void Queue (WaitCallback functor) => Enqueue(Functor.Of(functor));
	}
}
