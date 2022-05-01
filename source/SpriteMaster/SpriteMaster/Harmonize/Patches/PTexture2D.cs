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
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
static class PTexture2D {
	#region Cache Handlers

	// https://benbowen.blog/post/fun_with_makeref/

	// Always returns a duplicate of the array, since we do not own the source array.
	// It performs a shallow copy, which is fine.
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static unsafe byte[] GetByteArray<T>(T[] data, int startIndex, int elementCount, out int typeSize) where T : struct {
		// TODO : we shouldn't copy all the texture data from this, only the relevant parts from startIndex/elementCount
		typeSize = Marshal.SizeOf<T>();
		ReadOnlySpan<T> inData = data;
		inData = inData.Slice(startIndex, elementCount);
		var inDataBytes = MemoryMarshal.Cast<T, byte>(inData);
		var resultData = GC.AllocateUninitializedArray<byte>(inDataBytes.Length);
		inDataBytes.CopyTo(resultData);
		return resultData;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool Cacheable(Texture2D texture) => texture.LevelCount <= 1;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void SetDataPurge<T>(Texture2D texture, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount, bool animated) where T : struct {
		TextureCache.Remove(texture);

		if (!ManagedSpriteInstance.Validate(texture, clean: true)) {
			return;
		}

		if (texture.Format.IsBlock()) {
			ManagedSpriteInstance.FullPurge(texture, animated: animated);
			return;
		}

		int elementSize = 0;
		var byteData = Cacheable(texture) ? GetByteArray(data, startIndex, elementCount, out elementSize) : null;
		startIndex = 0;

#if ASYNC_SETDATA
			ThreadQueue.Queue((data) =>
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				ScaledTexture.Purge(
					reference: texture,
					bounds: rect,
					data: new DataRef<byte>(
						data: data,
						offset: startIndex * elementSize,
						length: elementCount * elementSize
					)
				), byteData);
#else
		ManagedSpriteInstance.Purge(
			reference: texture,
			bounds: rect,
			data: new DataRef<byte>(
				data: byteData,
				offset: startIndex * elementSize,
				length: elementCount * elementSize
			),
			animated: animated
		);
#endif
	}

	#endregion

	#region SetData

	private static bool CheckDataChange<T>(Texture2D instance, int level, int arraySlice, in XNA.Rectangle? inRect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		Bounds rect = inRect ?? instance.Bounds;

		if (instance.TryMeta(out var meta) && meta.CachedData is byte[] cachedData) {
			var dataSpan = data.AsReadOnlySpan().Cast<T, byte>();
			var cachedDataSpan = cachedData.AsReadOnlySpan();

			unsafe {
				var inSpan = dataSpan;
				int inOffset = 0;
				int inRowLength = rect.Width * sizeof(T);

				var cachedSpan = new Span2D<byte>(
					array: cachedData,
					offset: startIndex * sizeof(T),
					width: rect.Width * sizeof(T),
					height: rect.Height,
					pitch: (instance.Width - rect.Width) * sizeof(T)
				);

				bool equal = true;
				for (int y = 0; y < rect.Height; ++y) {
					var inSpanRow = inSpan.Slice(inOffset, inRowLength);
					var cachedSpanRow = cachedSpan.GetRowSpan(y);
					if (!inSpanRow.SequenceEqual(cachedSpanRow)) {
						equal = false;
						break;
					}
					inOffset += inRowLength;
				}

				return !equal;
			}
		}

		return true;
	}

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnSetData<T>(Texture2D __instance, T[] data) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}

		if (__instance.Format.IsBlock()) {
			return true;
		}

