/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using SpriteMaster.Types.Spans;
using SpriteMaster.Types;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Resample.Encoder;

internal static class StbBlockEncoder {
	internal static bool Encode(
		ReadOnlySpan<Color8> data,
		ref TextureFormat format,
		Vector2I dimensions,
		bool hasAlpha,
		bool isPunchthroughAlpha,
		bool isMasky,
		bool hasR,
		bool hasG,
		bool hasB,
		out PinnedSpan<byte> result
	) {
		TextureFormat resultFormat = hasAlpha ? TextureFormat.BC3 : TextureFormat.BC1;
		var resultBytes = SpanExt.MakePinned<byte>(RequiredSize(dimensions, hasAlpha));

		var compressionMode = Configuration.Config.Resample.BlockCompression.Quality switch {
			CompressionQuality.Low => CompressionMode.Normal,
			CompressionQuality.Medium => CompressionMode.Dither,
			CompressionQuality.High => CompressionMode.HighQuality,
			_ => ThrowHelper.ThrowInvalidOperationException<CompressionMode>($"Unknown Quality: '{Configuration.Config.Resample.BlockCompression.Quality}'")
		};

		CompressDxt(resultBytes, data.AsBytes(), dimensions, hasAlpha, compressionMode);

		result = resultBytes;
		format = resultFormat;
		return true;
	}

	[Flags]
	internal enum CompressionMode : uint {
		Normal = 0U,
		Dither = 1U << 0,     // use dithering. was always dubious, now deprecated. does nothing!
		HighQuality = 1U << 1 // high quality mode, does two refinement steps instead of 1. ~30-40% slower.
	}

	private const bool UseRoundingBias = false;

	private static class LumaCoefficients {
		internal const int Red = 299; // JPEG YCbCr luma coefs, scaled by 1000.
		internal const int Green = 587;
		internal const int Blue = 114;
	}

	private static ReadOnlySpan<byte> Match5Span => new byte[512] {
			0,  0 ,   0,  0 ,   0,  1 ,   0,  1 ,   1,  0 ,   1,  0 ,   1,  0 ,   1,  1 ,
			1,  1 ,   1,  1 ,   1,  2 ,   0,  4 ,   2,  1 ,   2,  1 ,   2,  1 ,   2,  2 ,
			2,  2 ,   2,  2 ,   2,  3 ,   1,  5 ,   3,  2 ,   3,  2 ,   4,  0 ,   3,  3 ,
			3,  3 ,   3,  3 ,   3,  4 ,   3,  4 ,   3,  4 ,   3,  5 ,   4,  3 ,   4,  3 ,
			5,  2 ,   4,  4 ,   4,  4 ,   4,  5 ,   4,  5 ,   5,  4 ,   5,  4 ,   5,  4 ,
			6,  3 ,   5,  5 ,   5,  5 ,   5,  6 ,   4,  8 ,   6,  5 ,   6,  5 ,   6,  5 ,
			6,  6 ,   6,  6 ,   6,  6 ,   6,  7 ,   5,  9 ,   7,  6 ,   7,  6 ,   8,  4 ,
			7,  7 ,   7,  7 ,   7,  7 ,   7,  8 ,   7,  8 ,   7,  8 ,   7,  9 ,   8,  7 ,
			8,  7 ,   9,  6 ,   8,  8 ,   8,  8 ,   8,  9 ,   8,  9 ,   9,  8 ,   9,  8 ,
			9,  8 ,  10,  7 ,   9,  9 ,   9,  9 ,   9, 10 ,   8, 12 ,  10,  9 ,  10,  9 ,
		 10,  9 ,  10, 10 ,  10, 10 ,  10, 10 ,  10, 11 ,   9, 13 ,  11, 10 ,  11, 10 ,
		 12,  8 ,  11, 11 ,  11, 11 ,  11, 11 ,  11, 12 ,  11, 12 ,  11, 12 ,  11, 13 ,
		 12, 11 ,  12, 11 ,  13, 10 ,  12, 12 ,  12, 12 ,  12, 13 ,  12, 13 ,  13, 12 ,
		 13, 12 ,  13, 12 ,  14, 11 ,  13, 13 ,  13, 13 ,  13, 14 ,  12, 16 ,  14, 13 ,
		 14, 13 ,  14, 13 ,  14, 14 ,  14, 14 ,  14, 14 ,  14, 15 ,  13, 17 ,  15, 14 ,
		 15, 14 ,  16, 12 ,  15, 15 ,  15, 15 ,  15, 15 ,  15, 16 ,  15, 16 ,  15, 16 ,
		 15, 17 ,  16, 15 ,  16, 15 ,  17, 14 ,  16, 16 ,  16, 16 ,  16, 17 ,  16, 17 ,
		 17, 16 ,  17, 16 ,  17, 16 ,  18, 15 ,  17, 17 ,  17, 17 ,  17, 18 ,  16, 20 ,
		 18, 17 ,  18, 17 ,  18, 17 ,  18, 18 ,  18, 18 ,  18, 18 ,  18, 19 ,  17, 21 ,
		 19, 18 ,  19, 18 ,  20, 16 ,  19, 19 ,  19, 19 ,  19, 19 ,  19, 20 ,  19, 20 ,
		 19, 20 ,  19, 21 ,  20, 19 ,  20, 19 ,  21, 18 ,  20, 20 ,  20, 20 ,  20, 21 ,
		 20, 21 ,  21, 20 ,  21, 20 ,  21, 20 ,  22, 19 ,  21, 21 ,  21, 21 ,  21, 22 ,
		 20, 24 ,  22, 21 ,  22, 21 ,  22, 21 ,  22, 22 ,  22, 22 ,  22, 22 ,  22, 23 ,
		 21, 25 ,  23, 22 ,  23, 22 ,  24, 20 ,  23, 23 ,  23, 23 ,  23, 23 ,  23, 24 ,
		 23, 24 ,  23, 24 ,  23, 25 ,  24, 23 ,  24, 23 ,  25, 22 ,  24, 24 ,  24, 24 ,
		 24, 25 ,  24, 25 ,  25, 24 ,  25, 24 ,  25, 24 ,  26, 23 ,  25, 25 ,  25, 25 ,
		 25, 26 ,  24, 28 ,  26, 25 ,  26, 25 ,  26, 25 ,  26, 26 ,  26, 26 ,  26, 26 ,
		 26, 27 ,  25, 29 ,  27, 26 ,  27, 26 ,  28, 24 ,  27, 27 ,  27, 27 ,  27, 27 ,
		 27, 28 ,  27, 28 ,  27, 28 ,  27, 29 ,  28, 27 ,  28, 27 ,  29, 26 ,  28, 28 ,
		 28, 28 ,  28, 29 ,  28, 29 ,  29, 28 ,  29, 28 ,  29, 28 ,  30, 27 ,  29, 29 ,
		 29, 29 ,  29, 30 ,  29, 30 ,  30, 29 ,  30, 29 ,  30, 29 ,  30, 30 ,  30, 30 ,
		 30, 30 ,  30, 31 ,  30, 31 ,  31, 30 ,  31, 30 ,  31, 30 ,  31, 31 ,  31, 31 ,
	};
	private static readonly unsafe byte* Match5 = (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(Match5Span.GetPinnableReference()));

