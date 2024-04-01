/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Diagnostics;

namespace SpriteMaster;

internal static partial class DrawState {
	[Conditional("DEBUG")]
	private static void CheckStates() {
		/*
#if DEBUG
		// Warn if we see some blend and sampler states that we don't presently handle
		if (CurrentSamplerState.AddressU is (TextureAddressMode.Border or TextureAddressMode.Mirror or TextureAddressMode.Wrap) && AlreadyPrintedSetSampler.Add(CurrentSamplerState)) {
			Debug.Trace($"SamplerState.AddressU: Unhandled Sampler State: {CurrentSamplerState.AddressU}");
		}
		if (CurrentSamplerState.AddressV is (TextureAddressMode.Border or TextureAddressMode.Mirror or TextureAddressMode.Wrap) && AlreadyPrintedSetSampler.Add(CurrentSamplerState)) {
			Debug.Trace($"SamplerState.AddressV: Unhandled Sampler State: {CurrentSamplerState.AddressV}");
		}
		if (CurrentBlendState != BlendState.AlphaBlend && AlreadyPrintedSetBlend.Add(CurrentBlendState)) {
			Debug.Trace($"BlendState: Unhandled Blend State: {CurrentBlendState.Dump()}");
		}
#endif
		*/
	}
}
