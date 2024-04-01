/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.InteropServices;
using static SpriteMaster.Resample.Decoder.BlockDecoderCommon;

namespace SpriteMaster.Resample.Decoder;

internal static class InternalBlockDecoder {
	// https://www.khronos.org/opengl/wiki/S3_Texture_Compression
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
	private unsafe struct ColorBlock {
		/* The endianness of the data in the documentation is a bit confusing to me. */

		// Color should appear as BGR, 565.
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 2)]
		private readonly struct Color565 {
			[FieldOffset(0)]
			internal readonly ushort Packed;

			private uint PackedInt => Packed;

			internal Color565(ushort packed) {
				Packed = packed;
			}

			private enum Mask : uint {
				M5 = (1U << 5) - 1U,
				M6 = (1U << 6) - 1U,

				B = BitSize.B == 5 ? M5 : M6,
				G = BitSize.G == 5 ? M5 : M6,
				R = BitSize.R == 5 ? M5 : M6,
			}

			private enum Multiplier : uint {
				M5 = 255U / Mask.M5,
				M6 = 255U / Mask.M6,

				B = BitSize.B == 5 ? M5 : M6,
				G = BitSize.G == 5 ? M5 : M6,
				R = BitSize.R == 5 ? M5 : M6,
			}

			private enum BitSize : uint {
				B = 5,
				G = 6,
				R = 5
			}

			private enum Offset : int {
				B = 0,
				G = B + (int)BitSize.B,
				R = G + (int)BitSize.G
			}

			private uint PackedB => PackedInt >> (int)Offset.B & (uint)Mask.B;
			private uint PackedG => PackedInt >> (int)Offset.G & (uint)Mask.G;
			private uint PackedR => PackedInt >> (int)Offset.R & (uint)Mask.R;

			internal byte B => (byte)((uint)Multiplier.B * PackedB & 0xFF);
			internal byte G => (byte)((uint)Multiplier.G * PackedG & 0xFF);
			internal byte R => (byte)((uint)Multiplier.R * PackedR & 0xFF);

			// https://stackoverflow.com/a/2442609
			internal uint AsPacked =>
				(uint)B << 16 |
				(uint)G << 8 |
				R;
		}

		[FieldOffset(0)]
		private readonly Color565 color0;
		[FieldOffset(2)]
		private readonly Color565 color1;
		[FieldOffset(4)]
		private fixed byte codes[4];

		private readonly byte GetCode(Vector2U position) {
			var code = codes[position.Y];
			return (byte)((byte)(code >> (int)(position.X << 1)) & 0b11);
		}

		private static uint Pack(uint b, uint g, uint r) =>
			b << 16 |
			g << 8 |
			r;

		// Returns the color, hopefully, as ABGR
		internal readonly uint GetColor(Vector2U position) {
			if (color0.Packed > color1.Packed) {
				var code = GetCode(position);
				switch (code) {
					case 0:
						return color0.AsPacked;
					case 1:
						return color1.AsPacked;
					case 2: {
							var b = (2U * color0.B + color1.B) / 3U;
							var g = (2U * color0.G + color1.G) / 3U;
							var r = (2U * color0.R + color1.R) / 3U;
							return Pack(b, g, r);
						}
					default:
					case 3: {
							var b = (color0.B + 2U * color1.B) / 3U;
							var g = (color0.G + 2U * color1.G) / 3U;
							var r = (color0.R + 2U * color1.R) / 3U;
							return Pack(b, g, r);
						}
				}
			}
			else {
				return GetColorDXT3(position);
			}
		}

		internal readonly uint GetColorDXT3(Vector2U position) {
			var code = GetCode(position);
			switch (code) {
				case 0:
					return color0.AsPacked;
				case 1:
					return color1.AsPacked;
				case 2: {
						var b = (uint)(color0.B + color1.B) >> 1;
						var g = (uint)(color0.G + color1.G) >> 1;
						var r = (uint)(color0.R + color1.R) >> 1;
						return Pack(b, g, r);
					}
				default:
				case 3: {
						return 0U;
					}
			}
		}
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
	private unsafe struct ColorBlockDxt3 {
		[FieldOffset(0)]
		private fixed ushort AlphaBlock[4];
		[FieldOffset(8)]
		private readonly ColorBlock Color;

		internal readonly uint GetColor(Vector2U position) {
			var alphaValue = (byte)(AlphaBlock[position.Y] >> (int)(position.X * 4) & 0b1111);
			uint alpha = alphaValue * 0b10001u << 24;
			return Color.GetColorDXT3(position) | alpha;
		}
	}

	internal static Span<byte> Decode(ReadOnlySpan<byte> data, SpriteInfo info) => Decode(
		data,
		info.ReferenceSize,
		info.Reference.Format
	);

	internal static Span<byte> Decode(ReadOnlySpan<byte> data, Vector2I size, SurfaceFormat format) {
		Vector2U uSize = size;

		if (!IsBlockMultiple(uSize)) {
			throw new ArgumentException($"{nameof(size)}: {uSize} not block multiple");
		}

		switch (format) {
			case SurfaceFormat.Dxt1: {
					var blocks = data.Cast<ColorBlock>();
					var outData = SpanExt.Make<byte>((int)uSize.Area);
					var outDataPacked = outData.Cast<uint>();

					var widthBlocks = uSize.Width >> 2;

					uint blockIndex = 0;
					foreach (var block in blocks) {
						var index = blockIndex++;
						var xOffset = (index & widthBlocks - 1) << 2;
						var yOffset = index / widthBlocks << 2;

						for (uint y = 0; y < 4; ++y) {
							var yOffsetInternal = yOffset + y;
							for (uint x = 0; x < 4; ++x) {
								var xOffsetInternal = xOffset + x;
								var offset = yOffsetInternal * uSize.Width + xOffsetInternal;
								outDataPacked[(int)offset] = block.GetColor((x, y)) | 0xFF000000U;
							}
						}
					}

					return outData;
				}
			case SurfaceFormat.Dxt3: {
					var blocks = data.Cast<ColorBlockDxt3>();
					var outData = SpanExt.Make<byte>((int)uSize.Area * sizeof(uint));
					var outDataPacked = outData.Cast<uint>();

					var widthBlocks = uSize.Width >> 2;

					uint blockIndex = 0;
					foreach (var block in blocks) {
						var index = blockIndex++;
						var xOffset = (index & widthBlocks - 1) << 2;
						var yOffset = index / widthBlocks << 2;

						for (uint y = 0; y < 4; ++y) {
							var yOffsetInternal = yOffset + y;
							for (uint x = 0; x < 4; ++x) {
								var xOffsetInternal = xOffset + x;
								var offset = yOffsetInternal * uSize.Width + xOffsetInternal;
								outDataPacked[(int)offset] = block.GetColor((x, y));
							}
						}
					}

					return outData;
				}
			default:
				throw new ArgumentOutOfRangeException(nameof(format));
		}
	}
}
