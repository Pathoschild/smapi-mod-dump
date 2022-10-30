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
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Passes;

internal static class Padding {
	private static readonly Color16 PadConstant = Color16.Zero;

	[StructLayout(LayoutKind.Auto)]
	private record struct PaddingParameters(
		Vector2I PaddedSize,
		int ExpectedPadding,
		QuadB HasPadding,
		QuadB SolidEdge
	);

	private static PaddingParameters? GetPaddingParameters(
		Vector2I spriteSize,
		uint scale,
		SpriteInfo input,
		bool forcePadding,
		in Analysis.LegacyResults analysis
	) {
		if (!Config.Resample.Padding.Enabled) {
			return null;
		}

		QuadB hasPadding = QuadB.True;
		QuadB solidEdge = (
			new(analysis.Repeat.Horizontal | analysis.Wrapped.X | spriteSize.Width <= 1),
			new(analysis.Repeat.Vertical | analysis.Wrapped.Y | spriteSize.Height <= 1)
		);

		if (!Config.Resample.Padding.PadSolidEdges) {
			hasPadding = solidEdge.Invert;
		}

		if (Config.Resample.Padding.AlwaysList.AnyF(prefix => input.Reference.NormalizedName().StartsWith(prefix))) {
			hasPadding = QuadB.True;
		}

		int minTexels = Config.Resample.Padding.MinimumSizeTexels;

		if (hasPadding.Any && (spriteSize.X <= minTexels && spriteSize.Y <= minTexels)) {
			hasPadding = QuadB.False;
		}
		else if (hasPadding.Any && (Config.Resample.Padding.IgnoreUnknown && !input.Reference.Anonymous())) {
			hasPadding = QuadB.False;
		}

		if (forcePadding) {
			hasPadding = QuadB.True;
		}

		if (hasPadding.None) {
			return null;
		}

		// TODO we only need to pad the edge that has texels. Double padding is wasteful.
		var paddedSpriteSize = spriteSize;

		var expectedPadding = (int)Math.Max(1U, scale / 2);

		int clampDimension = Config.ClampDimension;

		if (hasPadding.Left) {
			if ((paddedSpriteSize.X + expectedPadding) * scale > clampDimension) {
				hasPadding.Left = false;
			}
			else {
				paddedSpriteSize.X += expectedPadding;
			}
		}

		if (hasPadding.Right) {
			if ((paddedSpriteSize.X + expectedPadding) * scale > clampDimension) {
				hasPadding.Right = false;
			}
			else {
				paddedSpriteSize.X += expectedPadding;
			}
		}

		if (hasPadding.Top) {
			if ((paddedSpriteSize.Y + expectedPadding) * scale > clampDimension) {
				hasPadding.Top = false;
			}
			else {
				paddedSpriteSize.Y += expectedPadding;
			}
		}

		if (hasPadding.Bottom) {
			if ((paddedSpriteSize.Y + expectedPadding) * scale > clampDimension) {
				hasPadding.Bottom = false;
			}
			else {
				paddedSpriteSize.Y += expectedPadding;
			}
		}

		if (hasPadding.None) {
			return null;
		}

		return new(
			PaddedSize: paddedSpriteSize,
			ExpectedPadding: expectedPadding,
			HasPadding: hasPadding,
			SolidEdge: solidEdge
		);
	}

