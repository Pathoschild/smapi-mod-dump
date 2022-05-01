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
using System.Runtime.CompilerServices;

// TODO : Handle X or Y-only scaling, since the game has a lot of 1xY and Xx1 sprites - 1D textures.
namespace SpriteMaster.Resample.Scalers.EPX;

sealed class Config : Resample.Scalers.Config {
	internal const int MaxScale = 3;

	internal readonly double LuminanceWeight;
	internal readonly uint EqualColorTolerance;
	internal readonly bool UseRedmean;

	// Precalculated
	internal readonly double ChrominanceWeight;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Config(
		Vector2B wrapped,
		bool hasAlpha = true,
		double luminanceWeight = 1.0,
		bool gammaCorrected = true,
		uint equalColorTolerance = 30,
		bool useRedmean = false
	) : base(
		wrapped: wrapped,
		hasAlpha: hasAlpha,
		gammaCorrected: gammaCorrected
	) {
		EqualColorTolerance = equalColorTolerance << 8;
		UseRedmean = useRedmean;

		var adjustedLuminanceWeight = luminanceWeight / (luminanceWeight + 1.0);
		LuminanceWeight = adjustedLuminanceWeight * 2.0;
		ChrominanceWeight = (1.0 - adjustedLuminanceWeight) * 2.0;
	}
}
