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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster;

static partial class DrawState {

	private static readonly Func<SamplerState, SamplerState> SamplerStateClone =
		typeof(SamplerState).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic)?.CreateDelegate<Func<SamplerState, SamplerState>>() ?? throw new NullReferenceException(nameof(SamplerStateClone));

	private static readonly Func<BlendState, BlendState> BlendStateClone =
		typeof(BlendState).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic)?.CreateDelegate<Func<BlendState, BlendState>>() ?? throw new NullReferenceException(nameof(SamplerStateClone));

	private static readonly Func<RasterizerState, RasterizerState> RasterizerStateClone =
		typeof(RasterizerState).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic)?.CreateDelegate<Func<RasterizerState, RasterizerState>>() ?? throw new NullReferenceException(nameof(RasterizerStateClone));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static SamplerState ConditionallyClone(SamplerState value, SamplerState defaultValue) {
		if (value is null) {
			return defaultValue;
		}

		if (
			value == SamplerState.AnisotropicClamp ||
			value == SamplerState.AnisotropicWrap ||
			value == SamplerState.LinearClamp ||
			value == SamplerState.LinearWrap ||
			value == SamplerState.PointClamp ||
			value == SamplerState.PointWrap
		) {
			return value;
		}

		return SamplerStateClone(value);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static BlendState ConditionallyClone(BlendState value, BlendState defaultValue) {
		if (value is null) {
			return defaultValue;
		}

		if (
			value == BlendState.Additive ||
			value == BlendState.AlphaBlend ||
			value == BlendState.NonPremultiplied ||
			value == BlendState.Opaque
		) {
			return value;
		}

		return BlendStateClone(defaultValue);
	}

#if DEBUG
	private static string Dump(this BlendState blendState) {
		// TODO : use delegates instead of reflection here
		try {
			var sb = new StringBuilder();

			var properties = typeof(BlendState).GetProperties();
			int maxNameLen = int.MinValue;
			foreach (var property in properties) {
				if (property.Name.Length > maxNameLen) {
					maxNameLen = property.Name.Length;
				}
			}

			++maxNameLen;

			foreach (var property in properties) {
				try {
					var propertyValue = property.GetValue(blendState);
					sb.AppendLine($"{property.Name.PadLeft(maxNameLen)}: {(propertyValue is null ? "[null]" : propertyValue.ToString())}");
				}
				catch { }
			}

			return sb.ToString();
		}
		catch (Exception ex) {
			Debug.Warning(ex);
			return blendState.ToString();
		}
	}
#endif

	private static readonly HashSet<BlendState> AlreadyPrintedSetBlend = new();
	private static readonly HashSet<SamplerState> AlreadyPrintedSetSampler = new();

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
