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
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster;

// C#'s Thread Queue can block when inserting a task if no free threads are free. This prevents said blocking.
static class ThreadQueue {
	private const ThreadPriority QueueHandlerPriority = ThreadPriority.BelowNormal;

	internal delegate void QueueFunctor<T>(T state) where T : class;

	private readonly struct Functor {
		internal readonly WaitCallback Callback;
		internal readonly object State;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private Functor(WaitCallback callback, object state) {
			Callback = callback;
			State = state;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal static Functor Of(WaitCallback callback, object state) => new(callback, state);

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal static Functor Of<T>(QueueFunctor<T> callback, T state) where T : class => new((object o) => callback((T)o), state);

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal static Functor Of(WaitCallback callback) => new(callback, null);
	}

#if THREADQUEUE_PARALLEL
	private static readonly Thread QueueThread;
	private static readonly DoubleBuffer<List<Functor>> QueueList = new();
	private static readonly AutoResetEvent PendingEvent = new(false);

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	static ThreadQueue() {
		QueueThread = new Thread([MethodImpl(Runtime.MethodImpl.Hot)] () => {
			if (Thread.CurrentThread.Priority > QueueHandlerPriority) {
				Thread.CurrentThread.Priority = QueueHandlerPriority;
			}
			Thread.CurrentThread.Name = "ThreadQueue Thread";
			while (true) {
				PendingEvent.WaitOne();

				var CurrentQueue = QueueList.Current;
				lock (CurrentQueue) {
					if (CurrentQueue.Count == 0) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void Enqueue(in Functor functor) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Queue<T>(QueueFunctor<T> functor, T argument) where T : class => Enqueue(Functor.Of(functor, argument));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Queue(WaitCallback functor, object argument) => Enqueue(Functor.Of(functor, argument));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Queue(WaitCallback functor) => Enqueue(Functor.Of(functor));
}
