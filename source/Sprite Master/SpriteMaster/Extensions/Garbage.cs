using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Harmonize;
using SpriteMaster.Types;
using System;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Garbage {
		private static readonly MethodInfo CompactingCollect = null;
		public static volatile bool ManualCollection = false;

		static Garbage () {
			GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

			var methods = typeof(GC).GetMethods("Collect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var method in methods) {
				var methodParameters = method.GetParameters();
				if (methodParameters.Length != 4)
					continue;
				// +		method	{Void Collect(Int32, System.GCCollectionMode, Boolean, Boolean)}	System.Reflection.MethodInfo {System.Reflection.RuntimeMethodInfo}
				if (
					methodParameters[0].ParameterType == typeof(Int32) &&
					methodParameters[1].ParameterType == typeof(System.GCCollectionMode) &&
					methodParameters[2].ParameterType == typeof(Boolean) &&
					methodParameters[3].ParameterType == typeof(Boolean)
				) {
					CompactingCollect = method;
					break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MarkCompact() {
			Debug.TraceLn("Marking for Compact");
			//GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Collect(bool compact = false, bool blocking = false, bool background = true) {
			try {
				ManualCollection = true;

				Debug.TraceLn("Garbage Collecting");
				if (compact) {
					MarkCompact();
				}
				var latencyMode = GCSettings.LatencyMode;
				try {
					if (blocking) {
						GCSettings.LatencyMode = GCLatencyMode.Batch;
					}
					if (compact && CompactingCollect != null) {
						CompactingCollect.Invoke(null, new object[] {
							int.MaxValue,
							background ? GCCollectionMode.Optimized : GCCollectionMode.Forced,
							blocking,
							true
					});
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
			finally {
				ManualCollection = false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Mark(long size) {
			Contract.AssertPositiveOrZero(size);
			GC.AddMemoryPressure(size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Mark (Texture2D texture) {
			Contract.AssertNotNull(texture);
			Mark(texture.SizeBytes());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unmark(long size) {
			Contract.AssertPositiveOrZero(size);
			GC.RemoveMemoryPressure(size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unmark (Texture2D texture) {
			Contract.AssertNotNull(texture);
			Unmark(texture.SizeBytes());
		}

		public static void MarkOwned(SurfaceFormat format, int texels) {
			if (!Config.GarbageCollectAccountOwnedTexture)
				return;
			Contract.AssertPositiveOrZero(texels);
			var size = format.SizeBytes(texels);
			Mark(size);
		}

		public static void UnmarkOwned (SurfaceFormat format, int texels) {
			if (!Config.GarbageCollectAccountOwnedTexture)
				return;
			Contract.AssertPositiveOrZero(texels);
			var size = format.SizeBytes(texels);
			Unmark(size);
		}

		public static void MarkUnowned (SurfaceFormat format, int texels) {
			if (!Config.GarbageCollectAccountUnownedTextures)
				return;
			Contract.AssertPositiveOrZero(texels);
			var size = format.SizeBytes(texels);
			Mark(size);
		}

		public static void UnmarkUnowned (SurfaceFormat format, int texels) {
			if (!Config.GarbageCollectAccountUnownedTextures)
				return;
			Contract.AssertPositiveOrZero(texels);
			var size = format.SizeBytes(texels);
			Unmark(size);
		}
	}
}
