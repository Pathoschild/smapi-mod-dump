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
using TeximpNet.Compression;
using TeximpNet.DDS;

namespace SpriteMaster.Resample.Encoder;

static class TeximpBlockEncoder {
	// We set this to false if block compression fails, as we assume that for whatever reason nvtt does not work on that system.
	private static volatile bool BlockCompressionFunctional = true;

	private const int SwapIndex0 = 0;
	private const int SwapIndex1 = 2;
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static unsafe void FlipColorBytes(byte* p, int length) {
		for (int i = 0; i < length; i += 4) {
			int index0 = i + SwapIndex0;
			int index1 = i + SwapIndex1;
			var temp = p[index0];
			p[index0] = p[index1];
			p[index1] = temp;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe Span<byte> Encode(ReadOnlySpan<Color8> data, ref TextureFormat format, Vector2I dimensions, bool hasAlpha, bool isPunchthroughAlpha, bool isMasky, bool hasR, bool hasG, bool hasB) {
		if (!BlockCompressionFunctional) {
			return null;
		}

		var oldSpriteFormat = format;
		
		try {
			using var compressor = new Compressor();
			compressor.Input.AlphaMode = (hasAlpha) ? AlphaMode.Premultiplied : AlphaMode.None;
			compressor.Input.GenerateMipmaps = false;
			var textureFormat = BlockEncoderCommon.GetBestTextureFormat(hasAlpha, isPunchthroughAlpha, isMasky);
			compressor.Compression.Format = textureFormat;
			compressor.Compression.Quality = Config.Resample.BlockCompression.Quality;
			compressor.Compression.SetQuantization(true, true, isPunchthroughAlpha);

			{
				compressor.Compression.GetColorWeights(out var r, out var g, out var b, out var a);
				a = hasAlpha ? (a * 20.0f) : 0.0f;
				// Relative luminance of the various channels.
				r = hasR ? (r * 0.2126f) : 0.0f;
				g = hasG ? (g * 0.7152f) : 0.0f;
				b = hasB ? (b * 0.0722f) : 0.0f;

				compressor.Compression.SetColorWeights(r, g, b, a);
			}

			compressor.Output.IsSRGBColorSpace = true;
			compressor.Output.OutputHeader = false;

			//public MipData (int width, int height, int rowPitch, IntPtr data, bool ownData = true)
			fixed (byte* p = data.Cast<byte>()) {
				using var mipData = new MipData(dimensions.Width, dimensions.Height, dimensions.Width * sizeof(int), (IntPtr)p, false);
				compressor.Input.SetData(mipData, false);
				var memoryBuffer = new byte[((SurfaceFormat)textureFormat).SizeBytes(dimensions.Area)];
				using var stream = memoryBuffer.Stream();
				if (compressor.Process(stream)) {
					format = textureFormat;
					return memoryBuffer;
				}
				else {
					Debug.WarningLn($"Failed to use {(CompressionFormat)textureFormat} compression: " + compressor.LastErrorString);
					Debug.WarningLn($"Dimensions: [{dimensions.Width}, {dimensions.Height}]");
				}
			}
		}
		catch (Exception ex) {
			ex.PrintWarning();
			BlockCompressionFunctional = false;
		}
		format = oldSpriteFormat;

		return null;
	}
}
