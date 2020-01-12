using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using static SpriteMaster.Harmonize.Harmonize;
using static SpriteMaster.ScaledTexture;

using SpriteBatcher = System.Object;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PlatformRenderBatch {
		private static SamplerState GetNewSamplerState(Texture texture, SamplerState reference) {
			if (texture is ManagedTexture2D managedTexture && managedTexture.Texture != null) {
				var newState = new SamplerState() {
					AddressU = managedTexture.Texture.Wrapped.X ? TextureAddressMode.Wrap : reference.AddressU,
					AddressV = managedTexture.Texture.Wrapped.Y ? TextureAddressMode.Wrap : reference.AddressV,
					AddressW = reference.AddressW,
					MaxAnisotropy = reference.MaxAnisotropy,
					MaxMipLevel = reference.MaxMipLevel,
					MipMapLevelOfDetailBias = reference.MipMapLevelOfDetailBias,
					Name = "RescaledSampler",
					Tag = reference.Tag,
					Filter = (Config.DrawState.SetLinear) ? TextureFilter.Linear : reference.Filter
				};

				return newState;
			}

			/*
			else if (texture is RenderTarget2D) {
				var newState = new SamplerState() {
					AddressU = OriginalState.AddressU,
					AddressV = OriginalState.AddressV,
					AddressW = OriginalState.AddressW,
					MaxAnisotropy = OriginalState.MaxAnisotropy,
					MaxMipLevel = OriginalState.MaxMipLevel,
					MipMapLevelOfDetailBias = OriginalState.MipMapLevelOfDetailBias,
					Name = "RescaledSampler",
					Tag = OriginalState.Tag,
					Filter = (Config.DrawState.SetLinear) ? TextureFilter.Linear : OriginalState.Filter
				};
			}
			*/

			return reference;
		}

		[Harmonize(
			"Microsoft.Xna.Framework.Graphics",
			"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
			"FlushVertexArray",
			HarmonizeAttribute.Fixation.Prefix,
			PriorityLevel.First,
			platform: HarmonizeAttribute.Platform.Unix
		)]
		internal static bool OnFlushVertexArray (
			SpriteBatcher __instance,
			int start,
			int end,
			Effect effect,
			Texture texture,
			GraphicsDevice ____device,
			ref SamplerState __state
		) {
			try {
				var OriginalState = ____device?.SamplerStates[0] ?? SamplerState.PointClamp;
				__state = OriginalState;

				var newState = GetNewSamplerState(texture, OriginalState);

				if (newState != OriginalState && ____device?.SamplerStates != null)
					____device.SamplerStates[0] = newState;
			}
			catch (Exception ex) {
				ex.PrintError();
			}

			return true;
		}

		[Harmonize(
			"Microsoft.Xna.Framework.Graphics",
			"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
			"FlushVertexArray",
			HarmonizeAttribute.Fixation.Postfix,
			PriorityLevel.Last,
			platform: HarmonizeAttribute.Platform.Unix
		)]
		internal static void OnFlushVertexArrayPost (
			SpriteBatcher __instance,
			int start,
			int end,
			Effect effect,
			Texture texture,
			GraphicsDevice ____device,
			ref SamplerState __state
		) {
			if (__state == null) {
				return;
			}

			try {
				if (____device?.SamplerStates != null)
					____device.SamplerStates[0] = __state;
			}
			catch (Exception ex) {
				ex.PrintError();
			}
		}

		[Harmonize(
			"PlatformRenderBatch",
			HarmonizeAttribute.Fixation.Prefix,
			PriorityLevel.First,
			platform: HarmonizeAttribute.Platform.Windows
		)]
		internal static bool OnPlatformRenderBatch (
			SpriteBatch __instance,
			Texture2D texture,
			object[] sprites,
			int offset,
			int count,
			ref SamplerState ___samplerState,
			ref SamplerState __state
		) {
			try {
				var OriginalState = ___samplerState ?? SamplerState.PointClamp;
				__state = OriginalState;

				var newState = GetNewSamplerState(texture, OriginalState);

				if (newState != OriginalState) {
					if (__instance?.GraphicsDevice?.SamplerStates != null)
						__instance.GraphicsDevice.SamplerStates[0] = newState;

					___samplerState = newState;
				}
			}
			catch (Exception ex) {
				ex.PrintError();
			}

			return true;
		}

		[Harmonize(
			"PlatformRenderBatch",
			HarmonizeAttribute.Fixation.Postfix,
			PriorityLevel.Last,
			platform: HarmonizeAttribute.Platform.Windows
		)]
		internal static void OnPlatformRenderBatchPost (
			SpriteBatch __instance,
			Texture2D texture,
			object[] sprites,
			int offset,
			int count,
			ref SamplerState ___samplerState,
			ref SamplerState __state
		) {
			if (__state == null) {
				return;
			}

			try {
				if (__instance?.GraphicsDevice?.SamplerStates != null)
					__instance.GraphicsDevice.SamplerStates[0] = __state;
				___samplerState = __state;
			}
			catch (Exception ex) {
				ex.PrintError();
			}
		}
	}
}
