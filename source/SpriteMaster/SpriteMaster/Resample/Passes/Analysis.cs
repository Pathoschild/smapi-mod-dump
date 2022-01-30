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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Passes;

static class Analysis {
	internal readonly ref struct LegacyResults {
		internal readonly Vector2B Wrapped;
		internal readonly Vector2B RepeatX;
		internal readonly Vector2B RepeatY;
		internal readonly Vector2B EdgeX;
		internal readonly Vector2B EdgeY;
		internal readonly bool PremultipliedAlpha;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal LegacyResults(
			Vector2B wrapped,
			Vector2B repeatX,
			Vector2B repeatY,
			Vector2B edgeX,
			Vector2B edgeY,
			bool premultipliedAlpha
		) {
			Wrapped = wrapped;
			RepeatX = repeatX;
			RepeatY = repeatY;
			EdgeX = edgeX;
			EdgeY = edgeY;
			PremultipliedAlpha = premultipliedAlpha;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static byte GetAlpha(uint sample) {
		return (byte)(((uint)sample >> 24) & 0xFF);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe LegacyResults AnalyzeLegacy(Texture2D reference, ReadOnlySpan<Color8> data, Bounds bounds, Vector2B Wrapped) {
		Vector2B boundsInverted = bounds.Invert;

		if (bounds.Width < 0 || bounds.Height < 0) {
			Debug.ErrorLn($"Inverted Sprite Bounds Value leaked to AnalyzeLegacy: {bounds}");

			boundsInverted.X = bounds.Width < 0;
			boundsInverted.Y = bounds.Height < 0;

			bounds.Width = Math.Abs(bounds.Width);
			bounds.Height = Math.Abs(bounds.Height);
		}

		float edgeThreshold = Config.WrapDetection.edgeThreshold;

		if (!reference.Anonymous() && Config.Resample.Padding.StrictList.Contains(reference.SafeName())) {
			var ratio = (float)bounds.Extent.MaxOf / (float)bounds.Extent.MinOf;
			if (ratio >= 4.0f) {
				edgeThreshold = 2.0f;
			}
			else {
				edgeThreshold = 0.8f;
			}
		}

		var WrappedXY = Wrapped;
		Vector2B RepeatX = Vector2B.False;
		Vector2B RepeatY = Vector2B.False;

		if (Config.WrapDetection.Enabled) {
			long numSamples = 0;
			double meanAlphaF = 0.0f;
			if (!WrappedXY.All) {
				foreach (int y in 0.RangeTo(bounds.Height)) {
					int offset = (y + bounds.Top) * bounds.Width + bounds.Left;
					foreach (int x in 0.RangeTo(bounds.Width)) {
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
				foreach (int y in 0.RangeTo(bounds.Height)) {
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
				foreach (int i in 0.RangeTo(2)) {
					var yOffset = offsets[i];
					foreach (int x in 0.RangeTo(bounds.Width)) {
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

		bool premultipliedAlpha = true;

		// https://stackoverflow.com/a/9148428
		if (Config.Resample.PremultiplyAlpha && Config.Resample.PremultiplyAlphaAssume) {
			foreach (var element in data) {
				var alpha = element.A;
				var maxColor = MathExt.Max(
					element.R,
					element.G,
					element.B
				);
				int colorDifference = maxColor.Value - alpha.Value;
				if (colorDifference >= Config.Resample.PremultipliedAlphaThreshold) {
					premultipliedAlpha = false;
					break;
				}
			}
		}

		// TODO : Should we flip these values based upon boundsInverted?
		return new(
			wrapped: WrappedXY,
			repeatX: RepeatX,
			repeatY: RepeatY,
			edgeX: Vector2B.False,
			edgeY: Vector2B.False,
			premultipliedAlpha: premultipliedAlpha
		);
	}
}
