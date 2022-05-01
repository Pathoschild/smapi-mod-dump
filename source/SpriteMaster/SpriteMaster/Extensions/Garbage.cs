/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class Garbage {
	private delegate void CompactingCollectDelegate(int generation, GCCollectionMode mode, bool blocking, bool compacting);
	internal static volatile bool ManualCollection = false;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void EnterNonInteractive() {
		GCSettings.LatencyMode = GCLatencyMode.Interactive;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
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
				catch { }
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void MarkCompact() {
		Debug.Trace("Marking for Compact");
		try {
			GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
		}
		catch (Exception ex) {
			Debug.WarningOnce($"Failed to set LargeObjectHeapCompactionMode: {ex.GetTypeName()}: {ex.Message}");
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
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
					if (compact) {
						GC.Collect(
							int.MaxValue,
							background ? GCCollectionMode.Optimized : GCCollectionMode.Forced,
							blocking,
							true
						);
					}
					else {
						GC.Collect(
							generation: int.MaxValue,
							mode: background ? GCCollectionMode.Optimized : GCCollectionMode.Forced,
							blocking: blocking
						);
					}
				}
				finally {
					GCSettings.LatencyMode = latencyMode;
				}
			}
			catch (Exception ex) {
				Debug.Trace("Failed to call preferred GC Collect", ex);

				// Just in case the user's GC doesn't support the preivous properties like LatencyMode
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Mark(long size) {
		Contracts.AssertPositiveOrZero(size);
		GC.AddMemoryPressure(size);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Mark(Texture2D texture) {
		Contracts.AssertNotNull(texture);
		Mark(texture.SizeBytes());
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Unmark(long size) {
		Contracts.AssertPositiveOrZero(size);
		GC.RemoveMemoryPressure(size);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Unmark(Texture2D texture) {
		Contracts.AssertNotNull(texture);
		Unmark(texture.SizeBytes());
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void MarkOwned(SurfaceFormat format, int texels) {
		if (!Config.Garbage.CollectAccountOwnedTextures) {
			return;
		}
		Contracts.AssertPositiveOrZero(texels);
		var size = format.SizeBytes(texels);
		Mark(size);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void UnmarkOwned(SurfaceFormat format, int texels) {
		if (!Config.Garbage.CollectAccountOwnedTextures) {
			return;
		}
		Contracts.AssertPositiveOrZero(texels);
		var size = format.SizeBytes(texels);
		Unmark(size);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void MarkUnowned(SurfaceFormat format, int texels) {
		if (!Config.Garbage.CollectAccountUnownedTextures) {
			return;
		}
		Contracts.AssertPositiveOrZero(texels);
		var size = format.SizeBytes(texels);
		Mark(size);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void UnmarkUnowned(SurfaceFormat format, int texels) {
		if (!Config.Garbage.CollectAccountUnownedTextures) {
			return;
		}
		Contracts.AssertPositiveOrZero(texels);
		var size = format.SizeBytes(texels);
		Unmark(size);
	}
}
