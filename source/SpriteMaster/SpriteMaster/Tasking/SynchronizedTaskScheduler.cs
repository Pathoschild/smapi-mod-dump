/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Tasking;

[DebuggerTypeProxy(typeof(SynchronizedTaskSchedulerDebugView))]
[DebuggerDisplay("Id={Id}, ScheduledTasks = {DebugTaskCount}")]
internal sealed class SynchronizedTaskScheduler : TaskScheduler, IDisposable {
	internal static readonly SynchronizedTaskScheduler Instance = new();

	private static bool IsMainThread => ThreadingExt.IsMainThread;

	internal sealed class TextureActionTask : Task {
		internal readonly TextureAction ActionData;

		internal TextureActionTask(Action action, TextureAction actionData) : base(action) {
			ActionData = actionData;
		}
	}

	private class SynchronizedTaskSchedulerDebugView {
		private readonly SynchronizedTaskScheduler Scheduler;

		public SynchronizedTaskSchedulerDebugView(SynchronizedTaskScheduler scheduler) {
			Scheduler = scheduler;
		}

		public IEnumerable<Task> ScheduledTasks => Scheduler.GetScheduledTasks();
	}

	private readonly CancellationTokenSource DisposeCancellation = new();

	private readonly DoubleBuffer<List<Task>> PendingImmediate = new();
	private readonly DoubleBuffer<List<TextureActionTask>> PendingDeferred = new();

	private readonly TexelTimer TexelAverage = new();

	private int DebugTaskCount {
		get {
			var immediate = PendingImmediate.Both;
			var deferred = PendingDeferred.Both;
			lock (immediate.Item1) lock (immediate.Item2) lock (deferred.Item1) lock (deferred.Item2) {
				return
					immediate.Item1.Count +
					immediate.Item2.Count +
					deferred.Item1.Count +
					deferred.Item2.Count;
			}
		}
	}

	internal void Dispatch(TimeSpan remainingTime) {
		if (!Config.IsEnabled) {
			return;
		}

		try {
			DispatchInternal(remainingTime);
		}
		catch (ThreadAbortException) {
			if (!Environment.HasShutdownStarted && !AppDomain.CurrentDomain.IsFinalizingForUnload()) {
				Thread.ResetAbort();
			}
		}
		catch (OperationCanceledException) { /* do nothing */ }
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void InvokeTask(Task task) {
		if (TryExecuteTask(task) || task.IsCompleted) {
			task.Dispose();
		}
	}

	private void DispatchInternal(TimeSpan remainingTime) {
		var watch = Stopwatch.StartNew();
		{
			var pendingActions = PendingImmediate.Current;
			bool invoke;
			lock (pendingActions) {
				invoke = pendingActions.Count != 0;
			}

			if (invoke) {
				PendingImmediate.Swap();
				lock (pendingActions) {
					foreach (var task in pendingActions) {
						InvokeTask(task);
					}
					pendingActions.Clear();
				}
			}
		}

		if (!Config.AsyncScaling.Enabled) {
			// ReSharper disable once HeuristicUnreachableCode
			return;
		}

		var pendingLoads = PendingDeferred.Current;
		lock (pendingLoads) {
			if (pendingLoads.Count == 0) {
				return;
			}
		}

		PendingDeferred.Swap();
		lock (pendingLoads) {
			if (Config.AsyncScaling.ThrottledSynchronousLoads && !GameState.IsLoading) {
				int processed = 0;
				foreach (var task in pendingLoads) {
					var estimate = TexelAverage.Estimate(task.ActionData);
					if (DrawState.PushedUpdateWithin(0) && watch.Elapsed + estimate > remainingTime) {
						break;
					}

					DrawState.IsUpdatedThisFrame = true;
					var start = watch.Elapsed;
					InvokeTask(task);
					var duration = watch.Elapsed - start;
					Debug.Trace($"Sprite Finished: ['{task.ActionData.Name}'] Est: {estimate.TotalMilliseconds} ms, Act: {duration.TotalMilliseconds} ms  ({task.ActionData.Size.AsDataSize()}) (rem: {remainingTime.TotalMilliseconds} ms)");
					TexelAverage.Add(task.ActionData, duration);

					++processed;
				}

				// TODO : I'm not sure if this is necessary. The next frame, it'll probably come back around again to this buffer.
				if (processed < pendingLoads.Count) {
					var current = PendingDeferred.Current;
					lock (current) {
						current.AddRange(pendingLoads.GetRange(processed, pendingLoads.Count - processed));
					}
				}
			}
			else {
				foreach (var task in pendingLoads) {
					InvokeTask(task);
				}
			}
			pendingLoads.Clear();
		}
	}

	protected override void QueueTask(Task task) {
		if (!Config.IsEnabled) {
			return;
		}

		if (DisposeCancellation.IsCancellationRequested) {
			ThrowHelper.ThrowObjectDisposedException(GetType().Name);
			return;
		}

		if (task is TextureActionTask actionTask) {
			var current = PendingDeferred.Current;
			lock (current) {
				current.Add(actionTask);
			}
		}
		else {
			var current = PendingImmediate.Current;
			lock (current) {
				current.Add(task);
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void QueueImmediate(Action action) {
		if (!Config.IsEnabled) {
			return;
		}

		var task = new Task(action);
		task.Start(this);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void QueueDeferred(Action action, TextureAction actionData) {
		if (!Config.IsEnabled) {
			return;
		}

		var task = new TextureActionTask(action, actionData);
		task.Start(this);
	}

	protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => IsMainThread && TryExecuteTask(task);

	protected override IEnumerable<Task> GetScheduledTasks() {
		var immediate = PendingImmediate.Both;
		var deferred = PendingDeferred.Both;

		lock (immediate.Item1) lock (immediate.Item2) lock (deferred.Item1) lock (deferred.Item2) {
			List<Task> tasks = new(immediate.Item1.Count + immediate.Item2.Count + deferred.Item1.Count + deferred.Item2.Count);
			tasks.AddRange(immediate.Item1);
			tasks.AddRange(immediate.Item2);
			tasks.AddRange(deferred.Item1);
			tasks.AddRange(deferred.Item2);
			return tasks;
		}
	}

	public void Dispose() => DisposeCancellation.Cancel();
}
