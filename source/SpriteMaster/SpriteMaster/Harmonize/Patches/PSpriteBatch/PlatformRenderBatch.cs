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
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize.Patches.Game;
using SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using static SpriteMaster.Harmonize.Harmonize;

using SpriteMaster.Extensions.Reflection;
using System.Linq;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
internal static class PlatformRenderBatch {
	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowModeUnimplementedException<T>(string name, TextureAddressMode addressMode) =>
		throw new NotImplementedException($"{name} {addressMode} is unimplemented");

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowModeUnimplementedException<T>(string name, TextureFilter filter) =>
		throw new NotImplementedException($"{name} {filter} is unimplemented");

	private static SamplerState GetSamplerState(TextureAddressMode addressMode, TextureFilter filter) {
		return (addressMode, filter) switch {
			(TextureAddressMode.Wrap, TextureFilter.Point) => SamplerState.PointWrap,
			(TextureAddressMode.Wrap, TextureFilter.Linear) => SamplerState.LinearWrap,
			(TextureAddressMode.Wrap, TextureFilter.Anisotropic) => SamplerState.AnisotropicWrap,
			(TextureAddressMode.Clamp, TextureFilter.Point) => SamplerState.PointClamp,
			(TextureAddressMode.Clamp, TextureFilter.Linear) => SamplerState.LinearClamp,
			(TextureAddressMode.Clamp, TextureFilter.Anisotropic) => SamplerState.AnisotropicClamp,
			(TextureAddressMode.Border, TextureFilter.Point) => DrawState.SamplerStateExt.PointBorder.Value,
			(TextureAddressMode.Border, TextureFilter.Linear) => DrawState.SamplerStateExt.LinearBorder.Value,
			(TextureAddressMode.Border, TextureFilter.Anisotropic) => DrawState.SamplerStateExt.AnisotropicBorder.Value,
			(TextureAddressMode.Mirror, TextureFilter.Point) => DrawState.SamplerStateExt.PointMirror.Value,
			(TextureAddressMode.Mirror, TextureFilter.Linear) => DrawState.SamplerStateExt.LinearMirror.Value,
			(TextureAddressMode.Mirror, TextureFilter.Anisotropic) => DrawState.SamplerStateExt.AnisotropicMirror.Value,
			(_, TextureFilter.Point) => ThrowModeUnimplementedException<SamplerState>(nameof(TextureAddressMode), addressMode),
			(_, TextureFilter.Linear) => ThrowModeUnimplementedException<SamplerState>(nameof(TextureAddressMode), addressMode),
			(_, TextureFilter.Anisotropic) => ThrowModeUnimplementedException<SamplerState>(nameof(TextureAddressMode), addressMode),
			_ => ThrowModeUnimplementedException<SamplerState>(nameof(TextureFilter), filter),
		};
	}

