/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using MusicMaster.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace MusicMaster.Extensions;

internal static class Garbage {
	internal static volatile bool ManualCollection = false;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void MarkCompact() {
		Debug.Trace("Marking for Compact");
		try {
			GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
		}
		catch (Exception ex) {
			Debug.WarningOnce($"Failed to set LargeObjectHeapCompactionMode: {ex.GetTypeName()}: {ex.Message}");
		}
	}

	internal static void Collect(bool compact = false, bool blocking = false, bool background = true) {
		try {
			ManualCollection = true;

			Debug.Trace("Garbage Collecting");
			if (compact) {
				MarkCompact();
			}

			try {
				var latencyMode = GCSettings.LatencyMode;
				try {
					if (blocking) {
						GCSettings.LatencyMode = GCLatencyMode.Batch;
					}
					GC.Collect(
						int.MaxValue,
						background ? GCCollectionMode.Optimized : GCCollectionMode.Forced,
						blocking,
						compact
					);
				}
				finally {
					GCSettings.LatencyMode = latencyMode;
				}
			}
			catch (Exception ex) {
				Debug.Trace("Failed to call preferred GC Collect", ex);

				// Just in case the user's GC doesn't support the previous properties like LatencyMode
				GC.Collect(
					generation: int.MaxValue,
					mode: background ? GCCollectionMode.Optimized : GCCollectionMode.Forced,
					blocking: blocking
				);
			}
		}
		finally {
			ManualCollection = false;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Mark(long size) {
		size.AssertPositiveOrZero();
		GC.AddMemoryPressure(size);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Unmark(long size) {
		size.AssertPositiveOrZero();
		GC.RemoveMemoryPressure(size);
	}

	#region Console Command

	private static readonly Dictionary<string, ConsoleSupport.Command> CommandMap = new() {
		{ "help", new((_, _) => ConsoleSupport.InvokeHelp(CommandMap!), "Prints this command guide") }
	};

	[Command("gc", "Garbage Collection Commands")]
	public static void OnConsoleCommand(string command, Queue<string> arguments) {
		ConsoleSupport.Invoke(CommandMap, command, arguments);
	}

	#endregion
}
