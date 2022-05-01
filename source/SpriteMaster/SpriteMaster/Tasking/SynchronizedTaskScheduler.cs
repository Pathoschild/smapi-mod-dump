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
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Tasking;

[DebuggerTypeProxy(typeof(SynchronizedTaskSchedulerDebugView))]
[DebuggerDisplay("Id={Id}, ScheduledTasks = {DebugTaskCount}")]
sealed class SynchronizedTaskScheduler : TaskScheduler, IDisposable {
	internal static readonly SynchronizedTaskScheduler Instance = new();
	internal static readonly TaskFactory TaskFactory = new(Instance);

	internal static readonly Func<bool>? IsUIThread = typeof(XNA.Graphics.Texture2D).Assembly.GetType(
		"Microsoft.Xna.Framework.Threading"
	)?.GetMethod(
		"IsOnUIThread",
		BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static
	)?.CreateDelegate<Func<bool>>();

	internal sealed class TextureActionTask : Task {
		internal readonly TextureAction ActionData;

		internal TextureActionTask(Action action, TextureAction actionData) : base(action) {
			ActionData = actionData;
		}
	}

	private class SynchronizedTaskSchedulerDebugView {
		private readonly SynchronizedTaskScheduler Scheduler;

		public SynchronizedTaskSchedulerDebugView(SynchronizedTaskScheduler scheduler) {
			Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
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

	internal SynchronizedTaskScheduler() {}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Dispatch(in TimeSpan remainingTime) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void InvokeTask(Task task) {
		if (TryExecuteTask(task) || task.IsCompleted) {
			task.Dispose();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void DispatchInternal(in TimeSpan remainingTime) {
		var watch = System.Diagnostics.Stopwatch.StartNew();
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
						if (task is not null) {
							InvokeTask(task);
						}
					}
					pendingActions.Clear();
				}
			}
		}

		if (Config.AsyncScaling.Enabled) {
			var pendingLoads = PendingDeferred.Current;
			bool invoke;
			lock (pendingLoads) {
				invoke = pendingLoads.Count != 0;
			}

			if (invoke) {
				PendingDeferred.Swap();
				lock (pendingLoads) {
					if (Config.AsyncScaling.ThrottledSynchronousLoads && !GameState.IsLoading) {
						int processed = 0;
						foreach (var task in pendingLoads) {
							if (task is null) {
								++processed;
								continue;
							}

							var estimate = TexelAverage.Estimate(task.ActionData);
							if (DrawState.PushedUpdateWithin(0) && watch.Elapsed + estimate > remainingTime) {
								break;
							}

							DrawState.PushedUpdateThisFrame = true;
							var start = watch.Elapsed;
							InvokeTask(task);
							var duration = watch.Elapsed - start;
							Debug.Trace($"Sprite Finished: Est: {estimate.TotalMilliseconds} ms, Act: {duration.TotalMilliseconds} ms  ({task.ActionData.Size} B) (rem: {remainingTime.TotalMilliseconds} ms)");
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
							if (task is not null) {
								InvokeTask(task);
							}
						}
					}
					pendingLoads.Clear();
				}
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	protected override void QueueTask(Task task) {
		if (!Config.IsEnabled) {
			return;
		}

		if (DisposeCancellation.IsCancellationRequested) {
			throw new ObjectDisposedException(GetType().Name);
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void QueueImmediate(Action action) {
		if (!Config.IsEnabled) {
			return;
		}

		var task = new Task(action);
		task.Start(this);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void QueueDeferred(Action action, in TextureAction actionData) {
		if (!Config.IsEnabled) {
			return;
		}

		var task = new TextureActionTask(action, actionData);
		task.Start(this);
	}

	protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => (IsUIThread?.Invoke() ?? false) && TryExecuteTask(task);

	protected override IEnumerable<Task> GetScheduledTasks() {
		var immediate = PendingImmediate.Both;
		var deferred = PendingDeferred.Both;
		lock (immediate.Item1) lock (immediate.Item2) lock (deferred.Item1) lock (deferred.Item2) {
			IEnumerable<Task> enumerable = immediate.Item1;
			enumerable = enumerable.Concat(immediate.Item2);
			enumerable = enumerable.Concat(deferred.Item1);
			enumerable = enumerable.Concat(deferred.Item2);
			return enumerable;
		}
	}

	public void Dispose() => DisposeCancellation.Cancel();
}
