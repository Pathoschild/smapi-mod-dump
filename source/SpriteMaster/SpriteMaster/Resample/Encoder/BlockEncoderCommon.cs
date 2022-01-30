/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using TeximpNet.Compression;

namespace SpriteMaster.Resample.Encoder;

static class BlockEncoderCommon {
	internal static TextureFormat GetBestTextureFormat(bool hasAlpha, bool isPunchthroughAlpha, bool isMasky) =>
		(!hasAlpha) ?
			TextureFormat.WithNoAlpha :
			(
				(isPunchthroughAlpha && Config.Resample.BlockCompression.Quality != CompressionQuality.Fastest) ?
					TextureFormat.WithPunchthroughAlpha :
					isMasky ?
						TextureFormat.WithHardAlpha :
						TextureFormat.WithAlpha
			);
}
