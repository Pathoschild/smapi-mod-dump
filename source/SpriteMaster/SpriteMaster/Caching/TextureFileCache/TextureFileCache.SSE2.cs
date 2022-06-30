/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void ProcessTextureSse2Unrolled(Span<Color8> data) {
		uint registerElements = (uint)Vector128<uint>.Count * 4U;
		registerElements.AssertEqual((uint)(sizeof(Vector128<uint>) / sizeof(Color8)));

		uint offset;
		fixed (Color8* dataPtr8 = data) {
			uint* dataPtr = (uint*)dataPtr8;

			for (offset = 0; offset + (registerElements - 1U) < data.Length; offset += registerElements) {
				Vector128<uint> rawColor0 = Sse2.LoadVector128(dataPtr + offset + 0x0);
				Vector128<uint> rawColor1 = Sse2.LoadVector128(dataPtr + offset + 0x4);
				Vector128<uint> rawColor2 = Sse2.LoadVector128(dataPtr + offset + 0x8);
				Vector128<uint> rawColor3 = Sse2.LoadVector128(dataPtr + offset + 0xC);

				Vector128<uint> alphaMask = Vector128.Create(0xFF000000U);
				Vector128<uint> alpha0 = Sse2.And(rawColor0, alphaMask);
				Vector128<uint> alpha1 = Sse2.And(rawColor1, alphaMask);
				Vector128<uint> alpha2 = Sse2.And(rawColor2, alphaMask);
				Vector128<uint> alpha3 = Sse2.And(rawColor3, alphaMask);

				Vector128<ushort> lo0 = Sse2.UnpackLow(rawColor0.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> lo1 = Sse2.UnpackLow(rawColor1.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> lo2 = Sse2.UnpackLow(rawColor2.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> lo3 = Sse2.UnpackLow(rawColor3.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> hi0 = Sse2.UnpackHigh(rawColor0.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> hi1 = Sse2.UnpackHigh(rawColor1.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> hi2 = Sse2.UnpackHigh(rawColor2.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> hi3 = Sse2.UnpackHigh(rawColor3.AsByte(), Vector128<byte>.Zero).AsUInt16();

				Vector128<uint> alphaLo0, alphaHi0;
				Vector128<uint> alphaLo1, alphaHi1;
				Vector128<uint> alphaLo2, alphaHi2;
				Vector128<uint> alphaLo3, alphaHi3;
				if (Ssse3.IsSupported) {
					Vector128<byte> alphaShuffle = Vector128.Create(6, 0xFF, 6, 0xFF, 6, 0xFF, 6, 0xFF, 14, 0xFF, 14, 0xFF, 14, 0xFF, 14, 0xFF);

					alphaLo0 = Ssse3.Shuffle(lo0.AsByte(), alphaShuffle).AsUInt32();
					alphaLo1 = Ssse3.Shuffle(lo1.AsByte(), alphaShuffle).AsUInt32();
					alphaLo2 = Ssse3.Shuffle(lo2.AsByte(), alphaShuffle).AsUInt32();
					alphaLo3 = Ssse3.Shuffle(lo3.AsByte(), alphaShuffle).AsUInt32();
					alphaHi0 = Ssse3.Shuffle(hi0.AsByte(), alphaShuffle).AsUInt32();
					alphaHi1 = Ssse3.Shuffle(hi1.AsByte(), alphaShuffle).AsUInt32();
					alphaHi2 = Ssse3.Shuffle(hi2.AsByte(), alphaShuffle).AsUInt32();
					alphaHi3 = Ssse3.Shuffle(hi3.AsByte(), alphaShuffle).AsUInt32();
				}
				else {
					alphaLo0 = Sse2.UnpackLow(alpha0.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaLo1 = Sse2.UnpackLow(alpha1.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaLo2 = Sse2.UnpackLow(alpha2.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaLo3 = Sse2.UnpackLow(alpha3.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaHi0 = Sse2.UnpackHigh(alpha0.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaHi1 = Sse2.UnpackHigh(alpha1.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaHi2 = Sse2.UnpackHigh(alpha2.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaHi3 = Sse2.UnpackHigh(alpha3.AsByte(), Vector128<byte>.Zero).AsUInt32();

					Vector128<uint> alphaLo160 = Sse2.ShiftRightLogical(alphaLo0, 16);
					Vector128<uint> alphaLo161 = Sse2.ShiftRightLogical(alphaLo1, 16);
					Vector128<uint> alphaLo162 = Sse2.ShiftRightLogical(alphaLo2, 16);
					Vector128<uint> alphaLo163 = Sse2.ShiftRightLogical(alphaLo3, 16);
					Vector128<uint> alphaHi160 = Sse2.ShiftRightLogical(alphaHi0, 16);
					Vector128<uint> alphaHi161 = Sse2.ShiftRightLogical(alphaHi1, 16);
					Vector128<uint> alphaHi162 = Sse2.ShiftRightLogical(alphaHi2, 16);
					Vector128<uint> alphaHi163 = Sse2.ShiftRightLogical(alphaHi3, 16);
					alphaLo0 = Sse2.Or(alphaLo0, alphaLo160);
					alphaLo1 = Sse2.Or(alphaLo1, alphaLo161);
					alphaLo2 = Sse2.Or(alphaLo2, alphaLo162);
					alphaLo3 = Sse2.Or(alphaLo3, alphaLo163);
					alphaHi0 = Sse2.Or(alphaHi0, alphaHi160);
					alphaHi1 = Sse2.Or(alphaHi1, alphaHi161);
					alphaHi2 = Sse2.Or(alphaHi2, alphaHi162);
					alphaHi3 = Sse2.Or(alphaHi3, alphaHi163);

					Vector128<ulong> alphaLo320 = Sse2.ShiftRightLogical(alphaLo0.AsUInt64(), 32);
					Vector128<ulong> alphaLo321 = Sse2.ShiftRightLogical(alphaLo1.AsUInt64(), 32);
					Vector128<ulong> alphaLo322 = Sse2.ShiftRightLogical(alphaLo2.AsUInt64(), 32);
					Vector128<ulong> alphaLo323 = Sse2.ShiftRightLogical(alphaLo3.AsUInt64(), 32);
					Vector128<ulong> alphaHi320 = Sse2.ShiftRightLogical(alphaHi0.AsUInt64(), 32);
					Vector128<ulong> alphaHi321 = Sse2.ShiftRightLogical(alphaHi1.AsUInt64(), 32);
					Vector128<ulong> alphaHi322 = Sse2.ShiftRightLogical(alphaHi2.AsUInt64(), 32);
					Vector128<ulong> alphaHi323 = Sse2.ShiftRightLogical(alphaHi3.AsUInt64(), 32);
					alphaLo0 = Sse2.Or(alphaLo0.AsUInt64(), alphaLo320).AsUInt32();
					alphaLo1 = Sse2.Or(alphaLo1.AsUInt64(), alphaLo321).AsUInt32();
					alphaLo2 = Sse2.Or(alphaLo2.AsUInt64(), alphaLo322).AsUInt32();
					alphaLo3 = Sse2.Or(alphaLo3.AsUInt64(), alphaLo323).AsUInt32();
					alphaHi0 = Sse2.Or(alphaHi0.AsUInt64(), alphaHi320).AsUInt32();
					alphaHi1 = Sse2.Or(alphaHi1.AsUInt64(), alphaHi321).AsUInt32();
					alphaHi2 = Sse2.Or(alphaHi2.AsUInt64(), alphaHi322).AsUInt32();
					alphaHi3 = Sse2.Or(alphaHi3.AsUInt64(), alphaHi323).AsUInt32();
				}

				Vector128<ushort> prodLo0 = Sse2.MultiplyLow(lo0, alphaLo0.AsUInt16());
				Vector128<ushort> prodLo1 = Sse2.MultiplyLow(lo1, alphaLo1.AsUInt16());
				Vector128<ushort> prodLo2 = Sse2.MultiplyLow(lo2, alphaLo2.AsUInt16());
				Vector128<ushort> prodLo3 = Sse2.MultiplyLow(lo3, alphaLo3.AsUInt16());
				Vector128<ushort> prodHi0 = Sse2.MultiplyLow(hi0, alphaHi0.AsUInt16());
				Vector128<ushort> prodHi1 = Sse2.MultiplyLow(hi1, alphaHi1.AsUInt16());
				Vector128<ushort> prodHi2 = Sse2.MultiplyLow(hi2, alphaHi2.AsUInt16());
				Vector128<ushort> prodHi3 = Sse2.MultiplyLow(hi3, alphaHi3.AsUInt16());

				Vector128<ushort> addend = Vector128.Create((ushort)0x00FFU);

				var sumLo0 = Sse2.Add(prodLo0, addend);
				var sumLo1 = Sse2.Add(prodLo1, addend);
				var sumLo2 = Sse2.Add(prodLo2, addend);
				var sumLo3 = Sse2.Add(prodLo3, addend);
				var sumHi0 = Sse2.Add(prodHi0, addend);
				var sumHi1 = Sse2.Add(prodHi1, addend);
				var sumHi2 = Sse2.Add(prodHi2, addend);
				var sumHi3 = Sse2.Add(prodHi3, addend);

				var shiftLo0 = Sse2.ShiftRightLogical(sumLo0, 8);
				var shiftLo1 = Sse2.ShiftRightLogical(sumLo1, 8);
				var shiftLo2 = Sse2.ShiftRightLogical(sumLo2, 8);
				var shiftLo3 = Sse2.ShiftRightLogical(sumLo3, 8);
				var shiftHi0 = Sse2.ShiftRightLogical(sumHi0, 8);
				var shiftHi1 = Sse2.ShiftRightLogical(sumHi1, 8);
				var shiftHi2 = Sse2.ShiftRightLogical(sumHi2, 8);
				var shiftHi3 = Sse2.ShiftRightLogical(sumHi3, 8);

				var packed0 = Sse2.PackUnsignedSaturate(shiftLo0.AsInt16(), shiftHi0.AsInt16()).AsUInt32();
				var packed1 = Sse2.PackUnsignedSaturate(shiftLo1.AsInt16(), shiftHi1.AsInt16()).AsUInt32();
				var packed2 = Sse2.PackUnsignedSaturate(shiftLo2.AsInt16(), shiftHi2.AsInt16()).AsUInt32();
				var packed3 = Sse2.PackUnsignedSaturate(shiftLo3.AsInt16(), shiftHi3.AsInt16()).AsUInt32();

				var mask = Vector128.Create(0x00FFFFFFU);
				packed0 = Sse2.And(packed0, mask);
				packed1 = Sse2.And(packed1, mask);
				packed2 = Sse2.And(packed2, mask);
				packed3 = Sse2.And(packed3, mask);
				packed0 = Sse2.Or(packed0, alpha0);
				packed1 = Sse2.Or(packed1, alpha1);
				packed2 = Sse2.Or(packed2, alpha2);
				packed3 = Sse2.Or(packed3, alpha3);

				Sse2.Store(dataPtr + offset + 0x0, packed0);
				Sse2.Store(dataPtr + offset + 0x4, packed1);
				Sse2.Store(dataPtr + offset + 0x8, packed2);
				Sse2.Store(dataPtr + offset + 0xC, packed3);
			}
		}

		// This is unlikely to happen, but handle when there are still elements left (the texture size isn't aligned to 4)
		if (offset < data.Length) {
			ProcessTextureScalar(data.SliceUnsafe(offset));
		}
	}

	// https://godbolt.org/z/rdq3xP15E

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void ProcessTextureSse2(Span<Color8> data) {
		uint registerElements = (uint)Vector128<uint>.Count;
		registerElements.AssertEqual((uint)(sizeof(Vector128<uint>) / sizeof(Color8)));

		uint offset;
		fixed (Color8* dataPtr8 = data) {
			uint* dataPtr = (uint*)dataPtr8;

			for (offset = 0; offset + (registerElements - 1U) < data.Length; offset += registerElements) {
				Vector128<uint> rawColor = Sse2.LoadVector128(dataPtr + offset);

				Vector128<uint> alphaMask = Vector128.Create(0xFF000000U);
				Vector128<uint> alpha = Sse2.And(rawColor, alphaMask);

				Vector128<ushort> lo = Sse2.UnpackLow(rawColor.AsByte(), Vector128<byte>.Zero).AsUInt16();
				Vector128<ushort> hi = Sse2.UnpackHigh(rawColor.AsByte(), Vector128<byte>.Zero).AsUInt16();

				Vector128<uint> alphaLo, alphaHi;
				if (Ssse3.IsSupported) {
					Vector128<byte> alphaShuffle = Vector128.Create(6, 0xFF, 6, 0xFF, 6, 0xFF, 6, 0xFF, 14, 0xFF, 14, 0xFF, 14, 0xFF, 14, 0xFF);

					alphaLo = Ssse3.Shuffle(lo.AsByte(), alphaShuffle).AsUInt32();
					alphaHi = Ssse3.Shuffle(hi.AsByte(), alphaShuffle).AsUInt32();
				}
				else {
					alphaLo = Sse2.UnpackLow(alpha.AsByte(), Vector128<byte>.Zero).AsUInt32();
					alphaHi = Sse2.UnpackHigh(alpha.AsByte(), Vector128<byte>.Zero).AsUInt32();

					Vector128<uint> alphaLo16 = Sse2.ShiftRightLogical(alphaLo, 16);
					Vector128<uint> alphaHi16 = Sse2.ShiftRightLogical(alphaHi, 16);
					alphaLo = Sse2.Or(alphaLo, alphaLo16);
					alphaHi = Sse2.Or(alphaHi, alphaHi16);

					Vector128<ulong> alphaLo32 = Sse2.ShiftRightLogical(alphaLo.AsUInt64(), 32);
					Vector128<ulong> alphaHi32 = Sse2.ShiftRightLogical(alphaHi.AsUInt64(), 32);
					alphaLo = Sse2.Or(alphaLo.AsUInt64(), alphaLo32).AsUInt32();
					alphaHi = Sse2.Or(alphaHi.AsUInt64(), alphaHi32).AsUInt32();
				}

				Vector128<ushort> prodLo = Sse2.MultiplyLow(lo, alphaLo.AsUInt16());
				Vector128<ushort> prodHi = Sse2.MultiplyLow(hi, alphaHi.AsUInt16());

				Vector128<ushort> addend = Vector128.Create((ushort)0x00FFU);

				var sumLo = Sse2.Add(prodLo, addend);
				var sumHi = Sse2.Add(prodHi, addend);

				var shiftLo = Sse2.ShiftRightLogical(sumLo, 8);
				var shiftHi = Sse2.ShiftRightLogical(sumHi, 8);

				var packed = Sse2.PackUnsignedSaturate(shiftLo.AsInt16(), shiftHi.AsInt16()).AsUInt32();

				var mask = Vector128.Create(0x00FFFFFFU);
				packed = Sse2.And(packed, mask);
				packed = Sse2.Or(packed, alpha);

				Sse2.Store(dataPtr + offset, packed);
			}
		}

		// This is unlikely to happen, but handle when there are still elements left (the texture size isn't aligned to 4)
		if (offset < data.Length) {
			ProcessTextureScalar(data.SliceUnsafe(offset));
		}
	}

	/*

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void ProcessTextureSse2(Span<Color16> data) {
		uint registerElements = (uint)Vector128<ulong>.Count;
		registerElements.AssertEqual((uint)(sizeof(Vector128<ulong>) / sizeof(Color16)));

		uint offset;
		fixed (Color16* dataPtr16 = data) {
			ulong* dataPtr = (ulong*)dataPtr16;

			for (offset = 0; offset + (registerElements - 1U) < data.Length; offset += registerElements) {
				Vector128<ulong> rawColor = Sse2.LoadVector128(dataPtr + offset);

				Vector128<ulong> alphaMask = Vector128.Create(0xFFFF_0000_0000_0000UL);
				Vector128<ulong> alpha = Sse2.And(rawColor, alphaMask);

				Vector128<uint> lo = Sse2.UnpackLow(rawColor.AsUInt16(), Vector128<ushort>.Zero).AsUInt32();
				Vector128<uint> hi = Sse2.UnpackHigh(rawColor.AsUInt16(), Vector128<ushort>.Zero).AsUInt32();

				Vector128<ulong> alphaLo, alphaHi;
				if (Ssse3.IsSupported) {
					Vector128<byte> alphaShuffle = Vector128.Create(6, 6, 0xFF, 0xFF, 6, 6, 0xFF, 0xFF, 14, 14, 0xFF, 0xFF, 14, 14, 0xFF, 0xFF);

					alphaLo = Ssse3.Shuffle(lo.AsByte(), alphaShuffle).AsUInt64();
					alphaHi = Ssse3.Shuffle(hi.AsByte(), alphaShuffle).AsUInt64();
				}
				else {
					alphaLo = Sse2.UnpackLow(alpha.AsUInt16(), Vector128<ushort>.Zero).AsUInt64();
					alphaHi = Sse2.UnpackHigh(alpha.AsUInt16(), Vector128<ushort>.Zero).AsUInt64();

					Vector128<ulong> alphaLo32 = Sse2.ShiftRightLogical(alphaLo, 32);
					Vector128<ulong> alphaHi32 = Sse2.ShiftRightLogical(alphaHi, 32);
					alphaLo = Sse2.Or(alphaLo, alphaLo32);
					alphaHi = Sse2.Or(alphaHi, alphaHi32);

					alphaLo = Sse2.Shuffle(alphaLo.AsUInt32(), 0 | (1 << 2) | 0 | (1 << 6)).AsUInt64();
					alphaHi = Sse2.Shuffle(alphaHi.AsUInt32(), 0 | (1 << 2) | 0 | (1 << 6)).AsUInt64();
				}

				Vector128<uint> prodLo;
				Vector128<uint> prodHi;
				if (Sse41.IsSupported) {
					prodLo = Sse41.MultiplyLow(lo, alphaLo.AsUInt32());
					prodHi = Sse41.MultiplyLow(hi, alphaHi.AsUInt32());
				}
				else {
					//https://stackoverflow.com/a/17268337/5055153
					{
						var a13 = Sse2.Shuffle(lo, 0xF5);
						var b13 = Sse2.Shuffle(alphaLo.AsUInt32(), 0xF5);
						var prod02 = Sse2.Multiply(lo, alphaLo.AsUInt32());
						var prod13 = Sse2.Multiply(a13, b13);
						var prod01 = Sse2.UnpackLow(prod02, prod13);
						var prod23 = Sse2.UnpackHigh(prod02, prod13);
						prodLo = Sse2.UnpackLow(prod01.AsUInt64(), prod23.AsUInt64()).AsUInt32();
					}
					{
						var a13 = Sse2.Shuffle(hi, 0xF5);
						var b13 = Sse2.Shuffle(alphaHi.AsUInt32(), 0xF5);
						var prod02 = Sse2.Multiply(hi, alphaHi.AsUInt32());
						var prod13 = Sse2.Multiply(a13, b13);
						var prod01 = Sse2.UnpackLow(prod02, prod13);
						var prod23 = Sse2.UnpackHigh(prod02, prod13);
						prodHi = Sse2.UnpackLow(prod01.AsUInt64(), prod23.AsUInt64()).AsUInt32();
					}

				}

				Vector128<uint> addend = Vector128.Create(0x0000FFFFU);

				var sumLo = Sse2.Add(prodLo, addend);
				var sumHi = Sse2.Add(prodHi, addend);

				var shiftLo = Sse2.ShiftRightLogical(sumLo, 16);
				var shiftHi = Sse2.ShiftRightLogical(sumHi, 16);

				Vector128<ulong> packed;
				if (Sse41.IsSupported) {
					Sse41.PackUnsignedSaturate(shiftLo.AsInt32(), shiftHi.AsInt32()).AsUInt64();
				}
				else {

				}

				var mask = Vector128.Create(0x0000_FFFF_FFFF_FFFFUL);
				packed = Sse2.And(packed, mask);
				packed = Sse2.Or(packed, alpha);

				Sse2.Store(dataPtr + offset, packed);
			}
		}

		// This is unlikely to happen, but handle when there are still elements left (the texture size isn't aligned to 4)
		if (offset < data.Length) {
			ProcessTextureScalar(data.SliceUnsafe(offset));
		}
	}
	*/
}
