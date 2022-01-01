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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample;

// Temporary code lifted from the PPSSPP project, deposterize.h
static class Deposterize {
	private static readonly ColorSpace CurrentColorSpace = ColorSpace.sRGB_Precise;

	[StructLayout(LayoutKind.Explicit, Pack = sizeof(byte), Size = sizeof(uint))]
	unsafe struct ColorElement {
		internal static readonly ColorElement Zero = new(0, 0, 0, 0);

		[FieldOffset(0)]
		internal fixed byte Data[4];

		[FieldOffset(0)]
		internal byte R;
		[FieldOffset(1)]
		internal byte G;
		[FieldOffset(2)]
		internal byte B;
		[FieldOffset(3)]
		internal byte A;

		[FieldOffset(0)]
		internal fixed int AsInt[1];

		[FieldOffset(0)]
		internal fixed uint AsUInt[1];

		internal ColorElement ColorsOnly => new(R, G, B, 0);

		public static implicit operator int(ColorElement element) => element.AsInt[0];
		public static implicit operator uint(ColorElement element) => element.AsUInt[0];

		internal ref byte this[int index] {
			[MethodImpl(Runtime.MethodImpl.Hot)]
			get => ref Data[index];
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal ColorElement(byte r, byte g, byte b, byte a) {
			R = r;
			G = g;
			B = b;
			A = a;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal ColorElement(int color) : this(0, 0, 0, 0) {
			AsInt[0] = color;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal ColorElement(uint color) : this(0, 0, 0, 0) {
			AsUInt[0] = color;
		}
	}

	private class DeposterizeContext<T> where T : unmanaged {
		private readonly FixedSpan<T> Source;
		private readonly Vector2I Size;
		private readonly Vector2B Wrapped;
		private readonly int Passes;
		private readonly int Threshold;
		private readonly int ThresholdSq;
		private readonly int BlockSize;

		internal DeposterizeContext(
			in FixedSpan<T> source,
			in Vector2I size,
			in Vector2B wrapped,
			int passes,
			int threshold,
			int blockSize
		) {
			Source = source;
			Size = size;
			Wrapped = wrapped;
			Passes = passes;
			Threshold = threshold;
			ThresholdSq = threshold * threshold;
			BlockSize = blockSize;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private static double TexelDiff(uint texel1, uint texel2, int shift) {
			texel1 = (texel1 >> shift) & 0xFF;
			texel2 = (texel2 >> shift) & 0xFF;

			return Math.Abs((int)texel1 - (int)texel2);
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private static double Square(double value) => value * value;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private static double DistYCbCrImpl(uint pix1, uint pix2) {
			if ((pix1 & ~ColorConstant.Mask.Alpha) == (pix2 & ~ColorConstant.Mask.Alpha)) {
				return 0.0;
			}

			//http://en.wikipedia.org/wiki/YCbCr#ITU-R_BT.601_conversion
			//YCbCr conversion is a matrix multiplication => take advantage of linearity by subtracting first!
			var rDiff = TexelDiff(pix1, pix2, ColorConstant.Shift.Red); //we may delay division by 255 to after matrix multiplication
			var gDiff = TexelDiff(pix1, pix2, ColorConstant.Shift.Green);
			var bDiff = TexelDiff(pix1, pix2, ColorConstant.Shift.Blue);  //subtraction for int is noticeable faster than for double

			var coefficient = CurrentColorSpace.LumaCoefficient;
			var scale = CurrentColorSpace.LumaScale;

			var y = (coefficient.R * rDiff) + (coefficient.G * gDiff) + (coefficient.B * bDiff); //[!], analog YCbCr!
			var cB = scale.B * (bDiff - y);
			var cR = scale.R * (rDiff - y);

			// Skip division by 255.
			// Also skip square root here by pre-squaring the config option equalColorTolerance.
			return Square(y) + Square(cB) + Square(cR);
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private static double DistYCbCr(uint pix1, uint pix2) {
			if (pix1 == pix2) {
				return 0.0;
			}

			var distance = DistYCbCrImpl(pix1, pix2);

			return distance;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private bool Compare(byte reference, byte lower, byte higher) {
			return
				(lower != higher) &&
				(
					(lower == reference && Math.Abs(higher - reference) <= Threshold) ||
					(higher == reference && Math.Abs(lower - reference) <= Threshold)
				);
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private ColorElement Merge(ColorElement reference, ColorElement lower, ColorElement higher) {
			ColorElement result = reference;

			if (reference.A == lower.A && reference.A == higher.A) {
				bool doMerge = false;
				if (Config.Resample.Deposterization.UseYCbCr && lower != higher && (lower == reference || higher == reference)) {
					doMerge =
						(lower == reference && DistYCbCr(higher, reference) <= ThresholdSq) ||
						(higher == reference && DistYCbCr(lower, reference) <= ThresholdSq);
				}
				else {
					doMerge =
						reference.A == lower.A && reference.A == higher.A &&
						Compare(reference[0], lower[0], higher[0]) &&
						Compare(reference[1], lower[1], higher[1]) &&
						Compare(reference[2], lower[2], higher[2]);
				}

				if (doMerge) {
					result[0] = (byte)((lower[0] + higher[0]) >> 1);
					result[1] = (byte)((lower[1] + higher[1]) >> 1);
					result[2] = (byte)((lower[2] + higher[2]) >> 1);
				}
			}

			return result;
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private int GetX(int value) {
			if (Wrapped.X) {
				var result = value % Size.X;
				if (result < 0) {
					result = Size.X + result;
				}
				return result;
			}
			else {
				return Math.Clamp(value, 0, Size.X);
			}
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private int GetY(int value) {
			if (Wrapped.Y) {
				var result = value % Size.Y;
				if (result < 0) {
					result = Size.Y + result;
				}
				return result;
			}
			else {
				return Math.Clamp(value, 0, Size.Y);
			}
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private unsafe void DeposterizeH(in Span<ColorElement> inData, in Span<ColorElement> outData) {
			int minY = 0;
			int maxY = Size.Height;

			int minX = Wrapped.X ? -1 : 1;
			int maxX = Wrapped.X ? Size.Width + 1 : Math.Max(1, Size.Width - 1);

			foreach (int y in minY.RangeTo(maxY)) {
				int modulusY = GetY(y);

				var yIndex = modulusY * Size.X;

				if (!Wrapped.X) {
					outData[yIndex] = inData[yIndex];
					outData[yIndex + Size.Width - 1] = inData[yIndex + Size.Width - 1];
				}

				foreach (int x in minX.RangeTo(maxX)) {
					int modulusX = GetX(x);

					var index = yIndex + modulusX;

					var center = inData[index];
					var left = inData[yIndex + GetX(x - 1)];
					var right = inData[yIndex + GetX(x + 1)];

					outData[index] = Merge(center, left, right);
				}
			}
		}

		[MethodImpl(Runtime.MethodImpl.Hot)]
		private unsafe void DeposterizeV(in Span<ColorElement> inData, in Span<ColorElement> outData) {
			int minY = Wrapped.Y ? 0 : 1;
			int maxY = Wrapped.Y ? Size.Height : Math.Max(1, Size.Height - 1);

			int minX = Wrapped.X ? -(Size.X % BlockSize) : 0;
			int maxX = Wrapped.X ? Size.Width + -minX : Size.Width;

			int minXBlock = 0;
			int maxXBlock = (maxX / BlockSize) + 1;

			foreach (int xb in minXBlock.RangeTo(maxXBlock)) {
				var min = (xb + minX) * BlockSize;
				var max = Math.Min(maxX, min + BlockSize);

				if (!Wrapped.Y) {
					foreach (int x in min.RangeTo(max)) {
						int xOffset = GetX(x);
						int index0 = xOffset;
						int index1 = ((Size.Height - 1) * Size.X) + xOffset;
						outData[index0] = inData[index0];
						outData[index1] = inData[index1];
					}
				}

				foreach (int y in minY.RangeTo(maxY)) {
					var modulusY = GetY(y);
					var yIndex = modulusY * Size.X;
					var yIndexPrev = GetY(y - 1) * Size.X;
					var yIndexNext = GetY(y + 1) * Size.X;

					foreach (int x in min.RangeTo(max)) {
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

		internal unsafe T[] Execute() {
			var buffer1 = new T[Source.Length];
			var buffer2 = new T[Source.Length];

			var inData = new Span<ColorElement>(Source.TypedPointer, (Source.Length * FixedSpan<T>.TypeSize) / sizeof(ColorElement));
			var buffer1Data = MemoryMarshal.Cast<T, ColorElement>(buffer1.AsSpan());
			var buffer2Data = MemoryMarshal.Cast<T, ColorElement>(buffer2.AsSpan());

			DeposterizeH(inData, buffer1Data);
			DeposterizeV(buffer1Data, buffer2Data);
			for (int pass = 1; pass < Passes; ++pass) {
				DeposterizeH(buffer2Data, buffer1Data);
				DeposterizeV(buffer1Data, buffer2Data);
			}

			return buffer2;
		}
	}

	/*
	[MethodImpl(Runtime.MethodImpl.Optimize)]
	internal static unsafe T[] Enhance<T>(
		T[] data,
		in Vector2I size,
		in Vector2B wrapped) where T : unmanaged {
		return Enhance<T>(data.AsFixedSpan(), size, wrapped);
	}
	*/

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe T[] Enhance<T>(
		FixedSpan<T> data,
		in Vector2I size,
		in Vector2B wrapped,
		int? passes = null,
		int? threshold = null,
		int? blockSize = null
	) where T : unmanaged {
		var context = new DeposterizeContext<T>(
			source: data,
			size: size,
			wrapped: wrapped,
			passes: passes ?? Config.Resample.Deposterization.Passes,
			threshold: threshold ?? Config.Resample.Deposterization.Threshold,
			blockSize: blockSize ?? Config.Resample.Deposterization.BlockSize
		);
		return context.Execute();
	}
}
