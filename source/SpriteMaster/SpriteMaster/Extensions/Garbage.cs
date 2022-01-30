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

using SpriteMaster.Harmonize;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class Garbage {
	private delegate void CompactingCollectDelegate(int generation, GCCollectionMode mode, bool blocking, bool compacting);
	internal static volatile bool ManualCollection = false;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void ExecuteSafe(Action call) {
		try {
			call.Invoke();
		}
		catch (Exception) {
			// ignore the exception
		}
	}

	static Garbage() {
		ExecuteSafe(() => GCSettings.LatencyMode = GCLatencyMode.Interactive);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
#if NETFRAMEWORK && (NET20 || NET35 || NET40 || NET45)
		[Conditional("FALSE")]
#endif
	internal static void MarkCompact() {
		Debug.TraceLn("Marking for Compact");
#if !NETFRAMEWORK || !(NET20 || NET35 || NET40 || NET45)
		ExecuteSafe(() => GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce);
#endif
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Collect(bool compact = false, bool blocking = false, bool background = true) {
		try {
			ManualCollection = true;

			Debug.TraceLn("Garbage Collecting");
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
			catch (Exception) {
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
