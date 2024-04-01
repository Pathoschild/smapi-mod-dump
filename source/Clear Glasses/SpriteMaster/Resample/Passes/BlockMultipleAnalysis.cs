/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;

namespace SpriteMaster.Resample.Passes;

internal static class BlockMultipleAnalysis {
	private static bool BlockTest(ReadOnlySpan<Color8> data, Bounds textureBounds, Bounds spriteBounds, int stride, int block) {
		// If it doesn't align to the bounds, then we cannot analyze this multiple.
		if ((spriteBounds.Extent % block) != Vector2I.Zero) {
			return false;
		}

		int equalityThreshold = Config.Resample.BlockMultipleAnalysis.EqualityThreshold;

		// determine reference texel

		var initialOffset = spriteBounds.Top * stride + spriteBounds.Left;
		for (int y = 0; y < spriteBounds.Height; y += block) {
			var yBaseOffset = initialOffset + y * stride;
			for (int x = 0; x < spriteBounds.Width; x += block) {
				for (int subY = 0; subY < block; ++subY) {
					var yOffset = yBaseOffset + (stride * subY);
					var rowReferenceColor = data[yOffset + x];
					for (int subX = 1; subX < block; ++subX) {
						var xOffset = yOffset + x + subX;
						if (!data[xOffset].Equals(rowReferenceColor, equalityThreshold)) {
							return false;
						}
					}
				}
				var columnReferenceColor = data[yBaseOffset];
				// If each row passes, compare the first element of each row against one another
				for (int subY = 1; subY < block; ++subY) {
					var yOffset = yBaseOffset + (stride * subY);
					if (!data[yOffset].Equals(columnReferenceColor, equalityThreshold)) {
						return false;
					}
				}
			}
		}

		return true;
	}

	private static int QuickBlockTest(ReadOnlySpan<Color8> data, Bounds textureBounds, Bounds spriteBounds, int stride) {
		int equalityThreshold = Config.Resample.BlockMultipleAnalysis.EqualityThreshold;

		// Scan row by row, just seeing what the largest sequential sequence of texels is. Return the smallest.
		int minimumBlock = Math.Min(spriteBounds.Width, spriteBounds.Height);

		var initialOffset = spriteBounds.Top * stride + spriteBounds.Left;
		for (int y = 0; y < spriteBounds.Height; ++y) {
			var yBaseOffset = initialOffset + y * stride;

			int currentContiguousSpan = 1;
			var lastColor = data[yBaseOffset];
			for (int x = 1; x < spriteBounds.Width; ++x) {
				int offset = yBaseOffset + x;
				var currentColor = data[offset];

				if (currentColor.Equals(lastColor, equalityThreshold)) {
					++currentContiguousSpan;
				}
				else {
					minimumBlock = Math.Min(minimumBlock, currentContiguousSpan);
					currentContiguousSpan = 1;
				}

				lastColor = currentColor;
			}

			minimumBlock = Math.Min(minimumBlock, currentContiguousSpan);
		}

		return minimumBlock;
	}

	internal static int Analyze(ReadOnlySpan<Color8> data, Bounds textureBounds, Bounds spriteBounds, int stride) {
		// A very quick test to determine the maximum possible block size, just checking horizontal stride
		var maxBlockLimit = QuickBlockTest(data, textureBounds, spriteBounds, stride);

		// Common multiples
		for (int block = maxBlockLimit; block > 1; --block) {
			if (BlockTest(data, textureBounds, spriteBounds, stride, block)) {
				return block;
			}
		}
		return 1;
	}
}