	private static ReadOnlySpan<byte> Match6Span => new byte[512] {
			0,  0 ,   0,  1 ,   1,  0 ,   1,  1 ,   1,  1 ,   1,  2 ,   2,  1 ,   2,  2 ,
			2,  2 ,   2,  3 ,   3,  2 ,   3,  3 ,   3,  3 ,   3,  4 ,   4,  3 ,   4,  4 ,
			4,  4 ,   4,  5 ,   5,  4 ,   5,  5 ,   5,  5 ,   5,  6 ,   6,  5 ,   6,  6 ,
			6,  6 ,   6,  7 ,   7,  6 ,   7,  7 ,   7,  7 ,   7,  8 ,   8,  7 ,   8,  8 ,
			8,  8 ,   8,  9 ,   9,  8 ,   9,  9 ,   9,  9 ,   9, 10 ,  10,  9 ,  10, 10 ,
		 10, 10 ,  10, 11 ,  11, 10 ,   8, 16 ,  11, 11 ,  11, 12 ,  12, 11 ,   9, 17 ,
		 12, 12 ,  12, 13 ,  13, 12 ,  11, 16 ,  13, 13 ,  13, 14 ,  14, 13 ,  12, 17 ,
		 14, 14 ,  14, 15 ,  15, 14 ,  14, 16 ,  15, 15 ,  15, 16 ,  16, 14 ,  16, 15 ,
		 17, 14 ,  16, 16 ,  16, 17 ,  17, 16 ,  18, 15 ,  17, 17 ,  17, 18 ,  18, 17 ,
		 20, 14 ,  18, 18 ,  18, 19 ,  19, 18 ,  21, 15 ,  19, 19 ,  19, 20 ,  20, 19 ,
		 20, 20 ,  20, 20 ,  20, 21 ,  21, 20 ,  21, 21 ,  21, 21 ,  21, 22 ,  22, 21 ,
		 22, 22 ,  22, 22 ,  22, 23 ,  23, 22 ,  23, 23 ,  23, 23 ,  23, 24 ,  24, 23 ,
		 24, 24 ,  24, 24 ,  24, 25 ,  25, 24 ,  25, 25 ,  25, 25 ,  25, 26 ,  26, 25 ,
		 26, 26 ,  26, 26 ,  26, 27 ,  27, 26 ,  24, 32 ,  27, 27 ,  27, 28 ,  28, 27 ,
		 25, 33 ,  28, 28 ,  28, 29 ,  29, 28 ,  27, 32 ,  29, 29 ,  29, 30 ,  30, 29 ,
		 28, 33 ,  30, 30 ,  30, 31 ,  31, 30 ,  30, 32 ,  31, 31 ,  31, 32 ,  32, 30 ,
		 32, 31 ,  33, 30 ,  32, 32 ,  32, 33 ,  33, 32 ,  34, 31 ,  33, 33 ,  33, 34 ,
		 34, 33 ,  36, 30 ,  34, 34 ,  34, 35 ,  35, 34 ,  37, 31 ,  35, 35 ,  35, 36 ,
		 36, 35 ,  36, 36 ,  36, 36 ,  36, 37 ,  37, 36 ,  37, 37 ,  37, 37 ,  37, 38 ,
		 38, 37 ,  38, 38 ,  38, 38 ,  38, 39 ,  39, 38 ,  39, 39 ,  39, 39 ,  39, 40 ,
		 40, 39 ,  40, 40 ,  40, 40 ,  40, 41 ,  41, 40 ,  41, 41 ,  41, 41 ,  41, 42 ,
		 42, 41 ,  42, 42 ,  42, 42 ,  42, 43 ,  43, 42 ,  40, 48 ,  43, 43 ,  43, 44 ,
		 44, 43 ,  41, 49 ,  44, 44 ,  44, 45 ,  45, 44 ,  43, 48 ,  45, 45 ,  45, 46 ,
		 46, 45 ,  44, 49 ,  46, 46 ,  46, 47 ,  47, 46 ,  46, 48 ,  47, 47 ,  47, 48 ,
		 48, 46 ,  48, 47 ,  49, 46 ,  48, 48 ,  48, 49 ,  49, 48 ,  50, 47 ,  49, 49 ,
		 49, 50 ,  50, 49 ,  52, 46 ,  50, 50 ,  50, 51 ,  51, 50 ,  53, 47 ,  51, 51 ,
		 51, 52 ,  52, 51 ,  52, 52 ,  52, 52 ,  52, 53 ,  53, 52 ,  53, 53 ,  53, 53 ,
		 53, 54 ,  54, 53 ,  54, 54 ,  54, 54 ,  54, 55 ,  55, 54 ,  55, 55 ,  55, 55 ,
		 55, 56 ,  56, 55 ,  56, 56 ,  56, 56 ,  56, 57 ,  57, 56 ,  57, 57 ,  57, 57 ,
		 57, 58 ,  58, 57 ,  58, 58 ,  58, 58 ,  58, 59 ,  59, 58 ,  59, 59 ,  59, 59 ,
		 59, 60 ,  60, 59 ,  60, 60 ,  60, 60 ,  60, 61 ,  61, 60 ,  61, 61 ,  61, 61 ,
		 61, 62 ,  62, 61 ,  62, 62 ,  62, 62 ,  62, 63 ,  63, 62 ,  63, 63 ,  63, 63 ,
	};
	private static readonly unsafe byte* Match6 = (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(Match6Span.GetPinnableReference()));

