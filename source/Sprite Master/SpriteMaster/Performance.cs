//#define TRACK_PERFORMANCE

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal static class Performance {
#if !TRACK_PERFORMANCE
		internal struct DummyDisposable : IDisposable {

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose () {}
		}
		private static readonly DummyDisposable Dummy = new DummyDisposable();
#else
		internal struct PerformanceTrackerDisposable : IDisposable {
			internal readonly string Name;
			internal readonly DateTime Start;
			internal TimeSpan Duration { get; private set; }

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal PerformanceTrackerDisposable(string name) {
				Name = name;
				Start = DateTime.Now;
				Duration = TimeSpan.Zero;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
