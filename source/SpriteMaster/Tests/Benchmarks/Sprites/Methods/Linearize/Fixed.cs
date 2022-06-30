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
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster;
using SpriteMaster.Colors;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Benchmarks.Sprites.Methods.Linearize;

internal static class Fixed {
	private static readonly ColorSpace ColorSpace = ColorSpace.sRGB_Precise;
	private static readonly Converter ColorConverter = ColorSpace.GetConverter();
	private static readonly ConverterRef ColorConverterRef = ColorSpace.GetConverterRef();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void Linearize(Span<Color16> data, Vector2I size) {
		fixed (Color16* pData = data) {
			for (int i = 0; i < data.Length; ++i) {
				pData[i] = ColorSpace.Linearize(pData[i]);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeStatic(Span<Color16> data, Vector2I size) {
		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				*pData = ColorSpace.Linearize(*pData);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverter(Span<Color16> data, Vector2I size) {
		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				*pData = ColorConverter.Linearize(*pData);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterCopy(Span<Color16> data, Vector2I size) {
		var colorConverter = ColorConverter;

		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				*pData = colorConverter.Linearize(*pData);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRef(Span<Color16> data, Vector2I size) {
		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				*pData = ColorConverterRef.Linearize(*pData);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy(Span<Color16> data, Vector2I size) {
		var colorConverterRef = ColorConverterRef;

		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				*pData = colorConverterRef.Linearize(*pData);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy2(Span<Color16> data, Vector2I size) {
		var colorConverterRef = ColorConverterRef;

		fixed (Color16* pRefData = data) {
			ushort* pData = (ushort*)pRefData;
			for (int i = 0; i < data.Length; ++i, pData += 4) {
				pData[0] = colorConverterRef.Linearize(pData[0]);
				pData[1] = colorConverterRef.Linearize(pData[1]);
				pData[2] = colorConverterRef.Linearize(pData[2]);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy3(Span<Color16> data, Vector2I size) {
		var colorConverterRef = ColorConverterRef;

		fixed (Color16* pRefData = data) {
			ulong* pData = (ulong*)pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = colorConverterRef.Linearize((ushort)item);
				ulong res1 = colorConverterRef.Linearize((ushort)(item >> 16));
				ulong res2 = colorConverterRef.Linearize((ushort)(item >> 32));
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy4(Span<Color16> data, Vector2I size) {
		var table = ColorConverterRef.LinearizeTable16;

		fixed (Color16* pRefData = data) {
			ulong* pData = (ulong*)pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = table[(ushort)item];
				ulong res1 = table[(ushort)(item >> 16)];
				ulong res2 = table[(ushort)(item >> 32)];
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy5(Span<Color16> data, Vector2I size) {
		var table = ColorConverterRef.LinearizeTable16;

		fixed (ushort* pTable = table) {
			fixed (Color16* pRefData = data) {
				ulong* pData = (ulong*)pRefData;
				for (int i = 0; i < data.Length; ++i, ++pData) {
					ulong item = *pData;
					ulong res0 = pTable[(ushort)item];
					ulong res1 = pTable[(ushort)(item >> 16)];
					ulong res2 = pTable[(ushort)(item >> 32)];
					item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
					*pData = item;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy6(Span<Color16> data, Vector2I size) {
		var pTable = ColorConverterRef.LinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			ulong* pData = (ulong*)pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = pTable[(ushort)item];
				ulong res1 = pTable[(ushort)(item >> 16)];
				ulong res2 = pTable[(ushort)(item >> 32)];
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy7(Span<Color16> data, Vector2I size) {
		var pTable = ColorConverterRef.LinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			ulong* pData = (ulong*)pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = ((ushort)item == 0) ? 0ul : pTable[(ushort)item];
				ulong res1 = ((ushort)(item >> 16) == 0) ? 0ul : pTable[(ushort)(item >> 16)];
				ulong res2 = ((ushort)(item >> 32) == 0) ? 0ul : pTable[(ushort)(item >> 32)];
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy8(Span<Color16> data, Vector2I size) {
		var pTable = ColorConverterRef.LinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				Color16 item = *pData;
				item.SetRgbTest((ulong)pTable[item.R.Value], (ulong)pTable[item.G.Value], (ulong)pTable[item.B.Value]);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterRefCopy9(Span<Color16> data, Vector2I size) {
		var pTable = ColorConverterRef.LinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				Color16 item = *pData;
				ulong uItem = item.Packed;
				ulong res0 = pTable[(ushort)uItem];
				ulong res1 = pTable[(ushort)(uItem >> 16)];
				ulong res2 = pTable[(ushort)(uItem >> 32)];
				item.SetRgbTest((ulong)pTable[res0], (ulong)pTable[res1], (ulong)pTable[res2]);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeConverterUncachedSimd(Span<Color16> data, Vector2I size) {
		const int Elements = 2;
		Elements.AssertEqual(Vector128<ulong>.Count);

		var converter = ColorConverterRef;
		var pTable = converter.LinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			var pData = (ulong*)pRefData;

			int i = 0;
			for (; i + (Elements - 1) < data.Length; i += Elements, pData += Elements) {
				// rr0 gg0 bb0 aa0 rr1 gg1 bb1 aa1
				Vector128<ushort> channels = Sse2.LoadVector128((ushort*)pData);

				// rr0 gg0 bb0 aa0
				Vector128<int> channelsLo = Sse2.UnpackLow(channels, Vector128<ushort>.Zero).AsInt32();
				// rr1 gg1 bb1 aa1
				Vector128<int> channelsHi = Sse2.UnpackHigh(channels, Vector128<ushort>.Zero).AsInt32();

				Vector128<float> scalarMultiplier = Vector128.Create(65535.0f);
				Vector128<float> scalarInvMultiplier = Vector128.Create(1.0f / 65535.0f);

				// rr0 gg0 bb0 aa0
				Vector128<float> channelsRealLo = Sse2.ConvertToVector128Single(channelsLo);
				channelsRealLo = Sse2.Multiply(channelsRealLo, scalarInvMultiplier);
				// rr1 gg1 bb1 aa1
				Vector128<float> channelsRealHi = Sse2.ConvertToVector128Single(channelsHi);
				channelsRealHi = Sse2.Multiply(channelsRealHi, scalarInvMultiplier);

				Vector128<float> comparand = Vector128.Create(0.04045f);
				Vector128<uint> leftMaskLo = Sse2.CompareLessThan(channelsRealLo, comparand).AsUInt32();
				Vector128<uint> rightMaskLo = Sse2.Xor(leftMaskLo, Vector128<uint>.AllBitsSet);
				Vector128<uint> leftMaskHi = Sse2.CompareLessThan(channelsRealHi, comparand).AsUInt32();
				Vector128<uint> rightMaskHi = Sse2.Xor(leftMaskHi, Vector128<uint>.AllBitsSet);

				// LEQT side -	s * (1.0 / 12.92)
				Vector128<float> leftLo;
				Vector128<float> leftHi;
				{
					Vector128<float> lessThanEqMultiplier = Vector128.Create(1.0f / 12.92f);
					leftLo = Sse2.Multiply(channelsRealLo, lessThanEqMultiplier);
					leftHi = Sse2.Multiply(channelsRealHi, lessThanEqMultiplier);
				}
				// GT side   -	Math.Pow((s + 0.055) * (1.0 / 1.055), 2.4)
				Vector128<float> rightLo;
				Vector128<float> rightHi;
				{
					Vector128<float> addend = Vector128.Create(0.055f);
					Vector128<float> greaterThanMultiplier = Vector128.Create(1.0f / 1.055f);
					// Math.Pow((s + 0.055) * (1.0 / 1.055), 2.4)
					// (s + 0.055)
					//   * mult
					// pow( ^^ , 2.4)

					Vector128<float> sumLo = Sse2.Add(channelsRealLo, addend);
					Vector128<float> sumHi = Sse2.Add(channelsRealHi, addend);

					Vector128<float> productLo = Sse2.Multiply(sumLo, greaterThanMultiplier);
					Vector128<float> productHi = Sse2.Multiply(sumHi, greaterThanMultiplier);

					// There is no SSE2 pow. An approximation of x ^ 2.4 is
					// x^2 * (1.0 / (((sqrt(x) * 3) + x) / 4))
					// Written with a reciprocal sqrt:
					// x^2 * (4 / ((((3 / (1 / sqrt(x))) + x))))
					var squareLo = Sse2.Multiply(productLo, productLo);
					var squareHi = Sse2.Multiply(productHi, productHi);

					var recipSqrtLo = Sse2.Sqrt(productLo);
					var recipSqrtHi = Sse2.Sqrt(productHi);

					var dividendRecip = Vector128.Create(3.0f);
					var quotientLo = Sse2.Multiply(recipSqrtLo, dividendRecip);
					var quotientHi = Sse2.Multiply(recipSqrtHi, dividendRecip);

					var sum2Lo = Sse2.Add(quotientLo, productLo);
					var sum2Hi = Sse2.Add(quotientHi, productHi);

					var recipLo = Sse2.Reciprocal(sum2Lo);
					var recipHi = Sse2.Reciprocal(sum2Hi);

					var multiplicand = Vector128.Create(4.0f);
					var multiplicandLo = Sse2.Multiply(recipLo, multiplicand);
					var multiplicandHi = Sse2.Multiply(recipHi, multiplicand);

					var approxLo = Sse2.Multiply(squareLo, multiplicandLo);
					var approxHi = Sse2.Multiply(squareHi, multiplicandHi);

					// Eliminate NaNs from the scalar being 0
					var maskLo = Sse2.CompareOrdered(approxLo, approxLo);
					var maskHi = Sse2.CompareOrdered(approxHi, approxHi);

					rightLo = Sse2.And(approxLo.AsUInt32(), maskLo.AsUInt32()).AsSingle();
					rightHi = Sse2.And(approxHi.AsUInt32(), maskHi.AsUInt32()).AsSingle();
				}

				// Remix them together
				var leftMaskedLo = Sse2.And(leftLo.AsUInt32(), leftMaskLo).AsSingle();
				var leftMaskedHi = Sse2.And(leftHi.AsUInt32(), leftMaskHi).AsSingle();
				var rightMaskedLo = Sse2.And(rightLo.AsUInt32(), rightMaskLo).AsSingle();
				var rightMaskedHi = Sse2.And(rightHi.AsUInt32(), rightMaskHi).AsSingle();
				Vector128<float> resultLo = Sse2.Or(leftMaskedLo, rightMaskedLo).AsSingle();
				Vector128<float> resultHi = Sse2.Or(leftMaskedHi, rightMaskedHi).AsSingle();

				Vector128<float> resultScaledLo = Sse2.Multiply(resultLo, scalarMultiplier);
				Vector128<float> resultScaledHi = Sse2.Multiply(resultHi, scalarMultiplier);

				Vector128<int> resultIntLo = Sse2.ConvertToVector128Int32(resultScaledLo);
				Vector128<int> resultIntHi = Sse2.ConvertToVector128Int32(resultScaledHi);

				Vector128<ushort> result = Sse2.PackSignedSaturate(resultIntLo, resultIntHi).AsUInt16();

				result = Sse2.And(result, Vector128.Create(0x0000_FFFF_FFFF_FFFF).AsUInt16());
				result = Sse2.Or(result, Sse2.And(channels, Vector128.Create(0xFFFF_0000_0000_0000).AsUInt16()));

				Sse2.Store((ushort*)pData, result);
			}

			for (; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = pTable[(ushort)item];
				ulong res1 = pTable[(ushort)(item >> 16)];
				ulong res2 = pTable[(ushort)(item >> 32)];
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort ToLinear(Fixed16 value) {
		double s = value.ValueToScalar();
		double result = (s <= 0.04045) ? (s * (1.0 / 12.92)) : Math.Pow((s + 0.055) * (1.0 / 1.055), 2.4);
		return Fixed16.FromReal(result).Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort ToLinearF(Fixed16 value) {
		float s = value.ValueToScalarF();
		float result = (s <= 0.04045f) ? (s * (1.0f / 12.92f)) : MathF.Pow((s + 0.055f) * (1.0f / 1.055f), 2.4f);
		return Fixed16.FromReal(result).Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeUncached(Span<Color16> data, Vector2I size) {
		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i) {
				Color16 color = *pData;

				pData->SetRgb(
					ToLinear(color.R),
					ToLinear(color.G),
					ToLinear(color.B)
				);
				++pData;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeUncachedF(Span<Color16> data, Vector2I size) {
		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i) {
				Color16 color = *pData;

				pData->SetRgb(
					ToLinearF(color.R),
					ToLinearF(color.G),
					ToLinearF(color.B)
				);
				++pData;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void LinearizeUncachedUnwrapped(Span<Color16> data, Vector2I size) {
		fixed (Color16* pRefData = data) {
			Color16* pData = pRefData;
			for (int i = 0; i < data.Length; ++i) {
				Color16 color = *pData;

				float rS = color.R.ValueToScalarF();
				float gS = color.G.ValueToScalarF();
				float bS = color.B.ValueToScalarF();

				rS = (rS <= 0.04045f) ? (rS * (1.0f / 12.92f)) : MathF.Pow((rS + 0.055f) * (1.0f / 1.055f), 2.4f);
				gS = (gS <= 0.04045f) ? (gS * (1.0f / 12.92f)) : MathF.Pow((gS + 0.055f) * (1.0f / 1.055f), 2.4f);
				bS = (bS <= 0.04045f) ? (bS * (1.0f / 12.92f)) : MathF.Pow((bS + 0.055f) * (1.0f / 1.055f), 2.4f);

				pData->SetRgb(
					Fixed16.FromReal(rS).Value,
					Fixed16.FromReal(gS).Value,
					Fixed16.FromReal(bS).Value
				);
				++pData;
			}
		}
	}
}