	private static readonly float[] MidPoints5 = new float[32] {
		0.01568627543747425079345703125f, 0.0470588244497776031494140625f, 0.078431375324726104736328125f, 0.111764706671237945556640625f, 0.14509804546833038330078125f, 0.17647059261798858642578125f, 0.20784313976764678955078125f, 0.24117647111415863037109375f,
		0.2745098173618316650390625f, 0.3058823645114898681640625f, 0.3372549116611480712890625f, 0.370588243007659912109375f, 0.4039215743541717529296875f, 0.4352941215038299560546875f, 0.4666666686534881591796875f, 0.5f         ,
		0.533333361148834228515625f, 0.564705908298492431640625f, 0.596078455448150634765625f, 0.629411756992340087890625f, 0.66274511814117431640625f, 0.69411766529083251953125f, 0.72549021244049072265625f, 0.75882351398468017578125f,
		0.792156875133514404296875f, 0.823529422283172607421875f, 0.854901969432830810546875f, 0.888235270977020263671875f, 0.9215686321258544921875f, 0.9529411792755126953125f, 0.9843137264251708984375f, 1.0f         ,
	};
	//private static readonly unsafe float* MidPoints5 = (float*)Unsafe.AsPointer(ref Unsafe.AsRef(MidPoints5Span.GetPinnableReference()));

	private static readonly float[] MidPoints6 = new float[64] {
		0.007843137718737125396728515625f, 0.02352941222488880157470703125f, 0.0392156876623630523681640625f, 0.0549019612371921539306640625f, 0.070588238537311553955078125f, 0.086274512112140655517578125f, 0.101960785686969757080078125f, 0.117647059261798858642578125f,
		0.13333334028720855712890625f, 0.14901961386203765869140625f, 0.16470588743686676025390625f, 0.18039216101169586181640625f, 0.19607843458652496337890625f, 0.21176470816135406494140625f, 0.22745098173618316650390625f, 0.24509803950786590576171875f,
		0.2627451121807098388671875f, 0.2784313857555389404296875f, 0.2941176593303680419921875f, 0.3098039329051971435546875f, 0.3254902064800262451171875f, 0.3411764800548553466796875f, 0.3568627536296844482421875f, 0.3725490272045135498046875f,
		0.3882353007793426513671875f, 0.4039215743541717529296875f, 0.4196078479290008544921875f, 0.4352941215038299560546875f, 0.4509803950786590576171875f, 0.4666666686534881591796875f, 0.4823529422283172607421875f, 0.5f         ,
		0.517647087574005126953125f, 0.533333361148834228515625f, 0.549019634723663330078125f, 0.564705908298492431640625f, 0.580392181873321533203125f, 0.596078455448150634765625f, 0.611764729022979736328125f, 0.627451002597808837890625f,
		0.643137276172637939453125f, 0.658823549747467041015625f, 0.674509823322296142578125f, 0.690196096897125244140625f, 0.705882370471954345703125f, 0.721568644046783447265625f, 0.737254917621612548828125f, 0.754901945590972900390625f,
		0.77254903316497802734375f, 0.78823530673980712890625f, 0.80392158031463623046875f, 0.81960785388946533203125f, 0.83529412746429443359375f, 0.85098040103912353515625f, 0.86666667461395263671875f, 0.88235294818878173828125f,
		0.89803922176361083984375f, 0.91372549533843994140625f, 0.92941176891326904296875f, 0.94509804248809814453125f, 0.96078431606292724609375f, 0.97647058963775634765625f, 0.99215686321258544921875f, 1.0f         ,
	};
	//private static readonly unsafe float* MidPoints6 = (float*)Unsafe.AsPointer(ref Unsafe.AsRef(MidPoints6Span.GetPinnableReference()));

