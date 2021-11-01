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

namespace SpriteMaster.Resample {
	internal static class Edge {
		internal ref struct Results {
			public readonly Vector2B Wrapped;
			public readonly Vector2B WrappedX;
			public readonly Vector2B WrappedY;
			public readonly Vector2B EdgeX;
			public readonly Vector2B EdgeY;

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			internal Results(
				Vector2B wrapped,
				Vector2B wrappedX,
				Vector2B wrappedY,
				Vector2B edgeX,
				Vector2B edgeY
			) {
				Wrapped = wrapped;
				WrappedX = wrappedX;
				WrappedY = wrappedY;
				EdgeX = edgeX;
				EdgeY = edgeY;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static unsafe Results AnalyzeLegacy (Texture2D reference, in FixedSpan<int> data, Bounds rawSize, Bounds spriteSize, Vector2B Wrapped) {
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

			var WrappedX = new Vector2B(Wrapped.X);
			var WrappedY = new Vector2B(Wrapped.Y);

			if (Config.WrapDetection.Enabled && Config.Resample.EnableWrappedAddressing) {
				static byte GetAlpha (in int sample) {
					return unchecked((byte)(((uint)sample >> 24) & 0xFF));
				}

				var rawInputSize = rawSize;
				var spriteInputSize = spriteSize;

				long numSamples = 0;
				double meanAlphaF = 0.0f;
				if (!Wrapped.X || !Wrapped.Y) {
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
				//meanAlphaF *= (double)Config.WrapDetection.alphaThreshold / 255.0;
				byte alphaThreshold = Config.WrapDetection.alphaThreshold; //(byte)Math.Min(meanAlphaF.RoundToInt(), byte.MaxValue);

				// Count the fragments that are not alphad out completely on the edges.
				// Both edges must meet the threshold.
				if (!Wrapped.X) {
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
					WrappedX.Negative = samples[0] >= threshold;
					WrappedX.Positive = samples[1] >= threshold;
					Wrapped.X = WrappedX[0] && WrappedX[1];
				}
				if (!Wrapped.Y) {
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
					WrappedY.Negative = samples[0] >= threshold;
					WrappedY.Positive = samples[0] >= threshold;
					Wrapped.Y = WrappedY[0] && WrappedY[1];
				}
			}

			// TODO : Should we flip these values based upon boundsInverted?
			return new Results(
				wrapped: Wrapped,
				wrappedX: WrappedX,
				wrappedY: WrappedY,
				edgeX: Vector2B.False,
				edgeY: Vector2B.False
			);
		}
	}
}