	private static SamplerState GetNewSamplerState(Texture? texture, SamplerState reference) {
		if (!Config.DrawState.IsSetLinear) {
			return reference;
		}

		bool isInternalTexture = texture is InternalTexture2D;
		bool isLighting = !isInternalTexture && (texture?.NormalizedName().StartsWith(@"LooseSprites\Lighting\") ?? false);

		if (!isInternalTexture && !isLighting && !Config.DrawState.IsSetLinearUnresampled) {
			return reference;
		}

		IScalerInfo? scalerInfo = null;
		if (isInternalTexture && texture is ManagedTexture2D managedTexture) {
			scalerInfo = managedTexture.SpriteInstance.ScalerInfo;
		}

		var preferredFilter = scalerInfo?.Filter ?? TextureFilter.Linear;

		return reference.AddressU switch {
			TextureAddressMode.Wrap when reference.AddressV == TextureAddressMode.Wrap => GetSamplerState(
				addressMode: TextureAddressMode.Wrap, filter: preferredFilter
			),
			TextureAddressMode.Border when reference.AddressV == TextureAddressMode.Border => GetSamplerState(
				addressMode: TextureAddressMode.Border, filter: preferredFilter
			),
			TextureAddressMode.Mirror when reference.AddressV == TextureAddressMode.Mirror => GetSamplerState(
				addressMode: TextureAddressMode.Mirror, filter: preferredFilter
			),
			_ => GetSamplerState(addressMode: TextureAddressMode.Clamp, filter: preferredFilter)
		};
	}

	internal readonly record struct States(SamplerState? SamplerState, BlendState? BlendState);

	[HarmonizeTranspile(
		type: typeof(SpriteBatcher),
		"FlushVertexArray",
		argumentTypes: new[] { typeof(int), typeof(int), typeof(Effect), typeof(Texture) }
	)]
	public static IEnumerable<CodeInstruction> FlushVertexArrayTranspiler(IEnumerable<CodeInstruction> instructions) {
		var newMethod = new Action<GraphicsDevice, PrimitiveType, VertexPositionColorTexture[], int, int, short[], int, int, VertexDeclaration>(GL.GraphicsDeviceExt.DrawUserIndexedPrimitivesFlushVertexArray).Method;

		var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

		bool applied = false;

		static bool IsCall(OpCode opCode) {
			return opCode.Value == OpCodes.Call.Value || opCode.Value == OpCodes.Calli.Value || opCode.Value == OpCodes.Callvirt.Value;
		}

		IEnumerable<CodeInstruction> ApplyPatch() {
			foreach (var instruction in codeInstructions) {
				if (
					!IsCall(instruction.opcode) ||
					instruction.operand is not MethodInfo callee ||
					callee.Name != "DrawUserIndexedPrimitives"
				) {
					yield return instruction;
					continue;
				}

				yield return new(OpCodes.Call, newMethod) {
					labels = instruction.labels,
					blocks = instruction.blocks
				};
				//yield return new(OpCodes.Pop);
				applied = true;
			}
		}

		var result = ApplyPatch().ToArray();

		if (!applied) {
			Debug.Error("Could not apply SpriteBatcher FlushVertexArray patch.");
		}

		return result;
	}

	[Harmonize(
		"Microsoft.Xna.Framework.Graphics",
		"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
		"FlushVertexArray",
		Fixation.Prefix,
		PriorityLevel.First,
		platform: Platform.MonoGame
	)]
	public static void OnFlushVertexArray(
		SpriteBatcher __instance,
		int start,
		int end,
		Effect? effect,
		Texture? texture,
		GraphicsDevice? ____device,
		ref States __state
	) {
		if (!Config.IsEnabled) {
			return;
		}

		SamplerState? originalSamplerState = null;
		BlendState? originalBlendState = null;

		try {
			using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

			{
				var originalState = ____device?.SamplerStates[0] ?? SamplerState.PointClamp;

				var newState = GetNewSamplerState(texture, originalState);

				if (newState != originalState && ____device?.SamplerStates is not null) {
					originalSamplerState = originalState;
					____device.SamplerStates[0] = newState;
				}
				else {
					originalSamplerState = null;
				}
			}
			{
				if (____device is not null) {
					var originalState = ____device.BlendState;
					if (texture == Line.LineTexture.Value) {
						____device.BlendState = BlendState.AlphaBlend;
					}
					originalBlendState = originalState;
				}
				else {
					originalBlendState = null;
				}
			}
		}
		catch (Exception ex) {
			ex.PrintError();
		}

		__state = new(originalSamplerState, originalBlendState);
	}

	[Harmonize(
		"Microsoft.Xna.Framework.Graphics",
		"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
		"FlushVertexArray",
		Fixation.Postfix,
		PriorityLevel.Last,
		platform: Platform.MonoGame
	)]
	public static void OnFlushVertexArray(
		SpriteBatcher __instance,
		int start,
		int end,
		Effect? effect,
		Texture? texture,
		GraphicsDevice? ____device,
		States __state
	) {
		if (!Config.IsEnabled) {
			return;
		}

		try {
			using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

			if (__state.SamplerState is not null && ____device?.SamplerStates is not null && __state.SamplerState != ____device.SamplerStates[0]) {
				____device.SamplerStates[0] = __state.SamplerState;
			}
			if (__state.BlendState is not null && ____device?.BlendState is not null && __state.BlendState != ____device.BlendState) {
				____device.BlendState = __state.BlendState;
			}
		}
		catch (Exception ex) {
			ex.PrintError();
		}
	}

	private static bool EnsureArrayCapacityEnabled = true;

	[Harmonize(
		"Microsoft.Xna.Framework.Graphics",
		"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
		"EnsureArrayCapacity",
		Fixation.Prefix,
		PriorityLevel.Last,
		platform: Platform.MonoGame
	)]
	public static bool OnEnsureArrayCapacity(SpriteBatcher __instance, int numBatchItems) {
		if (!Config.IsEnabled || !EnsureArrayCapacityEnabled) {
			return true;
		}

		try {
			EnsureIndexCapacity(__instance, numBatchItems);
			EnsureVertexCapacity(__instance, numBatchItems);

			return false;
		}
		catch (MemberAccessException ex) {
			Debug.Error($"Disabling {nameof(OnEnsureArrayCapacity)} patch", ex);
			EnsureArrayCapacityEnabled = false;
			return true;
		}
		catch (InvalidCastException ex) {
			Debug.Error($"Disabling {nameof(OnEnsureArrayCapacity)} patch", ex);
			EnsureArrayCapacityEnabled = false;
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void EnsureIndexCapacity(SpriteBatcher @this, int numBatchItems) {
		@this._index = GL.GraphicsDeviceExt.SpriteBatcherValues.Indices16;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void EnsureVertexCapacity(SpriteBatcher @this, int numBatchItems) {
		int neededCapacity = numBatchItems << 2;
		if (@this._vertexArray is null || @this._vertexArray.Length < neededCapacity) {
			@this._vertexArray = GC.AllocateUninitializedArray<VertexPositionColorTexture>(neededCapacity);
		}
	}
}