		__instance.SetData(0, 0, null, data, 0, data.Length);
		return false;
	}

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnSetData<T>(Texture2D __instance, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}

		if (__instance.Format.IsBlock()) {
			return true;
		}

		__instance.SetData(0, 0, null, data, startIndex, elementCount);
		return false;
	}

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnSetData<T>(Texture2D __instance, int level, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}

		if (__instance.Format.IsBlock()) {
			return true;
		}

		__instance.SetData(0, level, rect, data, startIndex, elementCount);
		return false;
	}

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnSetData<T>(Texture2D __instance, int level, int arraySlice, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}

		if (__instance.Format.IsBlock()) {
			return true;
		}

		try {
			if (!CheckDataChange(__instance, level, arraySlice, rect, data, startIndex, elementCount)) {
				Debug.Trace($"SetData for '{__instance.NormalizedName()}' skipped; data unchanged ({rect?.ToString() ?? "null"})".Colorized(DrawingColor.LightGreen));
				return false;
			}
		}
		catch (Exception ex) {
			Debug.Warning($"Exception while processing SetData for '{__instance.NormalizedName()}'", ex);
		}

		__instance.Meta().IncrementRevision();

		return true;
	}

	[Harmonize("GetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnGetData<T>(Texture2D __instance, T[] data) where T : unmanaged {
		return OnGetData<T>(__instance, 0, null, data, 0, data.Length);
	}

	[Harmonize("GetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnGetData<T>(Texture2D __instance, T[] data, int startIndex, int elementCount) where T : unmanaged {
		return OnGetData<T>(__instance, 0, null, data, startIndex, elementCount);
	}

	[Harmonize("GetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static bool OnGetData<T>(Texture2D __instance, int level, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		return OnGetData<T>(__instance, level, 0, rect, data, startIndex, elementCount);
	}

	[Harmonize("GetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static unsafe bool OnGetData<T>(Texture2D __instance, int level, int arraySlice, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (!Config.IsEnabled || !Config.SMAPI.ApplyGetDataPatch) {
			return true;
		}

		if (__instance.Format.IsBlock()) {
			return true;
		}

		if (data is null) {
			throw new ArgumentNullException(nameof(data));
		}

		try {
			if (
				level == 0 &&
				arraySlice == 0 &&
				__instance.TryMeta(out var sourceMeta) && sourceMeta.CachedData is byte[] cachedSourceData
			) {
				Bounds bounds = rect ?? __instance.Bounds;

				if (cachedSourceData.Length < elementCount * sizeof(T)) {
					throw new ArgumentException($"{cachedSourceData.Length} < {elementCount * sizeof(T)}", nameof(elementCount));
				}

				if (data.Length < elementCount + startIndex) {
					throw new ArgumentException($"{data.Length} < {elementCount + startIndex}", nameof(data));
				}

				if (bounds == __instance.Bounds) {
					ReadOnlySpan<byte> sourceBytes = cachedSourceData;
					var source = sourceBytes.Cast<T>();
					Span<T> dest = data;
					source.CopyTo(dest, 0, startIndex, elementCount);

					return false;
				}
				else if (__instance.Bounds.Contains(bounds)) {
					// We need a subcopy
					var cachedData = cachedSourceData.AsReadOnlySpan<T>();
					var destData = data.AsSpan();
					int sourceStride = __instance.Width;
					int destStride = bounds.Width;
					int sourceOffset = (bounds.Top * sourceStride) + bounds.Left;
					int destOffset = startIndex;
					for (int y = 0; y < bounds.Height; ++y) {
						cachedData.Slice(sourceOffset, destStride).CopyTo(destData.Slice(destOffset, destStride));
						sourceOffset += sourceStride;
						destOffset += destStride;
					}

					return false;
				}
			}
		}
		catch (Exception ex) {
			Debug.Error("OnGetData optimization threw an exception", ex);
		}

		return true;
	}

	/*
	[Harmonize("SetData", Harmonize.Fixation.Reverse, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static void OnSetDataOriginal<T>(Texture2D __instance, int level, int arraySlice, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct {
		throw new NotImplementedException("Reverse Patch");
	}*/

	/*
	[Harmonize("SetData", Harmonize.Fixation.Postfix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	public static void OnSetDataPost<T>(Texture2D __instance, int level, int arraySlice, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return;
		}

		if (!CheckDataChange(__instance, level, arraySlice, rect, data, startIndex, elementCount)) {
			return;
		}

		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;
		SetDataPurge(
			__instance,
			rect,
			data,
			startIndex,
			elementCount
		);

		TextureCache.Remove(__instance);
	}
	*/

	#endregion

	//private void PlatformSetData<T>(int level, T[] data, int startIndex, int elementCount) where T : struct

	//private void PlatformSetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct


	private static readonly Assembly? CPAAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefaultF(assembly => assembly.GetName().Name == "ContentPatcherAnimations");
	private static bool IsFromContentPatcherAnimations() {
		if (CPAAssembly is null) {
			return false;
		}

		var stackTrace = new StackTrace(skipFrames: 2, fNeedFileInfo: false);
		foreach (var frame in stackTrace.GetFrames()) {
			if (frame.GetMethod() is MethodBase method && method.DeclaringType?.Assembly == CPAAssembly) {
				return true;
			}
		}

		return false;
	}

	[Harmonize("PlatformSetData", Harmonize.Fixation.Postfix, PriorityLevel.Average, Harmonize.Generic.Struct, platform: Harmonize.Platform.MonoGame)]
	public static void OnPlatformSetDataPost<T>(Texture2D __instance, int level, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return;
		}

		SetDataPurge(
			__instance,
			null,
			data,
			startIndex,
			elementCount,
			animated: IsFromContentPatcherAnimations()
		);
	}

	[Harmonize("PlatformSetData", Harmonize.Fixation.Postfix, PriorityLevel.Average, Harmonize.Generic.Struct, platform: Harmonize.Platform.MonoGame)]
	public static void OnPlatformSetDataPost<T>(Texture2D __instance, int level, int arraySlice, XNA.Rectangle rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return;
		}

		SetDataPurge(
			__instance,
			rect,
			data,
			startIndex,
			elementCount,
			animated: IsFromContentPatcherAnimations()
		);
	}
}
