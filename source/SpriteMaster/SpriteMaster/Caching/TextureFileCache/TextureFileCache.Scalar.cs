/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte FixedMultiply(byte a, byte b) {
		uint value = (uint)a * b;
		value += byte.MaxValue;
		return (byte)(value >> 8);
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte FixedMultiply(Fixed8 a, Fixed8 b) =>
		FixedMultiply(a.Value, b.Value);
	
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void ProcessTextureScalar(Span<Color8> data) {
		for (int i = 0; i < data.Length; i++) {
			var pixel = data[i];
			var alpha = pixel.A;

			switch (alpha.Value) {
				case 0:
					Unsafe.AsRef(data[i].Packed) = 0;
					break;
				case byte.MaxValue:
					break;
				default:
					data[i] = new(
						FixedMultiply(pixel.R, alpha),
						FixedMultiply(pixel.G, alpha),
						FixedMultiply(pixel.B, alpha),
						alpha
					);
					break;
			}
		}
	}
}
