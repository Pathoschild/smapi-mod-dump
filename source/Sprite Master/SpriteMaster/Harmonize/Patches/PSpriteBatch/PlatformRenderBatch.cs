using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using static SpriteMaster.Harmonize.Harmonize;

using SpriteBatcher = System.Object;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PlatformRenderBatch {
		private static SamplerState GetNewSamplerState(Texture texture, SamplerState reference) {
			if (!Config.DrawState.SetLinear) {
				return reference;
			}

			if (texture is ManagedTexture2D managedTexture && managedTexture.Texture != null) {
				return SamplerState.LinearClamp;
			}
			else if (reference.Filter == TextureFilter.Linear) {
				return SamplerState.PointClamp;
			}

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
			GraphicsDevice ____device
		) {
			try {
				var OriginalState = ____device?.SamplerStates[0] ?? SamplerState.PointClamp;

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
			ref SamplerState ___samplerState
		) {
			try {
				var OriginalState = ___samplerState ?? SamplerState.PointClamp;

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
	}
}
