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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
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
	private static unsafe byte[] GetByteArray<T>(T[] data, out int typeSize) where T : struct {
		// TODO : we shouldn't copy all the texture data from this, only the relevant parts from startIndex/elementCount
		typeSize = Marshal.SizeOf<T>();
		ReadOnlySpan<T> inData = data;
		var inDataBytes = MemoryMarshal.Cast<T, byte>(inData);
		var resultData = GC.AllocateUninitializedArray<byte>(inDataBytes.Length);
		inDataBytes.CopyTo(resultData);
		return resultData;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool Cacheable(Texture2D texture) => texture.LevelCount <= 1;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void SetDataPurge<T>(Texture2D texture, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct {
		if (!ManagedSpriteInstance.Validate(texture, clean: true)) {
			return;
		}

		int elementSize = 0;
		var byteData = Cacheable(texture) ? GetByteArray(data, out elementSize) : null;

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
			)
		);
#endif
	}

	#endregion

	#region SetData

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	private static bool OnSetData<T>(Texture2D __instance, T[] data) where T : struct {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}
		__instance.SetData(0, 0, null, data, 0, data.Length);
		return false;
	}

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	private static bool OnSetData<T>(Texture2D __instance, T[] data, int startIndex, int elementCount) where T : struct {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}
		__instance.SetData(0, 0, null, data, startIndex, elementCount);
		return false;
	}

	[Harmonize("SetData", Harmonize.Fixation.Prefix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	private static bool OnSetData<T>(Texture2D __instance, int level, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return true;
		}
		__instance.SetData(level, 0, rect, data, startIndex, elementCount);
		return false;
	}

	[Harmonize("SetData", Harmonize.Fixation.Postfix, PriorityLevel.Last, Harmonize.Generic.Struct)]
	private static void OnSetData<T>(Texture2D __instance, int level, int arraySlice, in XNA.Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return;
		}
		using var _ = Performance.Track("SetData");
		SetDataPurge(
			__instance,
			rect,
			data,
			startIndex,
			elementCount
		);
	}

	#endregion

	/*
	[Harmonize("PlatformSetData", Harmonize.Fixation.Postfix, PriorityLevel.Last, Harmonize.Generic.Struct, platform: Harmonize.Platform.MonoGame)]
	private static void OnPlatformSetData<T>(Texture2D __instance, int level, T[] data, int startIndex, int elementCount) where T : struct {
		using var _ = Performance.Track("OnPlatformSetData0");
		SetDataPurge(
			__instance,
			null,
			data,
			startIndex,
			elementCount
		);
	}

	[Harmonize("PlatformSetData", Harmonize.Fixation.Postfix, PriorityLevel.Last, Harmonize.Generic.Struct, platform: Harmonize.Platform.MonoGame)]
	private static void OnPlatformSetData<T>(Texture2D __instance, int level, int arraySlice, XNA.Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct {
		using var _ = Performance.Track("OnPlatformSetData1");
		SetDataPurge(
			__instance,
			null,
			data,
			startIndex,
			elementCount
		);
	}
	*/

	// A horrible, horrible hack to stop a rare-ish crash when zooming or when the device resets. It doesn't appear to originate in SpriteMaster, but SM most certainly
	// makes it worse. This will force the texture to regenerate on the fly if it is in a zombie state.
	[Harmonize("Microsoft.Xna.Framework", "Microsoft.Xna.Framework.Helpers", "CheckDisposed", Harmonize.Fixation.Prefix, PriorityLevel.Last, instance: false, platform: Harmonize.Platform.XNA)]
	private static unsafe bool CheckDisposed(object obj, ref IntPtr pComPtr) {
		if (obj is ManagedTexture2D) {
			return true;
		}

		if (obj is GraphicsResource resource) {
			if (pComPtr == IntPtr.Zero || resource.IsDisposed) {
				if (!resource.IsDisposed) {
					resource.Dispose();
				}

				if (resource is Texture2D texture) {
					Debug.WarningLn("CheckDisposed is going to throw, attempting to restore state");

					// TODO : we should probably use the helper function it calls instead, just in case the user defined a child class.
					var ctor = texture.GetType().GetConstructor(
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
						null,
						new[] {
								typeof(GraphicsDevice),
								typeof(int),
								typeof(int),
								typeof(bool),
								typeof(SurfaceFormat)
						},
						null
					);

					ctor?.Invoke(texture, new object[] { DrawState.Device, texture.Width, texture.Height, texture.LevelCount > 1, texture.Format });
					//pComPtr = (IntPtr)(void*)texture.GetField("pComPtr");
					return false;
				}
			}
		}
		return true;
	}
}
