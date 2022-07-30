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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using static SpriteMaster.Colors.ColorHelpers;

namespace SpriteMaster.Resample.Scalers;

internal static class Common {
	internal static uint ColorDistance(
		bool useRedmean,
		bool gammaCorrected,
		bool hasAlpha,
		Color16 pix1,
		Color16 pix2,
		in YccConfig yccConfig
	) {
		if (useRedmean) {
			return pix1.RedmeanDifference(pix2,
				linear: !gammaCorrected,
				alpha: hasAlpha
			);
		}
		else {
			return pix1.YccDifference(pix2,
				config: yccConfig,
				linear: !gammaCorrected,
				alpha: hasAlpha
			);
		}
	}

	internal static void ApplyValidate(
		Config config,
		uint scaleMultiplier,
		ReadOnlySpan<Color16> sourceData,
		Vector2I sourceSize,
		ref Span<Color16> targetData,
		Vector2I targetSize
	) {
		if (sourceSize.X * sourceSize.Y > sourceData.Length) {
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(sourceData), sourceSize.X * sourceSize.Y, "sourceSize larger than sourceData.Length");
		}

		var targetSizeCalculated = sourceSize * scaleMultiplier;
		if (targetSize != targetSizeCalculated) {
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(targetSize), targetSize, targetSizeCalculated.ToString());
		}

		if (targetData.IsEmpty) {
			targetData = SpanExt.MakePinned<Color16>(targetSize.Area);
		}
		else {
			if (targetSize.Area > targetData.Length) {
				ThrowHelper.ThrowArgumentOutOfRangeException(nameof(targetData), targetSize.Area, targetData.Length.ToString());
			}
		}
	}
}
