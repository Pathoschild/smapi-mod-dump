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
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
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

	internal delegate void PlatformConstructDelegate(
		XTexture2D instance, int width, int height, bool mipmap, SurfaceFormat format,
		SurfaceType type, bool shared
	);

	internal static readonly PlatformConstructDelegate? PlatformConstruct = typeof(XTexture2D).GetInstanceDelegate<PlatformConstructDelegate>("PlatformConstruct");

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

	private static readonly ThreadLocal<bool> IsInnerGetData = new(false);

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

		if (!IsInnerGetData.Value && data.Length != 0) {
			if (!GetCachedData<T>(__instance, level, arraySlice, rect, data, startIndex, elementCount).IsEmpty) {
				return false;
			}

			// Upon cache read failure, instead of doing a subtexture read, do a full read and use the data to repopulate the cache.
			if (!__instance.Format.IsBlock()) {
				try {
					IsInnerGetData.Value = true;

					var fullBounds = __instance.Bounds;
					var recacheData = GC.AllocateUninitializedArray<byte>(__instance.Format.SizeBytes(fullBounds.Width * fullBounds.Height));
					try {
						__instance.PlatformGetData(level, arraySlice, fullBounds, recacheData, 0, recacheData.Length);
						_ = OnPlatformSetDataPre(__instance, level, arraySlice, fullBounds, recacheData, 0, recacheData.Length);

						if (!GetCachedData<T>(__instance, level, arraySlice, rect, data, startIndex, elementCount).IsEmpty) {
							return false;
						}
					}
					catch {
						// Swallow Exception
					}
				}
				finally {
					IsInnerGetData.Value = false;
				}
			}
		}

		return !(arraySlice == 0 && GL.Texture2DExt.GetDataInternal(__instance, level, rect, data.AsSpan())); ;
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static Span<T> ThrowArgumentOutOfRangeLessThanException<T>(string name, int value, int constraint) =>
		throw new ArgumentOutOfRangeException(name, $"{value} < {constraint}");

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

		if (level != 0 || arraySlice != 0) {
			return default;
		}

		try {
			if (__instance.TryMeta(out var sourceMeta) && sourceMeta.HasCachedData && sourceMeta.CachedData is { } cachedSourceData) {
				int numElements;
				int byteSize;
				if (elementCount.HasValue) {
					numElements = elementCount.Value;
					byteSize = numElements * sizeof(T);
				}
				else {
					byteSize = __instance.Format.SizeBytes(rect.Area);
					numElements = byteSize / sizeof(T);
				}

				if (numElements == 0) {
					return default;
				}

				if (cachedSourceData.Length < byteSize) {
					return ThrowArgumentOutOfRangeLessThanException<T>(nameof(numElements), cachedSourceData.Length, byteSize);
				}

				if (data.IsEmpty && startIndex == 0) {
					data = SpanExt.Make<T>(numElements);
				}

				if (data.Length < numElements + startIndex) {
					return ThrowArgumentOutOfRangeLessThanException<T>(nameof(data), data.Length, numElements + startIndex);
				}

				if (rect == __instance.Bounds) {
					ReadOnlySpan<byte> sourceBytes = cachedSourceData;
					var source = sourceBytes.Cast<T>();
					source.CopyTo(data, 0, startIndex, numElements);

					return data;
				}
				if (__instance.Bounds.Contains(rect)) {
					// We need a subcopy
					var cachedData = cachedSourceData.AsReadOnlySpan<T>().Cast<T, uint>();
					var destData = data.Cast<T, uint>();
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

	private static readonly Assembly? CpaAssembly = AssemblyExt.GetAssembly("ContentPatcherAnimations");

	private static readonly VariableAccessor<StackTrace, StackFrame[]?>? GetStackFramesDelegate =
		typeof(StackTrace).GetInstanceVariable("_stackFrames")?.GetAccessor<StackTrace, StackFrame[]?>();
	private static readonly VariableAccessor<StackTrace, int>? GetNumFramesDelegate =
		typeof(StackTrace).GetInstanceVariable("_numOfFrames")?.GetAccessor<StackTrace, int>();

	[MemberNotNullWhen(true, "GetStackFramesDelegate", "GetNumFramesDelegate")]
	private static bool HasStackTraceDelegates { get; } =
		(GetStackFramesDelegate?.HasGetter ?? false) &&
		(GetNumFramesDelegate?.HasGetter ?? false);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ReadOnlySpan<StackFrame> GetStackFrames(int skipFrames) {
		var trace = new StackTrace(skipFrames: skipFrames, fNeedFileInfo: false);

		if (HasStackTraceDelegates) {
			ReadOnlySpan<StackFrame> stackFrames = GetStackFramesDelegate.Get(trace) ?? default;
			if (!stackFrames.IsEmpty) {
				int numFrames = GetNumFramesDelegate.Get(trace);
				return stackFrames.Slice(skipFrames, numFrames);
			}
		}

		return trace.GetFrames();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsFromContentPatcherAnimations(XTexture2D instance, Bounds bounds) {
		if (CpaAssembly is null) {
			return false;
		}

		if (instance.TryMeta(out var meta) && meta.IsAnyAnimated(instance, bounds)) {
			return true;
		}

		var stackFrames = GetStackFrames(skipFrames: 2);

		foreach (var frame in stackFrames) {
			if (ReferenceEquals(frame.GetMethod()?.DeclaringType?.Assembly, CpaAssembly)) {
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
		var preMethod = ((Func<XTexture2D, int, int, XRectangle, T[], int, int, bool>)OnPlatformSetDataPre).Method;
		var postMethod = ((Action<XTexture2D, int, int, XRectangle, T[], int, int>)OnPlatformSetDataPost).Method;

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

		return ApplyPatch(); 
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
		var preMethod = ((Func<XTexture2D, int, T[], int, int, bool>)OnPlatformSetDataPre).Method;
		var postMethod = ((Action<XTexture2D, int, T[], int, int>)OnPlatformSetDataPost).Method;

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

	[MethodImpl(Runtime.MethodImpl.Inline)]
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool OnPlatformSetDataPre<T>(
		XTexture2D __instance,
		int level,
		int arraySlice,
		XRectangle rect,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		if (__instance is not (ManagedTexture2D or InternalTexture2D)) {
			if (__instance.Format.IsBlock()) {
				__instance.Meta().IncrementRevision();
			}
			else {
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
			}
		}

		var span = data.AsReadOnlySpan().Slice(startIndex, elementCount);

		if (IsInnerGetData.Value) {
			return false;
		}

		return !(arraySlice == 0 && GL.Texture2DExt.SetDataInternal(__instance, level, rect, span));
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static void OnPlatformSetDataPost<T>(
		XTexture2D __instance,
		int level,
		T[] data,
		int startIndex,
		int elementCount
	) where T : unmanaged {
		OnPlatformSetDataPost(__instance, level, 0, __instance.Bounds(), data, startIndex, elementCount);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
