/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample;

static class TextureEncode {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe Span<byte> Encode(ReadOnlySpan<Color8> data, ref TextureFormat format, Vector2I dimensions, bool hasAlpha, bool isPunchthroughAlpha, bool isMasky, bool hasR, bool hasG, bool hasB) =>
		Encoder.TeximpBlockEncoder.Encode(data, ref format, dimensions, hasAlpha, isPunchthroughAlpha, isMasky, hasR, hasG, hasB);
}
