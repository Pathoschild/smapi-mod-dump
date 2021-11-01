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
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ActionList = System.Collections.Generic.List<System.Action>;
using LoadList = System.Collections.Generic.List<SpriteMaster.TextureAction>;

namespace SpriteMaster {
	internal static class SynchronizedTasks {
		private static DoubleBuffer<ActionList> PendingActions = Config.AsyncScaling.Enabled ? new() : null;
		private static DoubleBuffer<LoadList> PendingLoads = Config.AsyncScaling.Enabled ? new() : null;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void AddPendingAction (in Action action) {
			var current = PendingActions.Current;
			lock (current) {
				current.Add(action);
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void AddPendingLoad (in Action action, int texels) {
			var current = PendingLoads.Current;
			lock (current) {
				current.Add(new TextureAction(action, texels));
			}
		}

		private static readonly TexelTimer TexelAverage = new TexelTimer();

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void ProcessPendingActions (TimeSpan remainingTime) {
			var watch = System.Diagnostics.Stopwatch.StartNew();
			{
				var pendingActions = PendingActions.Current;
				bool invoke;
				lock (pendingActions) {
					invoke = pendingActions.Any();
				}

				if (invoke) {
					PendingActions.Swap();
					lock (pendingActions) {
						foreach (var action in pendingActions) {
							action.Invoke();
						}
						pendingActions.Clear();
					}
				}
			}

			if (Config.AsyncScaling.Enabled) {
				var pendingLoads = PendingLoads.Current;
				bool invoke;
				lock (pendingLoads) {
					invoke = pendingLoads.Any();
				}

				if (invoke) {
					PendingLoads.Swap();
					lock (pendingLoads) {
						if (Config.AsyncScaling.ThrottledSynchronousLoads) {
							int processed = 0;
							foreach (var action in pendingLoads) {
								var estimate = TexelAverage.Estimate(action);
								if (DrawState.PushedUpdateWithin(1) && watch.Elapsed + estimate > remainingTime) {
									break;
								}

								DrawState.PushedUpdateThisFrame = true;
								var start = watch.Elapsed;
								action.Invoke();
								var duration = watch.Elapsed - start;
								TexelAverage.Add(action, duration);

								++processed;
							}

							// TODO : I'm not sure if this is necessary. The next frame, it'll probably come back around again to this buffer.
							if (processed < pendingLoads.Count) {
								var current = PendingLoads.Current;
								lock (current) {
									current.AddRange(pendingLoads.GetRange(processed, pendingLoads.Count - processed));
								}
							}
						}
						else {
							foreach (var action in pendingLoads) {
								action.Invoke();
							}
						}
						pendingLoads.Clear();
					}
				}
			}
		}
	}
}
