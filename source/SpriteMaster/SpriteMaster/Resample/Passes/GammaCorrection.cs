/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Colors;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Passes;

static class GammaCorrection {
	private static readonly ColorSpace ColorSpace = ColorSpace.sRGB_Precise;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Delinearize(Span<Color16> data, in Vector2I size) {
		foreach (ref Color16 color in data) {
			color.R = ColorSpace.Delinearize(color.R);
			color.G = ColorSpace.Delinearize(color.G);
			color.B = ColorSpace.Delinearize(color.B);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Linearize(Span<Color16> data, in Vector2I size) {
		foreach (ref Color16 color in data) {
			color.R = ColorSpace.Linearize(color.R);
			color.G = ColorSpace.Linearize(color.G);
			color.B = ColorSpace.Linearize(color.B);
		}
	}
}
