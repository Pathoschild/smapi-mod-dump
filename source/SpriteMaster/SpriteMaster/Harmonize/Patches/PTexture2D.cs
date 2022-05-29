/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using HarmonyLib;
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using static Microsoft.Xna.Framework.Graphics.Texture2D;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
internal static class PTexture2D {
	#region Cache Handlers

	// https://benbowen.blog/post/fun_with_makeref/

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool Cacheable(XTexture2D texture) => texture.LevelCount <= 1;

	private static unsafe void SetDataPurge<T>(
		XTexture2D texture,
		XRectangle? rect,
		ReadOnlySpan<T> data,
		int startIndex,
		int elementCount,
		bool animated
	) where T : unmanaged {
		TextureCache.Remove(texture);

		if (!ManagedSpriteInstance.Validate(texture, clean: true)) {
			return;
		}

		if (texture.Format.IsBlock()) {
			ManagedSpriteInstance.FullPurge(texture, animated: animated);
			return;
		}

#if ASYNC_SETDATA
			var byteData = Cacheable(texture) ? GetByteArray(data, startIndex, elementCount, out int elementSize) : null;

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
		var byteData = Cacheable(texture) ? data : default;

		var span = byteData.IsEmpty ? default : byteData.Slice(startIndex, elementCount).AsBytes();

		ManagedSpriteInstance.Purge(
			reference: texture,
			bounds: rect,
			data: new(span),
			animated: animated
		);
#endif
	}

	#endregion

	#region SetData

	private static bool CheckIsDataChanged<T>(
		XTexture2D instance,
		int level,
		int arraySlice,
		XRectangle? inRect,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		Bounds rect = inRect ?? instance.Bounds;

		if (instance.TryMeta(out var meta) && meta.CachedData is { } cachedData) {
			var dataSpan = data.AsReadOnlySpan().Cast<T, byte>();

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

				for (int y = 0; y < rect.Height; ++y) {
					var inSpanRow = inSpan.Slice(inOffset, inRowLength);
					var cachedSpanRow = cachedSpan.GetRowSpan(y);
					if (!inSpanRow.SequenceEqual(cachedSpanRow)) {
						return true;
					}
					inOffset += inRowLength;
				}

				return false;
			}
		}

