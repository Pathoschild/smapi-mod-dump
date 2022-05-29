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
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;
using static SpriteMaster.Colors.ColorHelpers;

namespace SpriteMaster.Resample;

// Temporary code lifted from the PPSSPP project, deposterize.h
internal static class Deposterize {
	private static readonly ColorSpace CurrentColorSpace = ColorSpace.sRGB_Precise;

	private class DeposterizeContext {
		private readonly Vector2I Size;
		private readonly Vector2B Wrapped;
		private readonly int Passes;
		private readonly int Threshold;
		private readonly int BlockSize;
		private readonly bool UseRedmean;
		private readonly YccConfig YccConfiguration;

		internal DeposterizeContext(
			Vector2I size,
			Vector2B wrapped,
			int passes,
			int threshold,
			int blockSize,
			bool useRedmean = false
		) {
			Size = size;
			Wrapped = wrapped;
			Passes = passes;
			Threshold = threshold * 256;
			BlockSize = blockSize;
			UseRedmean = useRedmean;

			YccConfiguration = new() {
				LuminanceWeight = 1.0,
				ChrominanceWeight = 1.0
			};
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private uint ColorDifference(Color16 pix1, Color16 pix2) {
			if (UseRedmean) {
				return pix1.RedmeanDifference(pix2,
					linear: true,
					alpha: true
				);
			}
			else {
				return pix1.YccDifference(pix2,
					config: YccConfiguration,
					linear: true,
					alpha: true
				);
			}
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private bool Compare(Fixed16 reference, Fixed16 lower, Fixed16 higher) {
			return
				(lower != higher) &&
				(
					(lower == reference && Math.Abs(higher.Value - reference.Value) <= Threshold) ||
					(higher == reference && Math.Abs(lower.Value - reference.Value) <= Threshold)
				);
		}

		private Color16 Merge(Color16 reference, Color16 lower, Color16 higher) {
			Color16 result = reference;

			if (reference.A != lower.A || reference.A != higher.A) {
				return result;
			}

			bool doMerge;
			if (Config.Resample.Deposterization.UsePerceptualColor && lower != higher && (lower == reference || higher == reference)) {
				doMerge =
					(lower == reference && ColorDifference(higher, reference) <= Threshold) ||
					(higher == reference && ColorDifference(lower, reference) <= Threshold);
			}
			else {
				doMerge =
					reference.A == lower.A && reference.A == higher.A &&
					Compare(reference.R, lower.R, higher.R) &&
					Compare(reference.G, lower.G, higher.G) &&
					Compare(reference.B, lower.B, higher.B);
			}

			if (doMerge) {
				result.R = (ushort)((lower.R.Value + higher.R.Value) >> 1);
				result.G = (ushort)((lower.G.Value + higher.G.Value) >> 1);
				result.B = (ushort)((lower.B.Value + higher.B.Value) >> 1);
			}

			return result;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private int GetX(int value) {
			if (Wrapped.X) {
				var result = value % Size.X;
				if (result < 0) {
					result += Size.X;
				}
				return result;
			}
			else {
				return Math.Clamp(value, 0, Size.X - 1);
			}
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private int GetY(int value) {
			if (Wrapped.Y) {
				var result = value % Size.Y;
				if (result < 0) {
					result += Size.Y;
				}
				return result;
			}
			else {
				return Math.Clamp(value, 0, Size.Y - 1);
			}
		}

		private void DeposterizeH(ReadOnlySpan<Color16> inData, Span<Color16> outData) {
			int minY = 0;
			int maxY = Size.Height;

			int minX = -1;
			int maxX = Size.Width + 1;

			for (int y = minY; y < maxY; ++y) {
				int modulusY = GetY(y);

				var yIndex = modulusY * Size.X;

				for (int x = minX; x < maxX; ++x) {
					int modulusX = GetX(x);

					var index = yIndex + modulusX;

					var center = inData[index];
					var left = inData[yIndex + GetX(x - 1)];
					var right = inData[yIndex + GetX(x + 1)];

					outData[index] = Merge(center, left, right);
				}
			}
		}

		private void DeposterizeV2(ReadOnlySpan<Color16> inData, Span<Color16> outData) {
			int minY = -1;
			int maxY = Size.Height + 1;

			int minX = -(Size.X % BlockSize);
			int maxX = Size.Width + -minX;

			int minXBlock = 0;
			int maxXBlock = (maxX / BlockSize) + 1;

			for (int xb = minXBlock; xb < maxXBlock; ++xb) {
				var min = (xb + minX) * BlockSize;
				var max = Math.Min(maxX, min + BlockSize);

				for (int y = minY; y < maxY; ++y) {
					var yIndex =			GetY(y)			* Size.X;
					var yIndexPrev =	GetY(y - 1)	* Size.X;
					var yIndexNext =	GetY(y + 1)	* Size.X;

					for (int x = min; x < max; ++x) {
						var modulusX = GetX(x);

						var index = yIndex + modulusX;

						var center = inData[index];
						var upper = inData[yIndexPrev + modulusX];
						var lower = inData[yIndexNext + modulusX];

						outData[index] = Merge(center, upper, lower);
					}
				}
			}
		}

		private void DeposterizeV(ReadOnlySpan<Color16> inData, Span<Color16> outData) {
			//int minY = 1;
			int maxY = Size.Height;

			int minX = -(Size.X % BlockSize);
			int maxX = Size.Width + -minX;

			//int minXBlock = 0;
			int maxXBlock = (maxX / BlockSize) + 1;

			for (int xb = 0; xb < Size.X / BlockSize + 1; ++xb) {
				var min = (xb + minX) * BlockSize;
				var max = Math.Min(maxX, min + BlockSize);

				for (int y = 1; y < Size.Height; ++y) {
					var modulusY = GetY(y);
					var yIndex = modulusY * Size.X;
					var yIndexPrev = GetY(y - 1) * Size.X;
					var yIndexNext = GetY(y + 1) * Size.X;

					for (int x = xb * BlockSize; x < (xb + 1) * BlockSize && x < Size.Width; ++x) {
						var modulusX = GetX(x);

						var index = yIndex + modulusX;

						var center = inData[index];
						var upper = inData[yIndexPrev + modulusX];
						var lower = inData[yIndexNext + modulusX];

						outData[index] = Merge(center, upper, lower);
					}
				}
			}
		}

		internal Span<Color16> Execute(ReadOnlySpan<Color16> data) {
			var buffer1 = SpanExt.Make<Color16>(data.Length);
			var buffer2 = SpanExt.Make<Color16>(data.Length);

			DeposterizeH(data, buffer1);
			DeposterizeV(buffer1, buffer2);
			//buffer1Data.CopyTo(buffer2Data);
			for (int pass = 1; pass < Passes; ++pass) {
				DeposterizeH(buffer2, buffer1);
				DeposterizeV(buffer1, buffer2);
				//buffer1Data.CopyTo(buffer2Data);
			}

			return buffer2;
		}
	}

	/*
	[MethodImpl(Runtime.MethodImpl.Optimize)]
	internal static unsafe T[] Enhance<T>(
		T[] data,
		Vector2I size,
		Vector2B wrapped) where T : unmanaged {
		return Enhance<T>(data.AsFixedSpan(), size, wrapped);
	}
	*/

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<Color16> Enhance(
		ReadOnlySpan<Color16> data,
		Vector2I size,
		Vector2B wrapped,
		int? passes = null,
		int? threshold = null,
		int? blockSize = null
	) {
		var context = new DeposterizeContext(
			size: size,
			wrapped: wrapped,
			passes: passes ?? Config.Resample.Deposterization.Passes,
			threshold: threshold ?? Config.Resample.Deposterization.Threshold,
			blockSize: blockSize ?? Config.Resample.Deposterization.BlockSize,
			useRedmean: Config.Resample.Deposterization.UseRedmean
		);
		return context.Execute(data);
	}
}
