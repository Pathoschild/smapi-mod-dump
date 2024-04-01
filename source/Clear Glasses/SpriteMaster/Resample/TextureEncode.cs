/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample;

internal static class TextureEncode {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool Encode(
		ReadOnlySpan<Color8> data,
		ref TextureFormat format,
		Vector2I dimensions,
		bool hasAlpha,
		bool isPunchthroughAlpha,
		bool isMasky,
		bool hasR,
		bool hasG,
		bool hasB,
		out PinnedSpan<byte> result
	) =>
		Encoder.StbBlockEncoder.Encode(data, ref format, dimensions, hasAlpha, isPunchthroughAlpha, isMasky, hasR, hasG, hasB, out result);
}