		return true;
	}

	internal static readonly Action<XTexture2D, int, int, bool, SurfaceFormat, SurfaceType, bool>? PlatformConstruct =
		typeof(XTexture2D).GetInstanceMethod("PlatformConstruct")
			?.CreateDelegate<Action<XTexture2D, int, int, bool, SurfaceFormat, SurfaceType, bool>>();

	[Harmonize(".ctor", Fixation.Prefix, PriorityLevel.Last)]
	public static void OnConstructTexture2D(
		XTexture2D __instance,
		GraphicsDevice graphicsDevice,
		int width,
		int height,
		bool mipmap,
		SurfaceFormat format,
		ref SurfaceType type,
		bool shared,
		int arraySize,
		ref bool __state
	) {
		if (PlatformConstruct is not null && arraySize == 1 && type == SurfaceType.Texture) {
			type = SurfaceType.SwapChainRenderTarget;
			__state = true;
		}
		else {
			__state = false;
		}
	}

	[Harmonize(".ctor", Fixation.Postfix, PriorityLevel.First)]
	public static void OnConstructTexture2DPost(
		XTexture2D __instance,
		GraphicsDevice graphicsDevice,
		int width,
		int height,
		bool mipmap,
		SurfaceFormat format,
		SurfaceType type,
		bool shared,
		int arraySize,
		bool __state
	) {
		if (!__state) {
			return;
		}

		type = SurfaceType.Texture;

		if (!GL.Texture2DExt.Construct<byte>(__instance, default, (width, height), mipmap, format, type, shared)) {
			PlatformConstruct!(__instance, width, height, mipmap, format, type, shared);
		}
	}

	// private void PlatformGetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct

	[Harmonize("PlatformGetData", Fixation.Prefix, PriorityLevel.Last, Generic.Struct)]
	public static unsafe bool OnPlatformGetData<T>(
		XTexture2D __instance,
		int level,
		int arraySlice,
		XRectangle rect,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		if (!Config.IsEnabled || !Config.SMAPI.ApplyGetDataPatch) {
			return true;
		}

		if (data.Length != 0 && !GetCachedData<T>(__instance, level, arraySlice, rect, data, startIndex, elementCount).IsEmpty) {
			return true;
		}

		return true;
	}

	internal static unsafe Span<T> GetCachedData<T>(
		XTexture2D __instance,
		int level,
		int arraySlice,
		Bounds rect,
		Span<T> data,
		int startIndex = 0,
		int? elementCount = null
	) where T : unmanaged {
		// While we do technically cache block data, the internal routines for updating them don't work well
		// and thus we assume that purges are required. We do not presently have the ability to handle this
		// sanely.
		if (__instance.Format.IsBlock()) {
			return default;
		}

		try {
			if (
				level == 0 &&
				arraySlice == 0 &&
				__instance.TryMeta(out var sourceMeta) && sourceMeta.CachedData is { } cachedSourceData
			) {
				int numElements = elementCount ?? __instance.Format.SizeBytes(rect.Area) / sizeof(T);

				if (cachedSourceData.Length < numElements * sizeof(T)) {
					throw new ArgumentException($"{cachedSourceData.Length} < {numElements * sizeof(T)}", nameof(numElements));
				}

				if (data.IsEmpty && startIndex == 0) {
					data = SpanExt.Make<T>(numElements);
				}

				if (data.Length < numElements + startIndex) {
					throw new ArgumentException($"{data.Length} < {numElements + startIndex}", nameof(data));
				}

				if (rect == __instance.Bounds) {
					ReadOnlySpan<byte> sourceBytes = cachedSourceData;
					var source = sourceBytes.Cast<T>();
					source.CopyTo(data, 0, startIndex, numElements);

					return data;
				}
				if (__instance.Bounds.Contains(rect)) {
					// We need a subcopy
					var cachedData = cachedSourceData.AsReadOnlySpan<T>();
					var destData = data;
					int sourceStride = __instance.Width;
					int destStride = rect.Width;
					int sourceOffset = (rect.Top * sourceStride) + rect.Left;
					int destOffset = startIndex;
					for (int y = 0; y < rect.Height; ++y) {
						cachedData.Slice(sourceOffset, destStride).CopyTo(destData.Slice(destOffset, destStride));
						sourceOffset += sourceStride;
						destOffset += destStride;
					}

					return data;
				}
			}
		}
		catch (Exception ex) {
			Debug.Error("OnGetData optimization threw an exception", ex);
		}

		return default;
	}

	#endregion

	private static readonly Assembly? CpaAssembly = AppDomain.CurrentDomain.GetAssemblies().
		SingleOrDefaultF(assembly => assembly.GetName().Name == "ContentPatcherAnimations");

	private static bool IsFromContentPatcherAnimations(XTexture2D instance, Bounds bounds) {
		if (CpaAssembly is null) {
			return false;
		}

		if (instance.TryMeta(out var meta) && meta.IsAnyAnimated(instance, bounds)) {
			return true;
		}

		var stackTrace = new StackTrace(skipFrames: 2, fNeedFileInfo: false);
		foreach (var frame in stackTrace.GetFrames()) {
			if (frame.GetMethod() is { } method && method.DeclaringType?.Assembly == CpaAssembly) {
				return true;
			}
		}

		return false;
	}


	// XTexture2D __instance, int level, int arraySlice, ref XRectangle rect, T[] data, int startIndex, int elementCount
	[HarmonizeTranspile(
		type: typeof(XTexture2D),
		"PlatformSetData",
		argumentTypes: new [] { typeof(int), typeof(int), typeof(XRectangle), typeof(Array), typeof(int), typeof(int) },
		generic: Generic.Struct,
		platform: Platform.MonoGame
	)]
	public static IEnumerable<CodeInstruction> PlatformSetDataTranspiler2<T>(
		IEnumerable<CodeInstruction> instructions,
		ILGenerator generator
	) where T : unmanaged {
		static bool HasParameters(MethodInfo method) {
			var parameters = method.GetParameters().Types();

			return
				parameters.ElementAtOrDefaultF(0)?.RemoveRef() == typeof(XTexture2D) &&
				parameters.ElementAtOrDefaultF(1)?.RemoveRef() == typeof(int) &&
				parameters.ElementAtOrDefaultF(2)?.RemoveRef() == typeof(int) &&
				parameters.ElementAtOrDefaultF(3)?.RemoveRef() == typeof(XRectangle) &&
				(parameters.ElementAtOrDefaultF(4)?.IsAssignableTo(typeof(Array)) ?? false) &&
				parameters.ElementAtOrDefaultF(5)?.RemoveRef() == typeof(int) &&
				parameters.ElementAtOrDefaultF(6)?.RemoveRef() == typeof(int);
		}

		var preMethod = typeof(PTexture2D).GetMethods(
			name: "OnPlatformSetDataPre",
			bindingFlags: BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
		).FirstF(HasParameters)?.MakeGenericMethod(typeof(T));

		if (preMethod is null) {
			Debug.Error("Could not apply PlatformSetData patch: could not find MethodInfo for OnPlatformSetDataPre");
			return instructions;
		}

		var postMethod = typeof(PTexture2D).GetMethods(
			name: "OnPlatformSetDataPost",
			bindingFlags: BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
		).FirstF(HasParameters)?.MakeGenericMethod(typeof(T));

		if (postMethod is null) {
			Debug.Error("Could not apply PlatformSetData patch: could not find MethodInfo for OnPlatformSetDataPost");
			return instructions;
		}

		var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

		IEnumerable<CodeInstruction> ApplyPatch() {
			var earlyReturn = generator.DefineLabel();
			var postCall = generator.DefineLabel();

			yield return new(OpCodes.Ldarg_0);
			yield return new(OpCodes.Ldarg_1);
			yield return new(OpCodes.Ldarg_2);
			yield return new(OpCodes.Ldarg_3);
			yield return new(OpCodes.Ldarg, 4);
			yield return new(OpCodes.Ldarg, 5);
			yield return new(OpCodes.Ldarg, 6);
			yield return new(OpCodes.Call, preMethod);
			
			yield return new(OpCodes.Brfalse_S, earlyReturn);

			
			foreach (var instruction in codeInstructions) {
				if (instruction == codeInstructions.LastF()) {
					yield return new(OpCodes.Ldarg_0) { labels = { postCall } };
					yield return new(OpCodes.Ldarg_1);
					yield return new(OpCodes.Ldarg_2);
					yield return new(OpCodes.Ldarg_3);
					yield return new(OpCodes.Ldarg, 4);
					yield return new(OpCodes.Ldarg, 5);
					yield return new(OpCodes.Ldarg, 6);
					yield return new(OpCodes.Call, postMethod);

					instruction.labels.Add(earlyReturn);
					yield return instruction;
				}
				else if (instruction.opcode.Value == OpCodes.Ret.Value) {
					yield return new(OpCodes.Jmp, postCall);
				}
				else {
					yield return instruction;
				}
			}
		}

		return ApplyPatch(); ;
	}

	[HarmonizeTranspile(
		type: typeof(XTexture2D),
		"PlatformSetData",
		argumentTypes: new[] { typeof(int), typeof(Array), typeof(int), typeof(int) },
		generic: Generic.Struct,
		platform: Platform.MonoGame
	)]
	public static IEnumerable<CodeInstruction> PlatformSetDataTranspiler<T>(
		IEnumerable<CodeInstruction> instructions,
		ILGenerator generator
	) where T : unmanaged {
		static bool HasParameters(MethodInfo method) {
			var parameters = method.GetParameters().Types();

			return
				parameters.ElementAtOrDefaultF(0) == typeof(XTexture2D) &&
				parameters.ElementAtOrDefaultF(1) == typeof(int) &&
				(parameters.ElementAtOrDefaultF(2)?.IsAssignableTo(typeof(Array)) ?? false) &&
				parameters.ElementAtOrDefaultF(3) == typeof(int) &&
				parameters.ElementAtOrDefaultF(4) == typeof(int);
		}

		var preMethod = typeof(PTexture2D).GetMethods(
			name: "OnPlatformSetDataPre",
			bindingFlags: BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
		).FirstF(HasParameters)?.MakeGenericMethod(typeof(T));

		if (preMethod is null) {
			Debug.Error("Could not apply PlatformSetData patch: could not find MethodInfo for OnPlatformSetDataPre");
			return instructions;
		}

		var postMethod = typeof(PTexture2D).GetMethods(
			name: "OnPlatformSetDataPost",
			bindingFlags: BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
		).FirstF(HasParameters)?.MakeGenericMethod(typeof(T));

		if (postMethod is null) {
			Debug.Error("Could not apply PlatformSetData patch: could not find MethodInfo for OnPlatformSetDataPost");
			return instructions;
		}

		var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

		IEnumerable<CodeInstruction> ApplyPatch() {
			var earlyReturn = generator.DefineLabel();
			var postCall = generator.DefineLabel();

			yield return new(OpCodes.Ldarg_0);
			yield return new(OpCodes.Ldarg_1);
			yield return new(OpCodes.Ldarg_2);
			yield return new(OpCodes.Ldarg_3);
			yield return new(OpCodes.Ldarg, 4);
			yield return new(OpCodes.Call, preMethod);

			yield return new(OpCodes.Brfalse_S, earlyReturn);


			foreach (var instruction in codeInstructions) {
				if (instruction == codeInstructions.LastF()) {
					yield return new(OpCodes.Ldarg_0) { labels = { postCall } };
					yield return new(OpCodes.Ldarg_1);
					yield return new(OpCodes.Ldarg_2);
					yield return new(OpCodes.Ldarg_3);
					yield return new(OpCodes.Ldarg, 4);
					yield return new(OpCodes.Call, postMethod);

					instruction.labels.Add(earlyReturn);
					yield return instruction;
				}
				else if (instruction.opcode.Value == OpCodes.Ret.Value) {
					yield return new(OpCodes.Jmp, postCall);
				}
				else {
					yield return instruction;
				}
			}
		}

		return ApplyPatch();
	}

	//[Harmonize("PlatformSetData", Fixation.Prefix, PriorityLevel.Last, Generic.Struct, platform: Platform.MonoGame)]
	public static bool OnPlatformSetDataPre<T>(
		XTexture2D __instance,
		int level,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		XRectangle rect = __instance.Bounds();
		return OnPlatformSetDataPre(__instance, level, 0, rect, data, startIndex, elementCount);
	}

	//[Harmonize("PlatformSetData", Fixation.Prefix, PriorityLevel.Last, Generic.Struct, platform: Platform.MonoGame)]
	public static bool OnPlatformSetDataPre<T>(
		XTexture2D __instance,
		int level,
		int arraySlice,
		XRectangle rect,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		var rectVal = rect;

		unsafe bool TryInternal() {
			if (arraySlice == 0) {
				fixed (T* dataPtr = data) {
					ReadOnlyPinnedSpan<T> span = new(data, dataPtr + startIndex, elementCount);
					if (GL.Texture2DExt.SetDataInternal(__instance, level, rectVal, span)) {
						return true;
					}
				}
			}

			return false;
		}

		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return !TryInternal();
		}

		if (__instance.Format.IsBlock()) {
			__instance.Meta().IncrementRevision();
			return !TryInternal();
		}

		try {
			if (!CheckIsDataChanged(__instance, level, arraySlice, rect, data, startIndex, elementCount)) {
				Debug.Trace(
					$"SetData for '{__instance.NormalizedName()}' skipped; data unchanged ({rect})".Colorized(
						DrawingColor.LightGreen
					)
				);
				return false;
			}
		}
		catch (Exception ex) {
			Debug.Warning($"Exception while processing SetData for '{__instance.NormalizedName()}'", ex);
		}

		__instance.Meta().IncrementRevision();
		return !TryInternal();
	}

	//[Harmonize("PlatformSetData", Fixation.Postfix, PriorityLevel.Last, Generic.Struct, platform: Platform.MonoGame)]
	public static void OnPlatformSetDataPost<T>(
		XTexture2D __instance,
		int level,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		OnPlatformSetDataPost(__instance, level, 0, __instance.Bounds(), data, startIndex, elementCount);
	}

	//[Harmonize("PlatformSetData", Fixation.Postfix, PriorityLevel.Last, Generic.Struct, platform: Platform.MonoGame)]
	public static void OnPlatformSetDataPost<T>(
		XTexture2D __instance,
		int level,
		int arraySlice,
		XRectangle rect,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		OnPlatformSetDataPostInternal(__instance, level, arraySlice, rect, data.AsReadOnlySpan(), startIndex, elementCount);
	}

	internal static unsafe void OnPlatformSetDataPostInternal<T>(
		XTexture2D __instance,
		int level,
		int arraySlice,
		Bounds rect,
		ReadOnlySpan<T> data,
		int startIndex = 0,
		int? elementCount = null
	) where T : unmanaged {
		if (__instance is (ManagedTexture2D or InternalTexture2D)) {
			return;
		}

		SetDataPurge(
			__instance,
			rect,
			data,
			startIndex,
			elementCount ?? (__instance.Format.SizeBytes(rect.Area) / sizeof(T)),
			animated: IsFromContentPatcherAnimations(__instance, rect)
		);
	}
}
