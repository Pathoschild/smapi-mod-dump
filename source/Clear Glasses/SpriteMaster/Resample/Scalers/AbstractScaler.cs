/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Colors;
using SpriteMaster.Types;

namespace SpriteMaster.Resample.Scalers;

internal abstract class AbstractScaler<TConfig, TScale> where TConfig : Config where TScale : struct, IScale {
	protected readonly uint ScaleMultiplier;
	protected readonly TConfig Configuration;
	protected readonly ColorHelpers.YccConfig YccConfiguration = default;
	protected readonly Vector2I SourceSize;
	protected readonly Vector2I TargetSize;

	protected AbstractScaler(TConfig configuration, uint scaleMultiplier, Vector2I sourceSize, Vector2I targetSize) {
		TScale scale = default;

		if (scaleMultiplier < scale.Minimum || scaleMultiplier > scale.Maximum) {
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(scaleMultiplier), scaleMultiplier, $"< {scale.Minimum} or > {scale.Maximum}");
		}

		if (sourceSize.X <= 0 || sourceSize.Y <= 0) {
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(sourceSize), sourceSize, "degenerate");
		}

		ScaleMultiplier = scaleMultiplier;
		Configuration = configuration;
		SourceSize = sourceSize;
		TargetSize = targetSize;
		if (configuration is LuminanceConfig luminanceConfig) {
			YccConfiguration = new() {
					LuminanceWeight = luminanceConfig.LuminanceWeight,
					ChrominanceWeight = luminanceConfig.ChrominanceWeight
			};
		}
	}
}
