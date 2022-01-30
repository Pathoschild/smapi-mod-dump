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
using System.Collections.Generic;
using System.Diagnostics;

namespace SpriteMaster;

sealed class AsyncTracker : IDisposable {
#if DEBUG
	private const bool Enabled = false;
#endif

#if DEBUG
	private static readonly object TrackerLock = Enabled ? new() : null!;
	private static readonly HashSet<AsyncTracker> Trackers = Enabled ? new() : null!;

	private readonly string Name;
#endif

#if DEBUG
	[Conditional("DEBUG")]
	private static void DumpTrackers() {
		if (!Enabled) return;

		if (Trackers.Count == 0) {
			Debug.TraceLn("No Asynchronous Tasks In Flight");
			return;
		}

		var output = $"Current Asynchronous Tasks [{Trackers.Count}]:\n";
		foreach (var tracker in Trackers) {
			output += $"\t{tracker.Name}\n";
		}
		output.TrimEnd('\n');
		Debug.TraceLn(output);
	}
#endif

	internal AsyncTracker(string name) {
#if DEBUG
		Name = name;

		if (!Enabled) return;

		lock (TrackerLock) {
			Trackers.Add(this);
			DumpTrackers();
		}
#endif
	}

	public void Dispose() {
#if DEBUG
		if (!Enabled) return;

		lock (TrackerLock) {
			Trackers.Remove(this);
			DumpTrackers();
		}

		GC.SuppressFinalize(this);
#endif
	}
}
