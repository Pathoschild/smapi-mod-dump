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
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize.Patches.Game;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static SpriteMaster.Harmonize.Harmonize;
using SpriteBatcher = System.Object;

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
			(TextureAddressMode.Border, TextureFilter.Point) => DrawState.PointBorder.Value,
			(TextureAddressMode.Border, TextureFilter.Linear) => DrawState.LinearBorder.Value,
			(TextureAddressMode.Border, TextureFilter.Anisotropic) => DrawState.AnisotropicBorder.Value,
			(TextureAddressMode.Mirror, TextureFilter.Point) => DrawState.PointMirror.Value,
			(TextureAddressMode.Mirror, TextureFilter.Linear) => DrawState.LinearMirror.Value,
			(TextureAddressMode.Mirror, TextureFilter.Anisotropic) => DrawState.AnisotropicMirror.Value,
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
}
