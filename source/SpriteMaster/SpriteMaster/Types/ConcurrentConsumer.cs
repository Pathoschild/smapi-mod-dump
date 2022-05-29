/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SpriteMaster.Types;

internal sealed class ConcurrentConsumer<T> {
	internal delegate void CallbackDelegate(in T item);

	private readonly Thread ConsumerThread;
	private readonly AutoResetEvent Event = new(false);
	private readonly ConcurrentQueue<T> DataQueue = new();
	private readonly CallbackDelegate Callback;
	private readonly string Name;

	internal ConcurrentConsumer(string name, CallbackDelegate callback) {
		Name = name;
		Callback = callback;

		ConsumerThread = new(Loop) {
			Name = $"ConcurrentConsumer '{name}' Thread",
			Priority = ThreadPriority.BelowNormal,
			IsBackground = true
		};
		ConsumerThread.Start();
	}

	internal void Push(in T instance) {
		DataQueue.Enqueue(instance);
		Event.Set();
	}

	private void Loop() {
		while (true) {
			try {
				Event.WaitOne();
				while (DataQueue.TryDequeue(out var item)) {
					try {
						Callback(item);
					}
					catch (Exception ex) {
						Debug.Error($"Exception during ConcurrentConsumer '{Name}' Loop", ex);
					}
				}
			}
			catch (ThreadAbortException) {
				break;
			}
			catch (ObjectDisposedException) {
				break;
			}
			catch (AbandonedMutexException) {
			}
		}
	}
}
