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
using System.Runtime.CompilerServices;

namespace Benchmarks.Sprites.Methods.Linearize;

internal static class RefSpan {
	private static readonly ColorSpace ColorSpace = ColorSpace.sRGB_Precise;
	private static readonly Converter ColorConverter = ColorSpace.GetConverter();
	private static readonly ConverterRef ColorConverterRef = ColorSpace.GetConverterRef();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Linearize(Span<Color16> data, Vector2I size) {
		foreach (ref var color in data) {
			color = ColorSpace.Linearize(color);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LinearizeStatic(Span<Color16> data, Vector2I size) {
		foreach (ref var color in data) {
			color = ColorSpace.sRGB_Precise.Linearize(color);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LinearizeConverter(Span<Color16> data, Vector2I size) {
		foreach (ref var color in data) {
			color = ColorConverter.Linearize(color);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LinearizeConverterRef(Span<Color16> data, Vector2I size) {
		foreach (ref var color in data) {
			color = ColorConverterRef.Linearize(color);
		}
	}
}
