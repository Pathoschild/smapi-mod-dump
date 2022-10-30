/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster.Extensions;

internal static class Garbage {
	internal static volatile bool ManualCollection = false;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void EnterNonInteractive() {
		GCSettings.LatencyMode = GCLatencyMode.Interactive;
	}

	internal static void EnterInteractive() {
		//Debug.Error("Interactive GC");

		try {
			GCSettings.LatencyMode = Config.Garbage.LatencyMode;
			Debug.Info($"GC Latency Mode set to {Config.Garbage.LatencyMode}");
		}
		catch (Exception ex) {
			Debug.Warning($"Failed to set GC Latency Mode to '{Config.Garbage.LatencyMode}': {ex.GetTypeName()}: {ex.Message}, attempting to fall back...");

			foreach (var mode in new[] { GCLatencyMode.SustainedLowLatency, GCLatencyMode.LowLatency, GCLatencyMode.Interactive }) {
				try {
					GCSettings.LatencyMode = mode;
					Debug.Warning($"Set GC Latency Mode to '{mode}'");
					break;
				}
				catch {
					// ignored
				}
			}
		}
	}

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
	internal static void Mark(XTexture2D texture) {
		texture.AssertNotNull();
		Mark(texture.SizeBytesLong());
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Unmark(long size) {
		size.AssertPositiveOrZero();
		GC.RemoveMemoryPressure(size);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Unmark(XTexture2D texture) {
		texture.AssertNotNull();
		Unmark(texture.SizeBytesLong());
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void MarkOwned(SurfaceFormat format, int texels) {
		texels.AssertPositiveOrZero();
		var size = format.SizeBytesLong(texels);
		Mark(size);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void UnmarkOwned(SurfaceFormat format, int texels) {
		texels.AssertPositiveOrZero();
		var size = format.SizeBytesLong(texels);
		Unmark(size);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void MarkUnowned(SurfaceFormat format, int texels) {
		if (!Config.Garbage.CollectAccountUnownedTextures) {
			return;
		}
		texels.AssertPositiveOrZero();
		var size = format.SizeBytesLong(texels);
		Mark(size);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void UnmarkUnowned(SurfaceFormat format, int texels) {
		if (!Config.Garbage.CollectAccountUnownedTextures) {
			return;
		}
		texels.AssertPositiveOrZero();
		var size = format.SizeBytesLong(texels);
		Unmark(size);
	}

	#region Ephemeral Collection

	internal static class EphemeralCollection {
		internal static readonly InterlockedBool ShouldDump = new(false);
		internal static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

		internal static readonly MemberInfo EphemeralCollectPeriodMember =
			typeof(Config.Garbage).GetField(
				nameof(Config.Garbage.EphemeralCollectPeriod),
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
			) ?? throw new NullReferenceException(nameof(Config.Garbage.EphemeralCollectPeriod));
		internal static readonly Limit<TimeSpan> Limits = new() {
			Min = EphemeralCollectPeriodMember.GetCustomAttribute<Attributes.LimitsTimeSpanAttribute>()?.MinValue ?? TimeSpan.Zero,
			Max = EphemeralCollectPeriodMember.GetCustomAttribute<Attributes.LimitsTimeSpanAttribute>()?.MaxValue ?? TimeSpan.MaxValue,
		};

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static void Collect(ulong currentFrame) {
			if (Stopwatch.Elapsed < Config.Garbage.EphemeralCollectPeriod) {
				return;
			}

			Stopwatch.Restart();

			// No trace message as that would be _incredibly_ annoying.
			GC.Collect(
				generation: 1,
				mode: GCCollectionMode.Forced,
				blocking: false,
				compacting: false
			);
			AdjustHeuristic();
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private static void AdjustHeuristic() {
			var collectionInfo = GC.GetGCMemoryInfo(GCKind.Ephemeral);
			var pauseDurations = collectionInfo.PauseDurations;
			var totalPauseDuration = TimeSpan.Zero;
			foreach (var pauseDuration in pauseDurations) {
				totalPauseDuration += pauseDuration;
			}

			if (totalPauseDuration > TimeSpan.Zero) {
				double timeRatio = Config.Garbage.EphemeralCollectPauseGoal / totalPauseDuration;

				var newTimeSpan = Config.Garbage.EphemeralCollectPeriod * timeRatio;
				newTimeSpan = Limits.Clamp(newTimeSpan);
				Config.Garbage.EphemeralCollectPeriod = newTimeSpan;
			}

			// general stats
			if (ShouldDump.Exchange(false)) {
				Dump(collectionInfo);
			}
		}

		private static void Dump(in GCMemoryInfo collectionInfo) {
			var dumpBuilder = new StringBuilder("Ephemeral Collection Stats:");

			var properties = new (string Tag, object Value)[] {
				("FinalizationPendingCount", collectionInfo.FinalizationPendingCount),
				("FragmentedBytes", collectionInfo.FragmentedBytes.AsDataSize()),
				("HeapSizeBytes", collectionInfo.HeapSizeBytes.AsDataSize()),
				("MemoryLoadBytes", collectionInfo.MemoryLoadBytes.AsDataSize()),
				("PauseDurations", collectionInfo.PauseDurations.ToArray()),
				("PinnedObjectsCount", collectionInfo.PinnedObjectsCount),
				("PromotedBytes", collectionInfo.PromotedBytes.AsDataSize()),
				("TotalAvailableMemoryBytes", collectionInfo.TotalAvailableMemoryBytes.AsDataSize()),
				("TotalCommittedBytes", collectionInfo.TotalCommittedBytes.AsDataSize())
			};

			int maxTagLength = properties.MaxF(p => p.Tag.Length);

			foreach (var property in properties) {
				if (property.Value is IEnumerable valuesEnumerable) {
					var values = valuesEnumerable.Cast<object>().ToArray();
					switch (values.Length) {
						case 0:
							dumpBuilder.AppendLine($"  {property.Tag.PadRight(maxTagLength)} : []");
							break;
						case 1:
							dumpBuilder.AppendLine($"  {property.Tag.PadRight(maxTagLength)} : [ {values[0]} ]");
							break;
						default: {
							var emptySpace = "".PadRight(maxTagLength);

							dumpBuilder.AppendLine($"  {property.Tag.PadRight(maxTagLength)} : [");
							foreach (var value in values) {
								dumpBuilder.AppendLine($"  {emptySpace}     {value}");
							}

							dumpBuilder.AppendLine($"  {emptySpace}   ]");
						}
							break;
					}
				}
				else {
					dumpBuilder.AppendLine($"  {property.Tag.PadRight(maxTagLength)} : {property.Value}");
				}
			}

			Debug.Info(dumpBuilder.ToString());
		}
	}

	#endregion

	#region Console Command

	private static readonly Dictionary<string, ConsoleSupport.Command> CommandMap = new() {
		{ "help", new((_, _) => ConsoleSupport.InvokeHelp(CommandMap!), "Prints this command guide") },
		{ "start", new((_, _) => SpriteMaster.Self.MemoryMonitor.TriggerGarbageCollection(), "Trigger full GC") },
		{ "stats", new((_, _) => { EphemeralCollection.ShouldDump.Value = true; }, "Dump GC stats on next ephemeral collection") }
	};

	[Command("gc", "Garbage Collection Commands")]
	public static void OnConsoleCommand(string command, Queue<string> arguments) {
		ConsoleSupport.Invoke(CommandMap, command, arguments);
	}

	#endregion
}
