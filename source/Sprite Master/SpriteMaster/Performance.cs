/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

//#define TRACK_PERFORMANCE

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal static class Performance {
#if !TRACK_PERFORMANCE
		internal struct DummyDisposable : IDisposable {

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {}
		}
		private static readonly DummyDisposable Dummy = new();
#else
		internal struct PerformanceTrackerDisposable : IDisposable {
			internal readonly string Name;
			internal readonly DateTime Start;
			internal TimeSpan Duration { get; private set; }

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			internal PerformanceTrackerDisposable(string name) {
				Name = name;
				Start = DateTime.Now;
				Duration = TimeSpan.Zero;
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {
				Duration = DateTime.Now - Start;
				InsertDataPoint(in this);
			}
		}
#endif

#if TRACK_PERFORMANCE
		internal static IDisposable Track([CallerMemberName] string name = "") {
#else
		internal static IDisposable Track(string _ = null) {
#endif
#if TRACK_PERFORMANCE
			return new PerformanceTrackerDisposable(name);
#else
			return Dummy;
#endif
		}

#if TRACK_PERFORMANCE
		private static readonly Dictionary<string, TimeSpan> WorstTimes = new Dictionary<string, TimeSpan>();

		private static void InsertDataPoint (in PerformanceTrackerDisposable tracker) {
			bool isWorst = true;
			lock (WorstTimes) {
				TimeSpan worstTime;
				if (!WorstTimes.TryGetValue(tracker.Name, out worstTime)) {
					WorstTimes.Add(tracker.Name, tracker.Duration);
				}
				else {
					if (tracker.Duration > worstTime) {
						WorstTimes[tracker.Name] = tracker.Duration;
					}
					else {
						isWorst = false;
					}
				}
			}

			if (isWorst) {
				Debug.ErrorLn($"Worst Time Recorded for '{tracker.Name}': {tracker.Duration}");
			}
		}
#endif
	}
}
