/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Colors;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Passes;

static class Analysis {
	internal readonly struct LegacyResults {
		internal readonly Vector2B Wrapped;
		internal readonly Vector2B RepeatX;
		internal readonly Vector2B RepeatY;
		internal readonly Vector2B EdgeX;
		internal readonly Vector2B EdgeY;
		internal readonly Vector2B GradientAxial;
		internal readonly Vector2B GradientDiagonal;
		internal readonly int MaxChannelShades;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal LegacyResults(
			Vector2B wrapped,
			Vector2B repeatX,
			Vector2B repeatY,
			Vector2B edgeX,
			Vector2B edgeY,
			Vector2B gradientAxial,
			Vector2B gradientDiagonal,
			int maxChannelShades
		) {
			Wrapped = wrapped;
			RepeatX = repeatX;
			RepeatY = repeatY;
			EdgeX = edgeX;
			EdgeY = edgeY;
			GradientAxial = gradientAxial;
			GradientDiagonal = gradientDiagonal;
			MaxChannelShades = maxChannelShades;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte GetAlpha(uint sample) {
		return (byte)(((uint)sample >> 24) & 0xFF);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe LegacyResults AnalyzeLegacy(ReadOnlySpan<Color8> data, Bounds bounds, Vector2B wrapped, bool strict = true) {
		Vector2B boundsInverted = bounds.Invert;

		if (bounds.Width < 0 || bounds.Height < 0) {
			Debug.Error($"Inverted Sprite Bounds Value leaked to AnalyzeLegacy: {bounds}");

			boundsInverted.X = bounds.Width < 0;
			boundsInverted.Y = bounds.Height < 0;

			bounds.Width = Math.Abs(bounds.Width);
			bounds.Height = Math.Abs(bounds.Height);
		}

		float edgeThreshold = Config.WrapDetection.edgeThreshold;

		if (strict) {
			var ratio = (float)bounds.Extent.MaxOf / (float)bounds.Extent.MinOf;
			if (ratio >= 4.0f) {
				edgeThreshold = 2.0f;
			}
			else {
				edgeThreshold = 0.8f;
			}
		}

		var WrappedXY = wrapped;
		Vector2B RepeatX = Vector2B.False;
		Vector2B RepeatY = Vector2B.False;

		if (Config.WrapDetection.Enabled) {
			long numSamples = 0;
			double meanAlphaF = 0.0f;
			if (!WrappedXY.All) {
				for (int y = 0; y < bounds.Height; ++y) {
					int offset = (y + bounds.Top) * bounds.Width + bounds.Left;
					for (int x = 0; x < bounds.Width; ++x) {
						int address = offset + x;
						var sample = data[address];
						meanAlphaF += sample.A.Value;
						++numSamples;
					}
				}
			}
			//meanAlphaF /= numSamples;
			//meanAlphaF *= (double)Config.WrapDetection.alphaThreshold / ColorConstant.ScalarFactor;
			byte alphaThreshold = Config.WrapDetection.alphaThreshold; //(byte)Math.Min(meanAlphaF.RoundToInt(), byte.MaxValue);

			// Count the fragments that are not alphad out completely on the edges.
			// Both edges must meet the threshold.
			if (!WrappedXY.X) {
				var samples = stackalloc int[] { 0, 0 };
				for (int y = 0; y < bounds.Height; ++y) {
					int offset = (y + bounds.Top) * bounds.Width + bounds.Left;
					var sample0 = data[offset];
					var sample1 = data[offset + (bounds.Width - 1)];

					if (sample0.A.Value >= alphaThreshold) {
						samples[0]++;
					}
					if (sample1.A.Value >= alphaThreshold) {
						samples[1]++;
					}
				}
				int threshold = ((float)bounds.Height * edgeThreshold).NearestInt();
				var aboveThreshold = Vector2B.From(samples[0] >= threshold, samples[1] >= threshold);
				if (aboveThreshold.All) {
					WrappedXY.X = true;
				}
				else {
					RepeatX = aboveThreshold;
				}
			}
			if (!WrappedXY.Y) {
				var samples = stackalloc int[] { 0, 0 };
				var offsets = stackalloc int[] { bounds.Top * bounds.Width, (bounds.Bottom - 1) * bounds.Width };
				int sampler = 0;
				for (int i = 0; i < 2; ++i) {
					var yOffset = offsets[i];
					for (int x = 0; x < bounds.Width; ++x) {
						int offset = yOffset + x + bounds.Left;
						var sample = data[offset];
						if (sample.A.Value >= alphaThreshold) {
							samples[sampler]++;
						}
					}
					sampler++;
				}
				int threshold = ((float)bounds.Width * edgeThreshold).NearestInt();
				var aboveThreshold = Vector2B.From(samples[0] >= threshold, samples[1] >= threshold);
				if (aboveThreshold.All) {
					WrappedXY.Y = true;
				}
				else {
					RepeatY = aboveThreshold;
				}
			}
		}

		if (WrappedXY.Any) {
			// Perform tests against both sides of an edge to see if they match up. If they do not, convert
			// a wrapped edge into a repeat edge
			if (WrappedXY.X) {

			}
			if (WrappedXY.Y) {

			}
		}

		// Gradient analysis
		Vector2B gradientAxial = Vector2B.True;
		Vector2B gradientDiagonal = Vector2B.False;
		// Horizontal
		{
			for (int y = bounds.Top; gradientAxial.X && y < bounds.Bottom; ++y) {
				int offset = (y * bounds.Width) + bounds.Left;
				var prevColor = data[offset];
				for (int x = 1; x < bounds.Width; ++x) {
					var currColor = data[offset + x];

					uint difference;
					//if (Config.Resample.Analysis.UseRedmean)
					difference = ColorHelpers.RedmeanDifference(prevColor, currColor, false, true);

					if (difference >= Config.Resample.Analysis.MaxGradientColorDifference) {
						gradientAxial.X = false;
						break;
					}

					prevColor = currColor;
				}
			}
		}
		// Vertical
		{
			int offset = (bounds.Top * bounds.Width) + bounds.Left;
			var prevColor = data[offset];
			for (int y = 1; gradientAxial.Y && y < bounds.Height; ++y) {
				for (int x = 0; x < bounds.Width; ++x) {
					var currColor = data[offset + (y * bounds.Width) + x];
					uint difference;
					//if (Config.Resample.Analysis.UseRedmean)
					difference = ColorHelpers.RedmeanDifference(prevColor, currColor, false, true);

					if (difference >= Config.Resample.Analysis.MaxGradientColorDifference) {
						gradientAxial.Y = false;
						break;
					}

					prevColor = currColor;
				}
			}
		}
		// Diagonal
		// TODO : use Bresenham's Line Algorithm (https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm)

		Span<int> shadesR = stackalloc int[byte.MaxValue + 1];
		shadesR.Fill(0);
		Span<int> shadesG = stackalloc int[byte.MaxValue + 1];
		shadesG.Fill(0);
		Span<int> shadesB = stackalloc int[byte.MaxValue + 1];
		shadesB.Fill(0);
		Span<int> shadesA = stackalloc int[byte.MaxValue + 1];
		shadesA.Fill(0);
		foreach (var color in data) {
			shadesR[color.R.Value]++;
			shadesG[color.G.Value]++;
			shadesB[color.B.Value]++;
			shadesA[color.A.Value]++;
		}

		static int NumShades(Span<int> shades) {
			int total = 0;
			foreach (var count in shades) {
				if (count > 0) {
					total++;
				}
			}
			return total;
		}

		int maxNumShades = Math.Max(
			Math.Max(
				NumShades(shadesR),
				NumShades(shadesG)
			),
			Math.Max(
				NumShades(shadesB),
				NumShades(shadesA)
			)
		);

		// MinimumGradientShades

		// TODO : Should we flip these values based upon boundsInverted?
		return new(
			wrapped: WrappedXY,
			repeatX: RepeatX,
			repeatY: RepeatY,
			edgeX: Vector2B.False,
			edgeY: Vector2B.False,
			gradientAxial: gradientAxial,
			gradientDiagonal: gradientDiagonal,
			maxChannelShades: maxNumShades
		);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe LegacyResults AnalyzeLegacy(Texture2D? reference, ReadOnlySpan<Color8> data, in Bounds bounds, Vector2B wrapped) {
		return AnalyzeLegacy(
			data: data,
			bounds: bounds,
			wrapped: wrapped,
			strict: (reference is not null && (!reference.Anonymous() && Config.Resample.Padding.StrictList.Contains(reference.NormalizedName())))
		);
	}
}
