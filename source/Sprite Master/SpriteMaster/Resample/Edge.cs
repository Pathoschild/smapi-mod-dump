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

namespace SpriteMaster.Resample;
static class Edge {
	internal readonly ref struct Results {
		internal readonly Vector2B Wrapped;
		internal readonly Vector2B RepeatX;
		internal readonly Vector2B RepeatY;
		internal readonly Vector2B EdgeX;
		internal readonly Vector2B EdgeY;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal Results(
			Vector2B wrapped,
			Vector2B repeatX,
			Vector2B repeatY,
			Vector2B edgeX,
			Vector2B edgeY
		) {
			Wrapped = wrapped;
			RepeatX = repeatX;
			RepeatY = repeatY;
			EdgeX = edgeX;
			EdgeY = edgeY;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe Results AnalyzeLegacy(Texture2D reference, in FixedSpan<int> data, Bounds rawSize, Bounds spriteSize, Vector2B Wrapped) {
		Vector2B boundsInverted = spriteSize.Invert;

		if (spriteSize.Width < 0 || spriteSize.Height < 0) {
			Debug.ErrorLn($"Inverted Sprite Bounds Value leaked to AnalyzeLegacy: {spriteSize}");

			boundsInverted.X = spriteSize.Width < 0;
			boundsInverted.Y = spriteSize.Height < 0;

			spriteSize.Width = Math.Abs(spriteSize.Width);
			spriteSize.Height = Math.Abs(spriteSize.Height);
		}

		if (rawSize.Width < 0 || rawSize.Height < 0) {
			Debug.ErrorLn($"Inverted Raw Bounds Value leaked to AnalyzeLegacy: {rawSize}");

			rawSize.Width = Math.Abs(rawSize.Width);
			rawSize.Height = Math.Abs(rawSize.Height);
		}

		float edgeThreshold = Config.WrapDetection.edgeThreshold;

		if (!reference.Anonymous() && Config.Resample.Padding.StrictList.Contains(reference.SafeName())) {
			var ratio = (float)spriteSize.Extent.MaxOf / (float)spriteSize.Extent.MinOf;
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

		if (Config.WrapDetection.Enabled && Config.Resample.EnableWrappedAddressing) {
			static byte GetAlpha(int sample) {
				return (byte)(((uint)sample >> 24) & 0xFF);
			}

			var rawInputSize = rawSize;
			var spriteInputSize = spriteSize;

			long numSamples = 0;
			double meanAlphaF = 0.0f;
			if (!WrappedXY.All) {
				foreach (int y in 0.RangeTo(spriteInputSize.Height)) {
					int offset = (y + spriteInputSize.Top) * rawInputSize.Width + spriteInputSize.Left;
					foreach (int x in 0.RangeTo(spriteInputSize.Width)) {
						int address = offset + x;
						int sample = data[address];
						meanAlphaF += GetAlpha(sample);
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
				foreach (int y in 0.RangeTo(spriteInputSize.Height)) {
					int offset = (y + spriteInputSize.Top) * rawInputSize.Width + spriteInputSize.Left;
					int sample0 = data[offset];
					int sample1 = data[offset + (spriteInputSize.Width - 1)];

					if (GetAlpha(sample0) >= alphaThreshold) {
						samples[0]++;
					}
					if (GetAlpha(sample1) >= alphaThreshold) {
						samples[1]++;
					}
				}
				int threshold = ((float)spriteInputSize.Height * edgeThreshold).NearestInt();
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
				var offsets = stackalloc int[] { spriteInputSize.Top * rawInputSize.Width, (spriteInputSize.Bottom - 1) * rawInputSize.Width };
				int sampler = 0;
				foreach (int i in 0.RangeTo(2)) {
					var yOffset = offsets[i];
					foreach (int x in 0.RangeTo(spriteInputSize.Width)) {
						int offset = yOffset + x + spriteInputSize.Left;
						int sample = data[offset];
						if (GetAlpha(sample) >= alphaThreshold) {
							samples[sampler]++;
						}
					}
					sampler++;
				}
				int threshold = ((float)spriteInputSize.Width * edgeThreshold).NearestInt();
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

		// TODO : Should we flip these values based upon boundsInverted?
		return new(
			wrapped: WrappedXY,
			repeatX: RepeatX,
			repeatY: RepeatY,
			edgeX: Vector2B.False,
			edgeY: Vector2B.False
		);
	}
}