	internal static Span<Color16> Apply(
		ReadOnlySpan<Color16> data,
		Vector2I spriteSize,
		uint scale,
		SpriteInfo input,
		bool forcePadding,
		scoped in Analysis.LegacyResults analysis,
		out PaddingQuad padding,
		out Vector2I paddedSize
	) {
		if (GetPaddingParameters(spriteSize, scale, input, forcePadding, analysis) is not {} parameters) {
			padding = PaddingQuad.Zero;
			paddedSize = spriteSize;
			return data.ToSpanUnsafe();
		}

		var paddedSpriteSize = parameters.PaddedSize;
		var expectedPadding = parameters.ExpectedPadding;
		var hasPadding = parameters.HasPadding;

		// The actual padding logic. If we get to this point, we are actually performing padding.

		var paddedData = SpanExt.Make<Color16>(paddedSpriteSize.Area);
		paddedData.Clear();

		{
			int y = 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			int GetCurrentRowOffset() => y * paddedSpriteSize.Width;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void WritePaddingY(Span<Color16> data) {
				for (int i = 0; i < expectedPadding; ++i, ++y) {
					var strideOffset = GetCurrentRowOffset();
					data.Slice(strideOffset, paddedSpriteSize.Width).Fill(PadConstant);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			int WritePaddingX(Span<Color16> data, int xOffset) {
				data.Slice(xOffset, expectedPadding).Fill(PadConstant);
				return expectedPadding;
			}

			if (hasPadding.Top) {
				WritePaddingY(paddedData);
			}

			for (int i = 0; i < spriteSize.Height; ++i, ++y) {
				// Write a padded X line
				var xOffset = GetCurrentRowOffset();

				if (hasPadding.Left) {
					xOffset += WritePaddingX(paddedData, xOffset);
				}
				data.CopyTo(paddedData, i * spriteSize.Width, xOffset, spriteSize.Width);
				xOffset += spriteSize.Width;
				if (hasPadding.Right) {
					xOffset += WritePaddingX(paddedData, xOffset);
				}
			}

			if (hasPadding.Bottom) {
				WritePaddingY(paddedData);
			}
		}

		Vector2I startingOffset = (
			parameters.HasPadding.Left ? expectedPadding : 0,
			parameters.HasPadding.Top ? expectedPadding : 0
		);

		// If we had solid edges that we are padding, copy the color (but not the alpha) over by one.
		if (parameters.HasPadding.Left && parameters.SolidEdge.Left) {
			for (int y = 0; y < spriteSize.Height; ++y) {
				int yOffset = (y + startingOffset.Y) * paddedSpriteSize.Width;
				int offset = yOffset + startingOffset.X;

				paddedData[offset - 1] = paddedData[offset] with { A = 128 };
			}
		}
		if (parameters.HasPadding.Right && parameters.SolidEdge.Right) {
			for (int y = 0; y < spriteSize.Height; ++y) {
				int yOffset = (y + startingOffset.Y) * paddedSpriteSize.Width;

				int xSrcOffset = startingOffset.X + spriteSize.Width - 1;

				int offset = yOffset + xSrcOffset;

				paddedData[offset + 1] = paddedData[offset] with { A = 128 };
			}
		}

		if (parameters.HasPadding.Top && parameters.SolidEdge.Top) {
			Vector2B withXPadding = parameters.HasPadding.Horizontal & parameters.SolidEdge.Horizontal;

			int ySrcOffset = startingOffset.Y * paddedSpriteSize.Width;
			int yDstOffset = (startingOffset.Y - 1) * paddedSpriteSize.Width;

			int xOffset = withXPadding.X ? -1 : 0;
			int widthAdd = withXPadding.All ? 2 : withXPadding.Any ? 1 : 0;

			for (int x = 0; x < spriteSize.Width + widthAdd; ++x) {
				int offset = startingOffset.X + xOffset + x;

				paddedData[yDstOffset + offset] = paddedData[ySrcOffset + offset] with {A = 128};
			}
		}

		if (parameters.HasPadding.Bottom && parameters.SolidEdge.Bottom) {
			Vector2B withXPadding = parameters.HasPadding.Horizontal & parameters.SolidEdge.Horizontal;

			int ySrcOffset = (startingOffset.Y + spriteSize.Y - 1) * paddedSpriteSize.Width;
			int yDstOffset = (startingOffset.Y + spriteSize.Y) * paddedSpriteSize.Width;

			int xOffset = withXPadding.X ? -1 : 0;
			int widthAdd = withXPadding.All ? 2 : withXPadding.Any ? 1 : 0;

			for (int x = 0; x < spriteSize.Width + widthAdd; ++x) {
				int offset = startingOffset.X + xOffset + x;

				paddedData[yDstOffset + offset] = paddedData[ySrcOffset + offset] with { A = 128 }; ;
			}
		}

		padding = new PaddingQuad(
			new Vector2I(
				parameters.HasPadding.Left ? expectedPadding : 0,
				parameters.HasPadding.Right ? expectedPadding : 0
			) * scale,
			new Vector2I(
				parameters.HasPadding.Top ? expectedPadding : 0,
				parameters.HasPadding.Bottom ? expectedPadding : 0
			) * scale
		);
		paddedSize = paddedSpriteSize;
		return paddedData;
	}

	internal static bool IsBlacklisted(Bounds bounds, XTexture2D reference) {
		var normalizedName = reference.NormalizedName();

		foreach (var blacklistedRef in Config.Resample.Padding.BlackListS) {
			if (!blacklistedRef.Pattern.IsMatch(normalizedName)) {
				continue;
			}
			if (blacklistedRef.Bounds.IsEmpty || blacklistedRef.Bounds.Contains(bounds)) {
				return true;
			}
		}

		return false;
	}
}
