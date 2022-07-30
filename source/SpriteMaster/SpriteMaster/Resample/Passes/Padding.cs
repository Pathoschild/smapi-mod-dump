/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Passes;

internal static class Padding {
	private static readonly Color16 PadConstant = Color16.Zero;

	private record struct PaddingParameters(Vector2I PaddedSize, int ExpectedPadding, Vector2B HasPaddingX, Vector2B HasPaddingY, Vector2B SolidEdgeX, Vector2B SolidEdgeY);

	private static bool GetPaddingParameters(Vector2I spriteSize, uint scale, SpriteInfo input, bool forcePadding, in Analysis.LegacyResults analysis, out PaddingParameters parameters) {
		if (!Config.Resample.Padding.Enabled) {
			parameters = new();
			return false;
		}

		var hasPaddingX = Vector2B.True;
		var hasPaddingY = Vector2B.True;

		Vector2B solidEdgeX = (
			analysis.RepeatX | analysis.Wrapped.X | spriteSize.Width <= 1
		);

		Vector2B solidEdgeY = (
			analysis.RepeatY | analysis.Wrapped.Y | spriteSize.Height <= 1
		);

		if (!Config.Resample.Padding.PadSolidEdges) {
			hasPaddingX = solidEdgeX.Invert;
			hasPaddingY = solidEdgeY.Invert;
		}

		if (Config.Resample.Padding.AlwaysList.AnyF(prefix => input.Reference.NormalizedName().StartsWith(prefix))) {
			hasPaddingX = Vector2B.True;
			hasPaddingY = Vector2B.True;
		}

		int minTexels = Config.Resample.Padding.MinimumSizeTexels;

		if ((hasPaddingX.Any || hasPaddingY.Any) && (spriteSize.X <= minTexels && spriteSize.Y <= minTexels)) {
			hasPaddingX = Vector2B.False;
			hasPaddingY = Vector2B.False;
		}
		else if ((hasPaddingX.Any || hasPaddingY.Any) && (Config.Resample.Padding.IgnoreUnknown && !input.Reference.Anonymous())) {
			hasPaddingX = Vector2B.False;
			hasPaddingY = Vector2B.False;
		}

		if (forcePadding) {
			hasPaddingX = Vector2B.True;
			hasPaddingY = Vector2B.True;
		}

		if (hasPaddingX.None && hasPaddingY.None) {
			parameters = new();
			return false;
		}

		// TODO we only need to pad the edge that has texels. Double padding is wasteful.
		var paddedSpriteSize = spriteSize;

		var expectedPadding = (int)Math.Max(1U, scale / 2);

		int clampDimension = Config.ClampDimension;

		if (hasPaddingX.X) {
			if ((paddedSpriteSize.X + expectedPadding) * scale > clampDimension) {
				hasPaddingX.X = false;
			}
			else {
				paddedSpriteSize.X += expectedPadding;
			}
		}

		if (hasPaddingX.Y) {
			if ((paddedSpriteSize.X + expectedPadding) * scale > clampDimension) {
				hasPaddingX.Y = false;
			}
			else {
				paddedSpriteSize.X += expectedPadding;
			}
		}

		if (hasPaddingY.X) {
			if ((paddedSpriteSize.Y + expectedPadding) * scale > clampDimension) {
				hasPaddingY.X = false;
			}
			else {
				paddedSpriteSize.Y += expectedPadding;
			}
		}

		if (hasPaddingY.Y) {
			if ((paddedSpriteSize.Y + expectedPadding) * scale > clampDimension) {
				hasPaddingY.Y = false;
			}
			else {
				paddedSpriteSize.Y += expectedPadding;
			}
		}

		if (hasPaddingX.None && hasPaddingY.None) {
			parameters = new();
			return false;
		}

		parameters = new(
			PaddedSize: paddedSpriteSize,
			ExpectedPadding: expectedPadding,
			HasPaddingX: hasPaddingX,
			HasPaddingY: hasPaddingY,
			SolidEdgeX: solidEdgeX,
			SolidEdgeY: solidEdgeY
		);

		return true;
	}

