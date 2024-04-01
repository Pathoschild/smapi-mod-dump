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
using System.Runtime.CompilerServices;

// TODO : Handle X or Y-only scaling, since the game has a lot of 1xY and Xx1 sprites - 1D textures.
namespace SpriteMaster.Resample.Scalers.xBREPX;

internal sealed class Config : Resample.Scalers.LuminanceConfig {
	internal const int MaxScale = 4;

	// These are the default values:
	internal readonly uint EqualColorTolerance;
	internal readonly double DominantDirectionThreshold;
	internal readonly double SteepDirectionThreshold;
	internal readonly double CenterDirectionBias;
	internal readonly bool UseRedmean;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Config(
		Vector2B wrapped,
		bool hasAlpha = true,
		double luminanceWeight = 1.0,
		uint equalColorTolerance = 30,
		double dominantDirectionThreshold = 3.6,
		double steepDirectionThreshold = 2.2,
		double centerDirectionBias = 4.0,
		bool gammaCorrected = true,
		bool useRedmean = false
	) : base(
		wrapped: wrapped,
		hasAlpha: hasAlpha,
		gammaCorrected: gammaCorrected,
		luminanceWeight: luminanceWeight
	) {
		EqualColorTolerance = equalColorTolerance << 8;
		DominantDirectionThreshold = dominantDirectionThreshold;
		SteepDirectionThreshold = steepDirectionThreshold;
		CenterDirectionBias = centerDirectionBias;
		UseRedmean = useRedmean;
	}
}