	// some magic to save a lot of multiplies in the accumulating loop...
	// (precomputed products of weights for least squares system, accumulated inside one 32-bit register)
	private static ReadOnlySpan<byte> W1TabSpan => new byte[4] { 3, 0, 2, 1 };
	private static readonly unsafe byte* W1Tab = (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(W1TabSpan.GetPinnableReference()));
	private static ReadOnlySpan<byte> ProductsSpan => new byte[] { 0x00, 0x00, 0x09, 0x00, 0x00, 0x09, 0x00, 0x00, 0x02, 0x01, 0x04, 0x00, 0x02, 0x04, 0x01, 0x00 };
	private static readonly unsafe int* Products = (int*)Unsafe.AsPointer(ref Unsafe.AsRef(ProductsSpan.GetPinnableReference()));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static T ReadArray<T>(T[] array, int index) {
		ref T local = ref MemoryMarshal.GetArrayDataReference(array);
		return Unsafe.Add(ref local, index);
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Mul8Bit(int a, int b) {
		int t = a * b + 128;
		return (t + (t >> 8)) >> 8;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void From16Bit(byte* outPtr, ushort v) {
		int rv = (v & 0xf800) >> 11;
		int gv = (v & 0x07e0) >> 5;
		int bv = (v & 0x001f) >> 0;

		// expand to 8 bits via bit replication
		outPtr[0] = (byte)((rv * 33) >> 2);
		outPtr[1] = (byte)((gv * 65) >> 4);
		outPtr[2] = (byte)((bv * 33) >> 2);
		outPtr[3] = (byte)0;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort As16Bit(int r, int g, int b) {
		return (ushort)((Mul8Bit(r, 31) << 11) | (Mul8Bit(g, 63) << 5) | Mul8Bit(b, 31));
	}

	// linear interpolation at 1/3 point between a and b, using desired rounding type
	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Lerp13(int a, int b) {
		if (UseRoundingBias) {
			// with rounding bias
			return a + Mul8Bit(b - a, 0x55);
		}
		else {
			// without rounding bias
			// replace "/ 3" by "* 0xaaab) >> 17" if your compiler sucks or you really need every ounce of speed.
			return ((2 * a + b) * 0xAAAB) >> 17;
		}
	}

	// lerp RGB color
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void Lerp13RGB(byte* outPtr, byte* p1, byte* p2) {
		outPtr[0] = (byte)Lerp13(p1[0], p2[0]);
		outPtr[1] = (byte)Lerp13(p1[1], p2[1]);
		outPtr[2] = (byte)Lerp13(p1[2], p2[2]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void EvalColors(byte* color, ushort c0, ushort c1) {
		From16Bit(color + 0, c0);
		From16Bit(color + 4, c1);
		Lerp13RGB(color + 8, color + 0, color + 4);
		Lerp13RGB(color + 12, color + 4, color + 0);
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe uint MatchColorsBlock(byte* block, byte* color) {
		int dirR = color[0 * 4 + 0] - color[1 * 4 + 0];
		int dirG = color[0 * 4 + 1] - color[1 * 4 + 1];
		int dirB = color[0 * 4 + 2] - color[1 * 4 + 2];

		int* dots = stackalloc int[16] {
			block[0x0 * 4 + 0] * dirR + block[0x0 * 4 + 1] * dirG + block[0x0 * 4 + 2] * dirB,
			block[0x1 * 4 + 0] * dirR + block[0x1 * 4 + 1] * dirG + block[0x1 * 4 + 2] * dirB,
			block[0x2 * 4 + 0] * dirR + block[0x2 * 4 + 1] * dirG + block[0x2 * 4 + 2] * dirB,
			block[0x3 * 4 + 0] * dirR + block[0x3 * 4 + 1] * dirG + block[0x3 * 4 + 2] * dirB,
			block[0x4 * 4 + 0] * dirR + block[0x4 * 4 + 1] * dirG + block[0x4 * 4 + 2] * dirB,
			block[0x5 * 4 + 0] * dirR + block[0x5 * 4 + 1] * dirG + block[0x5 * 4 + 2] * dirB,
			block[0x6 * 4 + 0] * dirR + block[0x6 * 4 + 1] * dirG + block[0x6 * 4 + 2] * dirB,
			block[0x7 * 4 + 0] * dirR + block[0x7 * 4 + 1] * dirG + block[0x7 * 4 + 2] * dirB,
			block[0x8 * 4 + 0] * dirR + block[0x8 * 4 + 1] * dirG + block[0x8 * 4 + 2] * dirB,
			block[0x9 * 4 + 0] * dirR + block[0x9 * 4 + 1] * dirG + block[0x9 * 4 + 2] * dirB,
			block[0xA * 4 + 0] * dirR + block[0xA * 4 + 1] * dirG + block[0xA * 4 + 2] * dirB,
			block[0xB * 4 + 0] * dirR + block[0xB * 4 + 1] * dirG + block[0xB * 4 + 2] * dirB,
			block[0xC * 4 + 0] * dirR + block[0xC * 4 + 1] * dirG + block[0xC * 4 + 2] * dirB,
			block[0xD * 4 + 0] * dirR + block[0xD * 4 + 1] * dirG + block[0xD * 4 + 2] * dirB,
			block[0xE * 4 + 0] * dirR + block[0xE * 4 + 1] * dirG + block[0xE * 4 + 2] * dirB,
			block[0xF * 4 + 0] * dirR + block[0xF * 4 + 1] * dirG + block[0xF * 4 + 2] * dirB
		};

		int* stops = stackalloc int[4] {
			color[0x0 * 4 + 0] * dirR + color[0x0 * 4 + 1] * dirG + color[0x0 * 4 + 2] * dirB,
			color[0x1 * 4 + 0] * dirR + color[0x1 * 4 + 1] * dirG + color[0x1 * 4 + 2] * dirB,
			color[0x2 * 4 + 0] * dirR + color[0x2 * 4 + 1] * dirG + color[0x2 * 4 + 2] * dirB,
			color[0x3 * 4 + 0] * dirR + color[0x3 * 4 + 1] * dirG + color[0x3 * 4 + 2] * dirB
		};

		// think of the colors as arranged on a line; project point onto that line, then choose
		// next color out of available ones. we compute the crossover points for "best color in top
		// half"/"best in bottom half" and then the same inside that subinterval.
		//
		// relying on this 1d approximation isn't always optimal in terms of euclidean distance,
		// but it's very close and a lot faster.
		// http://cbloomrants.blogspot.com/2008/12/12-08-08-dxtc-summary.html

		int c0Point = (stops[1] + stops[3]);
		int halfPoint = (stops[3] + stops[2]);
		int c3Point = (stops[2] + stops[0]);

		uint mask = 0;
		for (int i = 15; i >= 0; i--) {
			int dot = dots[i] * 2;
			mask <<= 2;

			if (dot < halfPoint)
				mask |= ((dot >= c0Point).As<uint>() * 2U) + 1U;
			else
				mask |= (dot < c3Point).As<uint>() * 2U;
		}

		return mask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void OptimizeColorsBlock(byte* block, out ushort pMax16, out ushort pMin16) {
		const int IterPower = 4;

		// determine color distribution
		uint* mu = stackalloc uint[3];
		uint* min = stackalloc uint[3];
		uint* max = stackalloc uint[3];

		for (int ch = 0; ch < 3; ++ch) {
			byte* bp = block + ch;

			uint muV = bp[0];
			uint minV = muV;
			uint maxV = muV;
			for (int i = 4; i < 64; i += 4) {
				uint value = bp[i];
				muV += value;
				if (value < minV)
					minV = value;
				else if (value > maxV)
					maxV = value;
			}

			mu[ch] = (muV + 8) >> 4;
			min[ch] = minV;
			max[ch] = maxV;
		}

		// determine covariance matrix
		int* cov = stackalloc int[6] { 0, 0, 0, 0, 0, 0 };

		for (int i = 0; i < 16; ++i) {
			int r = (int)(block[i * 4 + 0] - mu[0]);
			int g = (int)(block[i * 4 + 1] - mu[1]);
			int b = (int)(block[i * 4 + 2] - mu[2]);

			cov[0] += r * r;
			cov[1] += r * g;
			cov[2] += r * b;
			cov[3] += g * g;
			cov[4] += g * b;
			cov[5] += b * b;
		}

		// convert covariance matrix to float, find principal axis via power iter
		float* covF = stackalloc float[6] {
			cov[0] / 255.0f,
			cov[1] / 255.0f,
			cov[2] / 255.0f,
			cov[3] / 255.0f,
			cov[4] / 255.0f,
			cov[5] / 255.0f,
		};

		var vfR = (float)(max[0] - min[0]);
		var vfG = (float)(max[1] - min[1]);
		var vfB = (float)(max[2] - min[2]);

		for (int i = 0; i < IterPower; ++i) {
			float r = vfR * covF[0] + vfG * covF[1] + vfB * covF[2];
			float g = vfR * covF[1] + vfG * covF[3] + vfB * covF[4];
			float b = vfR * covF[2] + vfG * covF[4] + vfB * covF[5];

			vfR = r;
			vfG = g;
			vfB = b;
		}

		float magnitude = MathF.Abs(vfR);
		float vfGAbs = MathF.Abs(vfG);
		float vfBAbs = MathF.Abs(vfB);
		if (vfGAbs > magnitude) magnitude = vfGAbs;
		if (vfBAbs > magnitude) magnitude = vfBAbs;

		int vR, vG, vB;
		if (magnitude < 4.0f) { // too small, default to luminance
			vR = LumaCoefficients.Red;
			vG = LumaCoefficients.Green;
			vB = LumaCoefficients.Blue;
		}
		else {
			if (Extensions.Simd.Support.Enabled && Sse2.IsSupported) {
				var vec = Vector128.Create(vfR, vfG, vfB, 0);
				var mag = Vector128.Create(512.0f / magnitude);
				var res = Sse2.Multiply(vec, mag);
				var result = Sse2.ConvertToVector128Int32WithTruncation(res);
				vR = result.GetElement(0);
				vG = result.GetElement(1);
				vB = result.GetElement(2);
			}
			else {
				magnitude = 512.0f / magnitude;
				vR = (int)(vfR * magnitude);
				vG = (int)(vfG * magnitude);
				vB = (int)(vfB * magnitude);
			}
		}

		byte* maxP;
		var minP = maxP = block;
		int maxD;
		var minD = maxD = block[0] * vR + block[1] * vG + block[2] * vB;
		// Pick colors at extreme points
		for (int i = 1; i < 16; ++i) {
			int dot = block[i * 4 + 0] * vR + block[i * 4 + 1] * vG + block[i * 4 + 2] * vB;

			if (dot < minD) {
				minD = dot;
				minP = block + i * 4;
			}

			if (dot > maxD) {
				maxD = dot;
				maxP = block + i * 4;
			}
		}

		pMax16 = As16Bit(maxP[0], maxP[1], maxP[2]);
		pMin16 = As16Bit(minP[0], minP[1], minP[2]);
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort Quantize5(float x) {
		x = x < 0 ? 0 : x > 1 ? 1 : x;  // saturate
		ushort q = (ushort)(x * 31);
		q += (x > ReadArray(MidPoints5, q)).As<ushort>();
		return q;
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort Quantize6(float x) {
		x = x < 0 ? 0 : x > 1 ? 1 : x;  // saturate
		ushort q = (ushort)(x * 63);
		q += (x > ReadArray(MidPoints6, q)).As<ushort>();
		return q;
	}

	// The refinement function. (Clever code, part 2)
	// Tries to optimize colors to suit block contents better.
	// (By solving a least squares system via normal equations+Cramer's rule)
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe bool RefineBlock(byte* block, ref ushort pMax16, ref ushort pMin16, uint mask) {

		ushort min16, max16;

		ushort oldMin = pMin16;
		ushort oldMax = pMax16;

		if ((mask ^ (mask << 2)) < 4) // all pixels have the same index?
		{
			// yes, linear system would be singular; solve using optimal
			// single-color match on average color
			uint r = 8, g = 8, b = 8;
			for (int i = 0; i < 16; ++i) {
				r += block[i * 4 + 0];
				g += block[i * 4 + 1];
				b += block[i * 4 + 2];
			}

			r >>= 4; g >>= 4; b >>= 4;

			uint rIndex = r << 1;
			uint gIndex = g << 1;
			uint bIndex = b << 1;

			max16 = (ushort)((Match5[rIndex + 0] << 11) | (Match6[gIndex + 0] << 5) | Match5[bIndex + 0]);
			min16 = (ushort)((Match5[rIndex + 1] << 11) | (Match6[gIndex + 1] << 5) | Match5[bIndex + 1]);
		}
		else {
			uint at1R = 0;
			uint at1G = 0;
			uint at1B = 0;
			uint at2R = 0;
			uint at2G = 0;
			uint at2B = 0;
			int accumulator = 0;
			uint cm = mask;
			for (int i = 0; i < 16; ++i, cm >>= 2) {
				int step = (int)(cm & 3U);
				uint w1 = W1Tab[step];
				uint r = block[i * 4 + 0];
				uint g = block[i * 4 + 1];
				uint b = block[i * 4 + 2];

				accumulator += Products[step];
				at1R += w1 * r;
				at1G += w1 * g;
				at1B += w1 * b;
				at2R += r;
				at2G += g;
				at2B += b;
			}

			at2R = 3 * at2R - at1R;
			at2G = 3 * at2G - at1G;
			at2B = 3 * at2B - at1B;

			// extract solutions and decide solvability
			int xx = accumulator >> 16;
			int yy = (accumulator >> 8) & 0xff;
			int xy = (accumulator >> 0) & 0xff;

			float f = 3.0f / 255.0f / (xx * yy - xy * xy);

			max16 = (ushort)(
				(Quantize5((at1R * yy - at2R * xy) * f) << 11) |
				(Quantize6((at1G * yy - at2G * xy) * f) << 5) |
				(Quantize5((at1B * yy - at2B * xy) * f) << 0)
			);

			min16 = (ushort)(
				(Quantize5((at2R * xx - at1R * xy) * f) << 11) |
				(Quantize6((at2G * xx - at1G * xy) * f) << 5) |
				(Quantize5((at2B * xx - at1B * xy) * f) << 0)
			);
		}

		pMin16 = min16;
		pMax16 = max16;
		return oldMin != min16 || oldMax != max16;
	}

	// Color block compression
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void CompressColorBlock(byte* dest, byte* block, CompressionMode mode) {
		int refineCount = mode.HasFlag(CompressionMode.HighQuality) ? 2 : 1;

		// check if block is constant
		int blockIter;
		uint refBlock = ((uint*)block)[0];
		for (blockIter = 1; blockIter < 16; ++blockIter) {
			if (((uint*)block)[blockIter] != refBlock) {
				break;
			}
		}

		ushort max16, min16;
		uint mask;
		if (blockIter == 16) { // constant color
			uint r = block[0], g = block[1], b = block[2];
			mask = 0xAAAA_AAAAU;
			uint rIndex = (uint)(r << 1);
			uint gIndex = (uint)(g << 1);
			uint bIndex = (uint)(b << 1);
			max16 = (ushort)((Match5[rIndex + 0] << 11) | (Match6[gIndex + 0] << 5) | Match5[bIndex + 0]);
			min16 = (ushort)((Match5[rIndex + 1] << 11) | (Match6[gIndex + 1] << 5) | Match5[bIndex + 1]);
		}
		else {
			// first step: PCA+map along principal axis
			OptimizeColorsBlock(block, out max16, out min16);
			byte* color = stackalloc byte[4 * 4];
			if (max16 != min16) {
				EvalColors(color, max16, min16);
				mask = MatchColorsBlock(block, color);
			}
			else {
				mask = 0;
			}

			// third step: refine (multiple times if requested)
			for (int i = 0; i < refineCount; i++) {
				uint lastMask = mask;

				if (RefineBlock(block, ref max16, ref min16, mask)) {
					if (max16 != min16) {
						EvalColors(color, max16, min16);
						mask = MatchColorsBlock(block, color);
					}
					else {
						mask = 0;
						break;
					}
				}

				if (mask == lastMask)
					break;
			}
		}

		// write the color block
		if (max16 < min16) {
			ushort t = min16;
			min16 = max16;
			max16 = t;
			mask ^= 0x5555_5555U;
		}

		*(ushort*)&dest[0] = max16;
		*(ushort*)&dest[2] = min16;
		*(uint*)&dest[4] = mask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void CompressAlphaBlock(byte* dest, byte* src) {
		// find min/max color
		uint mn = byte.MaxValue;
		uint mx = byte.MinValue;
		if (Sse2.IsSupported && Extensions.Simd.Support.Enabled && Extensions.Simd.Support.Sse41 && Extensions.Simd.Support.Ssse3) {
			var vec0 = Sse2.LoadVector128(src + (0 * 16));
			var vec1 = Sse2.LoadVector128(src + (1 * 16));
			var vec2 = Sse2.LoadVector128(src + (2 * 16));
			var vec3 = Sse2.LoadVector128(src + (3 * 16));

			var temp0 = Ssse3.Shuffle(vec0.AsByte(), Vector128.Create(
				0, 0xFF, 4, 0xFF, 8, 0xFF, 12, 0xFF,
				0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
			));
			var temp1 = Ssse3.Shuffle(vec1.AsByte(), Vector128.Create(
				0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
				0, 0xFF, 4, 0xFF, 8, 0xFF, 12, 0xFF
			));
			var temp2 = Ssse3.Shuffle(vec2.AsByte(), Vector128.Create(
				0, 0xFF, 4, 0xFF, 8, 0xFF, 12, 0xFF,
				0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
			));
			var temp3 = Ssse3.Shuffle(vec3.AsByte(), Vector128.Create(
				0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
				0, 0xFF, 4, 0xFF, 8, 0xFF, 12, 0xFF
			));

			var tempA4 = Sse2.Or(temp0, temp1).AsUInt16();
			var tempA5 = Sse2.Or(temp2, temp3).AsUInt16();

			var mask16 = Vector128.Create((ushort)0x00FF);
			var tmp1Lo2 = tempA4;
			var tmp1Hi = tempA5;
			var tmpMin1 = Sse41.Min(tmp1Lo2, tmp1Hi);
			var tmpMin2 = Sse41.MinHorizontal(tmpMin1);

			var tmp2Lo2 = Sse2.Xor(tmp1Lo2, mask16);
			var tmp2Hi = Sse2.Xor(tmp1Hi, mask16);
			var tmpMax1 = Sse41.Min(tmp2Lo2, tmp2Hi);
			var tmpMax2 = Sse41.MinHorizontal(tmpMax1);

			mn = (byte)Sse2.ConvertToInt32(tmpMin2.AsInt32());
			mx = (byte)~(byte)Sse2.ConvertToInt32(tmpMax2.AsInt32());
		}
		else {
			for (int i = 0; i < 64; i += 4) {
				byte value = src[i];
				if (value < mn) {
					mn = value;
				}
				if (value > mx) {
					mx = value;
				}
			}
		}

		// encode them
		dest[0] = (byte)mx;
		dest[1] = (byte)mn;
		dest += 2;

		// determine bias and emit color indices
		// given the choice of mx/mn, these indices are optimal:
		// http://fgiesen.wordpress.com/2009/12/15/dxt5-alpha-block-index-determination/
		var dist = mx - mn;
		var dist4 = dist * 4U;
		var dist2 = dist * 2U;
		var bias = (dist < 8U) ? (dist - 1U) : (dist / 2U + 2U);
		bias -= mn * 7;
		var bits = 0U;
		var mask = 0U;

		for (uint i = 0U; i < 64U; i += 8U) {
			{
				int a = (int)(src[i + 0x0] * 7U + bias);

				// select index. this is a "linear scale" lerp factor between 0 (val=min) and 7 (val=max).
				int t = (a >= dist4) ? -1 : 0;
				int ind = t & 4;
				a -= (int)dist4 & t;
				t = (a >= dist2) ? -1 : 0;
				ind += t & 2;
				a -= (int)dist2 & t;
				ind += (a >= dist) ? 1 : 0;

				// turn linear scale into DXT index (0/1 are extremal pts)
				ind = -ind & 7;
				ind ^= (2 > ind) ? 1 : 0;

				// write index
				mask |= (uint)(ind << (int)bits);
				if ((bits += 3U) >= 8U) {
					*dest++ = (byte)mask;
					mask >>= 8;
					bits -= 8U;
				}
			}
			{
				int a = (int)(src[i + 0x4] * 7U + bias);

				// select index. this is a "linear scale" lerp factor between 0 (val=min) and 7 (val=max).
				int t = (a >= dist4) ? -1 : 0;
				int ind = t & 4;
				a -= (int)dist4 & t;
				t = (a >= dist2) ? -1 : 0;
				ind += t & 2;
				a -= (int)dist2 & t;
				ind += (a >= dist) ? 1 : 0;

				// turn linear scale into DXT index (0/1 are extremal pts)
				ind = -ind & 7;
				ind ^= (2 > ind) ? 1 : 0;

				// write index
				mask |= (uint)(ind << (int)bits);
				if ((bits += 3U) >= 8U) {
					*dest++ = (byte)mask;
					mask >>= 8;
					bits -= 8U;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void CompressDxt1Block(byte* dest, byte* src, CompressionMode mode) {
		CompressColorBlock(dest, src, mode);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void CompressDxt5Block(byte* tempBlock, byte* dest, byte* src, CompressionMode mode) {
		CompressAlphaBlock(dest, src + 3);
		dest += 8;
		// make a new copy of the data in which alpha is opaque,
		// because code uses a fast test for color constancy
		if (Extensions.Simd.Support.Avx2) {
			var mask = Vector256.Create(0xFF00_0000U);

			var vec0 = Avx2.LoadVector256((uint*)(src + 0x00));
			var vec1 = Avx2.LoadVector256((uint*)(src + 0x20));

			vec0 = Avx2.Or(vec0, mask);
			vec1 = Avx2.Or(vec1, mask);

			Avx2.Store((uint*)(tempBlock + 0x00), vec0);
			Avx2.Store((uint*)(tempBlock + 0x20), vec1);
		}
		else if (Extensions.Simd.Support.Enabled && Sse2.IsSupported) {
			var mask = Vector128.Create(0xFF00_0000U);

			var vec0 = Sse2.LoadVector128((uint*)(src + 0x00));
			var vec1 = Sse2.LoadVector128((uint*)(src + 0x10));
			var vec2 = Sse2.LoadVector128((uint*)(src + 0x20));
			var vec3 = Sse2.LoadVector128((uint*)(src + 0x30));

			vec0 = Sse2.Or(vec0, mask);
			vec1 = Sse2.Or(vec1, mask);
			vec2 = Sse2.Or(vec2, mask);
			vec3 = Sse2.Or(vec3, mask);

			Sse2.Store((uint*)(tempBlock + 0x00), vec0);
			Sse2.Store((uint*)(tempBlock + 0x10), vec1);
			Sse2.Store((uint*)(tempBlock + 0x20), vec2);
			Sse2.Store((uint*)(tempBlock + 0x30), vec3);
		}
		else {
			const int elementCount = 64 / sizeof(uint);

			if (elementCount <= Vector<uint>.Count && (elementCount % Vector<uint>.Count) == 0) {
				var orMask = new Vector<uint>(0xFF00_0000);
				var sourceSpan = new ReadOnlySpan<byte>(src, 64).Cast<uint>();
				var dataSpan = new Span<byte>(tempBlock, 64).Cast<uint>();
				for (int i = 0; i < elementCount; i += Vector<uint>.Count) {
					var sourceVector = new Vector<uint>(sourceSpan.SliceUnsafe(i));
					var resultVector = Vector.BitwiseOr(sourceVector, orMask);
					resultVector.CopyTo(dataSpan.SliceUnsafe(i));
				}
			}
			else {
				ulong* uData = (ulong*)tempBlock;
				ulong* uSrc = (ulong*)src;
				uData[0x0] = uSrc[0x0] | 0xFF00_0000_FF00_0000UL;
				uData[0x1] = uSrc[0x1] | 0xFF00_0000_FF00_0000UL;
				uData[0x2] = uSrc[0x2] | 0xFF00_0000_FF00_0000UL;
				uData[0x3] = uSrc[0x3] | 0xFF00_0000_FF00_0000UL;
				uData[0x4] = uSrc[0x4] | 0xFF00_0000_FF00_0000UL;
				uData[0x5] = uSrc[0x5] | 0xFF00_0000_FF00_0000UL;
				uData[0x6] = uSrc[0x6] | 0xFF00_0000_FF00_0000UL;
				uData[0x7] = uSrc[0x7] | 0xFF00_0000_FF00_0000UL;
			}
		}

		src = tempBlock;

		CompressColorBlock(dest, src, mode);
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void CompressDxt1(Span<byte> destination, ReadOnlySpan<byte> source, Vector2I size, CompressionMode mode) {
		byte* blockRow = stackalloc byte[size.Width * 4 * 4];
		fixed (byte* sourcePtr = source) {
			fixed (byte* destPtr = destination) {
				byte* dest = destPtr;
				for (uint y = 0; y < size.Height; y += 4) {
					uint stride = (uint)size.Width * 4U;
					byte* sourceRowPtr = sourcePtr + stride * y;

					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x0 * 16) + (x * 16), sourceRowPtr + (stride * 0U) + (x * 4), 16U);
					}
					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x1 * 16) + (x * 16), sourceRowPtr + (stride * 1U) + (x * 4), 16U);
					}
					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x2 * 16) + (x * 16), sourceRowPtr + (stride * 2U) + (x * 4), 16U);
					}
					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x3 * 16) + (x * 16), sourceRowPtr + (stride * 3U) + (x * 4), 16U);
						CompressDxt1Block(dest, blockRow + (x * 16), mode);
						dest += 8;
					}
				}
			}
		}
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void CompressDxt5(Span<byte> destination, ReadOnlySpan<byte> source, Vector2I size, CompressionMode mode) {
		byte* blockRow = stackalloc byte[size.Width * 4 * 4];
		byte* tempBlock = stackalloc byte[64];
		fixed (byte* sourcePtr = source) {
			fixed (byte* destPtr = destination) {
				byte* dest = destPtr;
				for (uint y = 0; y < size.Height; y += 4) {
					uint stride = (uint)size.Width * 4U;
					byte* sourceRowPtr = sourcePtr + stride * y;

					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x0 * 16) + (x * 16), sourceRowPtr + (stride * 0U) + (x * 4), 16U);
					}
					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x1 * 16) + (x * 16), sourceRowPtr + (stride * 1U) + (x * 4), 16U);
					}
					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x2 * 16) + (x * 16), sourceRowPtr + (stride * 2U) + (x * 4), 16U);
					}
					for (uint x = 0; x < size.Width; x += 4) {
						Unsafe.CopyBlockUnaligned(blockRow + (0x3 * 16) + (x * 16), sourceRowPtr + (stride * 3U) + (x * 4), 16U);
						CompressDxt5Block(tempBlock, dest, blockRow + (x * 16), mode);
						dest += 16;
					}
				}
			}
		}
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int RequiredSize(Vector2I size, bool hasAlpha) {
		uint num = hasAlpha ? 16U : 8U;
		return (int)(size.Area * num / 16U);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void CompressDxt<T>(Span<byte> destination, ReadOnlySpan<T> source, Vector2I size, bool hasAlpha, CompressionMode mode) where T : unmanaged {
		var sourceBytes = source.AsBytes();

		if (sourceBytes.Length != size.Area * 4) {
			ThrowHelper.ThrowNotImplementedException("This method supports only RGBA images");
		}

		int requiredSize = RequiredSize(size, hasAlpha);
		if (destination.Length < requiredSize) {
			ThrowHelper.ThrowArgumentException("Destination span too small", nameof(destination));
		}

		if (hasAlpha) {
			CompressDxt5(destination, sourceBytes, size, mode);
		}
		else {
			CompressDxt1(destination, sourceBytes, size, mode);
		}
	}
}