	internal static Span<Color16> Apply(ReadOnlySpan<Color16> data, Vector2I spriteSize, uint scale, SpriteInfo input, bool forcePadding, in Analysis.LegacyResults analysis, out PaddingQuad padding, out Vector2I paddedSize) {
		if (!GetPaddingParameters(spriteSize, scale, input, forcePadding, analysis, out var parameters)) {
			padding = PaddingQuad.Zero;
			paddedSize = spriteSize;
			return data.ToSpanUnsafe();
		}

		var paddedSpriteSize = parameters.PaddedSize;
		var expectedPadding = parameters.ExpectedPadding;
		var hasPaddingX = parameters.HasPaddingX;
		var hasPaddingY = parameters.HasPaddingY;

		// The actual padding logic. If we get to this point, we are actually performing padding.

		var paddedData = SpanExt.Make<Color16>(paddedSpriteSize.Area);
		paddedData.Clear();

		{
			int y = 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void WritePaddingY(Span<Color16> data) {
				for (int i = 0; i < expectedPadding; ++i) {
					var strideOffset = y * paddedSpriteSize.Width;
					for (int x = 0; x < paddedSpriteSize.Width; ++x) {
						data.At(strideOffset + x) = PadConstant;
					}
					++y;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void WritePaddingX(Span<Color16> data, ref int xOffset) {
				for (int x = 0; x < expectedPadding; ++x) {
					data.At(xOffset++) = PadConstant;
				}
			}

			if (hasPaddingY.X) {
				WritePaddingY(paddedData);
			}

			for (int i = 0; i < spriteSize.Height; ++i) {
				// Write a padded X line
				var xOffset = y * paddedSpriteSize.Width;

				if (hasPaddingX.X) {
					WritePaddingX(paddedData, ref xOffset);
				}
				data.CopyTo(paddedData, i * spriteSize.Width, xOffset, spriteSize.Width);
				xOffset += spriteSize.Width;
				if (hasPaddingX.Y) {
					WritePaddingX(paddedData, ref xOffset);
				}
				++y;
			}

			if (hasPaddingY.Y) {
				WritePaddingY(paddedData);
			}
		}

		int startingYOffset = parameters.HasPaddingY.X ? expectedPadding : 0;
		int startingXOffset = parameters.HasPaddingX.X ? expectedPadding : 0;

		// If we had solid edges that we are padding, copy the color (but not the alpha) over by one.
		if (parameters.HasPaddingX.X && parameters.SolidEdgeX.X) {
			for (int y = 0; y < spriteSize.Height; ++y) {
				int yOffset = (y + startingYOffset) * paddedSpriteSize.Width;

				var src = paddedData.At(yOffset + startingXOffset);
				src.A = 128;
				paddedData.At(yOffset + startingXOffset - 1) = src;
			}
		}
		if (parameters.HasPaddingX.Y && parameters.SolidEdgeX.Y) {
			for (int y = 0; y < spriteSize.Height; ++y) {
				int yOffset = (y + startingYOffset) * paddedSpriteSize.Width;

				int xSrcOffset = startingXOffset + spriteSize.Width - 1;

				var src = paddedData.At(yOffset + xSrcOffset);
				src.A = 128;
				paddedData.At(yOffset + xSrcOffset + 1) = src;
			}
		}

		if (parameters.HasPaddingY.X && parameters.SolidEdgeY.X) {
			Vector2B withXPadding = parameters.HasPaddingX & parameters.SolidEdgeX;

			int ySrcOffset = startingYOffset * paddedSpriteSize.Width;
			int yDstOffset = (startingYOffset - 1) * paddedSpriteSize.Width;

			int xOffset = withXPadding.X ? -1 : 0;
			int widthAdd = withXPadding.All ? 2 : withXPadding.Any ? 1 : 0;

			for (int x = 0; x < spriteSize.Width + widthAdd; ++x) {
				var src = paddedData.At(ySrcOffset + startingXOffset + xOffset + x);
				src.A = 128;
				paddedData.At(yDstOffset + startingXOffset + xOffset + x) = src;
			}
		}

		if (parameters.HasPaddingY.Y && parameters.SolidEdgeY.Y) {
			Vector2B withXPadding = parameters.HasPaddingX & parameters.SolidEdgeX;

			int ySrcOffset = (startingYOffset + spriteSize.Y - 1) * paddedSpriteSize.Width;
			int yDstOffset = (startingYOffset + spriteSize.Y) * paddedSpriteSize.Width;

			int xOffset = withXPadding.X ? -1 : 0;
			int widthAdd = withXPadding.All ? 2 : withXPadding.Any ? 1 : 0;

			for (int x = 0; x < spriteSize.Width + widthAdd; ++x) {
				var src = paddedData.At(ySrcOffset + startingXOffset + xOffset + x);
				src.A = 128;
				paddedData.At(yDstOffset + startingXOffset + xOffset + x) = src;
			}
		}

		padding = new PaddingQuad(
			new Vector2I(
				parameters.HasPaddingX.X ? expectedPadding : 0,
				parameters.HasPaddingX.Y ? expectedPadding : 0
			) * scale,
			new Vector2I(
				parameters.HasPaddingY.X ? expectedPadding : 0,
				parameters.HasPaddingY.Y ? expectedPadding : 0
			) * scale
		);
		paddedSize = paddedSpriteSize;
		return paddedData;
	}

	internal static bool IsBlacklisted(Bounds bounds, XTexture2D reference) {
		var normalizedName = reference.NormalizedName();

		foreach (var blacklistedRef in Config.Resample.Padding.BlackListS) {
			if (!normalizedName.StartsWith(blacklistedRef.Texture)) {
				continue;
			}
			if (blacklistedRef.Bounds.IsEmpty || blacklistedRef.Bounds.Contains(bounds)) {
				return true;
			}
		}

		return false;
	}
}
